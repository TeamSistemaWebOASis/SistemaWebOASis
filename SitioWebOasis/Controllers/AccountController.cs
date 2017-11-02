using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using System.Web.Security;
using SitioWebOasis.CommonClasses.GestionUsuarios;

namespace SitioWebOasis.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn()
        {
            // Enviar una solicitud de inicio de sesión a OpenID Connect.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
            this.Session.Remove("UsuarioActual");
            this.Session.Add("UsuarioActual", new Usuario());

            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                // Redirigir a la página principal si el usuario está autenticado.
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
