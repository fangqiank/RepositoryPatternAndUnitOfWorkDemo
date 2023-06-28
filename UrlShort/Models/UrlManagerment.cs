namespace UrlShort.Models
{
    public class UrlManagerment
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;

    }
}
