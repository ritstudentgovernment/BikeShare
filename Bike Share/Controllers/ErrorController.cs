using System.Web.Mvc;

namespace BikeShare.Controllers
{
    /// <summary>
    /// Handles display of error pages.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Displays a generic 404 Error.
        /// </summary>
        /// <returns>View with no ViewModel.</returns>
        public ActionResult Error404()
        {
            return View();
        }

        /// <summary>
        /// Displays a generic error page.
        /// </summary>
        /// <returns>View with no ViewModel.</returns>
        public ActionResult Error()
        {
            return View();
        }

        public ActionResult authError()
        {
            return RedirectToAction("LogOnForm", "Account");
        }
    }
}