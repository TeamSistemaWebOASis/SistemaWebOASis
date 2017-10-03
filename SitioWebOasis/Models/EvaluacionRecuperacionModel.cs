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
    public class EvaluacionRecuperacionModel : EvaluacionesDocenteModel
    {
        public string jsonEvRecuperacion { get; set; }
        private WSGestorEvaluacion.dtstEvaluacion_Actas _dsEvRecuperacion = new WSGestorEvaluacion.dtstEvaluacion_Actas();

        public EvaluacionRecuperacionModel(string strCodNivel, string strCodAsignatura, string strCodParalelo)
        {
            this._strCodNivel = strCodNivel;
            this._strCodAsignatura = strCodAsignatura;
            this._strCodParalelo = strCodParalelo;
            this._cargarInformacionCarrera();
            this._dsEvRecuperacion = this._CargarNotasEvRecuperacion();

            this.jsonEvRecuperacion = (this._dsEvRecuperacion.Acta.Rows.Count > 0)
                                        ? JsonConvert.SerializeObject(this._dsEvRecuperacion.Acta)
                                        : "";
        }


        private WSGestorEvaluacion.dtstEvaluacion_Actas _CargarNotasEvRecuperacion()
        {
            WSGestorEvaluacion.dtstEvaluacion_Actas rstEvRecuperacion = new WSGestorEvaluacion.dtstEvaluacion_Actas();
            WSGestorEvaluacion.dtstEvaluacion_Actas dsEvRecuperacion = new WSGestorEvaluacion.dtstEvaluacion_Actas();

            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD.ToString());
                ge.set_fUbicacion(this._strUbicacion.ToString());

                rstEvRecuperacion = ge.crearActaArtificialSuspension(   this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                        this._strCodAsignatura,
                                                                        this._strCodNivel,
                                                                        this._strCodParalelo );
                dsEvRecuperacion = (rstEvRecuperacion != null) 
                                        ? rstEvRecuperacion 
                                        : new WSGestorEvaluacion.dtstEvaluacion_Actas();
            }
            catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_CargarNotasEvRecuperacion");
            }

            return dsEvRecuperacion;
        }


        public bool registrarEvaluacionRecuperacion(List<EvaluacionRecuperacion> dtaEvRecuperacion)
        {
            bool rst = false;

            try
            {
                //  Recorro DataTable Acta - registro por registro actualizando la informacion
                if (this._updEvRecuperacion(dtaEvRecuperacion))
                {
                    if (this._guardarEvRecuperacion()){
                        this._dsEvRecuperacion = this._CargarNotasEvRecuperacion();
                        this.jsonEvRecuperacion = (this._dsEvRecuperacion.Acta.Rows.Count > 0)
                                                        ? JsonConvert.SerializeObject(this._dsEvRecuperacion.Acta)
                                                        : "";

                        rst = true;
                    }
                }
            }catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionRecuperacion");
            }

            return rst;
        }



        private bool _updEvRecuperacion(List<EvaluacionRecuperacion> dtaEvRecuperacion)
        {
            bool rst = false;

            try
            {
                int numReg = dtaEvRecuperacion.Count;
                if (numReg > 0 && this._dsEvRecuperacion.Acta.Rows.Count > 0){
                    for (int x = 0; x < numReg; x++){
                        if (this._dsEvRecuperacion.Acta.Rows[0]["sintCodMatricula"].ToString() == dtaEvRecuperacion[0].sintCodMatricula.ToString()){
                            this._dsEvRecuperacion.Acta.Rows[x]["bytNota"] = Convert.ToByte(dtaEvRecuperacion[x].bytNota.ToString());
                        }
                    }

                    this._dsEvRecuperacion.AcceptChanges();
                    rst = true;
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_updEvRecuperacion");
            }

            return rst;
        }


        private bool _guardarEvRecuperacion()
        {
            bool rst = false;
            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fUbicacion(this._strUbicacion);
                ge.set_fBaseDatos(this._strNombreBD);

                //  Registro informacion de evaluacion recuperacion
                rst = ge.setActaArtificialSuspension(   this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        this._strCodAsignatura.ToString(),
                                                        this._strCodNivel.ToString(),
                                                        this._strCodParalelo.ToString(),
                                                        this._dsEvRecuperacion,
                                                        this.UsuarioActual.Nombre.ToString());

                //  Actualizar fecha de registro de notas
                ge.ActualizarRegistroFechaIngresoExSuspension(  this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                this._strCodAsignatura.ToString(),
                                                                this._strCodNivel.ToString(),
                                                                this._strCodParalelo.ToString(),
                                                                DateTime.Now);
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_guardarEvFinal");
            }

            return rst;
        }





        public string getHTML_EvaluacionRecuperacion()
        {
            string colorRow = "odd";
            string html = string.Empty;

            html += " <tr role='row' class='" + colorRow + "'>";
            html += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            html += " </tr>";

            if (this._dsEvRecuperacion.Acta.Rows.Count > 0)
            {
                html = string.Empty;
                string numMatricula = string.Empty;
                string promedio = string.Empty;
                string numNivel = string.Empty;
                string estadoCumplimiento = string.Empty;

                foreach (DataRow item in this._dsEvRecuperacion.Acta)
                {
                    colorRow = (colorRow == "even") ? "odd" : "even";

                    numMatricula = this._getNumOrdinal(item["bytNumMat"].ToString(), "matricula");
                    numNivel = this._getNumOrdinal(item["strCodNivel"].ToString(), "nivel");
                    estadoCumplimiento = this._getEstadoCumplimiento(   Convert.ToByte(item["Total"].ToString()),
                                                                        item["strCodEquiv"].ToString());

                    html += " <tr id='" + item["strCodigo"] + "' role='row' class='" + colorRow + "'>";
                    html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["No"] + "</td>";
                    html += "     <td style='width: 300px; align-content: center; vertical-align: middle; text-align: left; font-size: 12px;'>" + item["NombreCompleto"].ToString().Trim().ToUpper() + "</td>";
                    html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + numMatricula + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytAcumulado"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytNota"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["Total"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + estadoCumplimiento + "</td>";
                    html += "     <td style='width: 100px;align-content: center; vertical-align: middle; text-align: center;font-size: 12px;'>" + item["strObservaciones"] + "</td>";
                    html += " </tr>";
                }
            }

            return html;
        }



        private string _getEstadoCumplimiento(byte total, string strCodEquivalencia)
        {
            string estCumplimiento = "---";

            try
            {
                //  APROBADO
                if (total >= 28)
                {
                    estCumplimiento = "<span class='label label-success'>" + Language.es_ES.EST_LBL_APROBADO.ToUpper() + "</span>";
                }
                
                //  REPROBADO
                if (total < 28 )
                {
                    estCumplimiento = "<span class='label label-danger'>" + Language.es_ES.LBL_CUMPLIMIENTO_REPROBADO + "</span>"; ;
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