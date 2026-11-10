using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.BD.Datos.Entity
{
    public class Servicio : EntityBase
    {

        public string Nombre { get; set; }
        public string Duracion { get; set; }
        public decimal Precio { get; set; }

        //Un servicio puede tener varios turnos, pero cada turno corresponde a un solo servicio
        //public List<Turno> Turnos { get; set; }

    }
}
