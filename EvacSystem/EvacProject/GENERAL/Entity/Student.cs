using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvacProject.GENERAL.Entity
{
    [Table("students")]
    public class Student : User
    {
        [Required]
        [StringLength(8)]
        public string StudentNumber { get; set; }
        
        public int FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculty Faculty { get; set; }
        
        public int FormOfStudyId { get; set; }
        [ForeignKey("FormOfStudyId")]
        public FormOfStudy FormOfStudy { get; set; }

        public DateTime? AdmissionDate { get; set; }

        public DateTime? TicketIssueDate { get; set; }

        public DateTime? TicketExpiryDate { get; set; }
    
        public string? TelegramChatId { get; set; }
        
        [MaxLength(50)]
        public string CurrentState { get; set; } 
        [MaxLength(10)]
        public string SelectedCampus { get; set; }
    }
}