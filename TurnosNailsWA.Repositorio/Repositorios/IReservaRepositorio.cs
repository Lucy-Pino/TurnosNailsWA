using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public interface IReservaRepositorio : IRepositorio<Reserva> //hereda de IRepositorio con la entidad Reserva
    {
        Task<List<ReservaListadoDTO>> ListaReserva();
        Task<Reserva> InsertReservaCompleta(Reserva reserva, List<Turno> turnos);
        Task<Reserva?> SelectByCodigo(string codigo);
    }
}
