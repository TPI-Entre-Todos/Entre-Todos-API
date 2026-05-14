using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {

        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public IActionResult Add(UsuarioRequest request)
        {
            if (request == null)
                return BadRequest();

            var result = _usuarioService.Add(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var usuarios = _usuarioService.GetAll();
            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var usuario = _usuarioService.GetById(id);
            if (usuario == null)
                return NotFound();
            return Ok(usuario);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, UsuarioRequest request)
        {
            if (request.Nombre == null && request.Email == null && request.Password == null)
                return BadRequest();

            var updated = _usuarioService.Update(id, request);
            if (updated == null)
                return NotFound();

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