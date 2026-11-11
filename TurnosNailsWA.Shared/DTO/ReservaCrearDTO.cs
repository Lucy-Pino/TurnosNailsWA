using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class ReservaCrearDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos un turno.")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos un turno.")]
        public List<TurnoDTO> Turnos { get; set; } = new List<TurnoDTO>();
    }
}
