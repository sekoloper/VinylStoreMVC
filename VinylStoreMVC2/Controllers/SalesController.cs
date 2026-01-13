using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VinylStoreMVC.Data;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Controllers
{
    /// <summary>
    /// Контроллер для управления продажами виниловых пластинок.
    /// Предоставляет CRUD-операции для сущности Sale с поддержкой управления связанными записями о продажах.
    /// Обрабатывает логику продаж, включая обновление запасов и управление связями между продажами и пластинками.
    /// </summary>
    public class SalesController : Controller
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SalesController"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения для работы с данными продаж.</param>
        public SalesController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отображает список всех продаж с включенными связанными данными.
        /// </summary>
        /// <returns>Представление со списком всех продаж, включая информацию о пластинках и их исполнителей.</returns>
        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.Sales
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist);
            return View(await applicationContext.ToListAsync());
        }

        /// <summary>
        /// Отображает детальную информацию о конкретной продаже.
        /// Включает дополнительные данные о количестве и ценах проданных пластинок.
        /// </summary>
        /// <param name="id">Идентификатор продажи для отображения деталей.</param>
        /// <returns>
        /// Представление с детальной информацией о продаже, включая связанные пластинки и исполнители.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            ViewBag.RecordQuantities = await _context.SaleRecords
                .Where(sr => sr.SaleId == id)
                .ToDictionaryAsync(sr => sr.RecordId, sr => sr.Quantity);

            ViewBag.RecordPrices = await _context.SaleRecords
                .Where(sr => sr.SaleId == id)
                .ToDictionaryAsync(sr => sr.RecordId, sr => sr.Price);

            return View(sale);
        }

        /// <summary>
        /// Отображает форму для создания новой продажи.
        /// Фильтрует пластинки, доступные для продажи (с положительным количеством на складе).
        /// </summary>
        /// <returns>Представление с формой создания продажи, содержащей список доступных пластинок.</returns>
        // GET: Sales/Create
        public IActionResult Create()
        {
            var records = _context.Records
                .Where(r => r.StockQuantity > 0)
                .Include(r => r.Artist)
                .ToList();

            var recordList = records.Select(r => new
            {
                Id = r.Id,
                Name = $"{r.Name} ({r.Artist.Name}) - Цена: {r.CurrentPrice} ₽, В наличии: {r.StockQuantity}"
            });

            ViewData["RecordIds"] = new MultiSelectList(recordList, "Id", "Name");
            return View();
        }

        /// <summary>
        /// Обрабатывает отправку формы для создания новой продажи.
        /// Проверяет наличие достаточного количества пластинок на складе.
        /// Обновляет остатки на складе и статусы пластинок после создания продажи.
        /// </summary>
        /// <param name="sale">Данные продажи, связанные из формы.</param>
        /// <param name="selectedRecords">Массив идентификаторов выбранных пластинок.</param>
        /// <param name="recordQuantities">Словарь с количествами для каждой выбранной пластинки.</param>
        /// <returns>
        /// При успешной валидации и сохранении данных перенаправляет на список продаж.
        /// При ошибках валидации или недостаточном количестве товара возвращает форму с сообщениями об ошибках.
        /// </returns>
        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Date,InvoiceLink")] Sale sale,
            int[] selectedRecords,
            Dictionary<int, int> recordQuantities)
        {
            if (selectedRecords == null || selectedRecords.Length == 0)
            {
                ModelState.AddModelError("", "Необходимо выбрать хотя бы одну пластинку");
            }
            else
            {
                foreach (var recordId in selectedRecords)
                {
                    if (recordQuantities != null && recordQuantities.TryGetValue(recordId, out int quantity) && quantity > 0)
                    {
                        var record = await _context.Records.FindAsync(recordId);
                        if (record != null && quantity > record.StockQuantity)
                        {
                            ModelState.AddModelError($"recordQuantities[{recordId}]",
                                $"Недостаточно пластинок на складе. Доступно: {record.StockQuantity}");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError($"recordQuantities[{recordId}]", "Количество должно быть больше 0");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();

                if (selectedRecords != null && selectedRecords.Length > 0)
                {
                    foreach (var recordId in selectedRecords)
                    {
                        if (recordQuantities != null && recordQuantities.TryGetValue(recordId, out int quantity) && quantity > 0)
                        {
                            var record = await _context.Records.FindAsync(recordId);
                            if (record != null)
                            {
                                var saleRecord = new SaleRecord
                                {
                                    SaleId = sale.Id,
                                    RecordId = recordId,
                                    Quantity = quantity,
                                    Price = record.CurrentPrice
                                };
                                _context.SaleRecords.Add(saleRecord);

                                record.StockQuantity -= quantity;
                                if (record.StockQuantity < 0) record.StockQuantity = 0;

                                record.StatusId = record.StockQuantity > 0 ? 1 : 2;

                                _context.Update(record);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }

            var records = _context.Records
                .Where(r => r.StockQuantity > 0)
                .Include(r => r.Artist)
                .ToList();

            var recordList = records.Select(r => new
            {
                Id = r.Id,
                Name = $"{r.Name} ({r.Artist.Name}) - Цена: {r.CurrentPrice} ₽, В наличии: {r.StockQuantity}"
            });

            ViewData["RecordIds"] = new MultiSelectList(recordList, "Id", "Name", selectedRecords);
            return View(sale);
        }

        /// <summary>
        /// Отображает форму для редактирования существующей продажи.
        /// Загружает текущие выбранные пластинки и их количества.
        /// </summary>
        /// <param name="id">Идентификатор редактируемой продажи.</param>
        /// <returns>
        /// Представление с формой редактирования, предзаполненной данными продажи и выбранными пластинками.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            var selectedRecords = sale.Records.Select(r => r.Id).ToList();

            var allRecords = await _context.Records
                .Include(r => r.Artist)
                .ToListAsync();

            var recordList = allRecords.Select(r => new
            {
                Id = r.Id,
                Name = $"{r.Name} ({r.Artist.Name}) - Цена: {r.CurrentPrice} ₽, В наличии: {r.StockQuantity}"
            });

            ViewData["RecordIds"] = new MultiSelectList(recordList, "Id", "Name", selectedRecords);

            var currentQuantities = await _context.SaleRecords
                .Where(sr => sr.SaleId == id)
                .ToDictionaryAsync(sr => sr.RecordId, sr => sr.Quantity);

            ViewBag.CurrentQuantities = currentQuantities;

            return View(sale);
        }

        /// <summary>
        /// Обрабатывает отправку формы для обновления данных продажи.
        /// Выполняет сложную логику обновления связей с пластинками и корректировки остатков на складе.
        /// </summary>
        /// <param name="id">Идентификатор обновляемой продажи.</param>
        /// <param name="sale">Обновленные данные продажи, связанные из формы.</param>
        /// <param name="selectedRecords">Массив идентификаторов выбранных пластинок после редактирования.</param>
        /// <param name="recordQuantities">Словарь с обновленными количествами для каждой пластинки.</param>
        /// <returns>
        /// При успешном обновлении перенаправляет на список продаж.
        /// При несоответствии идентификаторов возвращает NotFound.
        /// При ошибках валидации или недостаточном количестве товара возвращает форму с сообщениями об ошибках.
        /// При возникновении конфликта параллельного доступа обрабатывает исключение DbUpdateConcurrencyException.
        /// </returns>
        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Date,InvoiceLink")] Sale sale,
            int[] selectedRecords,
            Dictionary<int, int> recordQuantities)
        {
            if (id != sale.Id)
            {
                return NotFound();
            }

            if (selectedRecords == null || selectedRecords.Length == 0)
            {
                ModelState.AddModelError("", "Необходимо выбрать хотя бы одну пластинку");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSale = await _context.Sales
                        .Include(s => s.Records)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (existingSale == null)
                    {
                        return NotFound();
                    }

                    var oldQuantities = new Dictionary<int, int>();
                    foreach (var record in existingSale.Records)
                    {
                        var saleRecord = await _context.SaleRecords
                            .FirstOrDefaultAsync(sr => sr.SaleId == id && sr.RecordId == record.Id);
                        if (saleRecord != null)
                        {
                            oldQuantities[record.Id] = saleRecord.Quantity;
                        }
                    }

                    _context.Entry(existingSale).CurrentValues.SetValues(sale);

                    var currentRecordIds = existingSale.Records.Select(r => r.Id).ToList();
                    var recordsToRemove = currentRecordIds.Except(selectedRecords ?? Array.Empty<int>()).ToList();
                    var recordsToAdd = (selectedRecords ?? Array.Empty<int>()).Except(currentRecordIds).ToList();

                    foreach (var recordId in recordsToRemove)
                    {
                        var record = existingSale.Records.FirstOrDefault(r => r.Id == recordId);
                        if (record != null)
                        {
                            existingSale.Records.Remove(record);
                            if (oldQuantities.TryGetValue(recordId, out int oldQty))
                            {
                                record.StockQuantity += oldQty;
                                record.StatusId = record.StockQuantity > 0 ? 1 : 2;
                                _context.Update(record);
                            }
                        }
                    }

                    if (selectedRecords != null)
                    {
                        foreach (var recordId in recordsToAdd)
                        {
                            if (recordQuantities != null && recordQuantities.TryGetValue(recordId, out int qty) && qty > 0)
                            {
                                var record = await _context.Records.FindAsync(recordId);
                                if (record != null && qty > record.StockQuantity)
                                {
                                    ModelState.AddModelError($"recordQuantities[{recordId}]",
                                        $"Недостаточно пластинок на складе. Доступно: {record.StockQuantity}");

                                    var allRecords = await _context.Records
                                        .Include(r => r.Artist)
                                        .ToListAsync();

                                    var recordList = allRecords.Select(r => new
                                    {
                                        Id = r.Id,
                                        Name = $"{r.Name} ({r.Artist.Name}) - Цена: {r.CurrentPrice} ₽, В наличии: {r.StockQuantity}"
                                    });

                                    ViewData["RecordIds"] = new MultiSelectList(recordList, "Id", "Name", selectedRecords);
                                    return View(sale);
                                }
                            }
                        }

                        foreach (var recordId in recordsToAdd)
                        {
                            var record = await _context.Records.FindAsync(recordId);
                            if (record != null)
                            {
                                existingSale.Records.Add(record);
                                if (recordQuantities != null && recordQuantities.TryGetValue(recordId, out int qty) && qty > 0)
                                {
                                    record.StockQuantity -= qty;
                                    if (record.StockQuantity < 0) record.StockQuantity = 0;

                                    record.StatusId = record.StockQuantity > 0 ? 1 : 2;

                                    _context.Update(record);
                                }
                            }
                        }
                    }

                    if (selectedRecords != null)
                    {
                        foreach (var recordId in selectedRecords.Intersect(currentRecordIds))
                        {
                            if (recordQuantities != null && recordQuantities.TryGetValue(recordId, out int newQty) && newQty > 0)
                            {
                                var record = await _context.Records.FindAsync(recordId);
                                if (record != null)
                                {
                                    if (oldQuantities.TryGetValue(recordId, out int oldQty))
                                    {
                                        int diff = newQty - oldQty;

                                        if (diff < 0 && Math.Abs(diff) > record.StockQuantity)
                                        {
                                            ModelState.AddModelError($"recordQuantities[{recordId}]",
                                                $"Недостаточно пластинок на складе для уменьшения. Доступно: {record.StockQuantity}");

                                            var allRecords = await _context.Records
                                                .Include(r => r.Artist)
                                                .ToListAsync();

                                            var recordList = allRecords.Select(r => new
                                            {
                                                Id = r.Id,
                                                Name = $"{r.Name} ({r.Artist.Name}) - Цена: {r.CurrentPrice} ₽, В наличии: {r.StockQuantity}"
                                            });

                                            ViewData["RecordIds"] = new MultiSelectList(recordList, "Id", "Name", selectedRecords);
                                            return View(sale);
                                        }

                                        if (diff != 0)
                                        {
                                            record.StockQuantity -= diff;
                                            if (record.StockQuantity < 0) record.StockQuantity = 0;

                                            record.StatusId = record.StockQuantity > 0 ? 1 : 2;

                                            _context.Update(record);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    if (selectedRecords != null)
                    {
                        foreach (var recordId in selectedRecords)
                        {
                            if (recordQuantities != null && recordQuantities.TryGetValue(recordId, out int qty) && qty > 0)
                            {
                                var saleRecord = await _context.SaleRecords
                                    .FirstOrDefaultAsync(sr => sr.SaleId == id && sr.RecordId == recordId);

                                if (saleRecord != null)
                                {
                                    saleRecord.Quantity = qty;
                                    _context.Update(saleRecord);
                                }
                                else
                                {
                                    var record = await _context.Records.FindAsync(recordId);
                                    int price = record?.CurrentPrice ?? 0;

                                    _context.SaleRecords.Add(new SaleRecord
                                    {
                                        SaleId = id,
                                        RecordId = recordId,
                                        Quantity = qty,
                                        Price = price
                                    });
                                }
                            }
                        }
                    }

                    foreach (var recordId in recordsToRemove)
                    {
                        var saleRecord = await _context.SaleRecords
                            .FirstOrDefaultAsync(sr => sr.SaleId == id && sr.RecordId == recordId);
                        if (saleRecord != null)
                        {
                            _context.SaleRecords.Remove(saleRecord);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.Id))
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

            var allRecordsForError = await _context.Records
                .Include(r => r.Artist)
                .ToListAsync();

            var recordListForError = allRecordsForError.Select(r => new
            {
                Id = r.Id,
                Name = $"{r.Name} ({r.Artist.Name}) - Цена: {r.CurrentPrice} ₽, В наличии: {r.StockQuantity}"
            });

            ViewData["RecordIds"] = new MultiSelectList(recordListForError, "Id", "Name", selectedRecords);
            return View(sale);
        }

        /// <summary>
        /// Отображает форму подтверждения удаления продажи.
        /// </summary>
        /// <param name="id">Идентификатор продажи, предлагаемой к удалению.</param>
        /// <returns>
        /// Представление с подтверждением удаления, содержащее информацию о продаже и связанных пластинках.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            ViewBag.RecordQuantities = await _context.SaleRecords
                .Where(sr => sr.SaleId == id)
                .ToDictionaryAsync(sr => sr.RecordId, sr => sr.Quantity);

            return View(sale);
        }

        /// <summary>
        /// Выполняет удаление продажи из базы данных после подтверждения.
        /// Восстанавливает количество пластинок на складе и обновляет их статусы.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой продажи.</param>
        /// <returns>
        /// Перенаправляет на список продаж после успешного удаления.
        /// Если продажа не найдена, операция пропускается.
        /// </returns>
        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale != null)
            {
                var saleRecords = await _context.SaleRecords
                    .Where(sr => sr.SaleId == id)
                    .ToListAsync();

                foreach (var saleRecord in saleRecords)
                {
                    var record = await _context.Records.FindAsync(saleRecord.RecordId);
                    if (record != null)
                    {
                        record.StockQuantity += saleRecord.Quantity;

                        record.StatusId = record.StockQuantity > 0 ? 1 : 2;

                        _context.Update(record);
                    }
                }

                await _context.SaveChangesAsync();

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Проверяет существование продажи с указанным идентификатором в базе данных.
        /// </summary>
        /// <param name="id">Идентификатор продажи для проверки.</param>
        /// <returns>
        /// <c>true</c> если продажа с указанным идентификатором существует; в противном случае <c>false</c>.
        /// </returns>
        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}