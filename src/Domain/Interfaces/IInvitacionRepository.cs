using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IInvitacionRepository
    {
        List<Invitacion> GetAll();
        Invitacion? GetById(int id);
        Invitacion? GetByToken(string token);
        Invitacion Add(Invitacion entity);
        void Update(Invitacion entity);
        void Delete(int id);
    }
}
