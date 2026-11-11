using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Server.Controllers
{
    [ApiController] // Indica que esta clase es un controlador de API
    [Route("api/turno")] // Define la ruta base para las acciones del controlador
    public class TurnoController : ControllerBase
    {
        private readonly AppDbContext context; // Tengo que inyectar el dbcontext para poder usarlo en el controlador, para eso uso el constructor,  en un campo de la clase, en una propiedad interna de la clase

        public TurnoController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet] // api/turno
        public async Task<ActionResult<List<Turno>>> Get()
        {
            var lista = await context.Turnos.ToListAsync();
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

        //[HttpGet("listaturno")] // api/turno/listaturno
        //public async Task<ActionResult<List<TurnoDTO>>> Get()
        //{
        //    var turnos = await context.Turnos
        //        .Include(t => t.Cliente)
        //        .Include(t => t.Servicio)
        //        .ToListAsync();
        //    if (turnos == null || turnos.Count == 0)
        //    {
        //        return NotFound("No se encontraron turnos.");
        //    }
        //    return Ok(turnos);
        //}


        //[HttpGet("[{id:int}]")] // api/turno/5
        //public async Task<ActionResult<TurnoDTO>> GetById(int id)
        //{
        //    var turno = await context.Turnos.FindAsync(id);
        //    if (turno == null)
        //    {
        //        return NotFound("No se encontró el turno con el ID especificado.");
        //    }
        //    return Ok(turno);
        //}


        // TEMPORAL: el endpoint pertenece al modelo anterior (turno directo con cliente y adelanto).
        // Se rehace en la etapa de controladores con el flujo Reserva + Turno + Pago.
        //[HttpPost("reservar")]
        //public async Task<ActionResult<TurnoDTO>> ReservarTurno(TurnoDTO turnoDTO)
        //{
        //    var cliente = await context.Clientes.FindAsync(turnoDTO.IdCliente);

        //    if (cliente == null)
        //    {
        //        return NotFound("El cliente no existe.");
        //    }

        //    var servicio = await context.Servicios.FindAsync(turnoDTO.IdServicio);

        //    if (servicio == null)
        //    {
        //        return NotFound("El servicio no existe.");
        //    }

        //    var turnoExistente = await context.Turnos
        //        .FirstOrDefaultAsync(t =>
        //            t.Fecha == turnoDTO.Fecha &&
        //            t.Hora == turnoDTO.Hora);

        //    if (turnoExistente != null)
        //    {
        //        return BadRequest("El horario seleccionado no está disponible.");
        //    }

        //    var turno = new Turno
        //    {
        //        IdCliente = turnoDTO.IdCliente,
        //        IdServicio = turnoDTO.IdServicio,
        //        Fecha = turnoDTO.Fecha,
        //        Hora = turnoDTO.Hora,
        //        Estado = "Pendiente de confirmación",
        //        Adelanto = servicio.Precio * 0.30m
        //    };

        //    context.Turnos.Add(turno);
        //    await context.SaveChangesAsync();

        //    return Ok(turno);
        //}

        [HttpGet("fecha")]
        public async Task<ActionResult<List<TurnoDTO>>> ObtenerTurnosPorFecha(DateTime fecha)
        {
            var turnos = await context.Turnos
                .Where(t => t.Fecha == fecha)
                .ToListAsync();

            if (turnos == null || turnos.Count == 0)
            {
                return NotFound("No se encontraron turnos para esa fecha.");
            }
            return Ok(turnos);
        }

        //[HttpGet] // Lo etiquetamos mediante el MVC HTTPGET atribut, con un atributo que es un Get.. api/turno
        //public string Get() //metodo GET que devuelve 
        //{
        //    return "Hola desde el controlador de Turno";
        //}

        //[HttpGet] // api/turno
        //public async Task<ActionResult<List<Turno>>> Get()
        //{
        //    var lista = await context.Turnos.ToListAsync();
        //    if (lista == null)
        //    {
        //        return NotFound("No se encontraron elementos de la lista, VERIFICAR.");
        //    }
        //    else if (lista.Count == 0)
        //    {
        //        return NotFound("Lista sin registros.");
        //    }
        //    return Ok(lista);
        //}
    }
}
