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
            if (!TelefonoHelper.ValidarYNormalizar(telefono, out var normalizado, out var error))
            {
                return BadRequest(error);
            }

            var entidad = await repositorio.SelectByTelefono(normalizado);
            if (entidad is null)
            {
                return NotFound($"No existe una clienta con el teléfono: {normalizado}.");
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
            if (string.IsNullOrWhiteSpace(DTO.Nombre))
            {
                return BadRequest("El nombre es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(DTO.Apellido))
            {
                return BadRequest("El apellido es obligatorio.");
            }

            if (!TelefonoHelper.ValidarYNormalizar(DTO.Telefono, out var telefono, out var error))
            {
                return BadRequest(error);
            }

            var existente = await repositorio.SelectByTelefono(telefono);
            if (existente is not null)
            {
                return BadRequest("Ya existe una clienta con ese teléfono.");
            }

            ClienteEntity entidad = new ClienteEntity();
            entidad.Nombre = DTO.Nombre.Trim();
            entidad.Apellido = DTO.Apellido.Trim();
            entidad.Telefono = telefono;
            await repositorio.Insert(entidad);
            return Ok(entidad.Id);
        }

        [HttpPut("{id:int}")] // api/cliente/5
        public async Task<ActionResult<bool>> Put(int id, ClienteDTO DTO)
        {
            if (id != DTO.Id)
            {
                return BadRequest("Datos incorrectos, no se actualizó.");
            }

            if (string.IsNullOrWhiteSpace(DTO.Nombre))
            {
                return BadRequest("El nombre es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(DTO.Apellido))
            {
                return BadRequest("El apellido es obligatorio.");
            }

            if (!TelefonoHelper.ValidarYNormalizar(DTO.Telefono, out var telefono, out var error))
            {
                return BadRequest(error);
            }

            var existente = await repositorio.SelectByTelefono(telefono);
            if (existente is not null && existente.Id != id)
            {
                return BadRequest("Ya existe una clienta con ese teléfono.");
            }

            ClienteEntity entidad = new ClienteEntity();
            entidad.Id = DTO.Id;
            entidad.Nombre = DTO.Nombre.Trim();
            entidad.Apellido = DTO.Apellido.Trim();
            entidad.Telefono = telefono;

            var resultado = await repositorio.Update(entidad);

            return Ok(resultado);
        }
    }
}