using EvacProject.GENERAL.Data.GenericRepo;
using EvacProject.GENERAL.Entity;
using Microsoft.EntityFrameworkCore;

namespace EvacProject.GENERAL.Data.CustomRepo.Students
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Student> GetByStudentNumberAsync(string studentNumber)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);
        }
    }
}