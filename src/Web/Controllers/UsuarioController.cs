using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Add(UsuarioRequest request)
        {
            var result = _usuarioService.Add(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
            var updated = _usuarioService.Update(id, request);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _usuarioService.Delete(id);
            return NoContent();
        }

    }
}