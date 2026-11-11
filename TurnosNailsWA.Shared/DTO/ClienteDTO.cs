using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class ClienteDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido del cliente es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El teléfono del cliente es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El teléfono no puede superar los 100 caracteres.")]
        public string Telefono { get; set; } = "";
    }
}
