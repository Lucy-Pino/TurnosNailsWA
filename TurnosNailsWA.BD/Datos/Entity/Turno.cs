using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.BD.Datos.Entity
{
    public class Turno : EntityBase
    {

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }
        public decimal Adelanto { get; set; }


        public int IdCliente { get; set; }
        public Cliente Cliente { get; set; }
        public int IdServicio { get; set; }
        public Servicio Servicio { get; set; }

    }
}
