using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IViajeService
    {
        ViajeDto Add(ViajeRequest request, int userIdClaim);
        List<ViajeDto> Get(int userId, bool esAdmin);
        ViajeDto? GetById(int id, int userId, bool esAdmin);

        Task DeleteAsync(int id, int usuarioAutenticadoId, bool esAdmin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reemplaza la foto de portada del viaje. Sólo puede hacerlo el organizador del
        /// viaje o un Admin.
        /// </summary>
        Task<ViajeDto> ActualizarPortadaAsync(
            int id,
            Stream contenido,
            long tamanio,
            int usuarioAutenticadoId,
            bool esAdmin,
            CancellationToken cancellationToken = default);
    }
}
