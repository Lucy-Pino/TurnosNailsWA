using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public interface ITurnoRepositorio : IRepositorio<Turno> //hereda de IRepositorio con la entidad Turno
    {
        Task<List<TurnoListadoDTO>> ListaTurno();
        Task<List<TurnoListadoDTO>> ListaTurnoPorReserva(int reservaId);
        Task<List<Turno>> SelectOcupadosPorFecha(DateTime fecha);
    }
}
