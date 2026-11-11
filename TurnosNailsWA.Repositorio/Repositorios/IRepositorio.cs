using System;
using System.Collections.Generic;
using System.Text;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public interface IRepositorio<E> where E : class //interfaz generica para repositorio
    {
        Task<bool> Delete(int id);
        Task<bool> Existe(E entity);
        Task<E> Insert(E entity);
        Task<List<E>> Select();
        Task<E?> SelectById(int id);
        Task<bool> Update(E entity);
    }
}
