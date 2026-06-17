using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        // GET: api/ParticipanteViaje/viaje/5 (Listar los de un viaje de usario)
        [HttpGet("viaje/{viajeId:int}")]
        public IActionResult GetPorViaje(int viajeId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            try
            {
                var participantes = _service.ObtenerPorViaje(viajeId, userId);
                return Ok(participantes);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/ParticipanteViaje/viaje/Admin/5 (Listar los de un viaje como admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("viaje/Admin{viajeId:int}")]
        public IActionResult GetPorViajeAdmin(int viajeId)
        {
            var participantes = _service.ObtenerPorViajeAdmin(viajeId);
            return Ok(participantes);
        }
        // GET: api/ParticipanteViaje (Listar todos los participantes)
        [HttpGet]
        public IActionResult GetAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();
            var esAdmin = User.IsInRole("Admin");
            var participantes = _service.ObtenerTodos(userId, esAdmin);
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