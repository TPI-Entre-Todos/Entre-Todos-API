using Application.Models;
using Application.Models.Requests;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IInvitacionService
    {
        List<InvitacionDto> GetAll();
        InvitacionDto? GetById(int id);
        InvitacionDto Add(InvitacionRequest request);
        void Delete(int id);
    }
}
