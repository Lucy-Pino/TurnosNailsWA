using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public class ServicioRepositorio : Repositorio<Servicio>, IServicioRepositorio
    {
        private readonly AppDbContext context;

        public ServicioRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ServicioListadoDTO>> ListaServicio()
        {
            var lista = await
            context.Servicios
                        .OrderBy(s => s.Nombre)
                        .Select(s => new ServicioListadoDTO
                        {
                            Id = s.Id,
                            Nombre = s.Nombre,
                            Categoria = s.Categoria,
                            Descripcion = s.Descripcion,
                            Duracion = s.Duracion,
                            Precio = s.Precio
                        })
                        .ToListAsync();
            return lista;
        }
    }
}
