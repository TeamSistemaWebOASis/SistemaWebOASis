using GestorErrores;
using Newtonsoft.Json;
using SitioWebOasis.Library;
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
                ge.setActaArtificialPrincipal(  this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
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

    }
}