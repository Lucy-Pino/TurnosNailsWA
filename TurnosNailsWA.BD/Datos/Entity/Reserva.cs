using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace TurnosNailsWA.BD.Datos.Entity
{
    [Index(nameof(CodigoReserva), IsUnique = true)]
    public class Reserva : EntityBase
    {
        [Required(ErrorMessage = "El código de reserva es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El código no puede superar los 20 caracteres.")]
        public string CodigoReserva { get; set; }

        public DateTime FechaCreacion { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "El importe total debe ser mayor o igual que cero.")]
        public decimal ImporteTotal { get; set; }

        [Required(ErrorMessage = "El estado de la reserva es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El estado no puede superar los 30 caracteres.")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "El cliente de la reserva es obligatorio.")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public Pago Pago { get; set; }
    }
}
