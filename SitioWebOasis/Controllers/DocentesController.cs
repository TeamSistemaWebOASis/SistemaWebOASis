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
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Services.Description;

namespace SitioWebOasis.Controllers
{
    [Authorize]
    [HandleError]
    public class DocentesController : Controller
    {
        // GET: Docentes
        public ActionResult Index(string strCodCarrera)
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

                foreach (Carrera item in CarrerasEstudiante)
                {
                    if (item.TipoEntidad == "CAR")
                    {
                        idCarrera = item.Codigo;
                        break;
                    }
                    else if (item.TipoEntidad == "CAA")
                    {
                        idCarrera = item.Codigo;
                        break;
                    }
                    else
                    {
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
                //  Implementamos un control de acceso con la finalidad que el usuario pueda ver solo las asignaturas
                //  a las que tiene acceso
                if (this._usuarioGestionaAsignatura(strCodNivel, strCodAsignatura, strCodParalelo))
                {
                    EvaluacionesDocenteModel edm = new EvaluacionesDocenteModel(strCodNivel,
                                                                            strCodAsignatura,
                                                                            strCodParalelo);

                    return View("GestionNotasDocente", edm);
                }
                else
                {
                    return RedirectToAction("SignOut", "Account");
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "EvaluacionAsignatura");

                return RedirectToAction("Index", "Error");
            }
        }


        private bool _usuarioGestionaAsignatura(string strCodNivel, string strCodAsignatura, string strCodParalelo)
        {
            bool ban = false;
            try
            {
                SitioWebOasis.Models.DatosAsignaturasDocenteModel daDocente = new DatosAsignaturasDocenteModel(this.UsuarioActual.CarreraActual.Codigo.ToString());
                ban = daDocente.getDocenteGestionaAsignatura(strCodNivel,
                                                                strCodAsignatura,
                                                                strCodParalelo);
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "EvaluacionAsignatura");
            }

            return ban;
        }


        [HttpPost]
        public ActionResult registrarEvaluacion(string strCodNivel, string strCodAsignatura, string strCodParalelo, string strParcialActivo, List<EvaluacionAcumulativa> dtaEvAcumulativa)
        {
            JsonResult rstGestionEvAcumulativa = default(JsonResult);
            try
            {

                if (this._usuarioGestionaAsignatura(strCodNivel, strCodAsignatura, strCodParalelo))
                {
                    EvaluacionAcumulativaModel evAcumulativa = new EvaluacionAcumulativaModel(strCodNivel,
                                                                                                strCodAsignatura,
                                                                                                strCodParalelo);

                    if (dtaEvAcumulativa.Count > 0)
                    {
                        if (evAcumulativa.registrarEvaluacionAcumulativa(dtaEvAcumulativa))
                        {
                            rstGestionEvAcumulativa = Json(new
                            {
                                dtaEvAcumulativaUpd = evAcumulativa.jsonEvAcumulativa,
                                MessageGestion = Language.es_ES.MSG_REGISTRO_EV_ACUMULATIVA_CORRECTA
                            });
                        }
                        else
                        {
                            rstGestionEvAcumulativa = Json(new
                            {
                                dtaEvAcumulativaUpd = "false",
                                MessageGestion = Language.es_ES.MSG_REGISTRO_EV_ACUMULATIVA_ERROR
                            });
                        }
                    }
                }
                else
                {
                    return RedirectToAction("SignOut", "Account");
                }
            }
            catch (Exception ex)
            {
                rstGestionEvAcumulativa = Json(new
                {
                    dtaEvAcumulativaUpd = "false",
                    MessageGestion = Language.es_ES.MSG_REGISTRO_EV_ACUMULATIVA_ERROR
                });

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
                if (this._usuarioGestionaAsignatura(strCodNivel, strCodAsignatura, strCodParalelo))
                {
                    EvaluacionFinalModel evFinal = new EvaluacionFinalModel(strCodNivel,
                                                                            strCodAsignatura,
                                                                            strCodParalelo);

                    if (dtaEvFinal.Count > 0)
                    {
                        if (evFinal.registrarEvaluacionFinal(dtaEvFinal))
                        {
                            rstEvFinal = Json(new
                            {
                                dtaEvFinalUpd = evFinal.jsonEvFinal,
                                MessageGestion = Language.es_ES.MSG_REGISTRO_EV_FINAL_CORRECTA
                            });
                        }
                        else
                        {
                            rstEvFinal = Json(new
                            {
                                dtaEvFinalUpd = "false",
                                MessageGestion = Language.es_ES.MSG_REGISTRO_EV_FINAL_ERROR
                            });
                        }
                    }
                }
                else
                {
                    return RedirectToAction("SignOut", "Account");
                }
            }
            catch (Exception ex)
            {
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
                if (this._usuarioGestionaAsignatura(strCodNivel, strCodAsignatura, strCodParalelo))
                {
                    EvaluacionRecuperacionModel evRecuperacion = new EvaluacionRecuperacionModel(strCodNivel,
                                                                                                    strCodAsignatura,
                                                                                                    strCodParalelo);

                    if (dtaEvRecuperacion.Count > 0)
                    {
                        if (evRecuperacion.registrarEvaluacionRecuperacion(dtaEvRecuperacion))
                        {
                            rstEvRecuperacion = Json(new
                            {
                                dtaEvRecuperacionUpd = evRecuperacion.jsonEvRecuperacion,
                                MessageGestion = Language.es_ES.MSG_REGISTRO_EV_RECUPERACION_CORRECTA
                            });
                        }
                        else
                        {
                            rstEvRecuperacion = Json(new
                            {
                                dtaEvRecuperacionUpd = "false",
                                MessageGestion = Language.es_ES.MSG_REGISTRO_EV_RECUPERACION_ERROR
                            });
                        }
                    }
                }
                else
                {
                    return RedirectToAction("SignOut", "Account");
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
            try
            {
                string[] dtaActa = idActa.Split('_');
                string[] dtaAsignatura = idAsignatura.Split('|');

                string nameFile = string.Empty;
                switch (dtaActa[0].ToString().ToUpper())
                {
                    //  EVALUACION ACUMULATIVA ( Parcial 1 / Parcial 2 / Parcial 3 )
                    case "PEA":
                        EvaluacionAcumulativaModel evAcumulativa = new EvaluacionAcumulativaModel(dtaAsignatura[1],
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
                        EvaluacionRecuperacionModel evRecuperacion = new EvaluacionRecuperacionModel(dtaAsignatura[1],
                                                                                                        dtaAsignatura[0],
                                                                                                        dtaAsignatura[2]);

                        nameFile = evRecuperacion.getDtaRptEvRecuperacion(dtaActa,
                                                                            dtaAsignatura,
                                                                            Server.MapPath("~/Reports"),
                                                                            Server.MapPath("~/Temp"));
                        break;
                }

                if (nameFile == "-1")
                {
                    return Json(new { fileName = "", errorMessage = Language.es_ES.MSG_ERROR_GENERAR_ARCHIVO });
                }
                else
                {
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
        public JsonResult descargaActasPeriodosAnteriores(string idCarrera, string idPeriodoAcademico, string idAsignatura)
        {
            try
            {
                string[] dtaActa = { "pdf" };
                string[] dtaAsignatura = idAsignatura.Split('_');
                string nameFileAcumulada = string.Empty;
                string nameFilePrincipal = string.Empty;
                string nameFileSuspencion = string.Empty;
                DescargarActasPA evaluaciones = new DescargarActasPA(idCarrera, idPeriodoAcademico, idAsignatura);
                nameFileAcumulada = evaluaciones.getActaEvaluacionAcumulativa(dtaActa,
                                                            dtaAsignatura,
                                                            Server.MapPath("~/Reports"),
                                                            Server.MapPath("~/Temp"));
                nameFilePrincipal = evaluaciones.getActaEvaluacionFinal(dtaActa,
                                                             dtaAsignatura,
                                                             Server.MapPath("~/Reports"),
                                                             Server.MapPath("~/Temp"));
                nameFileSuspencion = evaluaciones.getActaEvaluacionSuspencion(dtaActa,
                                                           dtaAsignatura,
                                                           Server.MapPath("~/Reports"),
                                                           Server.MapPath("~/Temp"));

                if (nameFileAcumulada == "-1" || nameFilePrincipal == "-1" || nameFileSuspencion == "-1")
                {
                    return Json(new { fileName = "", errorMessage = Language.es_ES.MSG_ERROR_GENERAR_ARCHIVO });
                }
                else
                {

                    return Json(new { fileName = nameFileAcumulada + "|" + nameFilePrincipal + "|" + nameFileSuspencion, errorMessage = "" });
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
        public JsonResult impresionNominaEstudiantes(string strCodAsignatura, string strCodTipoArchivo)
        {
            string nameFile = "-1";
            try
            {
                string[] dtaAsignatura = strCodAsignatura.Split('_');
                string tipoArchivo = (strCodTipoArchivo.ToUpper().CompareTo("EXCEL") == 0) ? "Excel" : "pdf";

                NominaEstudiantesModel NominaEstudiantes = new NominaEstudiantesModel(dtaAsignatura[0],
                                                                                        dtaAsignatura[1],
                                                                                        dtaAsignatura[2]);

                nameFile = NominaEstudiantes.getNameFileRptNominaEstudiantes(tipoArchivo,
                                                                                Server.MapPath("~/Reports"),
                                                                                Server.MapPath("~/Temp"));

                if (nameFile == "-1")
                {
                    return Json(new { fileName = "", errorMessage = Language.es_ES.MSG_ERROR_GENERAR_ARCHIVO });
                }
                else
                {
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
            try
            {
                if (!string.IsNullOrEmpty(file) && file != "-1")
                {
                    //  get the temp folder and file path in server
                    string fullPath = Path.Combine(Server.MapPath("~/temp"), file);
                    //return the file for download, this is an Excel 
                    //so I set the file content type to "application/vnd.ms-excel"
                    return File(fullPath, "application/vnd.ms-excel", file);
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            catch (Exception ex)
            {
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

            try
            {
                DatosCarrera dc = new DatosCarrera();
                rst = dc.getCodigoAutenticacion(User.Identity.Name.ToString());

                if (rst != "false")
                {
                    Session["codImpresionActa"] = rst;
                    ban = true;
                }
                else
                {
                    msgError = "Problema el enviar codigo de autenticación";
                }

            }
            catch (Exception ex)
            {
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
                if (string.Compare(Session["codImpresionActa"].ToString(), strCodImpresion) == 0)
                {
                    return this.impresionActas(idActa, idAsignatura);
                }
                else
                {
                    return Json(new { rstValidacionCodigo = ban, errorMessage = "Codigo 'no' valido, favor vuelva a intentarlo" });
                }
            }
            catch (Exception ex)
            {
                ban = false;
                Errores err = new Errores();
                err.SetError(ex, "ValidarCodigoImpresion");
            }

            return Json(new { rstValidacionCodigo = ban, errorMessage = mgmImpresion });
        }
        public ActionResult GestionArchivoDocente()
        {
            Session["objDatosPA"] = new DatosArchivosDocentesModel();
            return View("GestionArchivoDocente", (DatosArchivosDocentesModel)Session["objDatosPA"]);
        }

        [HttpPost]
        public JsonResult GetPeriodosCarreraDocente(string strCodCarrera)
        {
            string message = string.Empty;
            JsonResult rstPA = default(JsonResult);
            try
            {
                DatosArchivosDocentesModel objDatosPA = (DatosArchivosDocentesModel)Session["objDatosPA"];
                rstPA = Json(objDatosPA.getLstPeriodos(strCodCarrera));
            }
            catch (Exception ex)
            {
                message = "Error, favor volver a intentarlo";
                Errores err = new Errores();
                err.SetError(ex, "GetPeriodosCarreraDocente");
            }

            return rstPA;
        }

        [HttpPost]
        public string GetMateriasDocentePA(string strCodCarrera, string strCodPeriodo)
        {
            SitioWebOasis.Models.DatosArchivosDocentesModel objDocente = new DatosArchivosDocentesModel();
            return objDocente.getHTMLAsignaturasDocentePorPeriodoAcademico(strCodCarrera, strCodPeriodo);
        }

        //METODO PARA DESACARGAR LAS ACTAS DE PERIODEOS ANTERIORES 
        [HttpPost]
        public ActionResult DownloadEvaluacionesFiles(string strNombreArchivos)
        {
            string message = string.Empty;
            try
            {
                FileDownload obj = new FileDownload(strNombreArchivos);
                var filesCol = obj.GetFile().ToList();
                using (var memoryStream = new MemoryStream())
                {
                    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        for (int i = 0; i < filesCol.Count; i++)
                        {
                            ziparchive.CreateEntryFromFile(filesCol[i].FilePath, filesCol[i].FileName);
                        }
                    }
                    //Elimino los archivos creado en TEMP
                    for (int i = 0; i < filesCol.Count; i++)
                    {
                        string[] List = Directory.GetFiles(filesCol[i].FilePathOrigen, filesCol[i].FileName);
                        foreach (string f in List)
                        {
                            System.IO.File.Delete(f);
                        }
                    }
                    return File(memoryStream.ToArray(), "application/zip", "Evaluaciones.zip");
                }
            }
            catch (Exception ex)
            {
                message = "Error, favor volver a intentarlo";
                Errores err = new Errores();
                err.SetError(ex, "GetPeriodosCarreraDocente");
            }
            return null;
        }

        public ActionResult HorarioDocente(string strCursoParalelo = "")
        {
            //HorarioDocenteModel objHorario = new Models.HorarioDocenteModel();
            //return View("HorarioDocente", objHorario);
            Session["objHorario"] = new HorarioDocenteModel();
            return View("HorarioDocente", (HorarioDocenteModel)Session["objHorario"]);
        }
        [HttpPost]//CREA HORARIO POR ASIGANTURA
        public string HorarioDocenteAsignatura(string strCodMateria)
        {
            HorarioDocenteModel objHoario = new HorarioDocenteModel();
            objHoario.HorarioClaseMateria(strCodMateria);
            return objHoario.HTMLHorarioClases();
        }

        public ActionResult HorarioExamenDocente(string strCursoParalelo = "")
        {
            HorarioDocenteModel objHorario = new Models.HorarioDocenteModel();
            return View("HorarioExamenDocente", objHorario);
        }
        [HttpPost]//CREAR HORARIO ACADEMICO
        public JsonResult CrearFileHorarioAcademico(string idTypeFile, string strHorario)
        {
            try
            {
                string reportPath = string.Empty;
                string strNombreReporte = string.Empty;
                string[] strCadenas = idTypeFile.Split('|');

                if (strHorario.Equals("Clase"))
                {
                    reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptHorarioClaseDocente.rdlc");
                    strNombreReporte = Language.es_ES.EST_TB_HORARIO_ACADEMICO;
                }
                else
                {
                    reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptHorarioExamenDocente.rdlc");
                    strNombreReporte = Language.es_ES.EST_TB_HORARIO_EXAMENES;
                }
                if (strCadenas.Length > 1) { strHorario = strHorario + "|" + strCadenas[1] + "|" + strCadenas[2] + "|" + strCadenas[3]; }
                HorarioDocenteModel objHorarioDocente = new Models.HorarioDocenteModel(strHorario);
                LocalReport rptHorarioClaseDocente = objHorarioDocente.getReporteHorarios(reportPath);
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo = "<DeviceInfo>" +
                                   "   <OutputFormat>" + strCadenas[0] + "</OutputFormat>" +
                                   "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                renderedBytes = rptHorarioClaseDocente.Render(strCadenas[0],
                                                                       deviceInfo,
                                                                       out mimeType,
                                                                       out encoding,
                                                                       out fileNameExtension,
                                                                       out streams,
                                                                       out warnings);
                //  Creo el nombre del archivo
                string nameFile = strNombreReporte.Replace(" ", "_") + "_" + objHorarioDocente.UsuarioActual.CarreraActual.Nombre.ToString() + ((strCadenas[0] == "PDF") ? ".pdf" : ".xls");
                //  Direcciono la creacion del archivo a una ubicacion temporal
                string fullPath = Path.Combine(Server.MapPath("~/Temp"), nameFile);
                //  Creo el archivo en la ubicacion temporal
                System.IO.File.WriteAllBytes(fullPath, renderedBytes);
                return Json(new { fileName = nameFile, errorMessage = "" });
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "createFile");
                return Json(new { fileName = "none", errorMessage = "Problema al momento de crear el archivo" });
            }
        }
        public ActionResult HorarioCarrera(string strCursoParalelo = "")
        {
            HorarioEstudiante he = new Models.HorarioEstudiante();

            if (!string.IsNullOrEmpty(strCursoParalelo))
            {
                he = new Models.HorarioEstudiante(strCursoParalelo);
            }

            return View("HorarioCarrera", he);
        }
        //Mostrar Estadisticas de Acta Acumula
        [HttpPost]
        public JsonResult MostrarEstadisticas(string strIdCarrera, string strIdPeriodoAcademico, string strIdAsignatura)
        {
            try
            {
                DescargarActasPA evaluaciones = new DescargarActasPA(strIdCarrera, strIdPeriodoAcademico, strIdAsignatura);
                dynamic strEstadistica = evaluaciones.EstadisticaEvaluacionAcumulada();
                return Json(new { strEstadistica, errorMessage = "" });
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "MostrarEstadisticas");
                return Json(new { fileName = "none", errorMessage = "Problema al momento de crear estadisticas" });
            }
        }

    }
}