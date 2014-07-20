using BikeShare.Models;
using System.Linq;
using System.Web.Mvc;

namespace BikeShare.Controllers
{
    /// <summary>
    /// Handles Mail unsubscription attempts.
    /// </summary>
    public class MailSubController : Controller
    {
        
        /// <summary>
        /// Displays the unsubscription page.
        /// </summary>
        /// <param name="subId">Id of the subscription to cancel.</param>
        /// <returns>View with subscription Id.</returns>
        public ActionResult Unsub(int subId)
        {
            return View(subId);
        }

        /// <summary>
        /// Processes unsubscribe attempt. Either directs to success or failure page.
        /// </summary>
        /// <param name="subId">Id of the subscription to cancel.</param>
        /// <param name="email">Id of the email to validate the cancellation attempt.</param>
        /// <returns>Directs to success or failure page.</returns>
        [HttpPost]
        public ActionResult Unsub(int subId, string email)
        {
            return View();
        }

        /// <summary>
        /// Displays unsubscribe success page.
        /// </summary>
        /// <returns>Static View.</returns>
        public ActionResult Success()
        {
            return View();
        }

        /// <summary>
        /// Displays the unsubscribe failure page.
        /// </summary>
        /// <returns>Static view.</returns>
        public ActionResult Failure()
        {
            return View();
        }
    }
}