using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using System.Web.Mvc;

namespace SitioWebOasis.Controllers
{
    public class BreadCrumbController : Controller
    {
        //  GET: BreadCrumb
        public ActionResult breadCrumb()
        {
            try
            {
                string rol = (UsuarioActual != null) ? UsuarioActual.RolActual.ID.ToString()
                                                    : "";

                ViewBag.rolUsuario = ( rol.CompareTo("PublicoGeneral") != 0 )? rol : "#";
                return PartialView("_BreadCrumb");
            }
            catch (System.Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "BreadCrumbController - Menu");

                return PartialView("error", "Error");
            }
        }


        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }

    }
}