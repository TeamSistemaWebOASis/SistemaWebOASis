using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class HorarioExamenesEstudiante
    {
        private WSInfoCarreras.dtstPeriodoVigente _dsPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();
        private WSGestorDeReportesEvaluacion.dtstHorarioExamenes _dsHorariosExamenes = new WSGestorDeReportesEvaluacion.dtstHorarioExamenes();

        public HorarioExamenesEstudiante()
        {
            _dsPeriodoVigente = _getPeriodoVigenteCarrera();
            _dsHorariosExamenes = _getDatosHorarioEstudianteExamenes();
        }

        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        private WSInfoCarreras.dtstPeriodoVigente _getPeriodoVigenteCarrera()
        {
            WSInfoCarreras.dtstPeriodoVigente dsPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();

            try
            {
                ProxySeguro.InfoCarreras ic = new ProxySeguro.InfoCarreras();
                dsPeriodoVigente = ic.GetPeriodoVigenteCarrera(UsuarioActual.CarreraActual.Codigo.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getPeriodoVigenteCarrera");
            }

            return dsPeriodoVigente;
        }


        private WSGestorDeReportesEvaluacion.dtstHorarioExamenes _getDatosHorarioEstudianteExamenes()
        {
            WSGestorDeReportesEvaluacion.dtstHorarioExamenes dsHorarioExamenes = new WSGestorDeReportesEvaluacion.dtstHorarioExamenes();

            try
            {
                ProxySeguro.GestorDeReportesEvaluacion re = new ProxySeguro.GestorDeReportesEvaluacion();
                re.CookieContainer = new System.Net.CookieContainer();
                re.set_CodCarrera(UsuarioActual.CarreraActual.Codigo);

                dsHorarioExamenes = re.GetHorarioExamenesEstudiante(_dsPeriodoVigente.Periodos[0]["strCodigo"].ToString(), 
                                                                    UsuarioActual.Cedula.ToString());
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getDatosHorarioEstudianteExamenes");
            }

            return dsHorarioExamenes;
        }

        
        public string getHTMLHorarioExamenesEstudiante()
        {
            string rst = "";

            try
            {
                if (this._dsHorariosExamenes.Materias.Rows.Count > 0)
                {
                    string color = "odd";
                    int x = 0;
                    string asignatura = default(string);
                    string nivel = default(string);
                    string fchEvFinal = default(string);
                    string fchEvRecuperacion = default(string);

                    DateTime dtFEF = default(DateTime);
                    DateTime dtFER = default(DateTime);

                    foreach (DataRow item in this._dsHorariosExamenes.Materias)
                    {
                        color = (color == "odd")? "even"
                                                : "odd";

                        asignatura = string.IsNullOrEmpty(item["strNombreMateria"].ToString())
                                        ? "---"
                                        : item["strNombreMateria"].ToString();

                        nivel = string.IsNullOrEmpty(item["strDescripcionNivel"].ToString())
                                        ? "---"
                                        : item["strDescripcionNivel"].ToString();

                        fchEvFinal = (DateTime.TryParse(item["dtFechaExPrinc"].ToString(), out dtFEF))
                                        ? dtFEF.ToString("dd/MM/yyyy")
                                        : item["dtFechaExPrinc"].ToString();

                        fchEvRecuperacion = (DateTime.TryParse(item["dtFechaExSusp"].ToString(), out dtFER))
                                                ? dtFER.ToString("dd/MM/yyyy")
                                                : item["dtFechaExSusp"].ToString();

                        rst += "<tr role='row' class='" + color + "'>";
                        rst += "    <td style='vertical-align: middle; text-align: center;'>" + ++x + "</td>";
                        rst += "    <td style='vertical-align: middle; text-align: left;'>" + asignatura + "</td>";
                        rst += "    <td style='vertical-align: middle; text-align: center;'>" + nivel + "</td>";
                        rst += "    <td style='vertical-align: middle; text-align: center;'>" + fchEvFinal + "</td>";
                        rst += "    <td style='vertical-align: middle; text-align: center;'>" + fchEvRecuperacion + "</td>";
                        rst += "</tr>";
                    }
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getHTMLHorarioExamenesEstudiante");
            }

            return rst;
        }









    }
}