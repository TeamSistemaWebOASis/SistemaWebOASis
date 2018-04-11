using GestorErrores;
using Microsoft.Reporting.WebForms;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using SitioWebOasis.ProxySeguro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class HorarioDocenteModel : Usuario
    {
        private WSInfoCarreras.dtstPeriodoVigente _dsPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();
        private string strCodPeriodoVigente = string.Empty;
        private string strTipoHorario = string.Empty;
        public HorarioDocenteModel()
        {
            try
            {
                this.ObtenerPeriodoVigente();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "HorarioDocenteModel");
            }
        }
        public HorarioDocenteModel(string strTipoHorario)
        {
            try
            {
                this.ObtenerPeriodoVigente();
                this.strTipoHorario = strTipoHorario;
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "HorarioDocenteModel");
            }

        }

        private void ObtenerPeriodoVigente()
        {
            _dsPeriodoVigente = _getPeriodoVigenteCarrera();
            DataRow fila = _dsPeriodoVigente.Tables["Periodos"].Rows[0];
            strCodPeriodoVigente = fila["strCodigo"].ToString();
        }

        public void HorarioExamenes()
        {
            GestorDeReportesEvaluacion gr = new GestorDeReportesEvaluacion();
            gr.CookieContainer = new System.Net.CookieContainer();
            gr.set_CodCarrera(this.UsuarioActual.CarreraActual.Codigo);
            WSGestorDeReportesEvaluacion.dtstHorarioExamenes dsHorarioExamenes = gr.GetHorarioExamenesDocente(strCodPeriodoVigente, this.UsuarioActual.Cedula);
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
        private WSGestorDeReportesMatriculacion.dtstHorario _dsHorarioClase()
        {
            GestorDeReportesMatriculacion gr = new GestorDeReportesMatriculacion();
            gr.CookieContainer = new System.Net.CookieContainer();
            gr.SetCodCarrera(this.UsuarioActual.CarreraActual.Codigo);
            SitioWebOasis.WSGestorDeReportesMatriculacion.dtstHorario dtsHorario = gr.GetReporteHorarioDocente(this.UsuarioActual.Cedula, strCodPeriodoVigente);
            return dtsHorario;
        }
        public string HTMLHorarioClases()
        {
            string rst = string.Empty;

            rst += " <tr role='row'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS.ToUpper() + "</td>";
            rst += " </tr>";

            try
            {
                var dtsHorario = _dsHorarioClase();
                if (dtsHorario != null && dtsHorario.Tables["Horario"].Rows.Count > 0){
                    rst = string.Empty;
                    dtsHorario.Tables["Horario"].DefaultView.Sort = "strCodHora";//Ordena  la tabla del dataset
                    foreach (DataRow item in dtsHorario.Tables["Horario"].Rows)
                    {
                        rst += " <tr role='row' class='even'>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strDescripcionHora"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strInicioFinHora"].ToString() + "</ td >";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strLunes"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strMartes"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strMiercoles"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strJueves"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strViernes"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strSabado"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strDomingo"].ToString() + "</td>";
                        rst += " </tr>";
                    }
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "HTMLHorarioCarrera");
            }

            return rst;
        }
        private WSGestorDeReportesEvaluacion.dtstHorarioExamenes _dsHorarioExamenes()
        {
            GestorDeReportesEvaluacion gr = new GestorDeReportesEvaluacion();
            gr.CookieContainer = new System.Net.CookieContainer();
            gr.set_CodCarrera(this.UsuarioActual.CarreraActual.Codigo);
            WSGestorDeReportesEvaluacion.dtstHorarioExamenes dsHorarioExamenes = gr.GetHorarioExamenesDocente(strCodPeriodoVigente, this.UsuarioActual.Cedula);
            return dsHorarioExamenes;
        }

        public string HTMLHorarioExamen()
        {
            string rst = string.Empty;

            rst += " <tr role='row'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS.ToUpper() + "</td>";
            rst += " </tr>";

            try
            {
                WSGestorDeReportesEvaluacion.dtstHorarioExamenes dsHorarioExamenes = _dsHorarioExamenes();
                if (dsHorarioExamenes != null && dsHorarioExamenes.Tables["Materias"].Rows.Count > 0)
                {
                    rst = string.Empty;
                    foreach (DataRow item in dsHorarioExamenes.Tables["Materias"].Rows)
                    {
                        rst += " <tr role='row' class='even'>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strNombreMateria"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strDescripcionNivel"].ToString() + "</ td >";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strCodParalelo"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["dtFechaExPrinc"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["dtFechaExSusp"].ToString() + "</td>";
                        rst += " </tr>";
                    }
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "HTMLHorarioCarrera");
            }

            return rst;
        }

        public string getNombreDocente()
        {
            string nombreDocente = (this.UsuarioActual != null)
                                        ? this.UsuarioActual.Nombre
                                        : string.Empty;
            return nombreDocente;
        }
        public LocalReport getReporteHorarios(string reportPath)
        {
            LocalReport rptHorarioDocente = new LocalReport();
            try
            {
                if (this.strTipoHorario.Equals("Clase"))
                {
                    WSGestorDeReportesMatriculacion.dtstHorario dtsHorario = _dsHorarioClase();
                    List<HorarioClaseDocente> lstHorarioDocente = new List<HorarioClaseDocente>();
                    foreach (DataRow item in dtsHorario.Tables["Horario"].Rows)
                    {
                        HorarioClaseDocente objHorarioClase = new HorarioClaseDocente
                        {
                            StrCodHora = item.ItemArray[0].ToString(),
                            StrDescripcionHora = item.ItemArray[1].ToString(),
                            StrInicioFinHora = item.ItemArray[2].ToString(),
                            StrLunes = item.ItemArray[3].ToString(),
                            StrMartes = item.ItemArray[4].ToString(),
                            StrMiercoles = item.ItemArray[5].ToString(),
                            StrJueves = item.ItemArray[6].ToString(),
                            StrViernes = item.ItemArray[7].ToString(),
                            StrSabado = item.ItemArray[8].ToString(),
                            StrDomingo = item.ItemArray[9].ToString()
                        };
                        lstHorarioDocente.Add(objHorarioClase);
                    }
                    ReportDataSource rds = new ReportDataSource();
                    rds.Name = "dsHorarioClaseDocente";
                    rds.Value = lstHorarioDocente;
                    rptHorarioDocente.DataSources.Clear();
                    rptHorarioDocente.DataSources.Add(rds);
                    rptHorarioDocente.ReportPath = reportPath;
                    rptHorarioDocente.SetParameters(_getParametrosGeneralesReporte());
                    rptHorarioDocente.Refresh();
                }
                else
                {
                    WSGestorDeReportesEvaluacion.dtstHorarioExamenes dsHorarioExamenes = _dsHorarioExamenes();
                    List<HorarioExamenDocente> lstHorarioExDocente = new List<HorarioExamenDocente>();
                    foreach (DataRow item in  dsHorarioExamenes.Tables["Materias"].Rows)
                    {
                        HorarioExamenDocente objHorarioExamen = new HorarioExamenDocente
                        {
                            StrCodMateria=item.ItemArray[0].ToString(),
                            StrCodParalelo=item.ItemArray[1].ToString(),
                            StrCodNivel= item.ItemArray[2].ToString(),
                            StrDescripcionNivel=item.ItemArray[3].ToString(),
                            StrNombreMateria=item.ItemArray[4].ToString(),
                            DtFechaExPrinc= item.ItemArray[5].ToString(),
                            DtFechaExSusp= item.ItemArray[6].ToString(),
                            StrCedula= item.ItemArray[7].ToString(),
                            StrKeyMateria=item.ItemArray[8].ToString()
                        };
                        lstHorarioExDocente.Add(objHorarioExamen);
                    }
                    ReportDataSource rds = new ReportDataSource();
                    rds.Name = "dsHorarioExamenDocente";
                    rds.Value = lstHorarioExDocente;
                    rptHorarioDocente.DataSources.Clear();
                    rptHorarioDocente.DataSources.Add(rds);
                    rptHorarioDocente.ReportPath = reportPath;
                    rptHorarioDocente.SetParameters(_getParametrosGeneralesReporte());
                    rptHorarioDocente.Refresh();

                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getReporteHorarios");
            }

            return rptHorarioDocente;
        }

        private IEnumerable<ReportParameter> _getParametrosGeneralesReporte()
        {
            WSInfoCarreras.ParametrosCarrera pc = this._getParametrosCarrera();
            List<ReportParameter> lstPrmRptHorarioAcademico = new List<ReportParameter>();

            string lblFacultad = "FACULTAD:";
            string lblCarrera = "CARRERA:";
            string lblEscuela = "ESCUELA:";

            string facultad = default(string);
            string carrera = default(string);
            string escuela = default(string);
            string strDocente = default(string);

            try
            {
                ReportParameter prmRptHorarioAcademico = new ReportParameter();

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strPeriodoAcademico",
                                                _dsPeriodoVigente.Periodos[0]["strDescripcion"].ToString().ToUpper()));

                strDocente = getNombreDocente();
                switch (UsuarioActual.CarreraActual.TipoEntidad.ToString())
                {
                    case "CAR":
                        facultad = pc.NombreFacultad;
                        carrera = pc.NombreCarrera;
                        escuela = pc.NombreEscuela;
                        break;

                    case "CAA":
                        lblFacultad = "";
                        lblCarrera = "";
                        lblEscuela = "";

                        facultad = pc.NombreFacultad;
                        carrera = pc.NombreCarrera;
                        escuela = "";
                        break;
                }

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strInstitucion",
                                                                    Language.es_ES.STR_INSTITUCION));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblHorarioAcademico",
                                                                    Language.es_ES.STR_HORARIO_ACADEMICO));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblHorarioExamenes",
                                                                    Language.es_ES.STR_HORARIO_EXAMENES));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblPeriodoAcademico",
                                                                    Language.es_ES.STR_PERIODO_ACADEMICO));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblFacultad",
                                                                    lblFacultad));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblCarrera",
                                                                    lblCarrera));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblEscuela",
                                                                    lblEscuela));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strFacultad",
                                                                    facultad));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strEscuela",
                                                                    carrera));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strCarrera",
                                                                    escuela));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strDocente",
                                                                    strDocente));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strFuente",
                                                                    Language.es_ES.STR_FUENTE_REPORTE));
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDatosGeneralesReporte");
            }

            return lstPrmRptHorarioAcademico;
        }
        private WSInfoCarreras.ParametrosCarrera _getParametrosCarrera()
        {
            WSInfoCarreras.ParametrosCarrera pc = new WSInfoCarreras.ParametrosCarrera();
            try
            {
                WSInfoCarreras.InfoCarreras ic = new WSInfoCarreras.InfoCarreras();
                pc = ic.GetParametrosCarrera(this.UsuarioActual.CarreraActual.Codigo.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getParametrosCarrera");
            }

            return pc;
        }


    }
}