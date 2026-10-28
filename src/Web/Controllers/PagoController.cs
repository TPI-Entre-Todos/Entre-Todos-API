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
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpPost("simple")]
        public IActionResult PagarSimple(PagoSimpleRequest request)
        {

            var result = _pagoService.PagarSimple(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

        }

        [HttpPost("multiple")]
        public IActionResult PagarMultiple(PagoMultipleRequest request)
        {
            var result = _pagoService.PagarMultiple(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

        }

        [HttpGet]
        public IActionResult Get()
        {
            var pagos = _pagoService.GetAll();
            return Ok(pagos);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var pago = _pagoService.GetById(id);
            return Ok(pago);
        }

        [HttpPut("simple/{id:int}")]
        public IActionResult ActualizarSimple(int id, PagoSimpleRequest request)
        {
            var updated = _pagoService.ActualizarSimple(id, request);
            return Ok(updated);
            

        }

        [HttpPut("multiple/{id:int}")]
        public IActionResult ActualizarMultiple(int id, PagoMultipleRequest request)
        {

            var updated = _pagoService.ActualizarMultiple(id, request);
            return Ok(updated);

        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool esAdmin = User.IsInRole("Admin");
            _pagoService.Delete(id, usuarioId, esAdmin);
            return NoContent();
        }

        [HttpGet("viaje/{viajeId:int}")]
        public IActionResult GetByViajeId(int viajeId)
        {
            var pagos = _pagoService.GetByViajeId(viajeId);
            return Ok(pagos);
        }

        [HttpGet("participante/{participanteId:int}")]
        public IActionResult GetByParticipanteId(int participanteId)
        {
            var pagos = _pagoService.GetByParticipanteId(participanteId);
            return Ok(pagos);
        }
    }
}
