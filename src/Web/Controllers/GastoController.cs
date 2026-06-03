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
        private readonly GastoService _gastoService;

        public GastoController(GastoService gastoService)
        {
            _gastoService = gastoService;
        }

        // POST: api/Gasto
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

        // GET: api/Gasto/viaje/5
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

        // DELETE: api/Gasto/5
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