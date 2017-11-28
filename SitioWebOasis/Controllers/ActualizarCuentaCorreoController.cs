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
        public ActionResult ValidarNumeroCedula(string strNumCedula)
        {
            Models.DatosPersonalesUsuario dtsPU = new Models.DatosPersonalesUsuario(strNumCedula);
            Session["dtsPersonalesUsuario"] = dtsPU;

            return View("ActualizarCuentaCorreo", dtsPU );
        }

        [HttpPost]
        public JsonResult UpdCtaCorreo( string strNumCedula, string ctaMailAcceso)
        {
            Models.DatosPersonalesUsuario dpu = (Session["dtsPersonalesUsuario"] == null) 
                                                    ? new Models.DatosPersonalesUsuario(strNumCedula)
                                                    : (Models.DatosPersonalesUsuario)Session["dtsPersonalesUsuario"];

            //  Actualizo cta de correo 
            bool rstUpdCtaCorreo  = dpu.updCtaCorreoSistemaAcademico(ctaMailAcceso);
            string msmResultado = (rstUpdCtaCorreo) ?""
                                                    :"Favor volver a intentarlo, si el problema persiste consulte en la secretaria de carrera";

            return Json(new { ban = rstUpdCtaCorreo, mensaje = msmResultado });
        }

    }
}