using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application
{
    public interface IGastoService
    {
        Task<GastoDto> CrearGastoAsync(GastoCreateDto dto);
        Task<List<GastoDto>> ObtenerGastosPorViajeAsync(int viajeId);
        Task EliminarGastoAsync(int id);
    }
}