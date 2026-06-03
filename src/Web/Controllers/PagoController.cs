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

        [HttpPost]
        public IActionResult Add(PagoRequest request)
        {
            if (request == null)
                return BadRequest("La solicitud no puede ser nula");

            try
            {
                // Dejamos que el servicio haga todo el trabajo sucio de validar
                var result = _pagoService.Add(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                // Si el servicio encuentra un error, lo atrapamos acá y devolvemos el mensaje real
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

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, PagoRequest request)
        {
            if (request == null)
                return BadRequest("La solicitud no puede ser nula");

            try
            {
                var updated = _pagoService.Update(id, request);
                if (updated == null)
                    return NotFound("Pago no encontrado");

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
