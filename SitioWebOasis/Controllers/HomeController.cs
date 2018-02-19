﻿using GestorErrores;
using OAS_Seguridad.Cliente;
using SitioWebOasis.CommonClasses;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.WSSeguridad;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SitioWebOasis.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public RedirectToRouteResult Index()
        {
            if (this._existeRolUsuario())
            {
                //Interfaz para Estudiante
                if (UsuarioActual.RolActual.ID.ToString() == "Estudiantes"){
                    return RedirectToAction("Index", "Estudiantes");
                }

                //Interfaz para Docente
                if (UsuarioActual.RolActual.ID.ToString() == "Docentes"){
                    return RedirectToAction("Index", "Docentes");
                }
            }

            return RedirectToAction("Index", "ActualizarCuentaCorreo");
        }


        private string _getPeriodoVigente()
        {
            string periodoVigente = string.Empty;

            try{
                ProxySeguro.GestorAdministracionGeneral gag = new ProxySeguro.GestorAdministracionGeneral();
                periodoVigente = gag.getPeriodoVigente();
            }catch(Exception ex){
                periodoVigente = string.Empty;

                Errores err = new Errores();
                err.SetError(ex, "getPeriodoVigente");
            }

            return periodoVigente;
        }


        private bool _existeRolUsuario()
        {
            bool blnUsuarioValido = false;

            try
            {
                WSSeguridad.dtstUsuario dsUsuario = new WSSeguridad.dtstUsuario();
                string numIdentificacionUsr = this._getNumeroIdentificacion();

                if (!string.IsNullOrEmpty(numIdentificacionUsr)){
                    //  Add objeto seguridad a la cache del usuario
                    this._addObjetoSeguridad();

                    string periodoVigente = this._getPeriodoVigente();
                    SitioWebOasis.ProxySeguro.Seguridad seg = new ProxySeguro.Seguridad();

                    //  GESTIONA EL ROL DE UN USUARIO REGISTRADO
                    blnUsuarioValido = seg.AutenticarUsuario(   numIdentificacionUsr,
                                                                "e",
                                                                periodoVigente,
                                                                out dsUsuario);

                    //  Verificar si el usuario es válido
                    if (blnUsuarioValido){
                        //  Add objeto seguridad a la cache del usuario
                        this._addObjetoSeguridad();

                        // registrar datos del usuario en la sesión para futuras referencias
                        Usuario usr = this.RegistrarUsuarioEnSesion(dsUsuario);

                        // crear un ticket de autenticación
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(   1,
                                                                                            usr.Login,
                                                                                            DateTime.Now,
                                                                                            DateTime.Now.AddMinutes(20),
                                                                                            false,
                                                                                            usr.RolActual.ID.ToString());

                        //  Encriptar el ticket
                        string encTicket = FormsAuthentication.Encrypt(ticket);

                        //  Crear una cookie y añadir el ticket encriptado como datos
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

                        //  Añadir la cookie a la respuesta
                        Response.Cookies.Add(cookie);

                        //  Seteo el tiempo de session en "20" minutos
                        Session.Timeout = 20;
                    }else{
                        this.Session["UsuarioActual"] = new Usuario();
                    }
                }
            }catch (Exception ex){
                blnUsuarioValido = false;

                Errores err = new Errores();
                err.SetError(ex, "_existeRolUsuario");
            }

            return blnUsuarioValido;
        }


        private void _addObjetoSeguridad()
        {
            try
            {
                //  Login
                string strClaveSeg = "#_OASis2006_#";
                CommonServices.Seguridad seg = new CommonServices.Seguridad();
                string fLogin = seg.Encriptar(strClaveSeg, System.Configuration.ConfigurationManager.AppSettings["OAS_SitioWebLogin"]);
                string fPassword = seg.Encriptar(strClaveSeg, System.Configuration.ConfigurationManager.AppSettings["OAS_SitioWebPassword"]);
                this.LoginCache = new OAS_Seguridad.Cliente.OASisLogin( fLogin,
                                                                        fPassword,
                                                                        false);

                this.LoginCache.LoginUser();
                if (this.LoginCache.Authorized == false){
                    CacheConfig.Remove("OASisLogin");
                    throw new System.Security.SecurityException("Acceso no autorizado", null as Exception);
                }
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_addObjetoSeguridad");
            }
        }


        private OASisLogin LoginCache
        {
            get { return CacheConfig.Get("OASisLogin") as OASisLogin; }
            set { CacheConfig.Insert("OASisLogin", value); }
        }


        private string _getNumeroIdentificacion()
        {
            string numIdentificacion = string.Empty;

            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                string jsonDtaPersona = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_PERSONAS + "ServiciosPersona.svc" + "/ObtenerPorEmail/" + User.Identity.Name.ToString());
                var dtaPersona = System.Web.Helpers.Json.Decode(jsonDtaPersona);

                if (dtaPersona != null && !string.IsNullOrEmpty(dtaPersona.per_id.ToString())){
                    //  Consumo del servicio web ObtenerPorDocumento (cedula)
                    string jsonDtaIdentificacion = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_PERSONAS + "ServiciosDocumentoPersonal.svc" + "/ObtenerPorPersona/" + dtaPersona.per_id.ToString());
                    var _dtaIdentificacion = System.Web.Helpers.Json.Decode(jsonDtaIdentificacion);

                    if (_dtaIdentificacion.Length > 0){
                        foreach (var item in _dtaIdentificacion){
                            if (item.pid_activo == true){
                                numIdentificacion = (item.tdi_id != 1)
                                                        ? item.pid_valor
                                                        : this._addGuionCedula(item.pid_valor);
                            }
                        }
                    }
                }

                ////  ESTUDIANTE
                //  numIdentificacion = "060478969-3";
                //  numIdentificacion = "180475189-7";
                //  numIdentificacion = "131383905-0";
                //  numIdentificacion = "180552383-2";
                //  numIdentificacion = "220027162-1";

                //  numIdentificacion = "210093670-3";  <-- NO LE APARECE NOTAS EN QUIMICA EN EL SISTEMA NUEVO
                //  numIdentificacion = "060508575-2";

                //  //  DOCENTE
                //  numIdentificacion = "060292098-5";  //  <-- Miguel Duque
                //  numIdentificacion = "060356996-3";  //  <-- Pilar Hidalgo UAN - Docente
                //  numIdentificacion = "170929748-3";  //  CASO VINCULACION, PARALELOS EN FORMATO CON CARACTERES ESPECIALES
                //  numIdentificacion = "060399007-8";  //  <-- Bladimir Urgiles

                //  numIdentificacion = "060353546-9";  //  AIDA ADRIANA MIRANDA BARROS - Bioquimica Farmacia
                //  numIdentificacion = "120353525-5";  //  Vanesa Lorena Valverde Gonzales - Mecanica 

                numIdentificacion = "060451060-2";  //  MARIA DEL CARMEN SAENZ
            }
            catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getNumeroIdentificacion");
            }

            return numIdentificacion;
        }


        private string _addGuionCedula(string numCedula)
        {
            string rst = numCedula;

            if(rst.Length == 10){
                rst = numCedula.Insert(9, "-");
            }

            return rst;
        }


        private Usuario RegistrarUsuarioEnSesion(dtstUsuario dsUsuario)
        {
            Usuario usr = new Usuario(dsUsuario);
            Session["UsuarioActual"] = usr;

            return usr;
        }


        private Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        [HttpPost]
        public ContentResult getEstadoSesion()
        {
            //  ActionExecutingContext = 
            JsonResult rstEstSession = default(JsonResult);
            int dtSession = (HttpContext.Session != null)
                                ? HttpContext.Session.Timeout
                                : default(int);

            return Content($"estado: \"1\",  tiempo: \"{dtSession.ToString()}\"");
        }
    }
}