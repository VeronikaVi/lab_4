using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace lab4 
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicCatalogController : ControllerBase
    {
        private readonly MusicCatalogContext _context;

        public MusicCatalogController(MusicCatalogContext context)
        {
            _context = context;
        }

        // Добавление композиции
        [HttpPost]
        public ActionResult AddComposition([FromBody] MusicComposition composition)
        {
            _context.Catalog.Add(composition);
            _context.SaveChanges();
            return Ok();
        }

        // Получение списка всех композиций
        [HttpGet]
        public ActionResult<IEnumerable<MusicComposition>> GetAllCompositions()
        {
            var catalog = _context.Catalog.ToList();
            return Ok(catalog);
        }

        // Поиск композиции
        [HttpGet("{searchCriteria}")]
        public ActionResult<IEnumerable<MusicComposition>> SearchCompositions(string searchCriteria)
        {
            var searchResults = _context.Catalog
                .Where(comp =>
                    comp.Artist.ToLower().Contains(searchCriteria.ToLower()) ||
                    comp.Title.ToLower().Contains(searchCriteria.ToLower())
                )
                .ToList();

            return Ok(searchResults);
        }

        // Удаление композиции
        [HttpDelete("{artist}/{title}")]
        public ActionResult DeleteComposition(string artist, string title)
        {
            var compositionToDelete = _context.Catalog
                .FirstOrDefault(comp =>
                    comp.Artist.ToLower() == artist.ToLower() && comp.Title.ToLower() == title.ToLower()
                );

            if (compositionToDelete != null)
            {
                _context.Catalog.Remove(compositionToDelete);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound("Композиция не найдена в каталоге.");
            }
        }
    }
}
