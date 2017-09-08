using GestorErrores;
using Newtonsoft.Json;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using SitioWebOasis.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SitioWebOasis.Controllers
{
    [Authorize]
    [HandleError]
    public class DocentesController : Controller
    {
        // GET: Docentes
        public ActionResult Index( string idRol, string idCarrera )
        {
            SitioWebOasis.Models.DatosAcademicosDocente daDocente = new DatosAcademicosDocente(idCarrera);
            return View("Index", daDocente);
        }


        [OutputCache(Duration = 8000, VaryByParam = "strCodNivel, strCodAsignatura, strCodParalelo")]
        public ActionResult EvaluacionAsignatura (string strCodNivel, string strCodAsignatura, string strCodParalelo )
        {
            //  modelo - EVALUACION ACUMULATIVA
            EvaluacionAcumulativaModel evAcumulativa = new EvaluacionAcumulativaModel(  strCodNivel,
                                                                                        strCodAsignatura,
                                                                                        strCodParalelo);

            //  modelo - EVALUACION FINAL
            EvaluacionFinalModel evFinal = new EvaluacionFinalModel(strCodNivel,
                                                                    strCodAsignatura,
                                                                    strCodParalelo);

            //  modelo - EVALUACION RECUPERACION
            EvaluacionRecuperacionModel evRecuperacion = new EvaluacionRecuperacionModel(   strCodNivel,
                                                                                            strCodAsignatura,
                                                                                            strCodParalelo);

            return View("GestionNotasDocente", new EvaluacionesDocenteModel {   strCodNivel = strCodNivel,
                                                                                strCodAsignatura = strCodAsignatura,
                                                                                strCodParalelo = strCodParalelo,
                                                                                evAcumulativaModel = evAcumulativa,
                                                                                evFinalModel = evFinal,
                                                                                evRecuperacionModel = evRecuperacion });
        }


        [HttpPost]
        public ActionResult registrarEvaluacion(string strCodNivel, string strCodAsignatura, string strCodParalelo, string strParcialActivo, List<EvaluacionAcumulativa> dtaEvAcumulativa)
        {
            JsonResult rstEvAcumulativa = default(JsonResult);
            try
            {
                EvaluacionAcumulativaModel evAcumulativa = new EvaluacionAcumulativaModel(  strCodNivel,
                                                                                            strCodAsignatura,
                                                                                            strCodParalelo);

                if( dtaEvAcumulativa.Count > 0){
                    if( evAcumulativa.registrarEvaluacionAcumulativa(dtaEvAcumulativa)){
                        rstEvAcumulativa = Json(evAcumulativa.jsonEvAcumulativa, 
                                                JsonRequestBehavior.AllowGet);
                    }else{
                        rstEvAcumulativa = Json("false", 
                                                JsonRequestBehavior.AllowGet);
                    }
                }       
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionAcumulativa");
            }

            return rstEvAcumulativa;
        }


        [HttpPost]
        public ActionResult registrarEvaluacionFinal(string strCodNivel, string strCodAsignatura, string strCodParalelo, string strParcialActivo, List<EvaluacionFinal> dtaEvFinal)
        {
            JsonResult rstEvFinal = default(JsonResult);
            try
            {
                EvaluacionFinalModel evFinal = new EvaluacionFinalModel(strCodNivel,
                                                                        strCodAsignatura,
                                                                        strCodParalelo);

                if (dtaEvFinal.Count > 0){
                    if (evFinal.registrarEvaluacionFinal(dtaEvFinal)){
                        rstEvFinal = Json(  evFinal.jsonEvFinal,
                                            JsonRequestBehavior.AllowGet);
                    }else{
                        rstEvFinal = Json(  "false", 
                                            JsonRequestBehavior.AllowGet);
                    }
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionFinal");
            }

            return rstEvFinal;
        }


        [HttpPost]
        public ActionResult registrarEvaluacionRecuperacion(string strCodNivel, string strCodAsignatura, string strCodParalelo, string strParcialActivo, List<EvaluacionRecuperacion> dtaEvRecuperacion)
        {
            JsonResult rstEvRecuperacion = default(JsonResult);
            try
            {
                EvaluacionRecuperacionModel evRecuperacion = new EvaluacionRecuperacionModel(   strCodNivel,
                                                                                                strCodAsignatura,
                                                                                                strCodParalelo);

                if (dtaEvRecuperacion.Count > 0)
                {
                    if (evRecuperacion.registrarEvaluacionRecuperacion(dtaEvRecuperacion)){
                        rstEvRecuperacion = Json(   evRecuperacion.jsonEvRecuperacion,
                                                    JsonRequestBehavior.AllowGet);
                    }else{
                        rstEvRecuperacion = Json(  "false",
                                                    JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionFinal");
            }

            return rstEvRecuperacion;
        }


    }
}