using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class ReservaListadoDTO
    {
        public int Id { get; set; }
        public string CodigoReserva { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public decimal ImporteTotal { get; set; }
        public string Estado { get; set; } = "";
        public string ClienteNombre { get; set; } = "";

    }
}
