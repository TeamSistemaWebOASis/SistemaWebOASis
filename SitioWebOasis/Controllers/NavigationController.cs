using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SitioWebOasis.Controllers
{
    public class NavigationController : Controller
    {
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

            //  MENU INICIO
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

            //  MENU HORARIO ESTUDIANTES
            menu = new MenuViewModel(){ MenuID = 1,
                                        Action = "HorarioEstudiante",
                                        Controller = "Estudiantes",
                                        IsAction = true,
                                        Class = "text",
                                        SubMenu = null,
                                        icon = "fa fa-calendar",
                                        Title = Language.es_ES.EST_MN_HORARIO_ESTUDIANTE.ToString()};

            //  Submenu Datos Estudiante
            menu.SubMenu = new List<MenuViewModel>();
            subMenu = new MenuViewModel(){  Action = "HorarioEstudiante",
                                            Controller = "Estudiantes",
                                            IsAction = true,
                                            Class = "text",
                                            SubMenu = null,
                                            icon = "fa fa-calendar",
                                            Title = Language.es_ES.EST_MN_HORARIO_ESTUDIANTE.ToString()};

            menu.SubMenu.Add(subMenu);

            //  Submenu Notas Estudiante
            subMenu = new MenuViewModel(){  Action = "HorarioExamenesEstudiante",
                                            Controller = "Estudiantes",
                                            IsAction = true,
                                            Class = "text",
                                            SubMenu = null,
                                            icon = "fa fa-calendar-o",
                                            Title = Language.es_ES.EST_MN_HORARIO_EXAMENES.ToString()};

            menu.SubMenu.Add(subMenu);

            //  Agrego a la lista todo el Menu y los elemento de subMenu
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
                icon = "fa fa-home",
                Title = "Menú"
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
            return lstMVM;
        }
    }
}