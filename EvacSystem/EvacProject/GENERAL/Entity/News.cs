
namespace EvacProject.GENERAL.Entity
{
    public class NewsItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
    }

    public class NewsData
    {
        public List<NewsItem> News { get; set; }
    }
}
