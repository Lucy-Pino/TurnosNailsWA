using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class TurnoListadoDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Estado { get; set; } = "";
        public string ServicioNombre { get; set; } = "";
        public string CodigoReserva { get; set; } = "";
        public string ClienteNombre { get; set; } = "";

    }
}
