using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Models;
using BikeShare.Interfaces;

namespace BikeShare.Controllers
{
    public class RegistrationController : Controller
    {
        private IUserRepository userRepo;

        public RegistrationController(IUserRepository uParam)
        {
            userRepo = uParam;
        }
        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }
    }
}