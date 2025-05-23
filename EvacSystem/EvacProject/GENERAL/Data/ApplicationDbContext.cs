using EvacProject.GENERAL.Entity;
using Microsoft.EntityFrameworkCore;

namespace EvacProject.GENERAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<FormOfStudy> FormsOfStudy { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AcademicDegree> AcademicDegrees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        
            modelBuilder.Entity<Admin>().ToTable("admins");
            modelBuilder.Entity<Student>().ToTable("students");
            
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.TelegramChatId)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}