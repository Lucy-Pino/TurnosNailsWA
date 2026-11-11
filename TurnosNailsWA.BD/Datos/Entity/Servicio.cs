using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.BD.Datos.Entity
{
    public class Servicio : EntityBase
    {
        [Required(ErrorMessage = "El nombre del servicio es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La categoría del servicio es obligatoria.")]
        [MaxLength(50, ErrorMessage = "La categoría no puede superar los 50 caracteres.")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "La descripción del servicio es obligatoria.")]
        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor que cero.")]
        public int Duracion { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "El precio debe ser mayor o igual que cero.")]
        public decimal Precio { get; set; }
    }
}
