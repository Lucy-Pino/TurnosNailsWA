using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public interface IClienteRepositorio : IRepositorio<Cliente> //hereda de IRepositorio con la entidad Cliente
    {
        Task<List<ClienteListadoDTO>> ListaCliente();
        Task<Cliente?> SelectByTelefono(string telefono);
    }
}
