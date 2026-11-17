using System;
using System.ComponentModel.DataAnnotations;

namespace TurnosNailsWA.Shared.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string Usuario { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = "";
    }
}
