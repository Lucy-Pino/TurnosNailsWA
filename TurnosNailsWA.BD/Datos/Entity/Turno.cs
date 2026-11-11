using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.BD.Datos.Entity
{
    public class Turno : EntityBase
    {
        public DateTime Fecha { get; set; }

        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "El estado del turno es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El estado no puede superar los 30 caracteres.")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "La reserva del turno es obligatoria.")]
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }

        [Required(ErrorMessage = "El servicio del turno es obligatorio.")]
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
    }
}
