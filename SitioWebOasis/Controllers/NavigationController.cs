using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SitioWebOasis.Controllers
{
    public class NavigationController : Controller
    {
        private string _urlMA = string.Empty;

        // GET: Navigation
        public ActionResult Menu()
        {
            List<MenuViewModel> lstMVM = new List<MenuViewModel>();
            

            try
            {
                string rol = (UsuarioActual!=null)  ? UsuarioActual.RolActual.ID.ToString()
                                                    : "";

                switch (rol){
                    case "Estudiantes":
                        lstMVM = this._getMenuEstudiantes();
                        this._urlMA = this._getUrlMatriculacionAntigua();
                    break;

                    case "Docentes":
                        lstMVM = this._getMenuDocentes();
                        break;

                    default:
                        lstMVM = new List<MenuViewModel>();
                    break;
                }

                return PartialView("_Navigation", lstMVM);
            }catch(System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "NavigationController - Menu");

                return PartialView("error", "Error");
            }
        }


        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        private List<MenuViewModel> _getMenuEstudiantes()
        {
            List<MenuViewModel> lstMVM = new List<MenuViewModel>();

            /////////////////////////////////////
            //  MENU INICIO
            /////////////////////////////////////
            MenuViewModel menu = new MenuViewModel(){   MenuID = 1,
                                                        Action = "Index",
                                                        Controller = "Estudiantes",
                                                        IsAction = true,
                                                        Class = "text",
                                                        SubMenu = null,
                                                        icon = "fa fa-home",
                                                        Title = Language.es_ES.EST_MN_INICIO.ToString()};

            //  Submenu Datos Estudiante
            menu.SubMenu = new List<MenuViewModel>();
            MenuViewModel subMenu = new MenuViewModel(){    Action = "DatosEstudiantes",
                                                            Controller = "Estudiantes",
                                                            IsAction = true,
                                                            Class = "text",
                                                            SubMenu = null,
                                                            icon = "fa fa-user",
                                                            Title = Language.es_ES.EST_MN_DATOS_ESTUDIANTE.ToString()};
            menu.SubMenu.Add(subMenu);

            //  Submenu Notas Estudiante
            subMenu = new MenuViewModel(){  Action = "NotasEstudiantes",
                                            Controller = "Estudiantes",
                                            IsAction = true,
                                            Class = "text",
                                            SubMenu = null,
                                            icon = "fa fa-list",
                                            Title = Language.es_ES.EST_MN_NOTAS_ESTUDIANTE.ToString()};

            menu.SubMenu.Add(subMenu);

            //  Agrego a la lista todo el Menu y los elemento de subMenu
            lstMVM.Add(menu);

            //////////////////////////////////////////
            //  MENU HORARIO ESTUDIANTES
            //////////////////////////////////////////
            menu = new MenuViewModel(){ MenuID = 1,
                                        Action = "HorarioEstudiante",
                                        Controller = "Estudiantes",
                                        IsAction = true,
                                        Class = "text",
                                        SubMenu = null,
                                        icon = "fa fa-calendar",
                                        Title = Language.es_ES.EST_MN_HORARIO_ESTUDIANTE.ToString()};

            //  Submenu horario estudiante
            menu.SubMenu = new List<MenuViewModel>();
            subMenu = new MenuViewModel(){  Action = "HorarioEstudiante",
                                            Controller = "Estudiantes",
                                            IsAction = true,
                                            Class = "text",
                                            SubMenu = null,
                                            icon = "fa fa-calendar",
                                            Title = Language.es_ES.EST_MN_HORARIO_ESTUDIANTE.ToString()};

            menu.SubMenu.Add(subMenu);

            //  Submenu horario examenes estudiante
            subMenu = new MenuViewModel(){  Action = "HorarioExamenesEstudiante",
                                            Controller = "Estudiantes",
                                            IsAction = true,
                                            Class = "text",
                                            SubMenu = null,
                                            icon = "fa fa-calendar-o",
                                            Title = Language.es_ES.EST_MN_HORARIO_EXAMENES.ToString()};

            menu.SubMenu.Add(subMenu);
            lstMVM.Add(menu);

            //////////////////////////////////////////
            //  MENU MATRICULACION
            //////////////////////////////////////////
            menu = new MenuViewModel(){ MenuID = 1,
                                        Action = this._getUrlMatriculacionAntigua() + UsuarioActual.Cedula,
                                        Controller = "Estudiantes",
                                        IsAction = true,
                                        Class = "text",
                                        SubMenu = null,
                                        icon = "fa fa-pencil-square-o",
                                        Title = Language.es_ES.EST_MN_MATRICULACION.ToString() };
            menu.SubMenu = new List<MenuViewModel>();
            subMenu = new MenuViewModel()
            {
                Action = "ArchivoMatricula",
                Controller = "Estudiantes",
                IsAction = true,
                Class = "text",
                SubMenu = null,
                icon = "fa fa-file-pdf-o",
                Title = Language.es_ES.EST_CERTIFICADO_MATRICULA.ToString()
            };
            menu.SubMenu.Add(subMenu);
            //Agrego a la lista todo el Menu y los elemento de subMenu
            lstMVM.Add(menu);
            return lstMVM;
        }


        private List<MenuViewModel> _getMenuDocentes()
        {
            List<MenuViewModel> lstMVM = new List<MenuViewModel>();
            MenuViewModel menu = new MenuViewModel()
            {
                MenuID = 1,
                Action = "Index",
                Controller = "Docentes",
                IsAction = true,
                Class = "text",
                SubMenu = null,
                icon = "fa fa-folder-open-o",
                Title = "Archivos del Docente"
            };
            //Submenu Archivo Docentes
            menu.SubMenu = new List<MenuViewModel>();
            MenuViewModel subMenu = new MenuViewModel()
            {
                Action = "GestionArchivoDocente",
                Controller = "Docentes",
                IsAction = true,
                Class = "text",
                SubMenu = null,
                icon = "fa fa-folder",
                Title = Language.es_ES.DOC_GESTION_ARCHIVOS.ToString()
            };
            menu.SubMenu.Add(subMenu);
            lstMVM.Add(menu);
            //MENU HORARIO DE DOCENTE
            menu = new MenuViewModel() {
                                        MenuID=1,
                                        Action="HorarioDocente",
                                        Controller="Docentes",
                                        IsAction=true,
                                        Class="text",
                                        SubMenu=null,
                                        icon= "fa fa-calendar-plus-o",
                                        Title= Language.es_ES.DOC_MN_HORARIO_DOCENTE.ToString()
            };
            menu.SubMenu = new List<MenuViewModel>();
            subMenu = new MenuViewModel(){
                                        Action = "HorarioDocente",
                                        Controller = "Docentes",
                                        IsAction = true,
                                        Class = "text",
                                        SubMenu = null,
                                        icon = "fa fa-calendar",
                                        Title = Language.es_ES.DOC_HORARIO_CLASE.ToString()
            };
            menu.SubMenu.Add(subMenu);
            subMenu = new MenuViewModel() {
                Action = "HorarioExamenDocente",
                Controller = "Docentes",
                IsAction = true,
                Class = "text",
                SubMenu = null,
                icon = "fa fa-calendar",
                Title = Language.es_ES.DOC_MN_HORARIO_EXAMEN_DOCENTE.ToString()
            };
            menu.SubMenu.Add(subMenu);
            lstMVM.Add(menu);
                return lstMVM;
        }


        private string _getUrlMatriculacionAntigua()
        {
            string urlMA = string.Empty;

            try
            {
                var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                urlMA = appSettings.Get("urlMatriculacionSistemaAntiguo").ToString();
            }
            catch(Exception ex)
            {
                urlMA = string.Empty;
                Errores err = new Errores();
                err.SetError(ex, "_getUrlMatriculacionAntigua");
                err.setInfo("_getUrlMatriculacionAntigua", ex.Message + " - " + UsuarioActual.Cedula.ToString());
            }

            return urlMA;
        }

    }
}