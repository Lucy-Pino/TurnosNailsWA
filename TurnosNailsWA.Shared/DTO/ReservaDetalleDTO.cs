using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class ReservaDetalleDTO
    {
        public int Id { get; set; }
        public string CodigoReserva { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public decimal ImporteTotal { get; set; }
        public string Estado { get; set; } = "";
        public ClienteDTO Cliente { get; set; } = new ClienteDTO();
        public PagoDTO? Pago { get; set; }
        public List<TurnoListadoDTO> Turnos { get; set; } = new List<TurnoListadoDTO>();
    }
}
