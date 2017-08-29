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
    public class DocentesController : Controller
    {
        // GET: Docentes
        public ActionResult Index( string idRol, string idCarrera )
        {
            SitioWebOasis.Models.DatosAcademicosDocente daDocente = new DatosAcademicosDocente(idCarrera);
            return View("Index", daDocente);
        }



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

            return View("GestionNotasDocente", new EvaluacionesDocenteModel {   strCodNivel = strCodNivel,
                                                                                strCodAsignatura = strCodAsignatura,
                                                                                strCodParalelo = strCodParalelo,
                                                                                evAcumulativaModel = evAcumulativa,
                                                                                evFinalModel = evFinal });
        }


        [HttpPost]
        public ActionResult registrarEvaluacion(string strCodNivel, string strCodAsignatura, string strCodParalelo, string strParcialActivo, List<EvaluacionAcumulativa> dtaEvAcumulativa)
        {
            JsonResult rstDPA = default(JsonResult);
            try
            {
                switch (strParcialActivo)
                {
                    case "EF": break;
                    case "ER": break;
                    default:
                        EvaluacionAcumulativaModel evAcumulativa = new EvaluacionAcumulativaModel(  strCodNivel,
                                                                                                    strCodAsignatura,
                                                                                                    strCodParalelo);

                        if( dtaEvAcumulativa.Count > 0){
                            if( evAcumulativa.registrarEvaluacionAcumulativa(dtaEvAcumulativa)){
                                rstDPA = Json(  evAcumulativa.jsonEvAcumulativa, 
                                                JsonRequestBehavior.AllowGet);
                            }else{
                                rstDPA = Json( "false", JsonRequestBehavior.AllowGet);
                            }
                        }
                        
                    break;
                }
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionAcumulativa");
            }

            return rstDPA;
        }


        [HttpPost]
        public ActionResult registrarEvaluacionFinal(string strCodNivel, string strCodAsignatura, string strCodParalelo, string strParcialActivo, List<EvaluacionFinal> dtaEvFinal)
        {
            JsonResult rstDPA = default(JsonResult);
            try
            {
                EvaluacionFinalModel evFinal = new EvaluacionFinalModel(strCodNivel,
                                                                        strCodAsignatura,
                                                                        strCodParalelo);

                if (dtaEvFinal.Count > 0){
                    if (evFinal.registrarEvaluacionFinal(dtaEvFinal)){
                        rstDPA = Json(  evFinal.jsonEvFinal,
                                        JsonRequestBehavior.AllowGet);
                    }else{
                        rstDPA = Json(  "false", 
                                        JsonRequestBehavior.AllowGet);
                    }
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionFinal");
            }

            return rstDPA;
        }
    }
}