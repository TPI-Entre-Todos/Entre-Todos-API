using Application.Interfaces;
using Application.Models.Requests;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {

        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // El alta de usuarios la hace Cognito. Acá sólo se devuelve el perfil local,
        // que se crea automáticamente la primera vez que llega un token válido.
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var usuario = _usuarioService.GetOrCreateFromToken(User);
            return Ok(usuario);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Get()
        {
            var usuarios = _usuarioService.GetAll();
            return Ok(usuarios);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var usuario = _usuarioService.GetById(id);
            return Ok(usuario);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, UsuarioRequest request)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            var updated = _usuarioService.Update(id, request, usuarioId, esAdmin);
            return Ok(updated);
        }

        // El archivo llega como multipart/form-data en el campo "archivo".
        [HttpPost("{id:int}/avatar")]
        [RequestSizeLimit(3 * 1024 * 1024)]
        public async Task<IActionResult> SubirAvatar(int id, IFormFile archivo, CancellationToken cancellationToken)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo.");

            var (usuarioId, esAdmin) = ObtenerIdentidad();

            using var contenido = archivo.OpenReadStream();

            var actualizado = await _usuarioService.ActualizarAvatarAsync(
                id, contenido, archivo.Length, usuarioId, esAdmin, cancellationToken);

            return Ok(actualizado);
        }

        [HttpPatch("{id:int}/rol")]
        [Authorize(Roles = "Admin")]
        public IActionResult CambiarRol(int id, [FromBody] Rol rol)
        {
            var updated = _usuarioService.CambiarRol(id, rol);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var (usuarioId, esAdmin) = ObtenerIdentidad();
            _usuarioService.Delete(id, usuarioId, esAdmin);
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
