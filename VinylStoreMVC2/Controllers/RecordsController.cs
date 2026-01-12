using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VinylStoreMVC.Data;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Controllers
{
    /// <summary>
    /// Контроллер для управления виниловыми пластинками в магазине.
    /// Предоставляет полный набор CRUD-операций для сущности Record с поддержкой связей с артистами, жанрами и статусами.
    /// </summary>
    public class RecordsController : Controller
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RecordsController"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения для работы с данными пластинок.</param>
        public RecordsController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отображает список всех виниловых пластинок с включенными связанными данными.
        /// </summary>
        /// <returns>Представление со списком всех пластинок, включая информацию об артистах, статусах и жанрах.</returns>
        // GET: Records
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.Records
                .Include(r => r.Artist)
                .Include(r => r.Status)
                .Include(r => r.Genres);
            return View(await applicationContext.ToListAsync());
        }

        /// <summary>
        /// Отображает детальную информацию о конкретной пластинке.
        /// </summary>
        /// <param name="id">Идентификатор пластинки для отображения деталей.</param>
        /// <returns>
        /// Представление с детальной информацией о пластинке, включая связанные данные об артисте, статусе и жанрах.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @record = await _context.Records
                .Include(r => r.Artist)
                .Include(r => r.Status)
                .Include(r => r.Genres)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        /// <summary>
        /// Отображает форму для создания новой виниловой пластинки.
        /// Подготавливает списки артистов и жанров для выбора в форме.
        /// </summary>
        /// <returns>Представление с формой создания пластинки, содержащее выпадающие списки для связанных данных.</returns>
        // GET: Records/Create
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name");
            ViewData["GenreIds"] = new MultiSelectList(_context.Genres, "Id", "Name");
            return View();
        }

        /// <summary>
        /// Обрабатывает отправку формы для создания новой пластинки.
        /// Обрабатывает множественный выбор жанров для пластинки.
        /// </summary>
        /// <param name="record">Данные пластинки, связанные из формы.</param>
        /// <param name="selectedGenres">Массив идентификаторов выбранных жанров.</param>
        /// <returns>
        /// При успешной валидации и сохранении данных перенаправляет на список пластинок.
        /// При ошибках валидации возвращает форму с сохраненными данными и выбранными жанрами.
        /// </returns>
        // POST: Records/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ArtistId,Year,Label,CatalogNumber,CurrentPrice,StockQuantity,StatusId")] Record @record, int[] selectedGenres)
        {
            if (ModelState.IsValid)
            {
                if (selectedGenres != null && selectedGenres.Length > 0)
                {
                    var genres = await _context.Genres
                        .Where(g => selectedGenres.Contains(g.Id))
                        .ToListAsync();
                    foreach (var genre in genres)
                    {
                        record.Genres.Add(genre);
                    }
                }
                _context.Add(@record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", @record.ArtistId);
            ViewData["GenreIds"] = new MultiSelectList(_context.Genres, "Id", "Name", selectedGenres);
            return View(@record);
        }

        /// <summary>
        /// Отображает форму для редактирования существующей пластинки.
        /// Загружает связанные жанры для предварительного выбора в форме.
        /// </summary>
        /// <param name="id">Идентификатор редактируемой пластинки.</param>
        /// <returns>
        /// Представление с формой редактирования, предзаполненной данными пластинки и выбранными жанрами.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Records/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @record = await _context.Records.Include(r => r.Genres).FirstOrDefaultAsync(r => r.Id == id);
            if (@record == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", @record.ArtistId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", @record.StatusId);
            var selectedGenreIds = record.Genres.Select(g => g.Id).ToList();
            ViewData["GenreIds"] = new MultiSelectList(_context.Genres, "Id", "Name", selectedGenreIds);
            return View(@record);
        }

        /// <summary>
        /// Обрабатывает отправку формы для обновления данных пластинки.
        /// Обрабатывает сложную логику обновления связанных жанров с добавлением и удалением.
        /// </summary>
        /// <param name="id">Идентификатор обновляемой пластинки.</param>
        /// <param name="record">Обновленные данные пластинки, связанные из формы.</param>
        /// <param name="selectedGenres">Массив идентификаторов выбранных жанров после редактирования.</param>
        /// <returns>
        /// При успешном обновлении перенаправляет на список пластинок.
        /// При несоответствии идентификаторов возвращает NotFound.
        /// При ошибках валидации возвращает форму с сообщениями об ошибках.
        /// При возникновении конфликта параллельного доска обрабатывает исключение DbUpdateConcurrencyException.
        /// </returns>
        // POST: Records/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArtistId,Year,Label,CatalogNumber,CurrentPrice,StockQuantity,StatusId")] Record @record, int[] selectedGenres)
        {
            if (id != @record.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRecord = await _context.Records
                        .Include(r => r.Genres)
                        .FirstOrDefaultAsync(r => r.Id == id);
                    _context.Entry(existingRecord).CurrentValues.SetValues(record);

                    if (selectedGenres != null && selectedGenres.Length > 0)
                    {
                        var currentGenreIds = existingRecord.Genres.Select(g => g.Id).ToList();
                        var genresToRemove = currentGenreIds.Except(selectedGenres).ToList();

                        foreach (var genreId in genresToRemove)
                        {
                            var genreToRemove = existingRecord.Genres.FirstOrDefault(g => g.Id == genreId);
                            if (genreToRemove != null)
                            {
                                existingRecord.Genres.Remove(genreToRemove);
                            }
                        }

                        var newGenreIds = selectedGenres.Except(currentGenreIds).ToList();
                        if (newGenreIds.Any())
                        {
                            var newGenres = await _context.Genres
                                .Where(g => newGenreIds.Contains(g.Id))
                                .ToListAsync();

                            foreach (var genre in newGenres)
                            {
                                existingRecord.Genres.Add(genre);
                            }
                        }
                    }
                    else
                    {
                        existingRecord.Genres.Clear();
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordExists(@record.Id))
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
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", @record.ArtistId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", @record.StatusId);
            ViewData["GenreIds"] = new MultiSelectList(_context.Genres, "Id", "Name", selectedGenres);
            return View(@record);
        }

        /// <summary>
        /// Отображает форму подтверждения удаления пластинки.
        /// </summary>
        /// <param name="id">Идентификатор пластинки, предлагаемой к удалению.</param>
        /// <returns>
        /// Представление с подтверждением удаления, содержащее информацию о пластинке и связанных данных.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Records/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @record = await _context.Records
                .Include(r => r.Artist)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        /// <summary>
        /// Выполняет удаление пластинки из базы данных после подтверждения.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой пластинки.</param>
        /// <returns>
        /// Перенаправляет на список пластинок после успешного удаления.
        /// Если пластинка не найдена, операция удаления пропускается.
        /// </returns>
        // POST: Records/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @record = await _context.Records.FindAsync(id);
            if (@record != null)
            {
                _context.Records.Remove(@record);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Проверяет существование пластинки с указанным идентификатором в базе данных.
        /// </summary>
        /// <param name="id">Идентификатор пластинки для проверки.</param>
        /// <returns>
        /// <c>true</c> если пластинка с указанным идентификатором существует; в противном случае <c>false</c>.
        /// </returns>
        private bool RecordExists(int id)
        {
            return _context.Records.Any(e => e.Id == id);
        }
    }
}
