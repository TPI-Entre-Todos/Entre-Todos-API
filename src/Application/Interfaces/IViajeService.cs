using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IViajeService
    {
        ViajeDto Add(ViajeRequest request);
        List<ViajeDto> Get();
        ViajeDto? GetById(int id);
        void Delete(int id);
    }
}
