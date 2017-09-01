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
                if (numReg > 0 && this._dsEvRecuperacion.Acta.Rows.Count > 0)
                {
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
    }
}