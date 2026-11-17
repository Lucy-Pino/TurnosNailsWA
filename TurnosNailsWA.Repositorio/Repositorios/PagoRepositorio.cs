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
            int totalActualizados = 0;

            // 1. Regla 4: Nunca debe existir una reserva confirmada con pago pendiente de verificación o de aclaración.
            // Si la reserva ya fue confirmada, el pago asociado debe asegurarse en 'Acreditado'.
            var confirmadosDesfasados = await context.Pagos
                .Where(p => p.Reserva.Estado == "Reserva Confirmada"
                         && (p.Estado == "Pendiente de aclaración" || p.Estado == "Pendiente de verificación"))
                .ToListAsync();

            foreach (var pago in confirmadosDesfasados)
            {
                pago.Estado = "Acreditado";
                totalActualizados++;
            }

            // 2. Regla 3: Si la reserva fue cancelada, el pago pendiente asociado debe asegurarse en 'Cancelado'.
            var canceladosDesfasados = await context.Pagos
                .Where(p => p.Reserva.Estado == "Cancelada"
                         && (p.Estado == "Pendiente de aclaración" || p.Estado == "Pendiente de verificación"))
                .ToListAsync();

            foreach (var pago in canceladosDesfasados)
            {
                pago.Estado = "Cancelado";
                totalActualizados++;
            }

            // 3. Regla 1: Al cumplirse los 30 minutos, solamente el pago pasa a 'Pendiente de aclaración',
            // siempre que la reserva siga en 'Pendiente de verificación'.
            var vencidos = await context.Pagos
                .Where(p => p.Estado == "Pendiente de verificación"
                         && p.Reserva.Estado == "Pendiente de verificación"
                         && p.FechaComprobanteAdjuntado <= limite)
                .ToListAsync();

            foreach (var pago in vencidos)
            {
                pago.Estado = "Pendiente de aclaración";
                totalActualizados++;
            }

            if (totalActualizados > 0)
            {
                await context.SaveChangesAsync();
            }

            return totalActualizados;
        }

        public async Task<Pago?> SelectByReserva(int reservaId)
        {
            return await context.Pagos.FirstOrDefaultAsync(p => p.ReservaId == reservaId);
        }
    }
}
