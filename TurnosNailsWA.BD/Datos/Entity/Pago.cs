using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.BD.Datos.Entity
{
    public class Pago : EntityBase
    {
        [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "El monto debe ser mayor o igual que cero.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El método de pago no puede superar los 50 caracteres.")]
        public string MetodoPago { get; set; }

        [Required(ErrorMessage = "El comprobante es obligatorio.")]
        [MaxLength(255, ErrorMessage = "La ruta del comprobante no puede superar los 255 caracteres.")]
        public string Comprobante { get; set; }

        public DateTime FechaComprobanteAdjuntado { get; set; }

        [Required(ErrorMessage = "El estado del pago es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El estado no puede superar los 30 caracteres.")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "La reserva del pago es obligatoria.")]
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }
    }
}
