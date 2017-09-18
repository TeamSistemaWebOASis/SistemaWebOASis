using GestorErrores;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using SitioWebOasis.Models;
using System;
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
            SitioWebOasis.Models.DatosAcademicosDocente daDocente = new DatosAcademicosDocente(idCarrera);
            return View("Index", daDocente);
        }

        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        //  [OutputCache(Duration = 8000, VaryByParam = "strCodNivel, strCodAsignatura, strCodParalelo")]
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
        

        [HttpPost]
        public JsonResult impresionActas(string idActa, string idAsignatura)
        {
            try{
                string[] dtaActa = idActa.Split('_');
                string[] dtaAsignatura = idAsignatura.Split('|');

                string nameFile = string.Empty;
                string idTypeFile = string.Empty;

                switch (dtaActa[0].ToString().ToUpper())
                {
                    //  EVALUACION ACUMULATIVA ( Parcial 1 / Parcial 2 / Parcial 3 )
                    case "P1":
                    case "P2":
                    case "P3":
                        idTypeFile = dtaActa[1];
                        nameFile = this._getDtaRptEvAcumulativa(dtaActa, dtaAsignatura);
                    break;

                    //  EVALUACION FINAL
                    case "PF":
                        EvaluacionFinalModel evFinal = new EvaluacionFinalModel(dtaAsignatura[0],
                                                                                dtaAsignatura[1],
                                                                                dtaAsignatura[2]);
                    break;
                        
                    //  EVALUACION RECUPERACION
                    case "PR":
                        EvaluacionRecuperacionModel evRecuperacion = new EvaluacionRecuperacionModel(   dtaAsignatura[0],
                                                                                                        dtaAsignatura[1],
                                                                                                        dtaAsignatura[2]);
                    break;
                }

                return Json(new { fileName = nameFile, errorMessage = "" });
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "createFile");

                return Json(new { fileName = "none", errorMessage = "Problema al momento de crear el archivo" });
            }
        }



        private string _getDtaRptEvAcumulativa( string[] dtaActa, string[] dtaAsignatura)
        {
            //  Creo el nombre del archivo
            string nameFile = Language.es_ES.EST_TB_HORARIO_ACADEMICO.Replace(" ", "_") + "_" + this.UsuarioActual.Cedula.ToString() + ((dtaActa[1] == "PDF" || dtaActa[1] == "blc") ? ".pdf" : ".xls");
            string reportPath = string.Empty;
            string dtaParcial = dtaActa[0];
            string idTypeFile = dtaActa[1];

            try{
                EvaluacionAcumulativaModel evAcumulativa = new EvaluacionAcumulativaModel(  dtaAsignatura[1],
                                                                                            dtaAsignatura[0],
                                                                                            dtaAsignatura[2]);

                reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptActaEvaluacionesConNotas.rdlc");
                LocalReport rptEvAcumulativa = evAcumulativa.getRptEvAcumulativa(reportPath, dtaParcial);

                string reportType = idTypeFile;
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo = "<DeviceInfo>" +
                                    "   <OutputFormat>" + idTypeFile + "</OutputFormat>" +
                                    "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = rptEvAcumulativa.Render(    reportType,
                                                            deviceInfo,
                                                            out mimeType,
                                                            out encoding,
                                                            out fileNameExtension,
                                                            out streams,
                                                            out warnings);

                //  Direcciono la creacion del archivo a una ubicacion temporal
                string fullPath = Path.Combine(Server.MapPath("~/Temp"), nameFile);

                //  Creo el archivo en la ubicacion temporal
                System.IO.File.WriteAllBytes(fullPath, renderedBytes);
            }catch(Exception ex)
            {
                nameFile = "-1";

                Errores err = new Errores();
                err.SetError(ex, "_getDtaRptEvAcumulativa");
            }

            return nameFile;
        }

    }
}