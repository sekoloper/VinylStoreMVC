using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylStoreMVC.Data;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Controllers
{
    /// <summary>
    /// Контроллер для управления исполнителями в магазине виниловых пластинок.
    /// Предоставляет CRUD-операции для сущности Artist.
    /// </summary>
    public class ArtistsController : Controller
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ArtistsController"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения.</param>
        public ArtistsController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отображает список всех испполнителей.
        /// </summary>
        /// <returns>Представление со списком исполнителей.</returns>
        // GET: Artists
        public async Task<IActionResult> Index()
        {
            return View(await _context.Artists.ToListAsync());
        }

        /// <summary>
        /// Отображает детальную информацию об исполнителе.
        /// </summary>
        /// <param name="id">Идентификатор исполнител.</param>
        /// <returns>
        /// Представление с деталями исполнителя, если он найден.
        /// В противном случае возвращает NotFound.
        /// </returns>
        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        /// <summary>
        /// Отображает форму для создания нового исполнителя.
        /// </summary>
        /// <returns>Представление с формой создания исполнителя.</returns>
        // GET: Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Обрабатывает данные формы для создания нового исполнителя.
        /// </summary>
        /// <param name="artist">Данные исполнителя из формы.</param>
        /// <returns>
        /// Перенаправляет на список исполнителей при успешном создании.
        /// Возвращает форму с ошибками валидации, если модель невалидна.
        /// </returns>
        // POST: Artists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        /// <summary>
        /// Отображает форму для редактирования существующего исполнителя.
        /// </summary>
        /// <param name="id">Идентификатор редактируемого исполнителя.</param>
        /// <returns>
        /// Представление с формой редактирования, если исполнитель найден.
        /// В противном случае возвращает NotFound.
        /// </returns>
        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        /// <summary>
        /// Обрабатывает данные формы для обновления исполнителя.
        /// </summary>
        /// <param name="id">Идентификатор исполнителя.</param>
        /// <param name="artist">Обновленные данные исполнителя.</param>
        /// <returns>
        /// Перенаправляет на список исполнителей при успешном обновлении.
        /// Возвращает форму с ошибками валидации, если модель невалидна.
        /// Возвращает NotFound, если идентификаторы не совпадают или исполнитель не найден.
        /// </returns>
        // POST: Artists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        /// <summary>
        /// Отображает форму подтверждения удаления исполнителя.
        /// </summary>
        /// <param name="id">Идентификатор исполнителя для удаления.</param>
        /// <returns>
        /// Представление с подтверждением удаления, если исполнитель найден.
        /// В противном случае возвращает NotFound.
        /// </returns>
        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        /// <summary>
        /// Выполняет удаление исполнителя из базы данных.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого исполнителя.</param>
        /// <returns>Перенаправляет на список исполнителей после удаления.</returns>
        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Проверяет существование исполнителя с указанным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор исполнителя для проверки.</param>
        /// <returns>
        /// <c>true</c> если исполнитель существует; в противном случае <c>false</c>.
        /// </returns>
        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}
