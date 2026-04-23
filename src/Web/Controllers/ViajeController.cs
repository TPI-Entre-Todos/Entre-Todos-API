using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViajeController : ControllerBase
    {
        private readonly IViajeService _viajeService;

        public ViajeController(IViajeService viajeService)
        {
            _viajeService = viajeService;
        }

        [HttpPost]
        public IActionResult Add(Viaje viaje)
        {
            _viajeService.Add(viaje);
            return Ok();
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
            var viajes = _viajeService.GetById(id);
            return Ok(viajes);
        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _viajeService.Delete(id);
            return NoContent();
        }
    }
}