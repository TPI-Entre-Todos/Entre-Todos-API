using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
