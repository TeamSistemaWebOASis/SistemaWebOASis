using System.Web.Mvc;

namespace SitioWebOasis.Controllers
{
    public class BreadCrumbController : Controller
    {
        //  GET: BreadCrumb
        public ActionResult breadCrumb()
        {
            return PartialView("_BreadCrumb");
        }
    }
}