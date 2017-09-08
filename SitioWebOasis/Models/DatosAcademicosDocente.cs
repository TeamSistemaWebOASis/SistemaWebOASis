﻿using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Data;

namespace SitioWebOasis.Models
{
    public class DatosAcademicosDocente: DatosCarrera
    {
        private WSGestorDeReportesMatriculacion.dtstCursosDocente _dtstCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();

        public DatosAcademicosDocente( string idCarrera = "" )
        {
            if( !string.IsNullOrEmpty(idCarrera)){
                this.UsuarioActual.SetRolCarreraActual( Roles.Docente,
                                                        idCarrera);
            }

            this._dtstPeriodoVigente = this._dataPeriodoAcademicoVigente();
            this._dtstCursosDocente = this._dsAsignaturasDocente();
        }
        
        
        //  Lista de asignaturas por carrera
        private WSGestorDeReportesMatriculacion.dtstCursosDocente _dsAsignaturasDocente()
        {
            WSGestorDeReportesMatriculacion.dtstCursosDocente dsCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();
            WSGestorDeReportesMatriculacion.dtstCursosDocente rstCursoDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();

            try
            {
                ProxySeguro.GestorDeReportesMatriculacion rm = new ProxySeguro.GestorDeReportesMatriculacion();
                rm.CookieContainer = new System.Net.CookieContainer();
                rm.SetCodCarrera(this.UsuarioActual.CarreraActual.Codigo);

                rstCursoDocente = rm.GetCursosDocente(  this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        this.UsuarioActual.Cedula.ToString());

                if(rstCursoDocente != null){
                    dsCursosDocente = rstCursoDocente;
                }
            }
            catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getAsignaturasDocente");
            }

            return dsCursosDocente;
        }


        public string getNombreDocente()
        {
            string nombreDocente = (this.UsuarioActual != null) 
                                        ? this.UsuarioActual.Nombre
                                        : string.Empty;

            return nombreDocente;
        }


        public string getFacultadCarreraDocente()
        {
            string facultadCarreraDocente = string.Empty;

            if( this.UsuarioActual != null)
            {
                string facultad = (this.UsuarioActual.FacultadActual != null) 
                                        ? this.UsuarioActual.FacultadActual.Nombre.ToString() + " / "
                                        : "";

                string carrera = (this.UsuarioActual.CarreraActual != null) 
                                    ? this.UsuarioActual.CarreraActual.Nombre.ToString() 
                                    : "";

                facultadCarreraDocente = facultad + carrera;
            }

            return facultadCarreraDocente;
        }


        public string getHTMLAsignaturasDocente()
        {
            string rst = string.Empty;
            string alertaEquivalencia = "even";

            rst += " <tr role='row' class='" + alertaEquivalencia + "'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";

            if (this._dtstCursosDocente.Cursos.Count > 0)
            {
                int x = 0;
                rst = string.Empty;

                foreach (DataRow item in this._dtstCursosDocente.Cursos)
                {
                    rst += "<tr>";
                    rst += "    <td><a href='#'>"+ x++ +"</a></td>";
                    rst += "    <td><a href='/Docentes/EvaluacionAsignatura/" + item["strCodNivel"].ToString() + "/" + item["strCodMateria"].ToString() + "/"+ item["strCodParalelo"].ToString() + "'>"+ item["strNombreMateria"].ToString() +"</a></td>";
                    rst += "	<td>" + item["strCodNivel"].ToString() + "</td>";
                    rst += "	<td>" + item["strCodParalelo"].ToString() + "</td>";
                    rst += "	<td>";
                    rst += "		<div class='progress'>";
                    rst += "			<div class='progress-bar' data-transitiongoal='95' aria-valuenow='95' style='width: 95%'>95%</div>";
                    rst += "		</div>";
                    rst += "	</td>";
                    rst += "	<td><span class='label label-warning'>MEDIUM</span></td>";
                    rst += "</tr>";
                }
            }

            return rst;
        }

        
        public List<System.Web.Mvc.SelectListItem> getLstAsignaturasDocente(string strCodAsignatura = "")
        {
            List<System.Web.Mvc.SelectListItem> lstAsignaturasDocente = new List<System.Web.Mvc.SelectListItem>();
            System.Web.Mvc.SelectListItem asignatura = new System.Web.Mvc.SelectListItem();

            if (this._dtstCursosDocente != null && this._dtstCursosDocente.Cursos.Count > 0)
            {
                foreach (DataRow item in this._dtstCursosDocente.Cursos)
                {
                    asignatura = new System.Web.Mvc.SelectListItem();
                    asignatura.Value = item["strCodMateria"].ToString() + "|" + item["strCodNivel"].ToString() + "|" + item["strCodParalelo"].ToString();
                    asignatura.Text = item["strNombreMateria"].ToString();

                    if (strCodAsignatura == item["strCodMateria"].ToString())
                    {
                        asignatura.Selected = true;
                    }

                    lstAsignaturasDocente.Add(asignatura);
                }
            }

            return lstAsignaturasDocente;
        }


        public string getNombreAsignatura( string strCodAsignatura, string strCodNivel, string strCodParalelo )
        {
            string asignatura = string.Empty;
            if(this._dtstCursosDocente != null && this._dtstCursosDocente.Cursos.Rows.Count > 0){
                DataRow[] rst = this._dtstCursosDocente.Cursos.Select("strCodMateria = '"+ strCodAsignatura +"' AND strCodNivel ='"+ strCodNivel + "' AND strCodParalelo = '"+ strCodParalelo + "'" );

                asignatura = (rst.Length > 0)   ? rst[0]["strNombreMateria"] + " / " + rst[0]["strDescripcionNivel"] + " / " + rst[0]["strCodParalelo"]
                                                : string.Empty;

            }

            return asignatura;
        }
    }
}