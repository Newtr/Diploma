using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvacProject.GENERAL.Entity
{
    [Table("forms_of_study")]
    public class FormOfStudy
    {
        [Key]
        public int FormOfStudyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        public ICollection<Student> Students { get; set; }
    }
}