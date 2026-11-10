namespace TurnosNailsWA.Repositorio.Repositorios
{
    public interface IRepositorio<E> //interfaz generica para repositorio
    {
        Task<List<E>> Select();
    }
}