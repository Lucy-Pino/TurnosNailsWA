using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<Reserva> InsertReservaCompleta(Reserva reserva, List<Turno> turnos, Pago pago)
        {
            if (turnos == null || turnos.Count == 0)
            {
                throw new ArgumentException("La reserva debe tener al menos un turno.");
            }

            if (pago == null)
            {
                throw new ArgumentException("La reserva debe tener un pago registrado.");
            }

            await using var transaccion = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                // Segunda verificación dentro de la transacción: la validación del controlador
                // quedó fuera y dos altas simultáneas podrían reservar el mismo horario.
                await VerificarSuperposicion(turnos);

                context.Reservas.Add(reserva);
                await context.SaveChangesAsync();

                pago.ReservaId = reserva.Id;
                context.Pagos.Add(pago);

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

        // Verifica superposiciones leyendo los turnos ocupados dentro de la transacción Serializable,
        // con el mismo criterio de intervalos semiabiertos que usa TurnoController.
        private async Task VerificarSuperposicion(List<Turno> turnos)
        {
            var fechaMin = turnos.Min(t => t.Fecha.Date);
            var fechaMax = turnos.Max(t => t.Fecha.Date).AddDays(1);
            var idsServicio = turnos.Select(t => t.ServicioId).Distinct().ToList();

            var duraciones = await context.Servicios
                .Where(s => idsServicio.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Duracion);

            var ocupados = await context.Turnos
                .Where(t => t.Fecha >= fechaMin && t.Fecha < fechaMax && t.Estado != "Cancelado")
                .Select(t => new { t.Fecha, t.Hora, t.Servicio.Duracion })
                .ToListAsync();

            foreach (var turno in turnos)
            {
                var inicio = turno.Hora;
                var fin = inicio.Add(TimeSpan.FromMinutes(duraciones[turno.ServicioId]));

                foreach (var ocupado in ocupados)
                {
                    if (ocupado.Fecha.Date != turno.Fecha.Date)
                    {
                        continue;
                    }

                    var inicioOcupado = ocupado.Hora;
                    var finOcupado = inicioOcupado.Add(TimeSpan.FromMinutes(ocupado.Duracion));

                    if (inicio < finOcupado && inicioOcupado < fin)
                    {
                        throw new InvalidOperationException("El horario seleccionado ya no está disponible. Actualizá la disponibilidad e intentá nuevamente.");
                    }
                }
            }
        }
    }
}
