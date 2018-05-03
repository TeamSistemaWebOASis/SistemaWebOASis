using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace SitioWebOasis.Models
{
    public class DatosAcademicosEstudiante
    {
        public string periodoEstudiante = string.Empty;
        public string nivelEstudiante = string.Empty;

        private WSGestorDeReportesEvaluacion.dtstNotasPeriodoEstudiante _dsNotasEstudiante = new WSGestorDeReportesEvaluacion.dtstNotasPeriodoEstudiante();
        private WSNotasEstudiante.dtstNotasEstudiante _dsDetalleNotas = new WSNotasEstudiante.dtstNotasEstudiante();
        public WSGestorDeReportesMatriculacion.dtstPeriodos dtaPeriodosEstudiante = new WSGestorDeReportesMatriculacion.dtstPeriodos();
        public decimal promedioEstudiante = default(decimal);
        public string descripcionPeriodoEstudiante = string.Empty;
        public string HTMLNotasEVAcumulativa = string.Empty;
        private DataSet _dsConsolidadoNotas = null;

        protected EvaluacionActiva _evActiva;
        WSInfoCarreras.dtstPeriodoVigente periodoVigente = null;

        public DatosAcademicosEstudiante(string dtaPeriodoAcademico = "")
        {
            this.periodoEstudiante = (string.IsNullOrEmpty(dtaPeriodoAcademico))
                                        ? this._getUltimoPeriodoEstudiante()
                                        : dtaPeriodoAcademico;

            this._evActiva = new EvaluacionActiva();
            periodoVigente = this._dataPeriodoAcademicoVigente();
            this._dsConsolidadoNotas = this._dsConsolidadoNotasPeriodo();
        }


        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        private WSInfoCarreras.dtstPeriodoVigente _dataPeriodoAcademicoVigente()
        {
            WSInfoCarreras.dtstPeriodoVigente dsPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();

            try {
                ProxySeguro.InfoCarreras ic = new ProxySeguro.InfoCarreras();
                dsPeriodoVigente = ic.GetPeriodoVigenteCarrera(UsuarioActual.CarreraActual.Codigo.ToString());
            }
            catch (Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "_dataPeriodoAcademicoVigente - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return dsPeriodoVigente;
        }


        private string _getUltimoPeriodoEstudiante()
        {
            string ultimoPeriodo = "";

            try {
                ProxySeguro.DatosUsuario du = new ProxySeguro.DatosUsuario();
                ultimoPeriodo = du.getUltimoPeriodoEstudiante(this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                                this.UsuarioActual.Cedula.ToString());
            }
            catch (Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "_getUltimoPeriodoEstudiante - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return ultimoPeriodo;
        }


        public string getNivelEstudiante()
        {
            string nivel = "Sin Nivel";
            string rstNivel = "-1";

            try {
                ProxySeguro.DatosUsuario du = new ProxySeguro.DatosUsuario();
                nivel = du.getNivelEstudiantePeriodo(this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                        this.UsuarioActual.CarreraActual.codUsuario.ToString(),
                                                        this.periodoEstudiante.ToString());

                rstNivel = (nivel == "Sin Definir") ? "-1"
                                                    : nivel;
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getNivelEstudiante - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return rstNivel;
        }


        public void getDataAcademico(out decimal promedioEstudiante, out string descripcionPeriodoEstudiante)
        {
            try
            {
                this._dsNotasEstudiante = this._getNotasPeriodoEstudiante();
                promedioEstudiante = this._getPromedioEstudiante();
                descripcionPeriodoEstudiante = this._getDescripcionPeriodoEstudiante();
            } catch (Exception ex) {
                promedioEstudiante = default(decimal);
                descripcionPeriodoEstudiante = default(string);

                Errores err = new Errores();
                err.SetError(ex, "getDataAcademico - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

        }


        public string getDataEvaluacionActiva()
        {
            return this._evActiva.getDataEvaluacionActiva();
        }


        private WSNotasEstudiante.dtstDatosGradoEstudiante _getDtaGraduado()
        {
            WSNotasEstudiante.dtstDatosGradoEstudiante dtaGradoEstudiante = new WSNotasEstudiante.dtstDatosGradoEstudiante();
            try
            {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                dtaGradoEstudiante = ne.GetDatosGradoEstudiante(UsuarioActual.CarreraActual.Codigo.ToString(),
                                                                UsuarioActual.Cedula.ToString());
            } catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDtaGraduado - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return dtaGradoEstudiante;
        }


        private void _getDtaEgresamiento()
        {
            try
            {
                string CodEstudiante = string.Empty;
                string CodTitulo = string.Empty;
                DateTime dtFchAprobado = default(DateTime);
                string strResolucion = string.Empty;
                float fltPrmNotas = default(float);
                float fltNumCreditos = default(float);

                ProxySeguro.EgresamientoGraduacion eg = new ProxySeguro.EgresamientoGraduacion();
                eg.set_BaseDatos("");
                eg.set_Ubicacion("");

                if (eg.EsEgresado(UsuarioActual.Cedula.ToString(), UsuarioActual.CarreraActual.Codigo.ToString())) {
                    eg.getDatosEgresamiento(Convert.ToInt32(UsuarioActual.CarreraActual.Codigo.ToString()),
                                            ref CodEstudiante,
                                            ref CodTitulo,
                                            ref dtFchAprobado,
                                            ref strResolucion,
                                            ref fltPrmNotas,
                                            ref fltNumCreditos);
                }
            } catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDtaEgresamiento - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

        }


        private decimal _getPromedioEstudiante()
        {
            decimal rst = default(decimal);
            try
            {
                decimal prm = (this._dsNotasEstudiante.Estudiantes.Rows.Count > 0 && this._dsNotasEstudiante.Estudiantes.Rows[0]["fltPromedio"].ToString() != "NaN")
                                ? Convert.ToDecimal(this._dsNotasEstudiante.Estudiantes.Rows[0]["fltPromedio"])
                                : default(decimal);

                rst = Math.Round(prm, 2);
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getPromedioEstudiante - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return rst;
        }


        private string _getDescripcionPeriodoEstudiante()
        {
            DataRow[] rst = null;
            string periodo = string.Empty;

            try {
                WSGestorDeReportesMatriculacion.dtstPeriodos dtaPeriodosEstudiante = this._periodosMatriculasEstudiante();

                if (dtaPeriodosEstudiante != null && dtaPeriodosEstudiante.Periodos.Rows.Count > 0) {
                    rst = dtaPeriodosEstudiante.Periodos.Select("strCodigo = '" + this.periodoEstudiante + "'", "");
                    periodo = (rst.Length > 0)
                                ? rst[0]["strDescripcion"].ToString()
                                : string.Empty;
                }
            } catch (Exception ex) {
                periodo = string.Empty;
                Errores err = new Errores();
                err.SetError(ex, "_getDescripcionPeriodoEstudiante - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return periodo;
        }


        public string getNumOrdinal(string numero, string tpo)
        {
            string[] ciclosAcademicos = new string[11] { "0", "1er", "2do", "3er", "4to", "5to", "6to", "7mo", "8vo", "9no", "10mo" };
            string[] matricula = new string[3] { "1ra", "2da", "3ra" };

            return (tpo == "nivel") ? ciclosAcademicos[Convert.ToInt32(numero.ToString())]
                                    : matricula[Convert.ToInt32(numero.ToString()) - 1];
        }


        public string getAlertaFila(string strCodEquivalencia, string strCodMateria, ref string alertaEquivalencia, ref string lblEquivalencia, ref string smsEquivalencia)
        {
            string rst = "even";
            string estructura = "label label-";


            switch (strCodEquivalencia)
            {
                //  APROBADO
                case "A":
                    rst = alertaEquivalencia = lblEquivalencia = "success";
                    smsEquivalencia = Language.es_ES.EST_LBL_APROBADO.ToUpper();
                    break;

                //  CONVALIDA
                case "C":
                    rst = alertaEquivalencia = lblEquivalencia = "default";
                    smsEquivalencia = Language.es_ES.EST_LBL_CONVALIDA.ToUpper();
                    break;

                //  EXONERADO
                case "E":
                    rst = alertaEquivalencia = lblEquivalencia = "success";
                    smsEquivalencia = Language.es_ES.EST_LBL_EXONERADO.ToUpper();
                    break;

                //  EXONERADO
                case "EX":
                    rst = alertaEquivalencia = lblEquivalencia = "success";
                    smsEquivalencia = Language.es_ES.EST_LBL_EXONERADO.ToUpper();
                    break;

                //  REPROBADO
                case "R":
                    rst = alertaEquivalencia = lblEquivalencia = "danger";
                    smsEquivalencia = Language.es_ES.EST_LBL_REPROBADO.ToUpper().ToUpper();
                    break;

                //  SUSPENSION
                case "S":
                    rst = alertaEquivalencia = lblEquivalencia = "warning";
                    smsEquivalencia = Language.es_ES.EST_LBL_EV_RECUPERACION.ToUpper();
                    break;

                //  EVALUACION FINAL
                case "EF":
                    rst = alertaEquivalencia = lblEquivalencia = "default";
                    smsEquivalencia = Language.es_ES.EST_LBL_EV_FINAL.ToUpper();
                    break;

                //  EN BLANCO - SE BUSCA VERIFICANDO SI LA ASIGNATURA ESTA CONVALIDADA
                case "":
                    rst = (this._asignaturaConvalidada(strCodMateria))
                            ? alertaEquivalencia = lblEquivalencia = "primary"
                            : alertaEquivalencia = lblEquivalencia = estructura = "";

                    smsEquivalencia = Language.es_ES.EST_LBL_CONVALIDA.ToUpper();
                    break;
            }

            return estructura + rst;
        }


        private bool _asignaturaConvalidada(string strCodMateria)
        {
            bool estadoAsignatura = false;

            try
            {
                if (this._dsDetalleNotas != null && this._dsDetalleNotas.Convalidaciones.Rows.Count > 0) {
                    DataRow[] rst = this._dsDetalleNotas.Convalidaciones.Select("strCodMateria = '" + strCodMateria + "'", "");
                    estadoAsignatura = (rst.Length > 0) ? true : false;
                }
            }
            catch (Exception ex) {
                estadoAsignatura = false;

            }

            return estadoAsignatura;
        }



        private WSGestorDeReportesEvaluacion.dtstNotasPeriodoEstudiante _getNotasPeriodoEstudiante()
        {
            WSGestorDeReportesEvaluacion.dtstNotasPeriodoEstudiante dsNotasPeriodoEstudiante = new WSGestorDeReportesEvaluacion.dtstNotasPeriodoEstudiante();
            WSGestorDeReportesEvaluacion.dtstNotasPeriodoEstudiante rstConsulta;

            try
            {
                ProxySeguro.GestorDeReportesEvaluacion gre = new ProxySeguro.GestorDeReportesEvaluacion();
                gre.CookieContainer = new System.Net.CookieContainer();
                gre.set_CodCarrera(this.UsuarioActual.CarreraActual.Codigo);

                rstConsulta = gre.GetNotasPeriodoEstudiante(this.UsuarioActual.Cedula.ToString(),
                                                            "",
                                                            this.periodoEstudiante.ToString());

                dsNotasPeriodoEstudiante = (rstConsulta != null) ? rstConsulta
                                                                : new WSGestorDeReportesEvaluacion.dtstNotasPeriodoEstudiante();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getNivelEstudiante - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return dsNotasPeriodoEstudiante;
        }


        private WSNotasEstudiante.dtstNotasEstudiante _getDataNotasEstudiante(string periodoAcademico)
        {
            WSNotasEstudiante.dtstNotasEstudiante dsNE = new WSNotasEstudiante.dtstNotasEstudiante();

            try
            {
                ProxySeguro.NotasEstudiante wsNE = new ProxySeguro.NotasEstudiante();

                dsNE = wsNE.GetDatosNotasEstudiante(this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                    this.UsuarioActual.Cedula.ToString(),
                                                    this.periodoEstudiante.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getHTMLNotasEstudiante - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return dsNE;
        }


        public string getHTMLNotasEVAcumulativa()
        {
            string alertaEquivalencia = "even";
            string lblEquivalencia = "default";
            string msgEquivalencia = "---";
            string rst = string.Empty;

            rst += " <tr role='row' class='success'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='11'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";

            try
            {
                this._dsDetalleNotas = this._getDataNotasEstudiante(periodoEstudiante);
                bool esPeriodoActual = (periodoEstudiante.ToString().CompareTo(periodoVigente.Periodos[0]["strCodigo"].ToString()) == 0)
                                        ? true
                                        : false;

                if (this._dsDetalleNotas != null && this._dsDetalleNotas.EvAcumulativa.Rows.Count > 0)
                {
                    int x = 0;
                    rst = string.Empty;
                    string dtaEvActiva = this.getDataEvaluacionActiva();
                    string estadoNota = string.Empty;
                    string colorParcial1 = (dtaEvActiva == "1") ? "info" : "";
                    string colorParcial2 = (dtaEvActiva == "2") ? "info" : "";
                    string colorParcial3 = (dtaEvActiva == "3") ? "info" : "";

                    foreach (DataRow item in this._dsDetalleNotas.EvAcumulativa)
                    {

                        if (esPeriodoActual)
                        {
                            alertaEquivalencia = (dtaEvActiva.ToString().CompareTo("3") == 0 || dtaEvActiva.ToString().CompareTo("P") == 0 || dtaEvActiva.ToString().CompareTo("S") == 0)
                                                    ? this.getAlertaFila(item["strCodEquiv"].ToString(),
                                                                            item["strCodMateria"].ToString(),
                                                                            ref alertaEquivalencia,
                                                                            ref lblEquivalencia,
                                                                            ref msgEquivalencia)
                                                    : "";
                        }
                        else
                        {
                            alertaEquivalencia = this.getAlertaFila(item["strCodEquiv"].ToString(),
                                                                    item["strCodMateria"].ToString(),
                                                                    ref alertaEquivalencia,
                                                                    ref lblEquivalencia,
                                                                    ref msgEquivalencia);
                        }

                        string smsObservacion = (!string.IsNullOrEmpty(item["observaciones"].ToString()))
                                                    ? item["observaciones"].ToString()
                                                    : "---";

                        rst += " <tr role='row' style='align-content: center; vertical-align: middle; text-align: center;'>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + ++x + "</td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: left;'>" + item["strNombre"].ToString() + "</ td >";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["numMatricula"].ToString(), "matricula") + "</td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["nivelAsignatura"].ToString(), "nivel") + "</td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strCodParalelo"].ToString() + "</ td >";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;' class='" + colorParcial1 + "'>" + item["bytNota1"].ToString() + "</td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;' class='" + colorParcial2 + "'>" + item["bytNota2"].ToString() + "</td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;' class='" + colorParcial3 + "'>" + item["bytNota3"].ToString() + "</td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'><b>" + item["acumulado"].ToString() + "</b></td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'> <span class='" + alertaEquivalencia + "'>" + msgEquivalencia + "</span></td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'> " + smsObservacion + "</span> </td>";
                        rst += " </tr>";
                    }
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getHTMLNotasEVAcumulativa - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return rst;
        }

        public string getHTMLPeriodosAcademicos()
        {
            string rst = string.Empty;
            rst += " <tr role='row' class='success'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='10'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";
            try
            {
                dtaPeriodosEstudiante = this._periodosMatriculasEstudiante();

                if (dtaPeriodosEstudiante != null && dtaPeriodosEstudiante.Tables["Periodos"].Rows.Count > 0)
                {
                    int x = 0;
                    rst = string.Empty;
                    foreach (DataRow item in dtaPeriodosEstudiante.Tables["Periodos"].Rows)
                    {
                        rst += " <tr role='row' id='" + item["strCodigo"].ToString().Trim() + "' style ='align-content: center; vertical-align: middle; text-align: center;'>";
                        rst += "    <td>" + ++x + "</td>";
                        rst += "    <td style='text-align: left;'>" + item["strDescripcion"].ToString() + "</ td >";
                        rst += "	<td style='align-content: center; vertical-align: middle; text-align: center;'> <div class='btn-group btn-group-xs'><button type='button' id='btnImpresion'  class='btn btn-success'><span class='glyphicon glyphicon-download-alt'></span> DESCARGAR MATRÍCULA</button></div> </td>";//btn btn-success esta hecho referencia MatriculacionSA.js
                        rst += " </tr>";
                    }
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getHTMLNotasEVAcumulativa - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return rst;
        }

        public string getDtaEvFinal_EvRecuperacion(string tpoExamen)
        {
            string alertaEquivalencia = "even";
            string lblEquivalencia = "default";
            string smsEquivalencia = "";
            string rst = string.Empty;
            string notaFinal = "";
            int totalEvFR = default(int);
            string tipoExamen = (tpoExamen == "P") ? "PRI"
                                                    : (tpoExamen == "S") ? "SUS"
                                                                        : "";

            rst += " <tr role='row' class='success'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";

            try
            {
                this._dsDetalleNotas = this._getDataNotasEstudiante(periodoEstudiante);
                if (this._dsDetalleNotas != null && this._dsDetalleNotas.EvFinal_EvFormativa.Rows.Count > 0 && !string.IsNullOrEmpty(tipoExamen))
                {
                    DataRow[] drEvF_EvR = this._dsDetalleNotas.EvFinal_EvFormativa.Select("strCodTipoExamen = '" + tipoExamen + "'", "asignatura");

                    if (drEvF_EvR.Length > 0)
                    {
                        int x = 0;
                        rst = string.Empty;

                        foreach (DataRow item in drEvF_EvR)
                        {
                            alertaEquivalencia = this.getAlertaFila(item["strCodEquiv"].ToString(),
                                                                    item["strCodMateria"].ToString(),
                                                                    ref alertaEquivalencia,
                                                                    ref lblEquivalencia,
                                                                    ref smsEquivalencia);

                            notaFinal = (string.IsNullOrEmpty(item["bytNota"].ToString()) || item["bytNota"].ToString().CompareTo("0") == 0)
                                            ? "0"
                                            : item["bytNota"].ToString();

                            totalEvFR = this._getTotalEvFR(item["bytAcumulado"].ToString(),
                                                            item["bytNota"].ToString());

                            rst += " <tr role='row'>";
                            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + ++x + "</td>";
                            rst += "     <td style='align-content: center; vertical-align: middle; text-align: left;'>" + item["asignatura"].ToString() + "</ td >";
                            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["nivel"].ToString(), "nivel") + "</td>";
                            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["numMatricula"].ToString(), "matricula") + "</td>";
                            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["bytAcumulado"].ToString() + "</td>";
                            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + notaFinal + "</td>";
                            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'><b>" + totalEvFR + "</b></td>";
                            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'> <span class='label label-" + lblEquivalencia + "'>" + smsEquivalencia + "</span> </td>";
                            rst += " </tr>";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getHTMLNotasEVAcumulativa - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return rst;
        }



        private int _getTotalEvFR(string nota1, string nota2)
        {
            int n1 = (string.IsNullOrEmpty(nota1)) ? 0
                                                    : Convert.ToInt32(nota1.ToString());

            int n2 = (string.IsNullOrEmpty(nota2)) ? 0
                                                    : Convert.ToInt32(nota2.ToString());

            return n1 + n2;
        }


        private WSGestorDeReportesMatriculacion.dtstPeriodos _periodosMatriculasEstudiante()
        {
            WSGestorDeReportesMatriculacion.dtstPeriodos lstPeridosEstudiante = new WSGestorDeReportesMatriculacion.dtstPeriodos();

            try
            {
                //  Recuperar períodos para los que se tenga matrículas
                ProxySeguro.GestorDeReportesMatriculacion gr = new ProxySeguro.GestorDeReportesMatriculacion();
                gr.CookieContainer = new System.Net.CookieContainer();
                gr.SetCodCarrera(this.UsuarioActual.CarreraActual.Codigo);

                lstPeridosEstudiante = gr.GetPeriodosDeMatriculaEstudiante(this.UsuarioActual.Cedula,
                                                                            "");
            } catch (Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "periodosMatriculasEstudiante - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return lstPeridosEstudiante;
        }


        public List<System.Web.Mvc.SelectListItem> getLstPeriodosEstudiante()
        {
            dtaPeriodosEstudiante = this._periodosMatriculasEstudiante();
            List<System.Web.Mvc.SelectListItem> lstPeriodosEstudiante = new List<System.Web.Mvc.SelectListItem>();
            System.Web.Mvc.SelectListItem periodo = new System.Web.Mvc.SelectListItem();

            try
            {
                if (dtaPeriodosEstudiante != null && dtaPeriodosEstudiante.Periodos.Rows.Count > 0) {
                    foreach (DataRow item in dtaPeriodosEstudiante.Periodos) {
                        periodo = new System.Web.Mvc.SelectListItem();
                        periodo.Value = item["strCodigo"].ToString();
                        periodo.Text = item["strDescripcion"].ToString();

                        if (periodoEstudiante == item["strCodigo"].ToString()) {
                            periodo.Selected = true;
                        }

                        lstPeriodosEstudiante.Add(periodo);
                    }
                }
            }
            catch (Exception ex)
            {
                lstPeriodosEstudiante = new List<System.Web.Mvc.SelectListItem>();
                periodo = new System.Web.Mvc.SelectListItem();
                periodo.Value = "-1";
                periodo.Text = Language.es_ES.EST_LBL_SIN_REGISTROS;

                lstPeriodosEstudiante.Add(periodo);

                Errores err = new Errores();
                err.SetError(ex, "getLstPeriodosEstudiante - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return lstPeriodosEstudiante;
        }


        private DataSet _dsConsolidadoNotasPeriodo()
        {
            DataSet dsConsolidadoNotas = new DataSet();

            try
            {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                dsConsolidadoNotas = ne.getConsolidadoNotas(this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                            this.UsuarioActual.Cedula.ToString(),
                                                            periodoEstudiante);
            } catch (Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "_dsConsolidadoNotas - Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return dsConsolidadoNotas;
        }


        public string getHTMLConsolidadoNotas()
        {
            string rst = string.Empty;
            ConsolidadoEvaluacionAsignatura dtaRowConsolidado = null;

            string lblEquivalencia_EP = "default";
            string smsEquivalencia_EP = "";
            string alertaEquivalencia_EP = string.Empty;

            string lblEquivalencia_ES = "default";
            string smsEquivalencia_ES = "";
            string alertaEquivalencia_ES = string.Empty;

            rst += " <tr role='row' class='success'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='17'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";

            if (this._dsConsolidadoNotas.Tables["notas"].Rows.Count > 0) {
                int x = 0;
                rst = "";

                //  Asignaturas
                foreach (DataRow item in this._dsConsolidadoNotas.Tables["notas"].Rows) {
                    DataRow[] rectificaciones = this._dsConsolidadoNotas.Tables["rectificaciones"].Select("strCodPeriodo = '" + this.periodoEstudiante + "' AND strCodMateria = '" + item["strCodMateria"] + "'", "");

                    if (rectificaciones.Length > 0) {
                        dtaRowConsolidado = new ConsolidadoEvaluacionAsignatura(++x, item, rectificaciones);
                    } else {
                        dtaRowConsolidado = new ConsolidadoEvaluacionAsignatura(++x, item);
                    }

                    rst += dtaRowConsolidado.getRowAsignatura();
                }

                //  Convalidaciones
                foreach (DataRow item in this._dsConsolidadoNotas.Tables["convalidaciones"].Rows) {
                    if (item["strCodPeriodo"].ToString().CompareTo(this.periodoEstudiante) == 0) {
                        ConsolidadoEvaluacionAsignatura dtaRowConvalidaciones = new ConsolidadoEvaluacionAsignatura(++x, item);
                        rst += dtaRowConvalidaciones.getRowConvalidado();
                    }
                }

                //  Retiros
                foreach (DataRow item in this._dsConsolidadoNotas.Tables["retiros"].Rows) {
                    if (item["strCodPeriodo"].ToString().CompareTo(this.periodoEstudiante) == 0) {
                        ConsolidadoEvaluacionAsignatura dtaRowRetiro = new ConsolidadoEvaluacionAsignatura(++x, item);
                        rst += dtaRowRetiro.getRowRetiroConsolidado();
                    }
                }
            }

            return rst;
        }


        public string getDtaConvalidacion()
        {
            string rst = string.Empty;
            int x = default(Int16);

            rst += " <tr role='row' class='success'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";

            try
            {
                if (this._dsConsolidadoNotas != null && this._dsConsolidadoNotas.Tables["convalidaciones"].Rows.Count > 0)
                {
                    rst = string.Empty;

                    foreach (DataRow item in this._dsConsolidadoNotas.Tables["convalidaciones"].Rows)
                    {
                        rst += " <tr role='row'>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + ++x + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: left;'>" + item["asignatura"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["strCodNivel"].ToString(), "nivel") + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["periodoConvalidacion"].ToString() + "</ td >";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'><b>" + item["institucion"].ToString().ToUpper() + "</b></td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strResolucion"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'><b>" + item["notaConvalidacion"].ToString().ToUpper() + "</b></td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + Convert.ToDateTime(item["fchAprobacion"].ToString()).ToString("dd/MM/yyyy") + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'><b>" + item["strDescripcion"].ToString().ToUpper() + "</b></td>";
                        rst += " </tr>";
                    }
                }

            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "");
                err.setInfo("Datos Academicos Estudiante - _getHTMLNotasEVAcumulativa", "Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return rst;
        }


        public string getDtaRetiros()
        {
            string rst = string.Empty;
            int x = default(Int16);

            rst += " <tr role='row' class='success'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";

            try
            {
                if (this._dsConsolidadoNotas != null && this._dsConsolidadoNotas.Tables["retiros"].Rows.Count > 0)
                {
                    rst = string.Empty;

                    foreach (DataRow item in this._dsConsolidadoNotas.Tables["retiros"].Rows)
                    {
                        rst += " <tr role='row'>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + ++x + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: left;'>" + item["strNombre"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["bytNumMat"].ToString(), "") + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["strCodNivel"].ToString(), "nivel") + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strCodParalelo"].ToString() + "</ td >";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["periodoAcademico"].ToString() + "</ td >";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strResolucion"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + Convert.ToDateTime(item["dtFechaAprob"].ToString()).ToString("dd/MM/yyyy") + "</td>";
                        rst += " </tr>";
                    }
                }

            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "");
                err.setInfo("Datos Academicos Estudiante - _getHTMLNotasEVAcumulativa", "Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return rst;
        }


        public string getDtaRectificaciones()
        {
            string rst = string.Empty;
            int x = default(Int16);

            rst += " <tr role='row' class='success'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='11'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";

            try
            {
                if (this._dsConsolidadoNotas != null && this._dsConsolidadoNotas.Tables["rectificaciones"].Rows.Count > 0)
                {
                    rst = string.Empty;
                    string tipoExamen = string.Empty;

                    foreach (DataRow item in this._dsConsolidadoNotas.Tables["rectificaciones"].Rows)
                    {
                        tipoExamen = (item["strCodTipoExamen"].ToString().CompareTo("PRI") == 0)
                                        ? Language.es_ES.NOTAS_TB_COL_EV_FINAL
                                        : Language.es_ES.NOTAS_TB_COL_EV_RECUPERACION;

                        rst += " <tr role='row'>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + ++x + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: left;'>" + item["strNombre"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["bytNumMat"].ToString(), "") + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this.getNumOrdinal(item["strCodNivel"].ToString(), "nivel") + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strCodParalelo"].ToString() + "</ td >";

                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["periodoAcademico"].ToString() + "</ td >";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strResolucion"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + tipoExamen + "</ td >";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["bytNotaAnt"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + Convert.ToDateTime(item["dtFechaAprob"].ToString()).ToString("dd/MM/yyyy") + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strDescripcion"].ToString() + "</td>";
                        rst += " </tr>";
                    }
                }

            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "");
                err.setInfo("getDtaRectificaciones", "Usuario: " + UsuarioActual.Cedula.ToString() + " / " + UsuarioActual.CarreraActual.ToString() + " / " + UsuarioActual.CarreraActual.Codigo.ToString());
            }

            return rst;
        }


        public bool estudianteEnPeriodoVigente()
        {
            return (periodoEstudiante.ToString().CompareTo(periodoVigente.Periodos[0]["strCodigo"].ToString()) == 0);
        }
        public dynamic EstadisticaEvaluacionAcumulada()
        {
            dynamic json = string.Empty;
            List<dynamic> lstJson = new List<dynamic>();
            try
            {
                this._dsDetalleNotas = this._getDataNotasEstudiante(this.periodoEstudiante);

                if (this._dsDetalleNotas != null && this._dsDetalleNotas.EvAcumulativa.Rows.Count > 0)
                {
                    foreach (DataRow item in this._dsDetalleNotas.EvAcumulativa.Rows)
                    {
                        json = new JavaScriptSerializer().Serialize(new
                        {
                            Materia = item["strNombre"].ToString(),
                            Parcial1 = item["bytNota1"].ToString(),
                            Parcial2 = item["bytNota2"].ToString(),
                            Parcial3 = item["bytNota3"].ToString(),
                            Acumulado = item["acumulado"].ToString()
                        });
                        lstJson.Add(json);
                    }
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "");
                err.setInfo("EstadisticaEvaluacionAcumulada", "");
            }
           
            return lstJson;
        }

    }
}