using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitioWebOasis.Controllers
{
    [Authorize]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}