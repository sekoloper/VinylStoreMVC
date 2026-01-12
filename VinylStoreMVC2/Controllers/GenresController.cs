using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylStoreMVC.Data;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Controllers
{
    /// <summary>
    /// Контроллер для управления музыкальными жанрами в магазине виниловых пластинок.
    /// Предоставляет стандартные CRUD-операции для сущности Genre.
    /// </summary>
    public class GenresController : Controller
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GenresController"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения для доступа к данным жанров.</param>
        public GenresController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отображает список всех музыкальных жанров.
        /// </summary>
        /// <returns>Представление со списком всех жанров в системе.</returns>
        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.ToListAsync());
        }

        /// <summary>
        /// Отображает детальную информацию о конкретном жанре.
        /// </summary>
        /// <param name="id">Идентификатор жанра для отображения деталей.</param>
        /// <returns>
        /// Представление с детальной информацией о жанре, если он найден.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        /// <summary>
        /// Отображает форму для создания нового музыкального жанра.
        /// </summary>
        /// <returns>Представление с формой для ввода данных нового жанра.</returns>
        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Обрабатывает отправку формы для создания нового жанра.
        /// </summary>
        /// <param name="genre">Данные нового жанра, связанные из формы.</param>
        /// <returns>
        /// При успешной валидации и сохранении данных перенаправляет на список жанров.
        /// При ошибках валидации возвращает форму с сообщениями об ошибках.
        /// </returns>
        // POST: Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        /// <summary>
        /// Отображает форму для редактирования существующего жанра.
        /// </summary>
        /// <param name="id">Идентификатор редактируемого жанра.</param>
        /// <returns>
        /// Представление с формой редактирования, предзаполненной данными жанра, если он найден.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        /// <summary>
        /// Обрабатывает отправку формы для обновления данных жанра.
        /// </summary>
        /// <param name="id">Идентификатор обновляемого жанра.</param>
        /// <param name="genre">Обновленные данные жанра, связанные из формы.</param>
        /// <returns>
        /// При успешном обновлении перенаправляет на список жанров.
        /// При несоответствии идентификаторов возвращает NotFound.
        /// При ошибках валидации возвращает форму с сообщениями об ошибках.
        /// При возникновении конфликта параллельного доступа обрабатывает исключение DbUpdateConcurrencyException.
        /// </returns>
        // POST: Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Genre genre)
        {
            if (id != genre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.Id))
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
            return View(genre);
        }

        /// <summary>
        /// Отображает форму подтверждения удаления жанра.
        /// </summary>
        /// <param name="id">Идентификатор жанра, предлагаемого к удалению.</param>
        /// <returns>
        /// Представление с подтверждением удаления, содержащее информацию о жанре, если он найден.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        /// <summary>
        /// Выполняет удаление жанра из базы данных после подтверждения.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого жанра.</param>
        /// <returns>
        /// Перенаправляет на список жанров после успешного удаления.
        /// Если жанр не найден, операция удаления пропускается и происходит перенаправление на список.
        /// </returns>
        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Проверяет существование жанра с указанным идентификатором в базе данных.
        /// </summary>
        /// <param name="id">Идентификатор жанра для проверки.</param>
        /// <returns>
        /// <c>true</c> если жанр с указанным идентификатором существует; в противном случае <c>false</c>.
        /// </returns>
        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}
