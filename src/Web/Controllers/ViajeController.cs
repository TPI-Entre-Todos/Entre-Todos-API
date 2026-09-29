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
            var (userId, _) = ObtenerIdentidad();
            bool esAdmin = User.IsInRole("Admin");

            var viajes = _viajeService.Get(userId, esAdmin);
            return Ok(viajes);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            var viaje = _viajeService.GetById(id, userId, esAdmin);
            return Ok(viaje);
        }

        // El archivo llega como multipart/form-data en el campo "archivo".
        [HttpPost("{id:int}/portada")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> SubirPortada(int id, IFormFile archivo, CancellationToken cancellationToken)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo.");

            var (usuarioId, esAdmin) = ObtenerIdentidad();

            using var contenido = archivo.OpenReadStream();

            var actualizado = await _viajeService.ActualizarPortadaAsync(
                id, contenido, archivo.Length, usuarioId, esAdmin, cancellationToken);

            return Ok(actualizado);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            await _viajeService.DeleteAsync(id, usuarioId, esAdmin, cancellationToken);
            return NoContent();
        }

        private (int usuarioId, bool esAdmin) ObtenerIdentidad()
        {
            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool esAdmin = User.IsInRole("Admin");
            return (usuarioId, esAdmin);
        }
    }
}
