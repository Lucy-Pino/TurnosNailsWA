using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Shared.DTO
{
    public class TurnoDTO
    {
        public int IdCliente { get; set; }

        public int IdServicio { get; set; }

        public DateTime Fecha { get; set; }

        public string Hora { get; set; }

    }
}
