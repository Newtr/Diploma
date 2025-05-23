using System.ComponentModel.DataAnnotations;

namespace EvacProject.GENERAL.Entity
{
    public class HelpMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentFullName { get; set; } = string.Empty;

        [Required]
        public DateTime SentAt { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Required]
        public string MessageText { get; set; } = string.Empty;

        [Required]
        public string TelegramChatId { get; set; } = string.Empty;
    }
}
