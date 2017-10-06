using GestorErrores;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SitioWebOasis.CommonClasses;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using SitioWebOasis.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;

namespace SitioWebOasis.Controllers
{
    [Authorize]
    [HandleError]
    public class DocentesController : Controller
    {
        // GET: Docentes
        public ActionResult Index( string idRol, string idCarrera )
        {
            string strIdCarrera = (string.IsNullOrEmpty(idCarrera))
                                    ? this._getCodigoCarrera()
                                    : idCarrera;

            SitioWebOasis.Models.DatosAcademicosDocente daDocente = new DatosAcademicosDocente(strIdCarrera);
            return View("Index", daDocente);
        }

        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        private string _getCodigoCarrera()
        {
            string idCarrera = string.Empty;

            try
            {
                ArrayList CarrerasEstudiante = UsuarioActual.GetCarreras(Roles.Docentes);

                foreach (Carrera item in CarrerasEstudiante){
                    if (item.TipoEntidad == "CAR"){
                        idCarrera = item.Codigo;
                        break;
                    }else if (item.TipoEntidad == "CAA"){
                        idCarrera = item.Codigo;
                        break;
                    }else{
                        idCarrera = item.Codigo;
                    }
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getIdCarrera");
            }

            return idCarrera;
        }
        

        public ActionResult EvaluacionAsignatura (string strCodNivel, string strCodAsignatura, string strCodParalelo )
        {
            try
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

                return View("GestionNotasDocente", new EvaluacionesDocenteModel{strCodNivel = strCodNivel,
                                                                                strCodAsignatura = strCodAsignatura,
                                                                                strCodParalelo = strCodParalelo,
                                                                                evAcumulativaModel = evAcumulativa,
                                                                                evFinalModel = evFinal,
                                                                                evRecuperacionModel = evRecuperacion });
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "EvaluacionAsignatura");

                return RedirectToAction("Index", "Error");
            }
            
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
        
        
        [HttpPost]
        public JsonResult impresionActas(string idActa, string idAsignatura)
        {
            try{
                string[] dtaActa = idActa.Split('_');
                string[] dtaAsignatura = idAsignatura.Split('|');

                string nameFile = string.Empty;
                switch (dtaActa[0].ToString().ToUpper()){
                    //  EVALUACION ACUMULATIVA ( Parcial 1 / Parcial 2 / Parcial 3 )
                    case "PEA":
                        EvaluacionAcumulativaModel evAcumulativa = new EvaluacionAcumulativaModel(  dtaAsignatura[1], 
                                                                                                    dtaAsignatura[0],
                                                                                                    dtaAsignatura[2]);

                        nameFile = evAcumulativa.getDtaRptEvAcumulativa(dtaActa, 
                                                                        dtaAsignatura, 
                                                                        Server.MapPath("~/Reports"), 
                                                                        Server.MapPath("~/Temp"));
                    break;

                    //  EVALUACION FINAL
                    case "PEF":
                        EvaluacionFinalModel evFinal = new EvaluacionFinalModel(dtaAsignatura[1],
                                                                                dtaAsignatura[0],
                                                                                dtaAsignatura[2]);

                        nameFile = evFinal.getDtaRptEvFinal(dtaActa,
                                                            dtaAsignatura,
                                                            Server.MapPath("~/Reports"),
                                                            Server.MapPath("~/Temp"));
                    break;
                        
                    //  EVALUACION RECUPERACION
                    case "PER":
                        EvaluacionRecuperacionModel evRecuperacion = new EvaluacionRecuperacionModel(   dtaAsignatura[1],
                                                                                                        dtaAsignatura[0],
                                                                                                        dtaAsignatura[2]);

                        nameFile = evRecuperacion.getDtaRptEvRecuperacion(  dtaActa,
                                                                            dtaAsignatura,
                                                                            Server.MapPath("~/Reports"),
                                                                            Server.MapPath("~/Temp"));
                    break;
                }

                if( nameFile == "-1"){
                    return Json(new { fileName = "", errorMessage = Language.es_ES.MSG_ERROR_GENERAR_ARCHIVO });
                }else{
                    return Json(new { fileName = nameFile, errorMessage = "" });
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "createFile");

                return Json(new { fileName = "none", errorMessage = Language.es_ES.MSG_ERROR_GENERAR_ARCHIVO });
            }
        }



        [HttpPost]
        [DeleteFileAttribute]
        public ActionResult DownloadFile(string file)
        {
            try{
                if (!string.IsNullOrEmpty(file) && file != "-1"){
                    //  get the temp folder and file path in server
                    string fullPath = Path.Combine(Server.MapPath("~/temp"), file);

                    //return the file for download, this is an Excel 
                    //so I set the file content type to "application/vnd.ms-excel"
                    return File(fullPath, "application/vnd.ms-excel", file);
                }else{
                    return RedirectToAction("Index", "Error");
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "Download File");

                return RedirectToAction("Index", "Error");
            }
        }

    }
}