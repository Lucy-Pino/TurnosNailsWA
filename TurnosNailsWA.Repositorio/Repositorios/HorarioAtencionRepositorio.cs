using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public class HorarioAtencionRepositorio : Repositorio<HorarioAtencion>, IHorarioAtencionRepositorio
    {
        private readonly AppDbContext context;

        public HorarioAtencionRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<HorarioAtencion>> ListaHorarioAtencion()
        {
            return await
            context.HorariosAtencion
                        .OrderBy(h => h.DiaSemana)
                        .ThenBy(h => h.HoraInicio)
                        .ToListAsync();
        }

        public async Task<List<HorarioAtencion>> SelectActivosPorDia(int diaSemana)
        {
            return await
            context.HorariosAtencion
                        .Where(h => h.DiaSemana == diaSemana && h.Activo)
                        .OrderBy(h => h.HoraInicio)
                        .ToListAsync();
        }

        public async Task<bool> ExisteSuperposicion(int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idExcluir = null)
        {
            var consulta = context.HorariosAtencion
                        .Where(h => h.DiaSemana == diaSemana
                                 && h.Activo
                                 && horaInicio < h.HoraFin
                                 && h.HoraInicio < horaFin);

            if (idExcluir != null)
            {
                consulta = consulta.Where(h => h.Id != idExcluir.Value);
            }

            return await consulta.AnyAsync();
        }
    }
}
