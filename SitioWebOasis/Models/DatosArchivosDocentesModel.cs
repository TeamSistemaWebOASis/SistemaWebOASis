using GestorErrores;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class DatosArchivosDocentesModel: DatosCarrera
    {
        private System.Data.DataSet dsCDPA;
        public void getCarrerasPeriodosAnteriores()
        {
            try
            {
                ProxySeguro.Seguridad seg = new ProxySeguro.Seguridad();
                dsCDPA = seg.carrerasDocentesPeriodosAnteriores(this.UsuarioActual.Cedula.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getCarrerasPeriodosAnteriores");
            }
        }

        public List<System.Web.Mvc.SelectListItem> getLstCarrerasPA()
        {
            List<System.Web.Mvc.SelectListItem> lstCarerrasPA = new List<System.Web.Mvc.SelectListItem>();
            System.Web.Mvc.SelectListItem carrera = new System.Web.Mvc.SelectListItem();
            try
            {
                if(this.UsuarioActual != null)
                {
                    ProxySeguro.Seguridad seg = new ProxySeguro.Seguridad();
                    dsCDPA =seg.carrerasDocentesPeriodosAnteriores(this.UsuarioActual.Cedula.ToString());
                    string[] lstCarreras = { "strCodCarrera", "strCarrera" };
                    DataTable dtSelect = dsCDPA.Tables[0].DefaultView.ToTable(true, lstCarreras);//Realiza un select de las carreras usando el distinc
                    if (dtSelect != null && dtSelect.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtSelect.Rows)
                        {
                            carrera = new System.Web.Mvc.SelectListItem();
                            carrera.Value = item["strCodCarrera"].ToString();
                            carrera.Text = item["strCarrera"].ToString();
                            lstCarerrasPA.Add(carrera);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lstCarerrasPA = new List<System.Web.Mvc.SelectListItem>();
                Errores err = new Errores();
                err.SetError(ex, "getCarrerasPeriodosAnteriores");
            }
            return lstCarerrasPA;
        }

        public List<System.Web.Mvc.SelectListItem> getLstPeriodos(string strCodCarrera)
        {
            List<System.Web.Mvc.SelectListItem> lstPeriodos = new List<System.Web.Mvc.SelectListItem>();
            System.Web.Mvc.SelectListItem periodos = new System.Web.Mvc.SelectListItem();
            try
            {
                if (this.UsuarioActual != null)
                {
                    DataRow [] row = dsCDPA.Tables[0].Select("strCodCarrera='"+strCodCarrera+"'");

                    if (row != null && row.Count() > 0)
                    {
                        foreach (DataRow item in row)
                        {
                            periodos = new System.Web.Mvc.SelectListItem();
                            periodos.Value = item["strCodPeriodo"].ToString();
                            periodos.Text = item["strDescripcion"].ToString();
                            lstPeriodos.Add(periodos);
                        }
                    }
                    else
                    {
                        periodos = new System.Web.Mvc.SelectListItem();
                        periodos.Value = "-2";
                        periodos.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                        periodos.Selected = true;
                        lstPeriodos.Add(periodos);
                    }
                }
            }
            catch (Exception ex)
            {
                lstPeriodos = new List<System.Web.Mvc.SelectListItem>();
                Errores err = new Errores();
                err.SetError(ex, "getLstPeriodos");
            }
            return lstPeriodos;
        }

        public WSGestorDeReportesMatriculacion.dtstCursosDocente _dsAsignaturasDocentePA(string strCodCarrera, string strCodPeriodo)
        {
            WSGestorDeReportesMatriculacion.dtstCursosDocente dsCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();
            WSGestorDeReportesMatriculacion.dtstCursosDocente rstCursoDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();
            try
            {
                ProxySeguro.GestorDeReportesMatriculacion rm = new ProxySeguro.GestorDeReportesMatriculacion();
                rm.CookieContainer = new System.Net.CookieContainer();
                rm.SetCodCarrera(strCodCarrera);
                rstCursoDocente = rm.GetCursosDocente(strCodPeriodo, this.UsuarioActual.Cedula.ToString());
                if (rstCursoDocente != null)
                {
                    dsCursosDocente = rstCursoDocente;
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getAsignaturasDocente");
            }
            return dsCursosDocente;
        }

        public string getHTMLAsignaturasDocentePorPeriodoAcademico(string strCodCarrera, string strCodPeriodo)
        {
            string rst = string.Empty;
            string nivel = string.Empty;
            string color = "odd";
            WSGestorDeReportesMatriculacion.dtstCursosDocente dtstCursosDocente = this._dsAsignaturasDocentePA( strCodCarrera, 
                                                                                                                strCodPeriodo);
            rst += " <tr role='row' class='" + color + "'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";
            if (dtstCursosDocente.Cursos.Count > 0)
            {
                int x = 0;
                int posicion = 0;
                rst = string.Empty;
                foreach (DataRow item in dtstCursosDocente.Cursos)
                {
                    posicion = ++x;
                    color = (color == "odd") ? "even" : "odd";
                    nivel = this._getNumOrdinal(item["strCodNivel"].ToString());
                    rst += "<tr id=" + item["strCodMateria"].ToString().Trim() + "_" + item["strCodNivel"].ToString() + "_" + item["strCodParalelo"].ToString() + " role='row' class='" + color + "'>";
                    rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + posicion + "</td>";
                    rst += "    <td style='align-content: center; vertical-align: middle; text-align: left;'>" + item["strNombreMateria"].ToString().Trim() +    "</td>";
                    rst += "	<td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strCodParalelo"].ToString() + "</td>";
                    rst += "	<td style='align-content: center; vertical-align: middle; text-align: center;'>" + nivel + "</td>";
                    rst += "	<td style='align-content: center; vertical-align: middle; text-align: center;'> <div class='btn-group btn-group-xs'><button type='button' class='btn btn-info'><span class='glyphicon glyphicon-download-alt'></span> DESCARGAR EVALUACIONES</button></div> </td>";
                    rst += "</tr>";
                }
            }

            return rst;
        }

        public string getFacultadCarreraDocente()
        {
            string facultadCarreraDocente = string.Empty;
            try
            {
                if (this.UsuarioActual != null)
                {
                    string facultad = (this.UsuarioActual.FacultadActual != null)
                                            ? this.UsuarioActual.FacultadActual.Nombre.ToString() + " / "
                                            : "";

                    string carrera = (this.UsuarioActual.CarreraActual != null)
                                        ? this.UsuarioActual.CarreraActual.Nombre.ToString()
                                        : "";
                    facultadCarreraDocente = facultad + carrera;
                }
            }
            catch (Exception ex)
            {
                facultadCarreraDocente = string.Empty;
                Errores err = new Errores();
                err.SetError(ex, "getFacultadCarreraDocente");
            }
            return facultadCarreraDocente;
        }

        public string getNombreDocente()
        {
            string nombreDocente = (this.UsuarioActual != null)
                                        ? this.UsuarioActual.Nombre
                                        : string.Empty;
            return nombreDocente;
        }


    }
}