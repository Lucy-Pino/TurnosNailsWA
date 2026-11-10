using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.BD.Datos.Entity
{
    public class Cliente : EntityBase
    {
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(100, ErrorMessage = "Maxima longitud 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(100, ErrorMessage = "Maxima longitud 100 caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(100, ErrorMessage = "Maxima longitud 100 caracteres.")]
        public string Telefono { get; set; }

    }
}
