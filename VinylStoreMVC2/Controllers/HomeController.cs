using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VinylStoreMVC.Models;

namespace VinylStoreMVC.Controllers
{
    /// <summary>
    /// Основной контроллер приложения для управления общими страницами.
    /// Обрабатывает главную страницу, политику конфиденциальности и ошибки.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Отображает главную страницу приложения.
        /// </summary>
        /// <returns>Представление главной страницы магазина виниловых пластинок.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Отображает страницу с политикой конфиденциальности.
        /// </summary>
        /// <returns>Представление страницы политики конфиденциальности.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Отображает страницу ошибки при возникновении исключений в приложении.
        /// Не кэшируется для отображения актуальной информации об ошибке.
        /// </summary>
        /// <returns>
        /// Представление страницы ошибки с моделью <see cref="ErrorViewModel"/>,
        /// содержащей идентификатор запроса для диагностики.
        /// </returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
