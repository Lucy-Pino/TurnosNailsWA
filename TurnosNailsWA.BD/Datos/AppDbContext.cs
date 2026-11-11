using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos.Entity;

namespace TurnosNailsWA.BD.Datos
{
    public class AppDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; } //tabla Clientes
        public DbSet<Servicio> Servicios { get; set; } //tabla Servicios
        public DbSet<Reserva> Reservas { get; set; } //tabla Reservas
        public DbSet<Turno> Turnos { get; set; } //tabla Turnos
        public DbSet<Pago> Pagos { get; set; } //tabla Pagos
        public DbSet<HorarioAtencion> HorariosAtencion { get; set; } //tabla HorariosAtencion


        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }
    }
}
