using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitioWebOasis.Models
{
    public class MenuRolesModel
    {
        public IList<SelectListItem> lstRolesUsuario { get; set; }

        public MenuRolesModel()
        {
            lstRolesUsuario = new List<SelectListItem>();
            this._cargarListaRolesUsuario();
        }
        

        private Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        public string getRolActual(){
            return this.UsuarioActual.RolActual.Nombre.ToString();
        }


        private void _cargarListaRolesUsuario()
        {
            ArrayList lstCarreras = new ArrayList();
            SelectListItem item = new SelectListItem();
            string rolRegistrado = string.Empty;

            try{
                if (this.UsuarioActual != null && this.UsuarioActual.NumRolesDeCarrera > 0){
                    foreach( CommonClasses.GestionUsuarios.Rol elemento in this.UsuarioActual.GetRolesCarrera()){
                        if( rolRegistrado.CompareTo(elemento.ID.ToString()) == -1 ){
                            item = new SelectListItem();
                            item.Value = elemento.ID.ToString();
                            item.Text = elemento.Nombre.ToUpper();
                            lstRolesUsuario.Add(item);

                            rolRegistrado = elemento.ID.ToString();
                        }
                    }
                }else{
                    item = new SelectListItem();
                    item.Value = "-1";
                    item.Text = "SIN REGISTROS DISPONIBLES";
                    lstRolesUsuario.Add(item);
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "cargarDatos");

                item = new SelectListItem();
                item.Value = "-1";
                item.Text = "SIN REGISTROS DISPONIBLES";
                lstRolesUsuario.Add(item);
            }
        }
    }
}