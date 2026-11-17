using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public class TurnoRepositorio : Repositorio<Turno>, ITurnoRepositorio
    {
        private readonly AppDbContext context;

        public TurnoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TurnoListadoDTO>> ListaTurno()
        {
            var lista = await
            context.Turnos
                        .OrderBy(t => t.Fecha)
                        .ThenBy(t => t.Hora)
                        .Select(t => new TurnoListadoDTO
                        {
                            Id = t.Id,
                            Fecha = t.Fecha,
                            Hora = t.Hora,
                            Estado = t.Estado,
                            ServicioNombre = t.Servicio.Nombre,
                            Categoria = t.Servicio.Categoria,
                            Duracion = t.Servicio.Duracion,
                            Precio = t.Servicio.Precio,
                            CodigoReserva = t.Reserva.CodigoReserva,
                            ClienteNombre = t.Reserva.Cliente.Nombre + " " + t.Reserva.Cliente.Apellido
                        })
                        .ToListAsync();
            return lista;
        }

        public async Task<List<TurnoListadoDTO>> ListaTurnoPorReserva(int reservaId)
        {
            var lista = await
            context.Turnos
                        .Where(t => t.ReservaId == reservaId)
                        .OrderBy(t => t.Fecha)
                        .ThenBy(t => t.Hora)
                        .Select(t => new TurnoListadoDTO
                        {
                            Id = t.Id,
                            Fecha = t.Fecha,
                            Hora = t.Hora,
                            Estado = t.Estado,
                            ServicioNombre = t.Servicio.Nombre,
                            Categoria = t.Servicio.Categoria,
                            Duracion = t.Servicio.Duracion,
                            Precio = t.Servicio.Precio,
                            CodigoReserva = t.Reserva.CodigoReserva,
                            ClienteNombre = t.Reserva.Cliente.Nombre + " " + t.Reserva.Cliente.Apellido
                        })
                        .ToListAsync();
            return lista;
        }

        public async Task<List<Turno>> SelectOcupadosPorFecha(DateTime fecha)
        {
            return await
            context.Turnos
                        .Include(t => t.Servicio)
                        .Where(t => t.Fecha.Date == fecha.Date && t.Estado != "Cancelado")
                        .ToListAsync();
        }
    }
}