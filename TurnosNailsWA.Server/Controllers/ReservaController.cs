using Microsoft.AspNetCore.Mvc;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Repositorio.Repositorios;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Server.Controllers
{
    [ApiController] // Indica que esta clase es un controlador de API
    [Route("api/reserva")] // Define la ruta base para las acciones del controlador
    public class ReservaController : ControllerBase
    {
        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".pdf" };
        private const long TamanoMaximoComprobante = 5 * 1024 * 1024;
        private const decimal PorcentajeSenia = 0.30m;

        private readonly IReservaRepositorio reservaRepositorio;
        private readonly IClienteRepositorio clienteRepositorio;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly ITurnoRepositorio turnoRepositorio;
        private readonly IPagoRepositorio pagoRepositorio;
        private readonly IHorarioAtencionRepositorio horarioRepositorio;
        private readonly string carpetaComprobantes;

        public ReservaController(IReservaRepositorio reservaRepositorio,
                                 IClienteRepositorio clienteRepositorio,
                                 IServicioRepositorio servicioRepositorio,
                                 ITurnoRepositorio turnoRepositorio,
                                 IPagoRepositorio pagoRepositorio,
                                 IHorarioAtencionRepositorio horarioRepositorio,
                                 IWebHostEnvironment entorno)
        {
            this.reservaRepositorio = reservaRepositorio;
            this.clienteRepositorio = clienteRepositorio;
            this.servicioRepositorio = servicioRepositorio;
            this.turnoRepositorio = turnoRepositorio;
            this.pagoRepositorio = pagoRepositorio;
            this.horarioRepositorio = horarioRepositorio;
            carpetaComprobantes = Path.Combine(entorno.ContentRootPath, "Comprobantes");
        }

        [HttpGet] // api/reserva
        public async Task<ActionResult<List<ReservaListadoDTO>>> Get()
        {
            var lista = await reservaRepositorio.ListaReserva();
            if (lista.Count == 0)
            {
                return NotFound("Lista sin registros.");
            }
            return Ok(lista);
        }

        [HttpGet("{id:int}")] // api/reserva/5
        public async Task<ActionResult<ReservaDetalleDTO>> GetById(int id)
        {
            var reserva = await reservaRepositorio.SelectById(id);
            if (reserva is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }
            return Ok(await ArmarDetalle(reserva));
        }

        [HttpGet("codigo/{codigo}")] // api/reserva/codigo/TN-261120-A1B2
        public async Task<ActionResult<ReservaDetalleDTO>> GetByCodigo(string codigo)
        {
            var reserva = await reservaRepositorio.SelectByCodigo(codigo);
            if (reserva is null)
            {
                return NotFound($"No existe una reserva con el código: {codigo}.");
            }
            return Ok(await ArmarDetalle(reserva));
        }

        [HttpPost] // api/reserva (multipart/form-data: ReservaCrearDTO + comprobante)
        public async Task<ActionResult<ReservaDetalleDTO>> Post([FromForm] ReservaCrearDTO DTO, IFormFile? comprobante)
        {
            if (DTO.Turnos is null || DTO.Turnos.Count == 0)
            {
                return BadRequest("Debe seleccionar al menos un turno.");
            }

            var cliente = await clienteRepositorio.SelectById(DTO.ClienteId);
            if (cliente is null)
            {
                return NotFound($"No existe la clienta con id: {DTO.ClienteId}.");
            }

            var errorComprobante = ValidarComprobante(comprobante);
            if (errorComprobante is not null)
            {
                return BadRequest(errorComprobante);
            }

            var precios = new Dictionary<int, decimal>();
            var duraciones = new Dictionary<int, int>();
            foreach (var turnoDTO in DTO.Turnos)
            {
                if (precios.ContainsKey(turnoDTO.ServicioId))
                {
                    continue;
                }
                var servicio = await servicioRepositorio.SelectById(turnoDTO.ServicioId);
                if (servicio is null)
                {
                    return NotFound($"No existe el servicio con id: {turnoDTO.ServicioId}.");
                }
                precios[turnoDTO.ServicioId] = servicio.Precio;
                duraciones[turnoDTO.ServicioId] = servicio.Duracion;
            }

            var errorTurnos = await ValidarTurnos(DTO.Turnos, duraciones);
            if (errorTurnos is not null)
            {
                return BadRequest(errorTurnos);
            }

            decimal importeTotal = 0m;
            foreach (var turnoDTO in DTO.Turnos)
            {
                importeTotal += precios[turnoDTO.ServicioId];
            }
            var montoSenia = Math.Round(importeTotal * PorcentajeSenia, 2);

            var codigo = await GenerarCodigoUnico();
            if (codigo is null)
            {
                return BadRequest("No se pudo generar un código de reserva. Intentá nuevamente.");
            }

            var reserva = new Reserva
            {
                CodigoReserva = codigo,
                FechaCreacion = DateTime.Now,
                ImporteTotal = importeTotal,
                Estado = "Pendiente de verificación",
                ClienteId = DTO.ClienteId
            };

            var turnos = new List<Turno>();
            foreach (var turnoDTO in DTO.Turnos)
            {
                turnos.Add(new Turno
                {
                    Fecha = turnoDTO.Fecha.Date,
                    Hora = turnoDTO.Hora,
                    Estado = "Turno a confirmar",
                    ServicioId = turnoDTO.ServicioId
                });
            }

            var pago = new Pago
            {
                Monto = montoSenia,
                MetodoPago = "Transferencia",
                FechaComprobanteAdjuntado = DateTime.Now,
                Estado = "Pendiente de verificación"
            };

            var nombreArchivo = GuardarComprobante(comprobante!);
            pago.Comprobante = nombreArchivo;

            try
            {
                await reservaRepositorio.InsertReservaCompleta(reserva, turnos, pago);
            }
            catch (InvalidOperationException ex)
            {
                EliminarComprobante(nombreArchivo);
                return BadRequest(ex.Message);
            }
            catch
            {
                EliminarComprobante(nombreArchivo);
                return BadRequest("No se pudo registrar la reserva. Intentá nuevamente.");
            }

            return Ok(await ArmarDetalle(reserva));
        }

        [HttpPut("{id:int}/confirmar")] // api/reserva/5/confirmar
        public async Task<ActionResult<bool>> Confirmar(int id)
        {
            var reserva = await reservaRepositorio.SelectById(id);
            if (reserva is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }

            if (reserva.Estado == "Cancelada")
            {
                return BadRequest("La reserva está cancelada.");
            }

            if (reserva.Estado == "Reserva Confirmada")
            {
                return BadRequest("La reserva ya está confirmada.");
            }

            var turnos = await turnoRepositorio.ListaTurnoPorReserva(id);
            foreach (var turnoDTO in turnos)
            {
                if (turnoDTO.Estado != "Turno a confirmar")
                {
                    continue;
                }
                var turno = await turnoRepositorio.SelectById(turnoDTO.Id);
                if (turno is null)
                {
                    continue;
                }
                turno.Estado = "Turno confirmado";
                await turnoRepositorio.Update(turno);
            }

            reserva.Estado = "Reserva Confirmada";
            await reservaRepositorio.Update(reserva);
            return Ok(true);
        }

        [HttpPut("{id:int}/cancelar")] // api/reserva/5/cancelar
        public async Task<ActionResult<bool>> Cancelar(int id)
        {
            var reserva = await reservaRepositorio.SelectById(id);
            if (reserva is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }

            if (reserva.Estado == "Cancelada")
            {
                return BadRequest("La reserva ya está cancelada.");
            }

            var pago = await pagoRepositorio.SelectByReserva(id);
            if (pago is not null && pago.Estado != "Cancelado")
            {
                pago.Estado = "Cancelado";
                await pagoRepositorio.Update(pago);
            }

            var ahora = DateTime.Now;
            var turnos = await turnoRepositorio.ListaTurnoPorReserva(id);
            foreach (var turnoDTO in turnos)
            {
                if (turnoDTO.Estado == "Cancelado")
                {
                    continue;
                }
                if (turnoDTO.Fecha.Date + turnoDTO.Hora <= ahora)
                {
                    continue;
                }
                var turno = await turnoRepositorio.SelectById(turnoDTO.Id);
                if (turno is null)
                {
                    continue;
                }
                turno.Estado = "Cancelado";
                await turnoRepositorio.Update(turno);
            }

            reserva.Estado = "Cancelada";
            await reservaRepositorio.Update(reserva);
            return Ok(true);
        }

        // Arma el detalle reutilizando los DTOs y métodos de repositorio existentes.
        private async Task<ReservaDetalleDTO> ArmarDetalle(Reserva reserva)
        {
            var detalle = new ReservaDetalleDTO
            {
                Id = reserva.Id,
                CodigoReserva = reserva.CodigoReserva,
                FechaCreacion = reserva.FechaCreacion,
                ImporteTotal = reserva.ImporteTotal,
                Estado = reserva.Estado
            };

            var cliente = await clienteRepositorio.SelectById(reserva.ClienteId);
            if (cliente is not null)
            {
                detalle.Cliente = new ClienteDTO
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    Apellido = cliente.Apellido,
                    Telefono = cliente.Telefono
                };
            }

            var pago = await pagoRepositorio.SelectByReserva(reserva.Id);
            if (pago is not null)
            {
                detalle.Pago = new PagoDTO
                {
                    Id = pago.Id,
                    Monto = pago.Monto,
                    MetodoPago = pago.MetodoPago,
                    Comprobante = pago.Comprobante,
                    FechaComprobanteAdjuntado = pago.FechaComprobanteAdjuntado,
                    Estado = pago.Estado,
                    ReservaId = pago.ReservaId
                };
            }

            detalle.Turnos = await turnoRepositorio.ListaTurnoPorReserva(reserva.Id);
            return detalle;
        }

        // Valida fechas, horarios de atención, duración completa y superposiciones.
        private async Task<string?> ValidarTurnos(List<TurnoDTO> turnos, Dictionary<int, int> duraciones)
        {
            var ahora = DateTime.Now;
            var bloquesPorDia = new Dictionary<int, List<HorarioAtencion>>();
            var ocupadosPorFecha = new Dictionary<DateTime, List<Turno>>();

            for (int i = 0; i < turnos.Count; i++)
            {
                var turno = turnos[i];
                var fecha = turno.Fecha.Date;
                var duracion = TimeSpan.FromMinutes(duraciones[turno.ServicioId]);
                var fin = turno.Hora + duracion;

                if (fecha < ahora.Date)
                {
                    return "La fecha no puede ser anterior a hoy.";
                }

                if (fecha == ahora.Date && turno.Hora < ahora.TimeOfDay)
                {
                    return "El horario seleccionado ya pasó.";
                }

                int diaSemana = (int)fecha.DayOfWeek;
                if (!bloquesPorDia.ContainsKey(diaSemana))
                {
                    bloquesPorDia[diaSemana] = await horarioRepositorio.SelectActivosPorDia(diaSemana);
                }
                if (!EntraEnBloque(bloquesPorDia[diaSemana], turno.Hora, fin))
                {
                    return $"El servicio no entra en el horario de atención del {fecha:dd/MM/yyyy}.";
                }

                if (!ocupadosPorFecha.ContainsKey(fecha))
                {
                    ocupadosPorFecha[fecha] = await turnoRepositorio.SelectOcupadosPorFecha(fecha);
                }
                foreach (var ocupado in ocupadosPorFecha[fecha])
                {
                    var finOcupado = ocupado.Hora + TimeSpan.FromMinutes(ocupado.Servicio.Duracion);
                    if (SeSuperpone(turno.Hora, fin, ocupado.Hora, finOcupado))
                    {
                        return $"El horario {turno.Hora.ToString(@"hh\:mm")} del {fecha:dd/MM/yyyy} ya está ocupado.";
                    }
                }

                // Superposición entre los turnos de la misma solicitud.
                for (int j = 0; j < i; j++)
                {
                    var otro = turnos[j];
                    if (otro.Fecha.Date != fecha)
                    {
                        continue;
                    }
                    var duracionOtro = TimeSpan.FromMinutes(duraciones[otro.ServicioId]);
                    if (SeSuperpone(turno.Hora, fin, otro.Hora, otro.Hora + duracionOtro))
                    {
                        return "Hay turnos de la misma reserva que se superponen.";
                    }
                }
            }

            return null;
        }

        private async Task<string?> GenerarCodigoUnico()
        {
            for (int intento = 0; intento < 5; intento++)
            {
                var codigo = GenerarCodigo();
                var existente = await reservaRepositorio.SelectByCodigo(codigo);
                if (existente is null)
                {
                    return codigo;
                }
            }
            return null;
        }

        private static string GenerarCodigo()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var sufijo = new char[4];
            for (int i = 0; i < sufijo.Length; i++)
            {
                sufijo[i] = caracteres[Random.Shared.Next(caracteres.Length)];
            }
            return "TN-" + DateTime.Now.ToString("yyMMdd") + "-" + new string(sufijo);
        }

        private static string? ValidarComprobante(IFormFile? comprobante)
        {
            if (comprobante is null || comprobante.Length == 0)
            {
                return "El comprobante es obligatorio.";
            }

            if (comprobante.Length > TamanoMaximoComprobante)
            {
                return "El comprobante no puede superar los 5 MB.";
            }

            var extension = Path.GetExtension(comprobante.FileName).ToLowerInvariant();
            if (!ExtensionesPermitidas.Contains(extension))
            {
                return "El comprobante debe ser JPG, JPEG, PNG o PDF.";
            }

            return null;
        }

        private string GuardarComprobante(IFormFile comprobante)
        {
            Directory.CreateDirectory(carpetaComprobantes);
            var extension = Path.GetExtension(comprobante.FileName).ToLowerInvariant();
            var nombreArchivo = Guid.NewGuid().ToString("N") + extension;
            var ruta = Path.Combine(carpetaComprobantes, nombreArchivo);
            using var stream = new FileStream(ruta, FileMode.CreateNew);
            comprobante.CopyTo(stream);
            return nombreArchivo;
        }

        private void EliminarComprobante(string nombreArchivo)
        {
            try
            {
                var ruta = Path.Combine(carpetaComprobantes, nombreArchivo);
                if (System.IO.File.Exists(ruta))
                {
                    System.IO.File.Delete(ruta);
                }
            }
            catch
            {
                // Si no se puede eliminar, el archivo queda huérfano sin afectar la reserva.
            }
        }

        private static bool EntraEnBloque(List<HorarioAtencion> bloques, TimeSpan inicio, TimeSpan fin)
        {
            foreach (var bloque in bloques)
            {
                if (inicio >= bloque.HoraInicio && fin <= bloque.HoraFin)
                {
                    return true;
                }
            }
            return false;
        }

        // Intervalos semiabiertos: permite turnos consecutivos y evita superposiciones.
        private static bool SeSuperpone(TimeSpan inicio, TimeSpan fin, TimeSpan inicioOcupado, TimeSpan finOcupado)
        {
            return inicio < finOcupado && inicioOcupado < fin;
        }
    }
}
