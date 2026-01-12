using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylStoreMVC.Data;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Controllers
{
    /// <summary>
    /// Контроллер для управления поставщиками виниловых пластинок.
    /// Предоставляет стандартные CRUD-операции для сущности Supplier.
    /// Управляет информацией о компаниях и контактных лицах, поставляющих товары в магазин.
    /// </summary>
    public class SuppliersController : Controller
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SuppliersController"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных приложения для работы с данными поставщиков.</param>
        public SuppliersController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отображает список всех поставщиков.
        /// </summary>
        /// <returns>Представление со списком всех поставщиков в системе.</returns>
        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Suppliers.ToListAsync());
        }

        /// <summary>
        /// Отображает детальную информацию о конкретном поставщике.
        /// </summary>
        /// <param name="id">Идентификатор поставщика для отображения деталей.</param>
        /// <returns>
        /// Представление с детальной информацией о поставщике, если он найден.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        /// <summary>
        /// Отображает форму для создания нового поставщика.
        /// </summary>
        /// <returns>Представление с формой для ввода данных нового поставщика.</returns>
        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Обрабатывает отправку формы для создания нового поставщика.
        /// </summary>
        /// <param name="supplier">Данные нового поставщика, связанные из формы.</param>
        /// <returns>
        /// При успешной валидации и сохранении данных перенаправляет на список поставщиков.
        /// При ошибках валидации возвращает форму с сообщениями об ошибках.
        /// </returns>
        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ContactPerson,PhoneNumber")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        /// <summary>
        /// Отображает форму для редактирования существующего поставщика.
        /// </summary>
        /// <param name="id">Идентификатор редактируемого поставщика.</param>
        /// <returns>
        /// Представление с формой редактирования, предзаполненной данными поставщика, если он найден.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        /// <summary>
        /// Обрабатывает отправку формы для обновления данных поставщика.
        /// </summary>
        /// <param name="id">Идентификатор обновляемого поставщика.</param>
        /// <param name="supplier">Обновленные данные поставщика, связанные из формы.</param>
        /// <returns>
        /// При успешном обновлении перенаправляет на список поставщиков.
        /// При несоответствии идентификаторов возвращает NotFound.
        /// При ошибках валидации возвращает форму с сообщениями об ошибках.
        /// При возникновении конфликта параллельного доступа обрабатывает исключение DbUpdateConcurrencyException.
        /// </returns>
        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContactPerson,PhoneNumber")] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.Id))
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
            return View(supplier);
        }

        /// <summary>
        /// Отображает форму подтверждения удаления поставщика.
        /// </summary>
        /// <param name="id">Идентификатор поставщика, предлагаемого к удалению.</param>
        /// <returns>
        /// Представление с подтверждением удаления, содержащее информацию о поставщике, если он найден.
        /// В противном случае возвращает результат NotFound.
        /// </returns>
        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        /// <summary>
        /// Выполняет удаление поставщика из базы данных после подтверждения.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого поставщика.</param>
        /// <returns>
        /// Перенаправляет на список поставщиков после успешного удаления.
        /// Если поставщик не найден, операция удаления пропускается и происходит перенаправление на список.
        /// </returns>
        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Проверяет существование поставщика с указанным идентификатором в базе данных.
        /// </summary>
        /// <param name="id">Идентификатор поставщика для проверки.</param>
        /// <returns>
        /// <c>true</c> если поставщик с указанным идентификатором существует; в противном случае <c>false</c>.
        /// </returns>
        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }
    }
}
