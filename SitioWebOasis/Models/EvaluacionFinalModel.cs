using GestorErrores;
using Newtonsoft.Json;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;

namespace SitioWebOasis.Models
{
    public class EvaluacionFinalModel: EvaluacionesDocenteModel
    {
        public string jsonEvFinal { get; set; }
        private WSGestorEvaluacion.dtstEvaluacion_Actas _dsEvFinal = new WSGestorEvaluacion.dtstEvaluacion_Actas();


        public EvaluacionFinalModel(string strCodNivel, string strCodAsignatura, string strCodParalelo)
        {
            this._strCodAsignatura = strCodAsignatura;
            this._strCodNivel = strCodNivel;
            this._strCodParalelo = strCodParalelo;
            this._cargarInformacionCarrera();
            this._dsEvFinal = this._CargarNotasEvFinal();

            this.jsonEvFinal = (this._dsEvFinal.Acta.Rows.Count > 0)
                                    ? JsonConvert.SerializeObject(this._dsEvFinal.Acta)
                                    : "";
        }


        private WSGestorEvaluacion.dtstEvaluacion_Actas _CargarNotasEvFinal()
        {
            WSGestorEvaluacion.dtstEvaluacion_Actas dsEvFinal = new WSGestorEvaluacion.dtstEvaluacion_Actas();
            WSGestorEvaluacion.dtstEvaluacion_Actas rstEvFinal = new WSGestorEvaluacion.dtstEvaluacion_Actas();

            try
            {
                ProxySeguro.GestorEvaluacion gev = new ProxySeguro.GestorEvaluacion();
                gev.CookieContainer = new CookieContainer();
                gev.set_fBaseDatos(this._strNombreBD.ToString());
                gev.set_fUbicacion(this._strUbicacion.ToString());
                
                rstEvFinal = gev.crearActaArtificialPrincipalR1(    this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                    this._strCodAsignatura, 
                                                                    this._strCodNivel,
                                                                    this._strCodParalelo);

                dsEvFinal = (rstEvFinal != null)? rstEvFinal 
                                                : new WSGestorEvaluacion.dtstEvaluacion_Actas();
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_CargarNotasEvFinal");
            }

            return dsEvFinal;
        }


        public bool registrarEvaluacionFinal(List<EvaluacionFinal> dtaEvFinal)
        {
            bool rst = false;

            try{
                //  Recorro DataTable Acta - registro por registro actualizando la informacion
                if (this._updEvAcumulativa(dtaEvFinal)){
                    if (this._guardarEvFinal()){
                        this._dsEvFinal = this._CargarNotasEvFinal();
                        this.jsonEvFinal = (this._dsEvFinal.Acta.Rows.Count > 0)
                                                ? JsonConvert.SerializeObject(this._dsEvFinal.Acta)
                                                : "";

                        rst = true;
                    }                    
                }
            }catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionAcumulativa");
            }

            return rst;
        }
        

        private bool _updEvAcumulativa(List<EvaluacionFinal> dtaEvFinal)
        {
            bool rst = false;

            try{
                int numReg = dtaEvFinal.Count;
                if ( numReg > 0 && this._dsEvFinal.Acta.Rows.Count > 0 ){
                    for (int x = 0; x < numReg; x++){
                        if (this._dsEvFinal.Acta.Rows[0]["sintCodMatricula"].ToString() == dtaEvFinal[0].sintCodMatricula.ToString()){
                            this._dsEvFinal.Acta.Rows[x]["bytNota"] = Convert.ToByte(dtaEvFinal[x].bytNota.ToString());
                        }
                    }

                    this._dsEvFinal.AcceptChanges();
                    rst = true;
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_updEvEstudiantes");
            }

            return rst;
        }


        private bool _guardarEvFinal()
        {
            bool rst = false;
            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fUbicacion(this._strUbicacion);
                ge.set_fBaseDatos(this._strNombreBD);

                //  Registro informacion de evaluacion final
                rst = ge.setActaArtificialPrincipal(this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                    this._strCodAsignatura.ToString(),
                                                    this._strCodNivel.ToString(),
                                                    this._strCodParalelo.ToString(), 
                                                    this._dsEvFinal,
                                                    this.UsuarioActual.Nombre.ToString());

                //  Actualizar fecha de registro de notas
                ge.ActualizarRegistroFechaIngresoExPrincipal(   this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                this._strCodAsignatura.ToString(),
                                                                this._strCodNivel.ToString(),
                                                                this._strCodParalelo.ToString(),
                                                                DateTime.Now);
            }
            catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_guardarEvFinal");
            }

            return rst;
        }


        public string getHTML_EvaluacionFinal()
        {
            string colorRow = "odd";
            string html = string.Empty;

            html += " <tr role='row' class='" + colorRow + "'>";
            html += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            html += " </tr>";

            if (this._dsEvFinal.Acta.Rows.Count > 0)
            {
                html = string.Empty;
                string numMatricula = string.Empty;
                string promedio = string.Empty;
                string numNivel = string.Empty;
                string estadoCumplimiento = string.Empty;

                foreach (DataRow item in this._dsEvFinal.Acta)
                {
                    colorRow = (colorRow == "even") ? "odd" : "even";

                    numMatricula = this._getNumOrdinal(item["bytNumMat"].ToString(), "matricula");
                    numNivel = this._getNumOrdinal(item["strCodNivel"].ToString(), "nivel");
                    estadoCumplimiento = this._getEstadoCumplimiento(   Convert.ToByte(item["Total"].ToString()),
                                                                        Convert.ToByte(item["bytAsistencia"].ToString()),
                                                                        item["strCodEquiv"].ToString());

                    html += " <tr id='" + item["strCodigo"] + "' role='row' class='" + colorRow + "'>";
                    html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["No"] + "</td>";
                    html += "     <td style='width: 300px; align-content: center; vertical-align: middle; text-align: left; font-size: 12px;'>" + item["NombreCompleto"].ToString().Trim().ToUpper() + "</td>";
                    html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + numMatricula + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytAcumulado"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytAsistencia"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytNota"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["Total"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + estadoCumplimiento + "</td>";
                    html += "     <td style='width: 100px;align-content: center; vertical-align: middle; text-align: center;font-size: 12px;'>" + item["strObservaciones"] + "</td>";
                    html += " </tr>";
                }
            }

            return html;
        }



        private string _getEstadoCumplimiento( byte total, byte asistencia, string strCodEquivalencia )
        {
            string estCumplimiento = "---";

            try{
                //  APROBADO
                if ( total >= 28 && asistencia >= 70){
                    estCumplimiento = "<span class='label label-success'>" + Language.es_ES.EST_LBL_APROBADO.ToUpper() + "</span>";
                }

                //  EVALUACION RECUPERACION
                if (total >= 16 && total < 28 && asistencia >= 70){
                    estCumplimiento = "<span class='label label-warning'>" + Language.es_ES.EST_LBL_EV_RECUPERACION + "</span>"; ;
                }

                //  REPROBADO - FALTAS
                if (asistencia < 70 && strCodEquivalencia == "R"){
                    estCumplimiento = "<span class='label label-danger'>" + Language.es_ES.LBL_CUMPLIMIENTO_REPROBADO_FALTAS + "</span>"; ;
                }

                //  REPROBADO
                if ( total < 16 && asistencia >= 70 ){
                    estCumplimiento = "<span class='label label-danger'>" + Language.es_ES.LBL_CUMPLIMIENTO_REPROBADO + "</span>"; ;
                }

                //  EXONERADO
                if (total >= 28 && asistencia >= 70 && strCodEquivalencia == "E"){
                    estCumplimiento = "<span class='label label-success'>" + Language.es_ES.LBL_CUMPLIMIENTO_EXONERADO + "</span>"; ;
                }
            }
            catch (Exception ex)
            {
                estCumplimiento = "---";
                Errores err = new Errores();
                err.SetError(ex, "_guardarEvAcumulativa");
            }

            return estCumplimiento;
        }

    }
}