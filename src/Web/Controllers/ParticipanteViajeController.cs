using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipanteViajeController : ControllerBase
    {
        private readonly IParticipanteViajeService _service;

        // Inyectamos el servicio que acabamos de crear
        public ParticipanteViajeController(IParticipanteViajeService service)
        {
            _service = service;
        }

        // POST: api/ParticipanteViaje (Alta / Invitar)
        [HttpPost]
        public IActionResult Add([FromBody] ParticipanteViajeCreateRequest Request)
        {
            try
            {
                var resultado = _service.RegistrarParticipante(Request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/ParticipanteViaje/viaje/5 (Listar los de un viaje)
        [HttpGet("viaje/{viajeId:int}")]
        public IActionResult GetPorViaje(int viajeId)
        {
            var participantes = _service.ObtenerPorViaje(viajeId);
            return Ok(participantes);
        }

        // GET: api/ParticipanteViaje (Listar todos los participantes)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var participantes = _service.ObtenerTodos();
            return Ok(participantes);
        }
        // DELETE: api/ParticipanteViaje/5 (Baja)
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.EliminarParticipante(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}