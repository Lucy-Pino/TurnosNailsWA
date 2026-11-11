using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class PagoRegistrarDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "La reserva es obligatoria.")]
        public int ReservaId { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El método de pago no puede superar los 50 caracteres.")]
        public string MetodoPago { get; set; } = "";

        [Required(ErrorMessage = "El comprobante es obligatorio.")]
        [MaxLength(255, ErrorMessage = "La ruta del comprobante no puede superar los 255 caracteres.")]
        public string Comprobante { get; set; } = "";
    }
}
