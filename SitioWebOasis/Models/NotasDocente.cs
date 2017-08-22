using GestorErrores;
using Newtonsoft.Json;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using System;
using System.Data;
using System.Net;

namespace SitioWebOasis.Models
{
    public class NotasDocente
    {
        private string _strCodNivel;
        private string _strCodAsignatura;
        private string _strCodParalelo;
        private string _strNombreBD = string.Empty;
        private string _strUbicacion = string.Empty;
        public string promedioGeneral = string.Empty;

        public string jsonEvAcumulativa = string.Empty;
        public string jsonEvFinal = string.Empty;
        public string jsonEvRecuperacion = string.Empty;
        public string parcialActivo = "2";

        private WSInfoCarreras.dtstPeriodoVigente _dtstPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();


        public NotasDocente(string strCodNivel, string strCodAsignatura, string strCodParalelo)
        {
            this._strCodNivel = strCodNivel;
            this._strCodAsignatura = strCodAsignatura;
            this._strCodParalelo = strCodParalelo;

            this._cargarInformacionCarrera();
            this._CargarNotasEvAcumulativa();
        }


        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        private void _cargarInformacionCarrera()
        {
            try
            {
                //  Obtengo informacion de las carreras
                ProxySeguro.InfoCarreras ic = new ProxySeguro.InfoCarreras();

                WSInfoCarreras.dtstBDCarreras dsCarrera = ic.GetCarrera(UsuarioActual.CarreraActual.Codigo.ToString());
                this._strNombreBD = dsCarrera.BDCarreras.Rows[0]["strBaseDatos"].ToString();
                this._strUbicacion = UsuarioActual.CarreraActual.Codigo.ToString();

                //  Informacion del periodo vigente en carrera
                this._dtstPeriodoVigente = ic.GetPeriodoVigenteCarrera(UsuarioActual.CarreraActual.Codigo.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_cargarInformacionCarrera");
            }
        }

        #region EVALUACION ACUMULATIVA

        private WSGestorEvaluacion.dtstEvaluacion_Acumulados _CargarNotasEvAcumulativa()
        {
            WSGestorEvaluacion.dtstEvaluacion_Acumulados dsEvAcumulativa = new WSGestorEvaluacion.dtstEvaluacion_Acumulados();

            try {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD);
                ge.set_fUbicacion(this._strUbicacion);

                dsEvAcumulativa = ge.crearActaArtificialEvaluaciones(   this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                        this._strCodAsignatura,
                                                                        this._strCodNivel,
                                                                        this._strCodParalelo);

                this.jsonEvAcumulativa = (dsEvAcumulativa.Acta.Rows.Count > 0)  
                                            ? JsonConvert.SerializeObject(dsEvAcumulativa.Acta) 
                                            : "";
            } catch (System.Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "_getAsignaturasDocente");
            }

            return dsEvAcumulativa;
        }

        public string getHTML_EvaluacionAcumulativa()
        {
            string colorRow = "even";
            string html = string.Empty;

            WSGestorEvaluacion.dtstEvaluacion_Acumulados dsEvAcumulativa = this._CargarNotasEvAcumulativa();

            html += " <tr role='row' class='" + colorRow + "'>";
            html += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            html += " </tr>";

            if (dsEvAcumulativa.Acta.Rows.Count > 0)
            {
                html = string.Empty;
                string numMatricula = string.Empty;
                string promedio = string.Empty;
                string numNivel = string.Empty;

                foreach (DataRow item in dsEvAcumulativa.Acta) {
                    colorRow = (colorRow == "even") ? "odd" : "even";

                    numMatricula = this._getNumOrdinal(item["bytNumMat"].ToString(), "matricula");
                    numNivel = this._getNumOrdinal(item["strCodNivel"].ToString(), "nivel");
                    promedio = this._getPromedio(item);

                    html += " <tr id='" + item["strCodigo"] + "' role='row' class='" + colorRow + "'>";
                    html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center;'>" + item["No"] + "</td>";
                    html += "     <td style='width: 300px; align-content: center; vertical-align: middle; text-align: left;'>" + item["NombreEstudiante"].ToString().Trim() + "</td>";
                    html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center;'>" + numNivel + "</td>";
                    html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center;'>" + numMatricula + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center;'>" + item["bytNota1"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center;'>" + item["bytNota2"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center;'>" + item["bytNota3"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center;'>" + item["Total"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center;'>" + promedio + "</td>";
                    html += "     <td style='width: 40px;align-content: center; vertical-align: middle; text-align: center;'>" + item["bytAsistencia"] + "</td>";
                    html += "     <td style='width: 100px;align-content: center; vertical-align: middle; text-align: center;'>" + item["strObservaciones"] + "</td>";
                    html += " </tr>";
                }
            }

            return html;
        }

        public string getJson_EvaluacionActiva()
        {
            string ea = string.Empty;
            string opEdicionNota = string.Empty;
            string opEdicionAsistencia = string.Empty;
            string opFormatoFila = string.Empty;

            try
            {
                opEdicionNota = ", \"editable\": true, \"edittype\": \"text\", \"editoptions\": { \"size\": \"2\", \"maxlength\": \"2\" }, \"editrules\": { \"custom\": \"true\", \"custom_func\": \"validarNota\" }";
                opEdicionAsistencia = ", \"editable\": true, \"edittype\": \"text\", \"editoptions\": { \"size\": \"2\", \"maxlength\": \"4\" }, \"editrules\": { \"custom\": \"true\", \"custom_func\": \"validarAsistencia\" }";
                opFormatoFila = ", \"formatter\": { \"integer\" : { \"thousandsSeparator\": \"\" } }";

                ea += "[{ \"name\": \"No\", \"index\": \"No\", \"label\": \"No\", \"align\": \"center\", \"width\": \"30\", \"sortable\": false },";
                ea += "	{ \"name\": \"sintCodMatricula\", \"key\": true, \"hidden\": true },";
                ea += "	{ \"name\": \"NombreEstudiante\", \"label\": \""+ Language.es_ES.EST_TB_COL_ESTUDIANTE + "\", \"align\": \"left\", \"width\": \"300\", \"sortable\": false },";
                ea += "	{ \"name\": \"Nivel\", \"align\": \"center\", \"width\": \"70\", \"sortable\": false },";
                ea += "	{ \"name\": \"bytNumMat\", \"label\": \""+ Language.es_ES.EST_TB_COL_MATRICULA +"\", \"align\": \"center\", \"width\": \"70\", \"sortable\": false },";

                //  Gestion de Parcial 01
                if (this.parcialActivo == "1"){
                    ea += "	{ \"name\": \"bytNota1\", \"label\": \"" + Language.es_ES.EST_TB_COL_NOTA_UNO + "\", \"align\": \"center\", \"sortable\": false, \"width\": \"70\"" + opFormatoFila + opEdicionNota + "},";
                }else{
                    ea += "	{ \"name\": \"bytNota1\", \"label\": \"" + Language.es_ES.EST_TB_COL_NOTA_UNO + "\", \"sortable\": false, \"align\": \"center\", \"width\": \"70\"},";
                }

                //  Gestion de Parcial 02
                if (this.parcialActivo == "2"){
                    ea += "	{ \"name\": \"bytNota2\", \"label\": \"" + Language.es_ES.EST_TB_COL_NOTA_DOS + "\", \"align\": \"center\", \"sortable\": false, \"width\": \"70\" " + opEdicionNota + "},";
                }else{
                    ea += "	{ \"name\": \"bytNota2\", \"label\": \"" + Language.es_ES.EST_TB_COL_NOTA_DOS + "\", \"align\": \"center\", \"sortable\": false, \"width\": \"70\"},";
                }

                //  Gestion de Parcial 03
                if (this.parcialActivo == "3")
                {
                    ea += "	{ \"name\": \"bytNota3\", \"label\": \"" + Language.es_ES.EST_TB_COL_NOTA_TRES + "\", \"align\": \"center\", \"sortable\": false, \"width\": \"70\" " + opEdicionNota + "},";
                }else{
                    ea += "	{ \"name\": \"bytNota3\", \"label\": \"" + Language.es_ES.EST_TB_COL_NOTA_TRES + "\", \"align\": \"center\", \"sortable\": false, \"width\": \"70\"},";
                }

                ea += "	{ \"name\": \"Total\", \"label\": \"" + Language.es_ES.EST_TB_COL_TOTAL + "\", \"align\": \"center\", \"sortable\": false, \"width\": \"70\" },";

                //  Gestion de Porciento de asistencia
                ea += "	{ \"name\": \"bytAsistencia\", \"label\": \"" + Language.es_ES.EST_TB_COL_PORCIENTO_ASISTENCIA + "\", \"align\": \"center\", \"sortable\": false, \"width\": \"70\" " + opEdicionAsistencia + " },";

                ea += "	{ \"name\": \"strObservaciones\", \"label\": \"" + Language.es_ES.EST_TB_COL_OBSERVACION + "\", \"align\": \"left\", \"sortable\": false, \"width\": \"150\" }]";
            }
            catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getJson_EvaluacionActiva");
            }

            return ea;
        }
        

        #endregion


        #region EVALUCION FINAL

        private WSGestorEvaluacion.dtstEvaluacion_Actas _CargarNotasEvFinal()
        {
            WSGestorEvaluacion.dtstEvaluacion_Actas dsEvFinal = new WSGestorEvaluacion.dtstEvaluacion_Actas();
            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD);
                ge.set_fUbicacion(this._strUbicacion);

                dsEvFinal = ge.crearActaArtificialPrincipalR1(  this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                this._strCodAsignatura,
                                                                this._strCodNivel,
                                                                this._strCodParalelo);
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_CargarNotasEvFinal");
            }

            return dsEvFinal;
        }

        
        public string getHTML_EvaluacionFinal()
        {
            string colorRow = "even";
            string html = string.Empty;
            string alertaEquivalencia = "even";
            string lblEquivalencia = "default";
            string smsEquivalencia = "";

            WSGestorEvaluacion.dtstEvaluacion_Actas dsEvFinal = this._CargarNotasEvFinal();

            html += " <tr role='row' class='" + colorRow + "'>";
            html += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            html += " </tr>";

            try
            {
                if (dsEvFinal.Acta.Rows.Count > 0)
                {
                    html = string.Empty;
                    string numMatricula = string.Empty;
                    string promedio = string.Empty;
                    string numNivel = string.Empty;

                    foreach (DataRow item in dsEvFinal.Acta)
                    {
                        colorRow = (colorRow == "even") ? "odd" : "even";

                        numMatricula = this._getNumOrdinal(item["bytNumMat"].ToString(), "matricula");
                        numNivel = this._getNumOrdinal(item["strCodNivel"].ToString(), "nivel");
                        promedio = this._getPromedio(item);

                        alertaEquivalencia = this.getAlertaFila(colorRow,
                                                                item["strCodEquiv"].ToString(),
                                                                ref alertaEquivalencia,
                                                                ref lblEquivalencia,
                                                                ref smsEquivalencia);

                        html += " <tr id='" + item["strCodigo"] + "' class='" + alertaEquivalencia + "'>";
                        html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center;'>" + item["No"] + "</td>";
                        html += "     <td style='width: 300px; align-content: center; vertical-align: middle; text-align: left;'>" + item["NombreCompleto"].ToString().Trim() + "</td>";
                        html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center;'>" + numNivel + "</td>";
                        html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center;'>" + numMatricula + "</td>";

                        html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center;'>" + item["bytAcumulado"] + "</td>";
                        html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center;'>" + item["Total"] + "</td>";
                        html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center;'>" + item["strCodEquiv"] + "</td>";

                        html += "     <td style='width: 40px;align-content: center; vertical-align: middle; text-align: center;'>" + item["bytAsistencia"] + "</td>";
                        html += "     <td style='width: 100px;align-content: center; vertical-align: middle; text-align: center;'>" + item["strObservaciones"] + "</td>";
                        html += " </tr>";
                    }
                }
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getHTML_EvaluacionFinal");
            }
            
            return html;
        }

        
        #endregion




        private string _getNumOrdinal(string numero, string tpo)
        {
            string[] ciclosAcademicos = new string[10] { "1er", "2do", "3er", "4to", "5to", "6to", "7mo", "8vo", "9no", "10mo" };
            string[] matricula = new string[3] { "1ra", "2da", "3ra" };

            return (tpo == "nivel") ? ciclosAcademicos[Convert.ToInt32(numero.ToString()) - 1]
                                    : matricula[Convert.ToInt32(numero.ToString()) - 1];
        }


        private string _getPromedio(DataRow item)
        {
            string prm = string.Empty;
            decimal n1 = default(decimal);
            decimal n2 = default(decimal);
            decimal n3 = default(decimal);
            decimal rst = default(decimal);

            try
            {
                n1 = Convert.ToDecimal(item["bytNota1"]);
                n2 = Convert.ToDecimal(item["bytNota2"]);
                n3 = Convert.ToDecimal(item["bytNota3"]);
                rst = (n1 + n2 + n3);
                
                prm = Decimal.Round(Decimal.Divide(rst, Convert.ToDecimal("3")), 2).ToString();
            }
            catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getAsignaturasDocente");
            }

            return prm;
        }


        public string getAlertaFila(string colorRow, string equivalencia, ref string alertaEquivalencia, ref string lblEquivalencia, ref string smsEquivalencia)
        {
            string rst = colorRow;

            switch (equivalencia)
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
                    rst = alertaEquivalencia = lblEquivalencia = "danger";
                    smsEquivalencia = Language.es_ES.EST_LBL_EV_RECUPERACION.ToUpper();
                    break;

                //  EVALUACION FINAL
                case "EF":
                    rst = alertaEquivalencia = lblEquivalencia = "default";
                    smsEquivalencia = Language.es_ES.EST_LBL_EV_FINAL.ToUpper();
                    break;
            }

            return rst;
        }

    }
}