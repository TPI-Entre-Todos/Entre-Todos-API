using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IInvitacionRepository : IGenericRepository<Invitacion>
    {
        Invitacion? GetByToken(string token);
    }
}
