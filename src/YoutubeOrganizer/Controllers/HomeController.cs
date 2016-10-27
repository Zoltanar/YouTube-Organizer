using Microsoft.AspNetCore.Mvc;

namespace YoutubeOrganizer.Controllers
{
    /// <summary>
    /// Homepage controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Homepage.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// About page.
        /// </summary>
        /// <returns></returns>
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        /// <summary>
        /// Contact page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Error page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            return View();
        }
    }
}
