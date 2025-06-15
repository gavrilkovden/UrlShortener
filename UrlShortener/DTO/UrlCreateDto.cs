using System.ComponentModel.DataAnnotations;

namespace UrlShortener.DTO
{
    public class UrlCreateDto
    {
        [Required]
        [Url(ErrorMessage = "Введите корректный URL.")]
        public string OriginalUrl { get; set; } = string.Empty;
    }

}
