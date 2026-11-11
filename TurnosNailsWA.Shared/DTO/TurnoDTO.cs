using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class TurnoDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El servicio es obligatorio.")]
        public int ServicioId { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }
    }
}
