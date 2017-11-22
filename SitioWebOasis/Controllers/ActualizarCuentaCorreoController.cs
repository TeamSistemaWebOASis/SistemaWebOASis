using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitioWebOasis.Controllers
{
    public class ActualizarCuentaCorreoController : Controller
    {
        // GET: ActualizarCuentaCorreo
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string strNumCedula)
        {
            Models.DatosPersonalesUsuario dtsPU = new Models.DatosPersonalesUsuario(strNumCedula);
            return View("ActualizarCuentaCorreo", dtsPU );
        }

        [HttpPost]
        public ActionResult UpdCtaCorreo( string strNumCedula, string ctaMailAcceso)
        {
            Models.DatosPersonalesUsuario dpu = new Models.DatosPersonalesUsuario(strNumCedula);

            //  Actualizo cta de correo 
            dpu.updCtaCorreoSistemaAcademico(ctaMailAcceso);

            return View("Index");
        }

    }
}