using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public interface IPagoRepositorio : IRepositorio<Pago> //hereda de IRepositorio con la entidad Pago
    {
        Task<List<PagoListadoDTO>> ListaPago();
        Task<List<PagoListadoDTO>> ListaPendientesVerificacion();
        Task<List<PagoListadoDTO>> ListaPendientesAclaracion();
        Task<int> ActualizarVencidos(int minutos);
        Task<Pago?> SelectByReserva(int reservaId);
    }
}
