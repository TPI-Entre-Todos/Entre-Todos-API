using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IInvitacionRepository
    {
        List<Invitacion> GetAll();
        Invitacion? GetById(int id);
        Invitacion Add(Invitacion entity);
        void Delete(int id);
    }
}
