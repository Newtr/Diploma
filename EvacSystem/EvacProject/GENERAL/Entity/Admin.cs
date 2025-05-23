using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvacProject.GENERAL.Entity
{
    [Table("admins")]
    public class Admin  : User
    {
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        
        public int AcademicDegreeId { get; set; }
        [ForeignKey("AcademicDegreeId")]
        public AcademicDegree AcademicDegree { get; set; }

        public DateTime? PrivilegeGranted { get; set; }

        public DateTime? PrivilegeExpiry { get; set; }
        
        public string? TelegramChatId { get; set; }
    }
}

