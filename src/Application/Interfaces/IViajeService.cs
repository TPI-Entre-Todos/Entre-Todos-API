using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IViajeService
    {
        ViajeDto Add(ViajeRequest request, int userIdClaim);
        List<ViajeDto> Get(int userId, bool esAdmin);
        ViajeDto? GetById(int id, int userId, bool esAdmin);

        void Delete(int id);


    }
}
