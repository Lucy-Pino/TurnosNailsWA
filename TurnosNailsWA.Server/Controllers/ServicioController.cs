using Microsoft.AspNetCore.Mvc;
using TurnosNailsWA.Repositorio.Repositorios;
using TurnosNailsWA.Shared.DTO;
using ServicioEntity = TurnosNailsWA.BD.Datos.Entity.Servicio;

namespace TurnosNailsWA.Server.Controllers
{

    [ApiController]
    [Route("api/servicio")]

    public class ServicioController : ControllerBase
    {
        private readonly IServicioRepositorio repositorio;

        public ServicioController(IServicioRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet] // api/servicio
        public async Task<ActionResult<List<ServicioListadoDTO>>> Get()
        {
            var lista = await repositorio.ListaServicio();
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

        [HttpGet("{id:int}")] // api/servicio/5
        public async Task<ActionResult<ServicioDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }
            ServicioDTO DTO = new ServicioDTO();
            DTO.Id = entidad.Id;
            DTO.Nombre = entidad.Nombre;
            DTO.Categoria = entidad.Categoria;
            DTO.Descripcion = entidad.Descripcion;
            DTO.Duracion = entidad.Duracion;
            DTO.Precio = entidad.Precio;
            return Ok(DTO);
        }

        [HttpPost] // api/servicio
        public async Task<ActionResult<int>> Post(ServicioDTO DTO)
        {
            ServicioEntity entidad = new ServicioEntity();
            entidad.Nombre = DTO.Nombre;
            entidad.Categoria = DTO.Categoria;
            entidad.Descripcion = DTO.Descripcion;
            entidad.Duracion = DTO.Duracion;
            entidad.Precio = DTO.Precio;
            await repositorio.Insert(entidad);
            return Ok(entidad.Id);
        }

        [HttpPut("{id:int}")] // api/servicio/5
        public async Task<ActionResult<bool>> Put(int id, ServicioDTO DTO)
        {
            if (id != DTO.Id)
            {
                return BadRequest("Datos incorrectos, no se actualizó.");
            }
            ServicioEntity entidad = new ServicioEntity();
            entidad.Id = DTO.Id;
            entidad.Nombre = DTO.Nombre;
            entidad.Categoria = DTO.Categoria;
            entidad.Descripcion = DTO.Descripcion;
            entidad.Duracion = DTO.Duracion;
            entidad.Precio = DTO.Precio;
            var resultado = await repositorio.Update(entidad);
            return Ok(resultado);
        }
    }
}
