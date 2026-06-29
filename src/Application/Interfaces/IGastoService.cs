using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IGastoService
    {
        // ─── Creación ─────────────────────────────────────────────────────────────
        GastoDto CrearIgualitario(GastoIgualitarioRequest dto, int userId, bool esAdmin);
        GastoDto CrearPorPorcentaje(GastoPorPorcentajeRequest dto, int userId, bool esAdmin);
        GastoDto CrearPersonalizado(GastoPersonalizadoRequest dto, int userId, bool esAdmin);

        // ─── Consulta ─────────────────────────────────────────────────────────────
        List<GastoDto> ObtenerTodos(int userId, bool esAdmin);
        List<GastoDto> ObtenerGastosPorViaje(int viajeId, int userId, bool esAdmin);
        GastoDto? ObtenerGastoPorId(int id, int userId, bool esAdmin);

        // ─── Actualización ────────────────────────────────────────────────────────
        GastoDto ActualizarIgualitario(int id, GastoIgualitarioRequest dto, int userId, bool esAdmin);
        GastoDto ActualizarPorPorcentaje(int id, GastoPorPorcentajeRequest dto, int userId, bool esAdmin);
        GastoDto ActualizarPersonalizado(int id, GastoPersonalizadoRequest dto, int userId, bool esAdmin);

        // ─── Baja ─────────────────────────────────────────────────────────────────
        void EliminarGasto(int id, int userId, bool esAdmin);
    }
}
