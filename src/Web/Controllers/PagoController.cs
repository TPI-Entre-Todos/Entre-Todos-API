using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
            try
            {
                var result = _pagoService.PagarSimple(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("multiple")]
        public IActionResult PagarMultiple(PagoMultipleRequest request)
        {
            try
            {
                var result = _pagoService.PagarMultiple(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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
            if (pago == null)
                return NotFound("Pago no encontrado");

            return Ok(pago);
        }

        [HttpPut("simple/{id:int}")]
        public IActionResult ActualizarSimple(int id, PagoSimpleRequest request)
        {
            try
            {
                var updated = _pagoService.ActualizarSimple(id, request);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("multiple/{id:int}")]
        public IActionResult ActualizarMultiple(int id, PagoMultipleRequest request)
        {
            try
            {
                var updated = _pagoService.ActualizarMultiple(id, request);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _pagoService.Delete(id);
            return NoContent();
        }

        [HttpGet("viaje/{viajeId:int}")]
        public IActionResult GetByViajeId(int viajeId)
        {
            if (viajeId <= 0)
                return BadRequest("ViajeId debe ser válido");

            var pagos = _pagoService.GetByViajeId(viajeId);
            return Ok(pagos);
        }

        [HttpGet("participante/{participanteId:int}")]
        public IActionResult GetByParticipanteId(int participanteId)
        {
            if (participanteId <= 0)
                return BadRequest("ParticipanteId debe ser válido");

            var pagos = _pagoService.GetByParticipanteId(participanteId);
            return Ok(pagos);
        }
    }
}
