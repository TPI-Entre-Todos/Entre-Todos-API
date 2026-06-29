using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ViajeRepository : GenericRepository<Viaje>, IViajeRepository
{


    public ViajeRepository(ApplicationContext context) : base(context)
    {
    }
    
}