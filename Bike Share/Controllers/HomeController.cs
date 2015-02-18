using System.Web.Mvc;
using BikeShare.Models;
using BikeShare.Interfaces;

namespace BikeShare.Controllers
{
    
    /// <summary>
    /// Handles display of all of the mostly static home and info areas.
    /// </summary>
    public class HomeController : Controller
    {
        private ISettingRepository settingRepo;
        public HomeController (ISettingRepository sParam)
        {
            settingRepo = sParam;
        }
        /// <summary>
        /// Displays a generic homepage.
        /// </summary>
        /// <returns>View with no ViewModel.</returns>
        public ActionResult Index()
        {
            ViewBag.homeHTML = settingRepo.getHomeHTML();
            return View("Index");
        }

        /// <summary>
        /// Displays a page with information about the app.
        /// </summary>
        /// <returns>View with no ViewModel.</returns>
        public ActionResult About()
        {
            ViewBag.aboutHTML = settingRepo.getAboutHTML();
            return View("About");
        }
        public ActionResult Safety()
        {
            ViewBag.safetyHTML = settingRepo.getSafetyHTML();
            return View();
        }
        public ActionResult FAQ()
        {
            ViewBag.FAQHTML = settingRepo.getFAQHTML();
            return View();
        }
 
        public ActionResult Contact()
        {
            ViewBag.contactHTML = settingRepo.getContactHTML();
            return View();
        }

        [ChildActionOnly]
        public ActionResult FooterPartial()
        {
            ViewBag.FooterHTML = settingRepo.getFooterHTML();
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult HeaderPartial()
        {
            ViewBag.appName = settingRepo.getappName();
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult AnnouncementPartial()
        {
            ViewBag.announcementHTML = settingRepo.getAnnouncementHTML();
            return PartialView();
        }
    }
}