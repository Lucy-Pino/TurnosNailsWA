using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.BD.Datos.Entity
{
    public class Pago : EntityBase
    {
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; }

        public int IdTurno { get; set; }
        public Turno Turno { get; set; }

    }
}
