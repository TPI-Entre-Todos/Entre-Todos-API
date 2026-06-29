using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Models;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParticipanteViajeController : ControllerBase
    {
        private readonly IParticipanteViajeService _participanteViajeService;

        // Inyectamos el servicio que acabamos de crear
        public ParticipanteViajeController(IParticipanteViajeService participanteViajeService)
        {
            _participanteViajeService = participanteViajeService;
        }

        // POST: api/ParticipanteViaje (Alta / Invitar)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] ParticipanteViajeCreateRequest Request)
        {
            var resultado = _participanteViajeService.RegistrarParticipante(Request);
            return Ok(resultado);
        }

        // GET: api/ParticipanteViaje/viaje/5 (Listar los de un viaje de usario)
        [HttpGet("viaje/{viajeId:int}")]
        [Authorize(Roles = "User")]
        public IActionResult GetPorViaje(int viajeId)
        {
            int userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var participantes = _participanteViajeService.ObtenerPorViaje(viajeId, userIdClaim);
            return Ok(participantes);
        }

        // GET: api/ParticipanteViaje/viaje/Admin/5 (Listar los de un viaje como admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("viaje/Admin{viajeId:int}")]
        public IActionResult GetPorViajeAdmin(int viajeId)
        {
            var participantes = _participanteViajeService.ObtenerPorViajeAdmin(viajeId);
            return Ok(participantes);
        }
        // GET: api/ParticipanteViaje (Listar todos los participantes)
        [HttpGet]
        public IActionResult GetAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();
            var esAdmin = User.IsInRole("Admin");
            var participantes = _participanteViajeService.ObtenerTodos(userId, esAdmin);
            return Ok(participantes);
        }
        // PATCH: api/ParticipanteViaje/5/organizador (Cambiar estado de organizador)
        [HttpPatch("{id:int}/organizador")]
        public IActionResult CambiarEsOrganizador(int id, [FromBody] bool esOrganizador)
        {
            int userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool esAdmin = User.IsInRole("Admin");
            var actualizado = _participanteViajeService.CambiarEsOrganizador(id, esOrganizador, userIdClaim, esAdmin);
            return Ok(actualizado);
        }

        // DELETE: api/ParticipanteViaje/5 (Baja)
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {

            _participanteViajeService.EliminarParticipante(id);
            return NoContent();
        }

    }
}