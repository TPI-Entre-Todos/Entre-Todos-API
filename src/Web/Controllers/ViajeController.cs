using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ViajeController : ControllerBase
    {
        private readonly IViajeService _viajeService;

        public ViajeController(IViajeService viajeService)
        {
            _viajeService = viajeService;
        }

        [HttpPost]
        public IActionResult Add([FromBody] ViajeRequest request)
        {
            int userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = _viajeService.Add(request, userIdClaim);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var viajes = _viajeService.Get();
            return Ok(viajes);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var viaje = _viajeService.GetById(id);
            if (viaje == null)
                return NotFound();

            return Ok(viaje);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _viajeService.Delete(id);
            return NoContent();
        }
    }
}