using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnosNailsWA.Repositorio.Repositorios;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Server.Controllers
{
    [Authorize]
    [ApiController] // Indica que esta clase es un controlador de API
    [Route("api/pago")] // Define la ruta base para las acciones del controlador
    public class PagoController : ControllerBase
    {
        private readonly IPagoRepositorio pagoRepositorio;
        private readonly string carpetaComprobantes;

        public PagoController(IPagoRepositorio pagoRepositorio, IWebHostEnvironment entorno)
        {
            this.pagoRepositorio = pagoRepositorio;
            carpetaComprobantes = Path.Combine(entorno.ContentRootPath, "Comprobantes");
        }

        [HttpGet] // api/pago
        public async Task<ActionResult<List<PagoListadoDTO>>> Get()
        {
            var lista = await pagoRepositorio.ListaPago();
            if (lista.Count == 0)
            {
                return NotFound("Lista sin registros.");
            }
            return Ok(lista);
        }

        [HttpGet("pendientes")] // api/pago/pendientes
        public async Task<ActionResult<List<PagoListadoDTO>>> GetPendientes()
        {
            // Primero los pagos que necesitan aclaración y después los pendientes de verificación.
            var paraAclarar = await pagoRepositorio.ListaPendientesAclaracion();
            var paraVerificar = await pagoRepositorio.ListaPendientesVerificacion();
            var lista = paraAclarar.Concat(paraVerificar).ToList();
            return Ok(lista);
        }

        [HttpGet("{id:int}")] // api/pago/5
        public async Task<ActionResult<PagoDTO>> GetById(int id)
        {
            var pago = await pagoRepositorio.SelectById(id);
            if (pago is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }
            return Ok(ArmarPago(pago));
        }

        [HttpGet("reserva/{reservaId:int}")] // api/pago/reserva/5
        public async Task<ActionResult<PagoDTO>> GetPorReserva(int reservaId)
        {
            var pago = await pagoRepositorio.SelectByReserva(reservaId);
            if (pago is null)
            {
                return NotFound($"No existe un pago para la reserva con id: {reservaId}.");
            }
            return Ok(ArmarPago(pago));
        }

        [HttpGet("{id:int}/comprobante")] // api/pago/5/comprobante
        public async Task<ActionResult> GetComprobante(int id)
        {
            var pago = await pagoRepositorio.SelectById(id);
            if (pago is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }

            var ruta = Path.Combine(carpetaComprobantes, pago.Comprobante);
            if (!System.IO.File.Exists(ruta))
            {
                return NotFound("El archivo del comprobante no está disponible.");
            }

            var extension = Path.GetExtension(ruta).ToLowerInvariant();
            var tipoContenido = extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            return PhysicalFile(ruta, tipoContenido);
        }

        private static PagoDTO ArmarPago(TurnosNailsWA.BD.Datos.Entity.Pago pago)
        {
            return new PagoDTO
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
    }
}
