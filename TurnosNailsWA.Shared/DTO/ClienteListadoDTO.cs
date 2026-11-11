using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class ClienteListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Telefono { get; set; } = "";

    }
}
