using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvacProject.GENERAL.Entity
{
    [Table("departments")]
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        public ICollection<Admin> Admins { get; set; }
    }
}