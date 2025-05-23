using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvacProject.GENERAL.Entity
{
    [Table("faculties")]
    public class Faculty
    {
        [Key]
        public int FacultyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        public ICollection<Student> Students { get; set; }
    }
}