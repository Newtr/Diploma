using EvacProject.GENERAL.Data.GenericRepo;
using EvacProject.GENERAL.Entity;

namespace EvacProject.GENERAL.Data.CustomRepo.Admins
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context) { }
        
    }
}