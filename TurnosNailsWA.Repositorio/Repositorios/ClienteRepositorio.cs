using System;
using System.Collections.Generic;
using System.Text;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace TurnosNailsWA.Repositorio.Repositorios
{
    public class ClienteRepositorio : Repositorio<Cliente>, IClienteRepositorio
    {
        private readonly AppDbContext context;

        public ClienteRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ClienteListadoDTO>> ListaCliente()
        {
            var lista = await
            context.Clientes
                        .Select(c => new ClienteListadoDTO
                        {
                            Id = c.Id,
                            Nombre = c.Nombre,
                            Apellido = c.Apellido,
                            Telefono = c.Telefono
                        })
                        .ToListAsync();
            return lista;
        }

        public async Task<Cliente?> SelectByTelefono(string telefono)
        {
            return await context.Clientes.FirstOrDefaultAsync(c => c.Telefono == telefono);
        }
    }
}
