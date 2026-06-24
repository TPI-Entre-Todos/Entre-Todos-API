using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application
{
    public interface IGastoService
    {
        GastoDto CrearGasto(GastoRequest dto);
        List<GastoDto> ObtenerGastosPorViaje(int viajeId);
        void EliminarGasto(int id);
    }
}