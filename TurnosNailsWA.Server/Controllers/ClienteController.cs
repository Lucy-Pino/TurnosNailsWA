using Microsoft.AspNetCore.Mvc;
using TurnosNailsWA.Repositorio.Repositorios;
using TurnosNailsWA.Shared.DTO;
using ClienteEntity = TurnosNailsWA.BD.Datos.Entity.Cliente;

namespace TurnosNailsWA.Server.Controllers
{
    [ApiController]
    [Route("api/cliente")]

    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepositorio repositorio;

        public ClienteController(IClienteRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet] // api/cliente
        public async Task<ActionResult<List<ClienteListadoDTO>>> Get()
        {
            var lista = await repositorio.ListaCliente();
            if (lista == null)
            {
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");
            }
            else if (lista.Count == 0)
            {
                return NotFound("Lista sin registros.");
            }
            return Ok(lista);
        }

        [HttpGet("{id:int}")] // api/cliente/5
        public async Task<ActionResult<ClienteDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }
            ClienteDTO DTO = new ClienteDTO();
            DTO.Id = entidad.Id;
            DTO.Nombre = entidad.Nombre;
            DTO.Apellido = entidad.Apellido;
            DTO.Telefono = entidad.Telefono;
            return Ok(DTO);
        }

        [HttpGet("telefono/{telefono}")] // api/cliente/telefono/3515548895
        public async Task<ActionResult<ClienteDTO>> GetByTelefono(string telefono)
        {
            var entidad = await repositorio.SelectByTelefono(NormalizarTelefono(telefono));
            if (entidad is null)
            {
                return NotFound($"No existe una clienta con el teléfono: {telefono}.");
            }
            ClienteDTO DTO = new ClienteDTO();
            DTO.Id = entidad.Id;
            DTO.Nombre = entidad.Nombre;
            DTO.Apellido = entidad.Apellido;
            DTO.Telefono = entidad.Telefono;
            return Ok(DTO);
        }

        [HttpPost] // api/cliente
        public async Task<ActionResult<int>> Post(ClienteDTO DTO)
        {
            var telefono = NormalizarTelefono(DTO.Telefono);
            var existente = await repositorio.SelectByTelefono(telefono);
            if (existente is not null)
            {
                return BadRequest("Ya existe una clienta con ese teléfono.");
            }
            ClienteEntity entidad = new ClienteEntity();
            entidad.Nombre = DTO.Nombre;
            entidad.Apellido = DTO.Apellido;
            entidad.Telefono = telefono;
            await repositorio.Insert(entidad);
            return Ok(entidad.Id);
        }

        private static string NormalizarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
            {
                return "";
            }
            return telefono
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(".", "");
        }
    }
}
