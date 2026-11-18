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
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var result = _pagoService.PagarSimple(request, usuarioId, esAdmin);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("multiple")]
        public IActionResult PagarMultiple(PagoMultipleRequest request)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var result = _pagoService.PagarMultiple(request, usuarioId, esAdmin);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // Devuelve los pagos de todos los viajes: sólo para administradores.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Get()
        {
            var pagos = _pagoService.GetAll();
            return Ok(pagos);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var pago = _pagoService.GetById(id, usuarioId, esAdmin);
            return Ok(pago);
        }

        [HttpPut("simple/{id:int}")]
        public IActionResult ActualizarSimple(int id, PagoSimpleRequest request)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var updated = _pagoService.ActualizarSimple(id, request, usuarioId, esAdmin);
            return Ok(updated);
        }

        [HttpPut("multiple/{id:int}")]
        public IActionResult ActualizarMultiple(int id, PagoMultipleRequest request)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var updated = _pagoService.ActualizarMultiple(id, request, usuarioId, esAdmin);
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
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var pagos = _pagoService.GetByViajeId(viajeId, usuarioId, esAdmin);
            return Ok(pagos);
        }

        [HttpGet("participante/{participanteId:int}")]
        public IActionResult GetByParticipanteId(int participanteId)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var pagos = _pagoService.GetByParticipanteId(participanteId, usuarioId, esAdmin);
            return Ok(pagos);
        }
    }
}
