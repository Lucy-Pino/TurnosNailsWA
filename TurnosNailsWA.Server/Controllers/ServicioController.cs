using Microsoft.AspNetCore.Mvc;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Server.Controllers
{

    [ApiController]
    [Route("api/servicio")]

    public class ServicioController : ControllerBase
    {
        private readonly AppDbContext context;

        public ServicioController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet] // api/servicio
        public async Task<ActionResult<List<Servicio>>> Get()
        {
            var lista = await context.Servicios.ToListAsync();
            if (lista == null)
            {
                return NotFound("No se encontraron elementos de la lista, VERIFICAR.");
            }
            else if (lista.Count == 0)
            {
                return NotFound("Lista sin registros.");
            }
            return Ok(lista);
        }
    }
}
