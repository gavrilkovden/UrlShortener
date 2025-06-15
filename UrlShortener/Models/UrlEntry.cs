namespace UrlShortener.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UrlEntry
    {
        public int Id { get; set; }

        [Required]
        public string OriginalUrl { get; set; } = string.Empty;

        [Required]
        public string ShortCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int Clicks { get; set; }
    }

}
