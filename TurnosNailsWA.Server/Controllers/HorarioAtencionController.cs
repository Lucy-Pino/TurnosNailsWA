using Microsoft.AspNetCore.Mvc;
using TurnosNailsWA.BD.Datos.Entity;
using TurnosNailsWA.Repositorio.Repositorios;
using TurnosNailsWA.Shared.DTO;

namespace TurnosNailsWA.Server.Controllers
{
    [ApiController]
    [Route("api/horarioatencion")]
    public class HorarioAtencionController : ControllerBase
    {
        private readonly IHorarioAtencionRepositorio repositorio;

        public HorarioAtencionController(IHorarioAtencionRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet] //api/horarioatencion
        public async Task<ActionResult<List<HorarioAtencionDTO>>> Get()
        {
            var lista = await repositorio.ListaHorarioAtencion();
            if (lista == null)
            {
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");
            }
            else if (lista.Count == 0)
            {
                return NotFound("Lista sin registros.");
            }
            var listaDTO = lista
                            .Select(h => new HorarioAtencionDTO
                            {
                                Id = h.Id,
                                DiaSemana = h.DiaSemana,
                                HoraInicio = h.HoraInicio,
                                HoraFin = h.HoraFin,
                                Activo = h.Activo
                            })
                            .ToList();

            return Ok(listaDTO);
        }

        [HttpGet("{id:int}")]  //api/horarioatencion/5
        public async Task<ActionResult<HorarioAtencionDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }
            HorarioAtencionDTO DTO = new HorarioAtencionDTO();
            DTO.Id = entidad.Id;
            DTO.DiaSemana = entidad.DiaSemana;
            DTO.HoraInicio = entidad.HoraInicio;
            DTO.HoraFin = entidad.HoraFin;
            DTO.Activo = entidad.Activo;

            return Ok(DTO);
        }

        [HttpGet("dia/{diaSemana:int}")]  //api/horarioatencion/dia/1
        public async Task<ActionResult<List<HorarioAtencionDTO>>> GetByDia(int diaSemana)
        {
            if (diaSemana < 0 || diaSemana > 6)
            {
                return BadRequest("El día de la semana debe estar entre 0 (domingo) y 6 (sábado).");
            }
            var lista = await repositorio.SelectActivosPorDia(diaSemana);
            if (lista.Count == 0)
            {
                return NotFound("No hay horarios activos para ese día.");
            }
            var listaDTO = lista
                            .Select(h => new HorarioAtencionDTO
                            {
                                Id = h.Id,
                                DiaSemana = h.DiaSemana,
                                HoraInicio = h.HoraInicio,
                                HoraFin = h.HoraFin,
                                Activo = h.Activo
                            })
                            .ToList();

            return Ok(listaDTO);
        }

        [HttpPost] //api/horarioatencion
        public async Task<ActionResult<int>> Post(HorarioAtencionDTO DTO)
        {
            if (DTO.Activo)
            {
                var superpuesto = await repositorio.ExisteSuperposicion(DTO.DiaSemana, DTO.HoraInicio, DTO.HoraFin);
                if (superpuesto)
                {
                    return BadRequest("El bloque se superpone con otro horario de atención del mismo día.");
                }
            }
            HorarioAtencion entidad = new HorarioAtencion();
            entidad.DiaSemana = DTO.DiaSemana;
            entidad.HoraInicio = DTO.HoraInicio;
            entidad.HoraFin = DTO.HoraFin;
            entidad.Activo = DTO.Activo;

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        [HttpPut("{id:int}")] //api/horarioatencion/5
        public async Task<ActionResult<bool>> Put(int id, HorarioAtencionDTO DTO)
        {
            if (id != DTO.Id)
            {
                return BadRequest("Datos incorrectos, no se actualizó.");
            }
            if (DTO.Activo)
            {
                var superpuesto = await repositorio.ExisteSuperposicion(DTO.DiaSemana, DTO.HoraInicio, DTO.HoraFin, id);
                if (superpuesto)
                {
                    return BadRequest("El bloque se superpone con otro horario de atención del mismo día.");
                }
            }
            HorarioAtencion entidad = new HorarioAtencion();
            entidad.Id = DTO.Id;
            entidad.DiaSemana = DTO.DiaSemana;
            entidad.HoraInicio = DTO.HoraInicio;
            entidad.HoraFin = DTO.HoraFin;
            entidad.Activo = DTO.Activo;

            var resultado = await repositorio.Update(entidad);

            return Ok(resultado);
        }

        [HttpDelete("{id:int}")] //api/horarioatencion/5
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var resultado = await repositorio.Delete(id);
            if (!resultado)
            {
                return NotFound($"No existe el registro con id: {id}.");
            }

            return Ok(true);
        }
    }
}
