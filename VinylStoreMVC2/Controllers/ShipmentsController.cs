using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VinylStoreMVC.Data;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Controllers
{
    /// <summary>
    /// Контроллер для управления поставками виниловых пластинок от поставщиков.
    /// Предоставляет CRUD-операции для сущности Shipment с поддержкой управления связанными записями о поставках.
    /// Обрабатывает логику пополнения запасов и управление связями между поставками и пластинками.
    /// </summary>
    public class ShipmentsController : Controller
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ShipmentsController"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения для работы с данными поставок.</param>
        public ShipmentsController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отображает список всех поставок с включенными связанными данными.
        /// </summary>
        /// <returns>Представление со списком всех поставок, включая информацию о поставщиках, пластинках и их артистах.</returns>
        // GET: Shipments
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.Shipments
                .Include(s => s.Supplier)
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist);
            return View(await applicationContext.ToListAsync());
        }

        /// <summary>
        /// Отображает детальную информацию о конкретной поставке.
        /// Включает дополнительные данные о количестве поставленных пластинок.
        /// </summary>
        /// <param name="id">Идентификатор поставки для отображения деталей.</param>
        /// <returns>
        /// Представление с детальной информацией о поставке, включая связанных поставщиков, пластинки и артистов.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Shipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .Include(s => s.Supplier)
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipment == null)
            {
                return NotFound();
            }

            ViewBag.RecordQuantities = await _context.ShipmentRecords
                .Where(sr => sr.ShipmentId == id)
                .ToDictionaryAsync(sr => sr.RecordId, sr => sr.Quantity);

            return View(shipment);
        }

        /// <summary>
        /// Отображает форму для создания новой поставки.
        /// Предоставляет списки поставщиков и пластинок для выбора в форме.
        /// </summary>
        /// <returns>Представление с формой создания поставки, содержащей выпадающие списки поставщиков и пластинок.</returns>
        // GET: Shipments/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            ViewData["RecordIds"] = new MultiSelectList(_context.Records, "Id", "Name");
            return View();
        }

        /// <summary>
        /// Обрабатывает отправку формы для создания новой поставки.
        /// Обновляет остатки на складе и статусы пластинок после создания поставки.
        /// </summary>
        /// <param name="shipment">Данные поставки, связанные из формы.</param>
        /// <param name="selectedRecords">Массив идентификаторов выбранных пластинок.</param>
        /// <param name="recordQuantities">Словарь с количествами для каждой выбранной пластинки.</param>
        /// <returns>
        /// При успешной валидации и сохранении данных перенаправляет на список поставок.
        /// При ошибках валидации возвращает форму с сохраненными данными.
        /// </returns>
        // POST: Shipments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,SupplierId,Date,InvoiceLink")] Shipment shipment,
            int[] selectedRecords,
            Dictionary<int, int> recordQuantities)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shipment);
                await _context.SaveChangesAsync();

                if (selectedRecords != null && selectedRecords.Length > 0)
                {
                    foreach (var recordId in selectedRecords)
                    {
                        if (recordQuantities != null && recordQuantities.TryGetValue(recordId, out int quantity) && quantity > 0)
                        {
                            var shipmentRecord = new ShipmentRecord
                            {
                                ShipmentId = shipment.Id,
                                RecordId = recordId,
                                Quantity = quantity
                            };
                            _context.ShipmentRecords.Add(shipmentRecord);

                            var record = await _context.Records.FindAsync(recordId);
                            if (record != null)
                            {
                                record.StockQuantity += quantity;

                                record.StatusId = record.StockQuantity > 0 ? 1 : 2;

                                _context.Update(record);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", shipment.SupplierId);
            ViewData["RecordIds"] = new MultiSelectList(_context.Records, "Id", "Name", selectedRecords);
            return View(shipment);
        }

        /// <summary>
        /// Отображает форму для редактирования существующей поставки.
        /// Загружает текущие выбранные пластинки и их количества.
        /// </summary>
        /// <param name="id">Идентификатор редактируемой поставки.</param>
        /// <returns>
        /// Представление с формой редактирования, предзаполненной данными поставки и выбранными пластинками.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Shipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (shipment == null)
            {
                return NotFound();
            }

            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", shipment.SupplierId);

            var selectedRecords = shipment.Records.Select(r => r.Id).ToList();
            ViewData["RecordIds"] = new MultiSelectList(_context.Records, "Id", "Name", selectedRecords);

            var currentQuantities = await _context.ShipmentRecords
                .Where(sr => sr.ShipmentId == id)
                .ToDictionaryAsync(sr => sr.RecordId, sr => sr.Quantity);

            ViewBag.CurrentQuantities = currentQuantities;

            return View(shipment);
        }

        /// <summary>
        /// Обрабатывает отправку формы для обновления данных поставки.
        /// Выполняет сложную логику обновления связей с пластинками и корректировки остатков на складе.
        /// </summary>
        /// <param name="id">Идентификатор обновляемой поставки.</param>
        /// <param name="shipment">Обновленные данные поставки, связанные из формы.</param>
        /// <param name="selectedRecords">Массив идентификаторов выбранных пластинок после редактирования.</param>
        /// <param name="recordQuantities">Словарь с обновленными количествами для каждой пластинки.</param>
        /// <returns>
        /// При успешном обновлении перенаправляет на список поставок.
        /// При несоответствии идентификаторов возвращает NotFound.
        /// При ошибках валидации возвращает форму с сообщениями об ошибках.
        /// При возникновении конфликта параллельного доступа обрабатывает исключение DbUpdateConcurrencyException.
        /// </returns>
        // POST: Shipments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,SupplierId,Date,InvoiceLink")] Shipment shipment,
            int[] selectedRecords,
            Dictionary<int, int> recordQuantities)
        {
            if (id != shipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingShipment = await _context.Shipments
                        .Include(s => s.Records)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (existingShipment == null)
                    {
                        return NotFound();
                    }

                    var oldQuantities = new Dictionary<int, int>();
                    foreach (var record in existingShipment.Records)
                    {
                        var shipmentRecord = await _context.ShipmentRecords
                            .FirstOrDefaultAsync(sr => sr.ShipmentId == id && sr.RecordId == record.Id);
                        if (shipmentRecord != null)
                        {
                            oldQuantities[record.Id] = shipmentRecord.Quantity;
                        }
                    }

                    _context.Entry(existingShipment).CurrentValues.SetValues(shipment);

                    var currentRecordIds = existingShipment.Records.Select(r => r.Id).ToList();
                    var recordsToRemove = currentRecordIds.Except(selectedRecords ?? Array.Empty<int>()).ToList();
                    var recordsToAdd = (selectedRecords ?? Array.Empty<int>()).Except(currentRecordIds).ToList();

                    foreach (var recordId in recordsToRemove)
                    {
                        var record = existingShipment.Records.FirstOrDefault(r => r.Id == recordId);
                        if (record != null)
                        {
                            existingShipment.Records.Remove(record);
                            if (oldQuantities.TryGetValue(recordId, out int oldQty))
                            {
                                record.StockQuantity -= oldQty;
                                if (record.StockQuantity < 0) record.StockQuantity = 0;

                                record.StatusId = record.StockQuantity > 0 ? 1 : 2;

                                _context.Update(record);
                            }
                        }
                    }

                    if (selectedRecords != null)
                    {
                        foreach (var recordId in recordsToAdd)
                        {
                            var record = await _context.Records.FindAsync(recordId);
                            if (record != null)
                            {
                                existingShipment.Records.Add(record);
                                if (recordQuantities != null && recordQuantities.TryGetValue(recordId, out int qty) && qty > 0)
                                {
                                    record.StockQuantity += qty;

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
                                        if (diff != 0)
                                        {
                                            record.StockQuantity += diff;
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
                                var shipmentRecord = await _context.ShipmentRecords
                                    .FirstOrDefaultAsync(sr => sr.ShipmentId == id && sr.RecordId == recordId);

                                if (shipmentRecord != null)
                                {
                                    shipmentRecord.Quantity = qty;
                                    _context.Update(shipmentRecord);
                                }
                                else
                                {
                                    _context.ShipmentRecords.Add(new ShipmentRecord
                                    {
                                        ShipmentId = id,
                                        RecordId = recordId,
                                        Quantity = qty
                                    });
                                }
                            }
                        }
                    }

                    foreach (var recordId in recordsToRemove)
                    {
                        var shipmentRecord = await _context.ShipmentRecords
                            .FirstOrDefaultAsync(sr => sr.ShipmentId == id && sr.RecordId == recordId);
                        if (shipmentRecord != null)
                        {
                            _context.ShipmentRecords.Remove(shipmentRecord);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipmentExists(shipment.Id))
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
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", shipment.SupplierId);
            ViewData["RecordIds"] = new MultiSelectList(_context.Records, "Id", "Name", selectedRecords);
            return View(shipment);
        }

        /// <summary>
        /// Отображает форму подтверждения удаления поставки.
        /// </summary>
        /// <param name="id">Идентификатор поставки, предлагаемой к удалению.</param>
        /// <returns>
        /// Представление с подтверждением удаления, содержащее информацию о поставке и связанных пластинках.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Shipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .Include(s => s.Supplier)
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipment == null)
            {
                return NotFound();
            }

            ViewBag.RecordQuantities = await _context.ShipmentRecords
                .Where(sr => sr.ShipmentId == id)
                .ToDictionaryAsync(sr => sr.RecordId, sr => sr.Quantity);

            return View(shipment);
        }

        /// <summary>
        /// Выполняет удаление поставки из базы данных после подтверждения.
        /// Корректирует количество пластинок на складе и обновляет их статусы.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой поставки.</param>
        /// <returns>
        /// Перенаправляет на список поставок после успешного удаления.
        /// Если поставка не найдена, операция пропускается.
        /// </returns>
        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Records)
                    .ThenInclude(r => r.Artist)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment != null)
            {
                var shipmentRecords = await _context.ShipmentRecords
                    .Where(sr => sr.ShipmentId == id)
                    .ToListAsync();

                foreach (var shipmentRecord in shipmentRecords)
                {
                    var record = await _context.Records.FindAsync(shipmentRecord.RecordId);
                    if (record != null)
                    {
                        record.StockQuantity -= shipmentRecord.Quantity;
                        if (record.StockQuantity < 0) record.StockQuantity = 0;

                        record.StatusId = record.StockQuantity > 0 ? 1 : 2;

                        _context.Update(record);
                    }
                }

                await _context.SaveChangesAsync();

                _context.Shipments.Remove(shipment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Проверяет существование поставки с указанным идентификатором в базе данных.
        /// </summary>
        /// <param name="id">Идентификатор поставки для проверки.</param>
        /// <returns>
        /// <c>true</c> если поставка с указанным идентификатором существует; в противном случае <c>false</c>.
        /// </returns>
        private bool ShipmentExists(int id)
        {
            return _context.Shipments.Any(e => e.Id == id);
        }
    }
}