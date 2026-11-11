using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public interface IServicioRepositorio : IRepositorio<Servicio> //hereda de IRepositorio con la entidad Servicio
    {
        Task<List<ServicioListadoDTO>> ListaServicio();
    }
}
