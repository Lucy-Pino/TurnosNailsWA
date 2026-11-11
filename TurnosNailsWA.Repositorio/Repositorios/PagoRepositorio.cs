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
                        .Where(p => p.Estado == "Pendiente de verificación")
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

        public async Task<Pago?> SelectByReserva(int reservaId)
        {
            return await context.Pagos.FirstOrDefaultAsync(p => p.ReservaId == reservaId);
        }
    }
}
