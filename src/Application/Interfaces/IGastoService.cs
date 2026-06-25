using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IGastoService
    {
        // Creación por tipo de división
        GastoDto CrearIgualitario(GastoIgualitarioRequest dto, int userId, bool esAdmin);
        GastoDto CrearPorPorcentaje(GastoPorPorcentajeRequest dto, int userId, bool esAdmin);
        GastoDto CrearPersonalizado(GastoPersonalizadoRequest dto, int userId, bool esAdmin);

        // Consulta
        List<GastoDto> ObtenerGastosPorViaje(int viajeId, int userId, bool esAdmin);
        GastoDto? ObtenerGastoPorId(int id, int userId, bool esAdmin);

        // Modificación y baja
        GastoDto ActualizarGasto(int id, GastoConDetallesRequest dto, int userId, bool esAdmin);
        void EliminarGasto(int id, int userId, bool esAdmin);
    }
}
