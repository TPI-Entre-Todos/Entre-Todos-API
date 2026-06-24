using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application;
using Application.Models;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GastoController : ControllerBase
    {
        // 👇 Cambiamos GastoService por IGastoService
        private readonly IGastoService _gastoService;

        // 👇 Acá también cambiamos el parámetro por IGastoService
        public GastoController(IGastoService gastoService)
        {
            _gastoService = gastoService;
        }

       
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GastoCreateDto dto)
        {
            var resultado = await _gastoService.CrearGastoAsync(dto);
            return Ok(resultado);
        }

        [HttpGet("viaje/{viajeId}")]
        public async Task<IActionResult> GetPorViaje(int viajeId)
        {
            var gastos = await _gastoService.ObtenerGastosPorViajeAsync(viajeId);
            return Ok(gastos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _gastoService.EliminarGastoAsync(id);
            return NoContent();
        }
    }
}
