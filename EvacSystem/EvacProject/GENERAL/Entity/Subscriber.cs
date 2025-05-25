namespace EvacProject.GENERAL.Entity
{
    public class Subscriber
    {
        public string StudentNumber { get; set; }
        public string TelegramChatId { get; set; }
    }

    public class SubscribersData
    {
        public List<Subscriber> Subscribers { get; set; } = new List<Subscriber>();
    }
}