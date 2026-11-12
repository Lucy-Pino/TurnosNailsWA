using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public class PagoRepositorio : Repositorio<Pago>, IPagoRepositorio
    {
        private readonly AppDbContext context;

        public PagoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<PagoListadoDTO>> ListaPago()
        {
            var lista = await
            context.Pagos
                        .OrderByDescending(p => p.FechaComprobanteAdjuntado)
                        .Select(p => new PagoListadoDTO
                        {
                            Id = p.Id,
                            Monto = p.Monto,
                            MetodoPago = p.MetodoPago,
                            Comprobante = p.Comprobante,
                            Estado = p.Estado,
                            FechaComprobanteAdjuntado = p.FechaComprobanteAdjuntado,
                            CodigoReserva = p.Reserva.CodigoReserva,
                            ClienteNombre = p.Reserva.Cliente.Nombre + " " + p.Reserva.Cliente.Apellido
                        })
                        .ToListAsync();
            return lista;
        }

        public async Task<List<PagoListadoDTO>> ListaPendientesVerificacion()
        {
            var lista = await
            context.Pagos
                        .Where(p => p.Estado == "Pendiente de verificación"
                                 && p.Reserva.Estado == "Pendiente de verificación")
                        .OrderBy(p => p.FechaComprobanteAdjuntado)
                        .Select(p => new PagoListadoDTO
                        {
                            Id = p.Id,
                            Monto = p.Monto,
                            MetodoPago = p.MetodoPago,
                            Comprobante = p.Comprobante,
                            Estado = p.Estado,
                            FechaComprobanteAdjuntado = p.FechaComprobanteAdjuntado,
                            CodigoReserva = p.Reserva.CodigoReserva,
                            ClienteNombre = p.Reserva.Cliente.Nombre + " " + p.Reserva.Cliente.Apellido
                        })
                        .ToListAsync();
            return lista;
        }

        public async Task<List<PagoListadoDTO>> ListaPendientesAclaracion()
        {
            var lista = await
            context.Pagos
                        .Where(p => p.Estado == "Pendiente de aclaración"
                                 && p.Reserva.Estado == "Pendiente de verificación")
                        .OrderBy(p => p.FechaComprobanteAdjuntado)
                        .Select(p => new PagoListadoDTO
                        {
                            Id = p.Id,
                            Monto = p.Monto,
                            MetodoPago = p.MetodoPago,
                            Comprobante = p.Comprobante,
                            Estado = p.Estado,
                            FechaComprobanteAdjuntado = p.FechaComprobanteAdjuntado,
                            CodigoReserva = p.Reserva.CodigoReserva,
                            ClienteNombre = p.Reserva.Cliente.Nombre + " " + p.Reserva.Cliente.Apellido
                        })
                        .ToListAsync();
            return lista;
        }

        public async Task<int> ActualizarVencidos(int minutos)
        {
            if (minutos <= 0)
            {
                throw new ArgumentException("Los minutos deben ser mayores que cero.");
            }

            var limite = DateTime.Now.AddMinutes(-minutos);

            var vencidos = await
            context.Pagos
                        .Where(p => p.Estado == "Pendiente de verificación"
                                 && p.Reserva.Estado == "Pendiente de verificación"
                                 && p.FechaComprobanteAdjuntado <= limite)
                        .ToListAsync();

            if (vencidos.Count == 0)
            {
                return 0;
            }

            foreach (var pago in vencidos)
            {
                pago.Estado = "Pendiente de aclaración";
            }

            await context.SaveChangesAsync();
            return vencidos.Count;
        }

        public async Task<Pago?> SelectByReserva(int reservaId)
        {
            return await context.Pagos.FirstOrDefaultAsync(p => p.ReservaId == reservaId);
        }
    }
}
