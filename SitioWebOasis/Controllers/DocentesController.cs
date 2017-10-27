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
        public ActionResult Index( string strCodCarrera )
        {
            string _strCodCarrera = (string.IsNullOrEmpty(strCodCarrera))
                                        ? this._getCodigoCarrera()
                                        : strCodCarrera;

            SitioWebOasis.Models.DatosAsignaturasDocenteModel daDocente = new DatosAsignaturasDocenteModel(_strCodCarrera);
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


        public ActionResult EvaluacionAsignatura(string strCodNivel, string strCodAsignatura, string strCodParalelo)
        {
            try
            {
                EvaluacionesDocenteModel edm = new EvaluacionesDocenteModel(strCodNivel,
                                                                            strCodAsignatura,
                                                                            strCodParalelo);

                return View("GestionNotasDocente", edm);
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "EvaluacionAsignatura");

                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public JsonResult registrarEvaluacion(string strCodNivel, string strCodAsignatura, string strCodParalelo, string strParcialActivo, List<EvaluacionAcumulativa> dtaEvAcumulativa)
        {
            JsonResult rstGestionEvAcumulativa = default(JsonResult);
            try{
                EvaluacionAcumulativaModel evAcumulativa = new EvaluacionAcumulativaModel(  strCodNivel,
                                                                                            strCodAsignatura,
                                                                                            strCodParalelo);

                if( dtaEvAcumulativa.Count > 0){
                    if( evAcumulativa.registrarEvaluacionAcumulativa(dtaEvAcumulativa)){
                        rstGestionEvAcumulativa = Json(new {dtaEvAcumulativaUpd = evAcumulativa.jsonEvAcumulativa,
                                                            MessageGestion = Language.es_ES.MSG_REGISTRO_EV_ACUMULATIVA_CORRECTA });
                    }
                    else{
                        rstGestionEvAcumulativa = Json(new{ dtaEvAcumulativaUpd = "false",
                                                            MessageGestion = Language.es_ES.MSG_REGISTRO_EV_ACUMULATIVA_ERROR
                        });
                    }
                }       
            }catch(Exception ex){
                rstGestionEvAcumulativa = Json(new{ dtaEvAcumulativaUpd = "false",
                                                    MessageGestion = Language.es_ES.MSG_REGISTRO_EV_ACUMULATIVA_ERROR });

                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionAcumulativa");
            }

            return rstGestionEvAcumulativa;
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
        public JsonResult impresionNominaEstudiantes( string strCodAsignatura, string strCodTipoArchivo)
        {
            string nameFile = "-1";
            try{
                string[] dtaAsignatura = strCodAsignatura.Split('_');
                string tipoArchivo = (strCodTipoArchivo.ToUpper().CompareTo("EXCEL") == 0) ? "Excel" : "pdf";

                NominaEstudiantesModel NominaEstudiantes = new NominaEstudiantesModel(  dtaAsignatura[0], 
                                                                                        dtaAsignatura[1], 
                                                                                        dtaAsignatura[2]);

                nameFile = NominaEstudiantes.getNameFileRptNominaEstudiantes(   tipoArchivo,
                                                                                Server.MapPath("~/Reports"),
                                                                                Server.MapPath("~/Temp"));

                if (nameFile == "-1"){
                    return Json(new { fileName = "", errorMessage = Language.es_ES.MSG_ERROR_GENERAR_ARCHIVO });
                }else{
                    return Json(new { fileName = nameFile, errorMessage = "" });
                }
            }
            catch (Exception ex) {
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


        [HttpPost]
        public JsonResult EnviarCorreoValidacionImpresion()
        {
            bool ban = false;
            string rst = "false";
            string msgError = string.Empty;

            try{
                DatosCarrera dc = new DatosCarrera();
                rst = dc.getCodigoAutenticacion(User.Identity.Name.ToString());

                if(rst != "false"){
                    Session["codImpresionActa"] = rst;
                    ban = true;
                }else{
                    msgError = "Problema el enviar codigo de autenticación";
                }

            }catch(Exception ex){
                ban = false;
                Errores err = new Errores();
                err.SetError(ex, "EnviarCorreoValidacionImpresion");
            }

            return Json(new { banEnviocodAutenticacion = ban, errorMessage = msgError });
        }


        [HttpPost]
        public JsonResult ValidarCodigoImpresion(string strCodImpresion, string idActa, string idAsignatura)
        {
            bool ban = false;
            string mgmImpresion = string.Empty;

            try
            {
                if (string.Compare(Session["codImpresionActa"].ToString(), strCodImpresion) == 0) {
                    return this.impresionActas( idActa, idAsignatura);
                }else{
                    return Json(new { rstValidacionCodigo = ban, errorMessage = "Codigo 'no' valido, favor vuelva a intentarlo" });
                }
            }catch(Exception ex){
                ban = false;
                Errores err = new Errores();
                err.SetError(ex, "ValidarCodigoImpresion");
            }

            return Json(new { rstValidacionCodigo = ban, errorMessage = mgmImpresion });
        }

    }
}