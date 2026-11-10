using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public class TurnoRepositorio : Repositorio<Turno>, ITurnoRepositorio
    {
        public TurnoRepositorio(AppDbContext context) : base(context)
        {
        }
    }
}
