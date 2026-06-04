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

        [HttpPost]
        public IActionResult Add(InvitacionRequest request)
        {
            if (request == null)
                return BadRequest();

            var result = _invitacionService.Add(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{token}/aceptar")]
        public IActionResult AceptarInvitacion([FromRoute] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token inválido.");

            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(claim) || !int.TryParse(claim, out var usuarioId))
                return Unauthorized();

            try
            {
                var result = _invitacionService.AceptarInvitacion(token, usuarioId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{token}/rechazar")]
        public IActionResult RechazarInvitacion([FromRoute] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token inválido.");

            try
            {
                var result = _invitacionService.RechazarInvitacion(token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            var invitaciones = _invitacionService.GetAll();
            return Ok(invitaciones);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var invitacion = _invitacionService.GetById(id);
            if (invitacion == null)
                return NotFound();

            return Ok(invitacion);
        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _invitacionService.Delete(id);
            return NoContent();
        }
    }
}
