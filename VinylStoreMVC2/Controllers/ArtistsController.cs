using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylStoreMVC.Data;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Controllers
{
    /// <summary>
    /// Контроллер для управления артистами в магазине виниловых пластинок.
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
        /// Отображает список всех артистов.
        /// </summary>
        /// <returns>Представление со списком артистов.</returns>
        // GET: Artists
        public async Task<IActionResult> Index()
        {
            return View(await _context.Artists.ToListAsync());
        }

        /// <summary>
        /// Отображает детальную информацию об артисте.
        /// </summary>
        /// <param name="id">Идентификатор артиста.</param>
        /// <returns>
        /// Представление с деталями артиста, если он найден.
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
        /// Отображает форму для создания нового артиста.
        /// </summary>
        /// <returns>Представление с формой создания артиста.</returns>
        // GET: Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Обрабатывает данные формы для создания нового артиста.
        /// </summary>
        /// <param name="artist">Данные артиста из формы.</param>
        /// <returns>
        /// Перенаправляет на список артистов при успешном создании.
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
        /// Отображает форму для редактирования существующего артиста.
        /// </summary>
        /// <param name="id">Идентификатор редактируемого артиста.</param>
        /// <returns>
        /// Представление с формой редактирования, если артист найден.
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
        /// Обрабатывает данные формы для обновления артиста.
        /// </summary>
        /// <param name="id">Идентификатор артиста.</param>
        /// <param name="artist">Обновленные данные артиста.</param>
        /// <returns>
        /// Перенаправляет на список артистов при успешном обновлении.
        /// Возвращает форму с ошибками валидации, если модель невалидна.
        /// Возвращает NotFound, если идентификаторы не совпадают или артист не найден.
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
        /// Отображает форму подтверждения удаления артиста.
        /// </summary>
        /// <param name="id">Идентификатор артиста для удаления.</param>
        /// <returns>
        /// Представление с подтверждением удаления, если артист найден.
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
        /// Выполняет удаление артиста из базы данных.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого артиста.</param>
        /// <returns>Перенаправляет на список артистов после удаления.</returns>
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
        /// Проверяет существование артиста с указанным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор артиста для проверки.</param>
        /// <returns>
        /// <c>true</c> если артист существует; в противном случае <c>false</c>.
        /// </returns>
        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}
