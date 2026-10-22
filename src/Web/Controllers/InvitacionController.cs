using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvitacionController : ControllerBase
    {
        private readonly IInvitacionService _invitacionService;

        public InvitacionController(IInvitacionService invitacionService)
        {
            _invitacionService = invitacionService;
        }

        // Invita a un email a un viaje (solo organizador o Admin). El invitador sale del token.
        [HttpPost]
        public IActionResult Add([FromBody] InvitacionRequest request)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var result = _invitacionService.Add(request, usuarioId, esAdmin);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // Las invitaciones enviadas al email del usuario logueado.
        [HttpGet("mias")]
        public IActionResult GetMias()
        {
            var (usuarioId, _) = ObtenerIdentidad();
            return Ok(_invitacionService.GetMias(usuarioId));
        }

        // Las invitaciones de un viaje (solo organizador o Admin). No incluyen el token.
        [HttpGet("viaje/{viajeId:int}")]
        public IActionResult GetPorViaje([FromRoute] int viajeId)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            return Ok(_invitacionService.GetPorViaje(viajeId, usuarioId, esAdmin));
        }

        // Todas las invitaciones del sistema, con tokens: solo Admin.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Get()
        {
            return Ok(_invitacionService.GetAll());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            return Ok(_invitacionService.GetById(id, usuarioId, esAdmin));
        }

        // Los errores los traduce GlobalExceptionHandlingMiddleware (problem+json con `detail`).
        [HttpPut("{token}/aceptar")]
        public IActionResult AceptarInvitacion([FromRoute] string token)
        {
            var (usuarioId, _) = ObtenerIdentidad();
            return Ok(_invitacionService.AceptarInvitacion(token, usuarioId));
        }

        [HttpPut("{token}/rechazar")]
        public IActionResult RechazarInvitacion([FromRoute] string token)
        {
            var (usuarioId, _) = ObtenerIdentidad();
            return Ok(_invitacionService.RechazarInvitacion(token, usuarioId));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            _invitacionService.Delete(id, usuarioId, esAdmin);
            return NoContent();
        }

        private (int usuarioId, bool esAdmin) ObtenerIdentidad()
        {
            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return (usuarioId, User.IsInRole("Admin"));
        }
    }
}
