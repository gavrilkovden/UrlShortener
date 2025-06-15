using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.DTO;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlsController : ControllerBase
    {
        private readonly UrlShortenerDbContext _context;

        public UrlsController(UrlShortenerDbContext context)
        {
            _context = context;
        }

        // GET: api/urls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrlEntry>>> GetUrls()
        {
            return await _context.UrlEntries.OrderByDescending(u => u.CreatedAt).ToListAsync();
        }

        // GET: /s/{shortCode}
        [HttpGet("/s/{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var urlEntry = await _context.UrlEntries.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (urlEntry == null)
                return NotFound("Короткая ссылка не найдена.");

            urlEntry.Clicks++;
            await _context.SaveChangesAsync();

            return Redirect(urlEntry.OriginalUrl);
        }

        // GET: api/urls/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UrlEntry>> GetUrlById(int id)
        {
            var urlEntry = await _context.UrlEntries.FindAsync(id);
            if (urlEntry == null) return NotFound();
            return urlEntry;
        }

        // POST: api/urls
        [HttpPost]
        public async Task<ActionResult<UrlEntry>> CreateUrl([FromBody] UrlCreateDto input)
        {
            var newEntry = new UrlEntry
            {
                OriginalUrl = input.OriginalUrl,
                ShortCode = await GenerateUniqueCodeAsync(),
                CreatedAt = DateTime.UtcNow,
                Clicks = 0
            };

            _context.UrlEntries.Add(newEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUrls), new { id = newEntry.Id }, newEntry);
        }

        // PUT: api/urls/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUrl(int id, [FromBody] UrlCreateDto updated)
        {
            var url = await _context.UrlEntries.FindAsync(id);
            if (url == null) return NotFound();

            url.OriginalUrl = updated.OriginalUrl;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/urls/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUrl(int id)
        {
            var url = await _context.UrlEntries.FindAsync(id);
            if (url == null) return NotFound();

            _context.UrlEntries.Remove(url);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Метод генерирует уникальный короткий код длиной 8 символов
        private async Task<string> GenerateUniqueCodeAsync()
        {
            // Допустимые символы для генерации кода: строчные, заглавные буквы и цифры
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();

            string code; 

            // Повторяем генерацию до тех пор, пока не получим уникальный код
            do
            {
                var buffer = new char[8];

                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = chars[random.Next(chars.Length)];

                code = new string(buffer);
            }
            // Проверяем, существует ли уже такая короткая ссылка в базе данных
            // Если да — повторяем генерацию
            while (await _context.UrlEntries.AnyAsync(e => e.ShortCode == code));

            return code;
        }

    }

}
