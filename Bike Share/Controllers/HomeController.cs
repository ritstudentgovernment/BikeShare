using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using BikeShare.Models;

namespace BikeShare.Controllers
{
    
    /// <summary>
    /// Handles display of all of the mostly static home and info areas.
    /// </summary>
    public class HomeController : Controller
    {
        BikesContext context;
        public HomeController ()
        {
            context = new BikesContext();
        }
        /// <summary>
        /// Displays a generic homepage.
        /// </summary>
        /// <returns>View with no ViewModel.</returns>
        public ActionResult Index()
        {
            ViewBag.homeHTML = context.settings.First().homeHTML;
            return View("Index");
        }

        /// <summary>
        /// Displays a page with information about the app.
        /// </summary>
        /// <returns>View with no ViewModel.</returns>
        public ActionResult About()
        {
            ViewBag.aboutHTML = context.settings.First().aboutHTML;
            return View("About");
        }
        public ActionResult Safety()
        {
            ViewBag.safetyHTML = context.settings.First().safetyHTML;
            return View();
        }
        public ActionResult FAQ()
        {
            ViewBag.FAQHTML = context.settings.First().FAQHTML;
            return View();
        }
 
        public ActionResult Contact()
        {
            ViewBag.contactHTML = context.settings.First().contactHTML;
            return View();
        }

        [ChildActionOnly]
        public ActionResult FooterPartial()
        {
            ViewBag.FooterHTML = context.settings.First().footerHTML;
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult HeaderPartial()
        {
            ViewBag.appName = context.settings.First().appName;
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult AnnouncementPartial()
        {
            ViewBag.announcementHTML = context.settings.First().announcementHTML;
            return PartialView();
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}