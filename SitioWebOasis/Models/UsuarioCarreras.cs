using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SitioWebOasis.Models
{
    public class UsuarioCarreras
    {
        public string idCarrera { get; set; }

        public IList<SelectListItem> lstCarrerasUsuario { get; set; }

        public UsuarioCarreras()
        {
            lstCarrerasUsuario = new List<SelectListItem>();
            cargarListaCarrerasUsuario();
        }


        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        public Roles RolAMostrar
        {
            get
            {
                if(this.UsuarioActual!= null)
                {
                    Usuario usr = this.UsuarioActual;
                    object rol = null;

                    switch (usr.RolActual.ID.ToString())
                    {
                        case "Estudiante":
                            rol = Roles.Estudiante;
                            break;

                        case "Docente":
                            rol = Roles.Docente;
                            break;
                    }

                    if (rol != null)
                        return (Roles)rol;
                    else
                        return Roles.PublicoGeneral;
                }else{
                    return Roles.PublicoGeneral;
                }
            }
        }

        
        private void cargarListaCarrerasUsuario()
        {
            ArrayList lstCarreras = new ArrayList();
            SelectListItem item = new SelectListItem();

            try
            {
                if(this.UsuarioActual != null)
                {
                    Roles rolUsuario = this.RolAMostrar;
                    lstCarreras = this.UsuarioActual.GetCarreras(rolUsuario);

                    if (lstCarreras.Count > 0)
                    {
                        foreach (CommonClasses.GestionUsuarios.Carrera elemento in lstCarreras)
                        {
                            item = new SelectListItem();
                            item.Value = elemento.Codigo;
                            item.Text = elemento.Nombre;
                            lstCarrerasUsuario.Add(item);
                        }
                    }
                    else
                    {
                        item = new SelectListItem();
                        item.Value = "-1";
                        item.Text = "SIN REGISTROS DISPONIBLES";
                        lstCarrerasUsuario.Add(item);
                    }
                }else
                {
                    item = new SelectListItem();
                    item.Value = "-1";
                    item.Text = "SIN REGISTROS DISPONIBLES";
                    lstCarrerasUsuario.Add(item);
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "cargarDatos");

                item = new SelectListItem();
                item.Value = "-1";
                item.Text = "SIN REGISTROS DISPONIBLES";
                lstCarrerasUsuario.Add(item);
            }
        }
    }
}