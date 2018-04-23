using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Library
{
    public class ConsolidadoEvaluacionAsignatura
    {
        private int _numElemento = 0;
        private DataRow _item = null;
        private DataRow[] _rectificacion = null;

        public ConsolidadoEvaluacionAsignatura( int elemento, DataRow regConsolidadoAsignatura )
        {
            this._numElemento = elemento;
            this._item = regConsolidadoAsignatura;
        }


        public ConsolidadoEvaluacionAsignatura( int elemento, DataRow regConsolidadoAsignatura, DataRow[] rectificacion )
        {
            this._numElemento = elemento;
            this._item = regConsolidadoAsignatura;
            this._rectificacion = rectificacion;
        }


        public string getRowAsignatura()
        {
            string rst = string.Empty;

            string lblEquivalencia_EP = "default";
            string smsEquivalencia_EP = "";
            string alertaEquivalencia_EP = string.Empty;

            string lblEquivalencia_ES = "default";
            string smsEquivalencia_ES = "";
            string alertaEquivalencia_ES = string.Empty;
            string contenido = string.Empty;
            string tipoExamen = string.Empty;

            string eqEA = (!string.IsNullOrEmpty(this._item["eq_AC"].ToString())) 
                            ? this._item["eq_AC"].ToString()
                            : this._item["eq_EP"].ToString();

            alertaEquivalencia_EP = this._getAlertaFila(eqEA,
                                                        this._item["strCodMateria"].ToString(),
                                                        ref alertaEquivalencia_EP,
                                                        ref lblEquivalencia_EP,
                                                        ref smsEquivalencia_EP);

            alertaEquivalencia_ES = this._getAlertaFila(this._item["eq_ES"].ToString(),
                                                        this._item["strCodMateria"].ToString(),
                                                        ref alertaEquivalencia_ES,
                                                        ref lblEquivalencia_ES,
                                                        ref smsEquivalencia_ES);

            rst += " <tr role='row' style='align-content: center; vertical-align: middle; text-align: center;'>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this._numElemento + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: left;'>" + this._item["strNombre"].ToString() + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this._getNumOrdinal(this._item["bytNumMat"].ToString(), "") + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this._getNumOrdinal(this._item["strCodNivel"].ToString(), "nivel") + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this._item["strCodParalelo"].ToString() + "</td>";

            //  notas acumulado
            rst += "    <td style='border-left: 1px solid darkgray; align-content: center; vertical-align: middle; text-align: center;'>" + this._item["bytNota1"].ToString() + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this._item["bytNota2"].ToString() + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this._item["bytNota3"].ToString() + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + this._item["bytAsistencia"].ToString() + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'><strong>" + this._item["acumulado_EP"].ToString() + "</strong></td>";

            //  nota evaluacion final (principal)
            rst += "    <td style='border-left: 1px dotted darkgray; align-content: center; vertical-align: middle; text-align: center;'>" + this._item["nota_EP"].ToString() + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'><strong>" + this._item["TOTAL_EP"].ToString() + "</strong></td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'> <span class='label label-" + lblEquivalencia_EP + "'>" + smsEquivalencia_EP + "</span> </td>";

            //  nota evaluacion recuperacion (suspension)
            rst += "    <td style='border-left: 1px dotted darkgray; align-content: center; vertical-align: middle; text-align: center;'>" + this._item["acumulado_ES"].ToString() + "</td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'><strong>" + this._item["nota_ES"].ToString() + "</strong></td>";
            rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'><strong>" + this._item["TOTAL_ES"].ToString() + "</strong></td>";
            rst += "    <td style='border-right: 1px solid darkgray; align-content: center; vertical-align: middle; text-align: center;'> <span class='label label-" + lblEquivalencia_ES + "'>" + smsEquivalencia_ES + "</span> </td>";

            rst += this._getMensajeObservacion( this._item );
            
            rst += " </tr>";

            return rst;
        }


        private string _getMensajeObservacion( DataRow item )
        {
            string mensaje = "<td style='align-content: center; vertical-align: middle; text-align: center;'>---</td>";

            if(this._rectificacion != null){
                mensaje = this._getMensajeRectificacion();
            }else if ( Convert.ToInt16( item["acumulado_EP"].ToString() ) < 4 && Convert.ToInt16(item["bytAsistencia"].ToString()) >= 70){
                mensaje = this._getMensajeReprobadoEvAcumulativa();
            }else if( Convert.ToInt16( item["Total_EP"].ToString() ) < 16 && Convert.ToInt16(item["bytAsistencia"].ToString()) >= 70 ){
                mensaje = this._getMensajeReprobadoEvFinal();
            }else if( item["eq_AC"].ToString().CompareTo("RF") == 0 ){
                mensaje = this._getMensajeRepruebaFaltas();
            }else if (item["eq_EP"].ToString().CompareTo("E") == 0){
                mensaje = this._getMensajeExonerado();
            }

            return mensaje;
        }



        private string _getMensajeRectificacion()
        {
            string tipoExamen = (this._rectificacion[0]["strCodTipoExamen"].ToString().CompareTo("PRI") == 0)
                                ? Language.es_ES.NOTAS_TB_COL_EV_FINAL
                                : Language.es_ES.NOTAS_TB_COL_EV_RECUPERACION;

            string contenido = "Tipo de examen: <strong>{0}</strong> <br> Resolución: <strong>{1}</strong> <br> Fecha aprobación: <strong>{2}</strong>";
            contenido = string.Format(  contenido,
                                        tipoExamen,
                                        this._rectificacion[0]["strResolucion"].ToString(),
                                        Convert.ToDateTime(this._rectificacion[0]["dtFechaAprob"]).ToString("dd/MM/yyyy"));

            return "<td style='align-content: center; vertical-align: middle; text-align: center;'> <button type='button' rel=popover id='popover-title' class='btn btn-info btn-xs' data-content='" + contenido + "' data-original-title='" + Language.es_ES.STR_RECTIFICACION_NOTAS + "'> <i class='fa fa-eye'></i> </button> </td>";
        }


        private string _getMensajeRepruebaFaltas()
        {
            string contenido = "<strong>" + Language.es_ES.STR_ART_REPROBADO_FALTAS + "</strong> <br> {0}";
            contenido = string.Format(contenido, Language.es_ES.STR_RRA_REPRUEBA_FALTAS.ToString());

            return "    <td style='align-content: center; vertical-align: middle; text-align: center;'> <button type='button' rel=popover id='popover-title' class='btn btn-info btn-xs' data-content='" + contenido + "' data-original-title='" + Language.es_ES.STR_REPRUEBA_FALTAS_NOTAS + "'> <i class='fa fa-eye'></i> </button> </td>";
        }


        private string _getMensajeExonerado()
        {
            string contenido = "<strong>" + Language.es_ES.STR_ART_EXONERADO + "</strong> <br> {0}";
            contenido = string.Format(contenido, Language.es_ES.STR_RRA_EXONERADO.ToString());

            return "<td style='align-content: center; vertical-align: middle; text-align: center;'> <button type='button' rel=popover id='popover-title' class='btn btn-info btn-xs' data-content='" + contenido + "' data-original-title='" + Language.es_ES.STR_EXONERADO + "'> <i class='fa fa-eye'></i> </button> </td>";
        }


        private string _getMensajeReprobadoEvAcumulativa()
        {
            string contenido = "<strong>" + Language.es_ES.STR_ART_REPROBADO_EV_ACUMULATIVA + "</strong> <br> {0}";
            contenido = string.Format(contenido, Language.es_ES.STR_RRA_REPROBADO_EV_ACUMULATIVA.ToString());

            return "<td style='align-content: center; vertical-align: middle; text-align: center;'> <button type='button' rel=popover id='popover-title' class='btn btn-info btn-xs' data-content='" + contenido + "' data-original-title='" + Language.es_ES.STR_REPROBADO_EV_ACUMULATIVA + "'> <i class='fa fa-eye'></i> </button> </td>";
        }


        private string _getMensajeReprobadoEvFinal()
        {
            string contenido = "<strong>" + Language.es_ES.STR_ART_REPROBADO_EV_FINAL + "</strong> <br> {0}";
            contenido = string.Format(contenido, Language.es_ES.STR_RRA_REPROBADO_EV_FINAL.ToString());

            return "<td style='align-content: center; vertical-align: middle; text-align: center;'> <button type='button' rel=popover id='popover-title' class='btn btn-info btn-xs' data-content='" + contenido + "' data-original-title='" + Language.es_ES.STR_REPROBADO_EV_FINAL + "'> <i class='fa fa-eye'></i> </button> </td>";
        }


        public string getRowRetiroConsolidado()
        {
            string rst = string.Empty;

            rst += " <tr class='danger' role='row' style='align-content: center; vertical-align: middle; text-align: center;'>";
            rst += "    <td>" + this._numElemento + "</td>";
            rst += "    <td style='text-align: left;'>" + this._item["strNombre"].ToString() + "</td>";

            rst += "    <td>" + this._getNumOrdinal(this._item["strCodNivel"].ToString(), "nivel") + "</td>";
            rst += "    <td>" + this._getNumOrdinal(this._item["bytNumMat"].ToString(), "") + "</td>";
            rst += "    <td colspan='12' style='border-left: 1px solid darkgray; border-right: 1px solid darkgray; align-content: center; vertical-align: middle; text-align: center;'><strong>" + Language.es_ES.STR_RETIRO_ASIGNATURA + "</strong></td>";

            if (!string.IsNullOrEmpty(this._item["strResolucion"].ToString())){
                string contenido = "Resolucion: <strong>{0}</strong> <br> Fecha aprobacion: <strong>{1}</strong>";
                contenido = string.Format(contenido, this._item["strResolucion"].ToString(), Convert.ToDateTime(this._item["dtFechaAprob"]).ToString("dd/MM/yyyy"));
                rst += "    <td> <button type='button' rel=popover id='popover-title' class='btn btn-info btn-xs' data-content='" + contenido + "' data-original-title='"+ Language.es_ES.STR_RETIRO_ASIGNATURA + "'> <i class='fa fa-eye'></i> </button> </td>";
            }

            rst += " </tr>";

            return rst;
        }


        public string getRowConvalidado()
        {
            string rst = string.Empty;

            rst += " <tr class='success' role='row' style='align-content: center; vertical-align: middle; text-align: center;'>";
            rst += "    <td>" + this._numElemento + "</td>";
            rst += "    <td style='text-align: left;'>" + this._item["asignatura"].ToString() + "</td>";
            rst += "    <td>" + this._getNumOrdinal(this._item["strCodNivel"].ToString(), "nivel") + "</td>";
            rst += "    <td>" + this._getNumOrdinal(this._item["bytNumMat"].ToString(), "") + "</td>";
            rst += "    <td colspan='12' style='border-left: 1px solid darkgray; border-right: 1px solid darkgray; align-content: center; vertical-align: middle; text-align: center;'><strong>" + Language.es_ES.STR_RETIRO_CONVALIDA + "</strong></td>";

            if (!string.IsNullOrEmpty(this._item["strResolucion"].ToString())){
                string contenido = "Resolucion: <strong>{0}</strong> <br> Institución: <strong>{1}</strong> <br> Fecha aprobacion: <strong>{2}</strong>";
                contenido = string.Format(  contenido, 
                                            this._item["strResolucion"].ToString(),
                                            this._item["institucion"].ToString(),
                                            Convert.ToDateTime(this._item["fchAprobacion"]).ToString("dd/MM/yyyy"));

                rst += "    <td> <button type='button' rel=popover id='popover-title' class='btn btn-info btn-xs' data-content='" + contenido + "' data-original-title='" + Language.es_ES.STR_RETIRO_CONVALIDA + "'> <i class='fa fa-eye'></i> </button> </td>";
            }

            rst += " </tr>";

            return rst;
        }


        private string _getAlertaFila(string strCodEquivalencia, string strCodMateria, ref string alertaEquivalencia, ref string lblEquivalencia, ref string smsEquivalencia)
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
                case "EX":
                    rst = alertaEquivalencia = lblEquivalencia = "primary";
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


                //  REPROBADO FALTAS
                case "RF":
                    rst = alertaEquivalencia = lblEquivalencia = "danger";
                    smsEquivalencia = Language.es_ES.EST_LBL_REPROBADO_FALTAS.ToUpper().ToUpper();
                break;
            }

            return estructura + rst;
        }


        private string _getNumOrdinal(string numero, string tpo)
        {
            string[] ciclosAcademicos = new string[11] { "0", "1er", "2do", "3er", "4to", "5to", "6to", "7mo", "8vo", "9no", "10mo" };
            string[] matricula = new string[3] { "1ra", "2da", "3ra" };

            return (tpo == "nivel") ? ciclosAcademicos[Convert.ToInt32(numero.ToString())]
                                    : matricula[Convert.ToInt32(numero.ToString()) - 1];
        }

    }
}