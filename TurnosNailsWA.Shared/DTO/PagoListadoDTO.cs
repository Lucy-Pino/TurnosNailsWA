using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class PagoListadoDTO
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = "";
        public string Comprobante { get; set; } = "";
        public string Estado { get; set; } = "";
        public DateTime FechaComprobanteAdjuntado { get; set; }
        public string CodigoReserva { get; set; } = "";
        public string ClienteNombre { get; set; } = "";

    }
}
