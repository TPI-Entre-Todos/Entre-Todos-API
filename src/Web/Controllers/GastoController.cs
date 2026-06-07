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
            try
            {
                var resultado = await _gastoService.CrearGastoAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("viaje/{viajeId}")]
        public async Task<IActionResult> GetPorViaje(int viajeId)
        {
            try
            {
                var gastos = await _gastoService.ObtenerGastosPorViajeAsync(viajeId);
                return Ok(gastos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _gastoService.EliminarGastoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}