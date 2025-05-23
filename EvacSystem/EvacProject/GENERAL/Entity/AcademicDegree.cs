using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvacProject.GENERAL.Entity
{
    [Table("academic_degrees")]
    public class AcademicDegree
    {
        [Key]
        public int AcademicDegreeId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        public ICollection<Admin> Admins { get; set; }
    }
}