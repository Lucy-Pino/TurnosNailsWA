using Microsoft.AspNetCore.Mvc;
using TurnosNailsWA.Repositorio.Repositorios;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Server.Controllers
{
    [ApiController] // Indica que esta clase es un controlador de API
    [Route("api/turno")] // Define la ruta base para las acciones del controlador
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoRepositorio turnoRepositorio;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IHorarioAtencionRepositorio horarioRepositorio;
        private readonly IReservaRepositorio reservaRepositorio;

        public TurnoController(ITurnoRepositorio turnoRepositorio,
                               IServicioRepositorio servicioRepositorio,
                               IHorarioAtencionRepositorio horarioRepositorio,
                               IReservaRepositorio reservaRepositorio)
        {
            this.turnoRepositorio = turnoRepositorio;
            this.servicioRepositorio = servicioRepositorio;
            this.horarioRepositorio = horarioRepositorio;
            this.reservaRepositorio = reservaRepositorio;
        }

        // Grilla fija de horarios ofrecidos: cada 15 minutos, independiente de la duración del servicio.
        private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(15);

        [HttpGet] // api/turno
        public async Task<ActionResult<List<TurnoListadoDTO>>> Get()
        {
            var lista = await turnoRepositorio.ListaTurno();
            if (lista.Count == 0)
            {
                return NotFound("Lista sin registros.");
            }
            return Ok(lista);
        }

        [HttpGet("reserva/{reservaId:int}")] // api/turno/reserva/5
        public async Task<ActionResult<List<TurnoListadoDTO>>> GetPorReserva(int reservaId)
        {
            var lista = await turnoRepositorio.ListaTurnoPorReserva(reservaId);
            if (lista.Count == 0)
            {
                return NotFound("Lista sin registros.");
            }
            return Ok(lista);
        }

        [HttpGet("disponibilidad")] // api/turno/disponibilidad?fecha=2026-11-20&servicioId=1
        public async Task<ActionResult<List<TurnoDTO>>> GetDisponibilidad(DateTime fecha, int servicioId)
        {
            var servicio = await servicioRepositorio.SelectById(servicioId);
            if (servicio is null)
            {
                return NotFound("No existe el servicio.");
            }

            if (fecha.Date < DateTime.Today)
            {
                return BadRequest("La fecha no puede ser anterior a hoy.");
            }

            if (servicio.Duracion <= 0)
            {
                return BadRequest("La duración del servicio no es válida.");
            }

            int diaSemana = (int)fecha.DayOfWeek;
            var bloques = await horarioRepositorio.SelectActivosPorDia(diaSemana);
            if (bloques.Count == 0)
            {
                return NotFound("No hay horarios activos para ese día.");
            }

            var ocupados = await turnoRepositorio.SelectOcupadosPorFecha(fecha);
            var duracion = TimeSpan.FromMinutes(servicio.Duracion);
            var ahora = DateTime.Now.TimeOfDay;
            var disponibles = new List<TurnoDTO>();

            foreach (var bloque in bloques)
            {
                // Cada bloque de atención se recorre por separado.
                var hora = bloque.HoraInicio;

                while (hora + duracion <= bloque.HoraFin)
                {
                    var fin = hora + duracion;

                    // Si la fecha es hoy, no se ofrecen horarios que ya pasaron.
                    if (fecha.Date == DateTime.Today && hora < ahora)
                    {
                        hora += Intervalo;
                        continue;
                    }

                    // Busca el turno ocupado que termina más temprano y se superpone con el candidato.
                    TimeSpan? finOcupado = null;
                    foreach (var ocupado in ocupados)
                    {
                        var finTurno = ocupado.Hora + TimeSpan.FromMinutes(ocupado.Servicio.Duracion);
                        if (SeSuperpone(hora, fin, ocupado.Hora, finTurno)
                            && (finOcupado is null || finTurno < finOcupado.Value))
                        {
                            finOcupado = finTurno;
                        }
                    }

                    // Si hay superposición, avanza hasta el siguiente horario de la grilla
                    // posterior al final del turno ocupado y recalcula desde allí.
                    if (finOcupado is not null)
                    {
                        hora = SiguienteHorario(finOcupado.Value, bloque.HoraInicio);
                        continue;
                    }

                    disponibles.Add(new TurnoDTO
                    {
                        ServicioId = servicio.Id,
                        Fecha = fecha.Date,
                        Hora = hora
                    });

                    // El siguiente candidato avanza siempre 15 minutos, sin importar la duración del servicio.
                    hora += Intervalo;
                }
            }

            if (disponibles.Count == 0)
            {
                return NotFound("No hay horarios disponibles para esa fecha.");
            }

            return Ok(disponibles);
        }

        [HttpPut("{id:int}/cancelar")] // api/turno/5/cancelar (cancelación individual solicitada por la clienta)
        public async Task<ActionResult<bool>> Cancelar(int id, [FromHeader(Name = "X-Codigo-Reserva")] string? codigoReserva)
        {
            var turno = await turnoRepositorio.SelectById(id);
            if (turno is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }

            if (turno.Estado == "Cancelado")
            {
                return BadRequest("El turno ya está cancelado.");
            }

            // La clienta solo puede cancelar turnos de una reserva ya confirmada por la administración.
            var reserva = await reservaRepositorio.SelectById(turno.ReservaId);
            if (reserva is null)
            {
                return NotFound($"No existe la reserva del turno con id: {id}.");
            }

            if (string.IsNullOrWhiteSpace(codigoReserva) ||
                !string.Equals(reserva.CodigoReserva, codigoReserva.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("El código de reserva es inválido o no coincide con la reserva.");
            }

            if (reserva.Estado != "Reserva Confirmada")
            {
                return BadRequest("Solo se pueden cancelar turnos de una reserva confirmada.");
            }

            // Un turno deja de poder cancelarse en el momento en que llega su horario.
            if (turno.Fecha.Date + turno.Hora <= DateTime.Now)
            {
                return BadRequest("No se puede cancelar un turno que ya comenzó.");
            }

            turno.Estado = "Cancelado";
            var actualizado = await turnoRepositorio.Update(turno);
            if (!actualizado)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }

            // D4: si ya no queda ningún turno activo en la reserva, la reserva pasa a "Cancelada".
            // El pago acreditado se conserva como historial: no se devuelve el adelanto.
            var turnosReserva = await turnoRepositorio.ListaTurnoPorReserva(turno.ReservaId);
            var todosCancelados = turnosReserva.All(t => t.Estado == "Cancelado");
            if (todosCancelados && reserva.Estado != "Cancelada")
            {
                reserva.Estado = "Cancelada";
                await reservaRepositorio.Update(reserva);
            }

            return Ok(true);
        }

        // Primer horario de la grilla (anclada al inicio del bloque) que sea mayor o igual a "hora".
        private static TimeSpan SiguienteHorario(TimeSpan hora, TimeSpan inicioBloque)
        {
            var transcurrido = hora - inicioBloque;
            if (transcurrido <= TimeSpan.Zero)
            {
                return inicioBloque;
            }

            var resto = transcurrido.Ticks % Intervalo.Ticks;
            return resto == 0 ? hora : hora + TimeSpan.FromTicks(Intervalo.Ticks - resto);
        }

        // Intervalos semiabiertos: permite turnos consecutivos y evita superposiciones.
        private static bool SeSuperpone(TimeSpan inicio, TimeSpan fin, TimeSpan inicioOcupado, TimeSpan finOcupado)
        {
            return inicio < finOcupado && inicioOcupado < fin;
        }
    }
}
