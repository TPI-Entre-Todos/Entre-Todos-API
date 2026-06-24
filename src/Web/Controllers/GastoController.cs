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
        public IActionResult Post([FromBody] GastoRequest dto)
        {
            try
            {
                var resultado = _gastoService.CrearGasto(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("viaje/{viajeId}")]
        public IActionResult GetPorViaje(int viajeId)
        {
            try
            {
                var gastos = _gastoService.ObtenerGastosPorViaje(viajeId);
                return Ok(gastos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _gastoService.EliminarGasto(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}