using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MVCFW48Azure.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var theClaims = claimsIdentity.Claims.ToList();

            ViewBag.ContactName = theClaims.FirstOrDefault(c => c.Type == "name")?.Value;
            ViewBag.PreferredUserName = theClaims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }
    }
}