using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class PagoEstadoDTO
    {
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El estado no puede superar los 30 caracteres.")]
        public string Estado { get; set; } = "";
    }
}
