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
        private readonly IPagoRepositorio pagoRepositorio;

        public TurnoController(ITurnoRepositorio turnoRepositorio,
                               IServicioRepositorio servicioRepositorio,
                               IHorarioAtencionRepositorio horarioRepositorio,
                               IReservaRepositorio reservaRepositorio,
                               IPagoRepositorio pagoRepositorio)
        {
            this.turnoRepositorio = turnoRepositorio;
            this.servicioRepositorio = servicioRepositorio;
            this.horarioRepositorio = horarioRepositorio;
            this.reservaRepositorio = reservaRepositorio;
            this.pagoRepositorio = pagoRepositorio;
        }

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
                        hora += duracion;
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

                    // Si hay superposición, avanza hasta el final del turno ocupado y recalcula desde allí.
                    if (finOcupado is not null)
                    {
                        hora = finOcupado.Value;
                        continue;
                    }

                    disponibles.Add(new TurnoDTO
                    {
                        ServicioId = servicio.Id,
                        Fecha = fecha.Date,
                        Hora = hora
                    });

                    // El siguiente horario consecutivo comienza al terminar el servicio elegido.
                    hora += duracion;
                }
            }

            if (disponibles.Count == 0)
            {
                return NotFound("No hay horarios disponibles para esa fecha.");
            }

            return Ok(disponibles);
        }

        [HttpPut("{id:int}/cancelar")] // api/turno/5/cancelar
        public async Task<ActionResult<bool>> Cancelar(int id)
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

            if (turno.Fecha.Date + turno.Hora < DateTime.Now)
            {
                return BadRequest("No se puede cancelar un turno que ya pasó.");
            }

            turno.Estado = "Cancelado";
            var actualizado = await turnoRepositorio.Update(turno);
            if (!actualizado)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }

            // D4: si ya no queda ningún turno activo en la reserva, la reserva pasa a "Cancelada".
            var turnosReserva = await turnoRepositorio.ListaTurnoPorReserva(turno.ReservaId);
            var todosCancelados = turnosReserva.All(t => t.Estado == "Cancelado");
            if (todosCancelados)
            {
                var reserva = await reservaRepositorio.SelectById(turno.ReservaId);
                if (reserva is not null && reserva.Estado != "Cancelada")
                {
                    reserva.Estado = "Cancelada";
                    await reservaRepositorio.Update(reserva);
                }

                // Mismo criterio que ReservaController.Cancelar: la reserva cancelada deja el pago en "Cancelado".
                var pago = await pagoRepositorio.SelectByReserva(turno.ReservaId);
                if (pago is not null && pago.Estado != "Cancelado")
                {
                    pago.Estado = "Cancelado";
                    await pagoRepositorio.Update(pago);
                }
            }

            return Ok(true);
        }

        // Intervalos semiabiertos: permite turnos consecutivos y evita superposiciones.
        private static bool SeSuperpone(TimeSpan inicio, TimeSpan fin, TimeSpan inicioOcupado, TimeSpan finOcupado)
        {
            return inicio < finOcupado && inicioOcupado < fin;
        }
    }
}
