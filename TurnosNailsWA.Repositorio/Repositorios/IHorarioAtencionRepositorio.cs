using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos.Entity;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public interface IHorarioAtencionRepositorio : IRepositorio<HorarioAtencion> //hereda de IRepositorio con la entidad HorarioAtencion
    {
        Task<bool> ExisteSuperposicion(int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null);
        Task<List<HorarioAtencion>> ListaHorarioAtencion();
        Task<List<HorarioAtencion>> SelectActivosPorDia(int diaSemana);
    }
}
