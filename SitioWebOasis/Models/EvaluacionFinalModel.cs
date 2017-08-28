using GestorErrores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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


    }
}