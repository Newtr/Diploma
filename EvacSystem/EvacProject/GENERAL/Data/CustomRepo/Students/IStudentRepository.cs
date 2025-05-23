using EvacProject.GENERAL.Data.GenericRepo;
using EvacProject.GENERAL.Entity;

namespace EvacProject.GENERAL.Data.CustomRepo.Students
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student> GetByStudentNumberAsync(string studentNumber);
    }
}