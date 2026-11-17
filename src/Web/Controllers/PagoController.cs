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
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            await _pagoService.DeleteAsync(id, usuarioId, esAdmin, ct);
            return NoContent();
        }

        // ─── Comprobante ───────────────────────────────────────────────────────────
        // El archivo llega como multipart/form-data en el campo "archivo".
        [HttpPost("{id:int}/comprobante")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> SubirComprobante(int id, IFormFile archivo, CancellationToken ct)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest(new { message = "No se recibió ningún archivo." });

            var (usuarioId, esAdmin) = ObtenerIdentidad();
            using var contenido = archivo.OpenReadStream();
            var resultado = await _pagoService.SubirComprobanteAsync(id, contenido, archivo.Length, usuarioId, esAdmin, ct);
            return Ok(resultado);
        }

        // Devuelve una URL firmada de S3, válida por 15 minutos, para ver o descargar el comprobante.
        [HttpGet("{id:int}/comprobante")]
        public IActionResult GetComprobante(int id)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var url = _pagoService.ObtenerUrlComprobante(id, usuarioId, esAdmin);
            return Ok(new { url });
        }

        private (int usuarioId, bool esAdmin) ObtenerIdentidad()
        {
            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return (usuarioId, User.IsInRole("Admin"));
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
