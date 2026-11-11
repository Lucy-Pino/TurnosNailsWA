using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public class ReservaRepositorio : Repositorio<Reserva>, IReservaRepositorio
    {
        private readonly AppDbContext context;

        public ReservaRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ReservaListadoDTO>> ListaReserva()
        {
            var lista = await
            context.Reservas
                        .OrderByDescending(r => r.FechaCreacion)
                        .Select(r => new ReservaListadoDTO
                        {
                            Id = r.Id,
                            CodigoReserva = r.CodigoReserva,
                            FechaCreacion = r.FechaCreacion,
                            ImporteTotal = r.ImporteTotal,
                            Estado = r.Estado,
                            ClienteNombre = r.Cliente.Nombre + " " + r.Cliente.Apellido
                        })
                        .ToListAsync();
            return lista;
        }

        public async Task<Reserva> InsertReservaCompleta(Reserva reserva, List<Turno> turnos)
        {
            if (turnos == null || turnos.Count == 0)
            {
                throw new ArgumentException("La reserva debe tener al menos un turno.");
            }

            await using var transaccion = await context.Database.BeginTransactionAsync();

            try
            {
                context.Reservas.Add(reserva);
                await context.SaveChangesAsync();

                foreach (var turno in turnos)
                {
                    turno.ReservaId = reserva.Id;
                }

                context.Turnos.AddRange(turnos);
                await context.SaveChangesAsync();

                await transaccion.CommitAsync();
                return reserva;
            }
            catch
            {
                await transaccion.RollbackAsync();
                throw;
            }
        }

        public async Task<Reserva?> SelectByCodigo(string codigo)
        {
            return await context.Reservas.FirstOrDefaultAsync(r => r.CodigoReserva == codigo);
        }
    }
}
