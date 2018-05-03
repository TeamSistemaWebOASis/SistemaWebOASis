using GestorErrores;
using Microsoft.Reporting.WebForms;
using SitioWebOasis.CommonClasses;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using SitioWebOasis.Models;
using System;
using System.Collections;
using System.IO;
using System.Web.Mvc;

namespace SitioWebOasis.Controllers
{
    [Authorize]
    public class EstudiantesController : Controller
    {
        public EstudiantesController(){
            ViewBag.estado = "hidden";
        }

        private Usuario UsuarioActual{
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        //
        //  GET: Estudiantes
        //
        public ActionResult Index(string strCodCarrera = "")
        {
            string strIdCarrera = (string.IsNullOrEmpty(strCodCarrera))
                                    ? this._getIdCarrera()
                                    : strCodCarrera;

            if (!string.IsNullOrEmpty(strIdCarrera)) {
                UsuarioActual.SetRolCarreraActual(  Roles.Estudiantes,
                                                    strIdCarrera);
                                              

                return View("Index", new SitioWebOasis.Models.DatosAcademicosEstudiante());
            }else{
                return View("Index", "Error");
            }
        }


        private string _getIdCarrera()
        {
            string idCarrera = string.Empty;

            try
            {
                ArrayList CarrerasEstudiante = UsuarioActual.GetCarreras(Roles.Estudiantes);
                
                foreach (Carrera item in CarrerasEstudiante){
                    if(item.TipoEntidad == "CAR"){
                        idCarrera = item.Codigo;
                        break;
                    }else if(item.TipoEntidad == "CAA"){
                        idCarrera = item.Codigo;
                        break;
                    }else{
                        idCarrera = item.Codigo;
                    }
                }
            }
            catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "getIdCarrera");
            }

            return idCarrera;
        }



        public ActionResult NotasEstudiantes(string periodoAcademico = "")
        {
            DatosAcademicosEstudiante dtaNotasEstudiante = (!string.IsNullOrEmpty(periodoAcademico))
                                                                ? new DatosAcademicosEstudiante(periodoAcademico)
                                                                : new DatosAcademicosEstudiante();

            return View("DatosAcademicosEstudiante", dtaNotasEstudiante);
        }


        #region REPORTES HORARIO ACADEMICO / HORARIO DE EXAMENES

        public ActionResult HorarioEstudiante(string strCursoParalelo = "")
        {
            HorarioEstudiante he = new Models.HorarioEstudiante();

            if( !string.IsNullOrEmpty(strCursoParalelo)){
                he = new Models.HorarioEstudiante(strCursoParalelo);
            }

            return View("HorarioEstudiante", he);
        }


        public ActionResult HorarioExamenesEstudiante()
        {
            HorarioExamenesEstudiante hee = new Models.HorarioExamenesEstudiante();
            return View("HorarioExamenesEstudiante", hee);
        }

        [HttpPost]
        public JsonResult createFileHorarioAcademico( string idCurso, string idTypeFile)
        {
            try
            {
                string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptHorarioAcademico.rdlc");
                
                HorarioEstudiante he = new Models.HorarioEstudiante(idCurso);                
                LocalReport rptHorariosAcademicoEstudiante = he.getReporteHorarios(reportPath);

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

                renderedBytes = rptHorariosAcademicoEstudiante.Render(  reportType,
                                                                        deviceInfo,
                                                                        out mimeType,
                                                                        out encoding,
                                                                        out fileNameExtension,
                                                                        out streams,
                                                                        out warnings);

                //  Creo el nombre del archivo
                string nameFile = Language.es_ES.EST_TB_HORARIO_ACADEMICO.Replace(" ", "_") + "_" + he.UsuarioActual.Cedula.ToString() + ((idTypeFile == "PDF") ? ".pdf" : ".xls");

                //  Direcciono la creacion del archivo a una ubicacion temporal
                string fullPath = Path.Combine(Server.MapPath("~/Temp"), nameFile);

                //  Creo el archivo en la ubicacion temporal
                System.IO.File.WriteAllBytes(fullPath, renderedBytes);

                return Json(new { fileName = nameFile, errorMessage = "" });
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "createFile" );

                return Json(new { fileName = "none", errorMessage = "Problema al momento de crear el archivo" });
            }
        }


        [HttpPost]
        public JsonResult createFileHorarioExamenes(string idTypeFile)
        {
            HorarioEstudiante he = new Models.HorarioEstudiante();

            try
            {
                string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptHorarioExamenEstudiantes.rdlc");                
                LocalReport rptHorarios = he.getReporteHorariosExamenes(reportPath);

                string reportType = idTypeFile;
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo = "   <DeviceInfo>" +
                                    "       <OutputFormat>" + idTypeFile + "</OutputFormat>" +
                                    "   </DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = rptHorarios.Render( reportType,
                                                    deviceInfo,
                                                    out mimeType,
                                                    out encoding,
                                                    out fileNameExtension,
                                                    out streams,
                                                    out warnings);

                //  Creo el nombre del archivo
                string nameFile = Language.es_ES.EST_TB_HORARIO_EXAMENES + "_" + he.UsuarioActual.Cedula.ToString() + ((idTypeFile == "PDF") ? ".pdf" : ".xls");

                //  Direcciono la creacion del archivo a una ubicacion temporal
                string fullPath = Path.Combine(Server.MapPath("~/Temp"), nameFile);

                //  Creo el archivo en la ubicacion temporal
                System.IO.File.WriteAllBytes(fullPath, renderedBytes);
                
                return Json(new { fileName = nameFile, errorMessage = "" });
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "createFileHorarioExamenes - "+ he.UsuarioActual.Cedula.ToString() + " - " + he.UsuarioActual.CarreraActual.Codigo.ToString());

                return Json(new { fileName = "none", errorMessage = "Problema al momento de crear el archivo" });
            }
        }


        [HttpPost]
        [DeleteFileAttribute]
        public ActionResult DownloadFile(string file)
        {
            try
            {
                if (!string.IsNullOrEmpty(file)){
                    //get the temp folder and file path in server
                    string fullPath = Path.Combine(Server.MapPath("~/temp"), file);

                    //return the file for download, this is an Excel 
                    //so I set the file content type to "application/vnd.ms-excel"
                    return File(fullPath, "application/vnd.ms-excel", file);
                }else{
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

        #endregion


        #region DATOS ESTUDIANTES
        public ActionResult DatosEstudiantes()
        {
            DatosPersonalesUsuario dpe = new DatosPersonalesUsuario();
            return View("DatosPersonalesEstudiante", dpe);
        }

        [HttpPost]
        public ActionResult registrarDatosEstudiante()
        {
            DatosPersonalesUsuario dpe = new DatosPersonalesUsuario();
            bool rstUPD = dpe.updDatosEstudiantes(Request);

            if (rstUPD){
                ViewBag.visibilidad = "";
                ViewBag.estadoTransaccion = "alert alert-success";
                ViewBag.mensaje = Language.es_ES.EST_SMS_REGISTRO_CORRECTO;
            }else{
                ViewBag.visibilidad = "";
                ViewBag.estadoTransaccion = "alert alert-danger alert-dismissable";
                ViewBag.mensaje = Language.es_ES.EST_SMS_REGISTRO_INCORRECTO;
                ViewBag.recomendacion = Language.es_ES.EST_SMS_CONSULTA_SECRETARIA_CARRERA;
            }

            return View("DatosPersonalesEstudiante", dpe);
        }


        private bool updDatosPersonalesEstudiante( Persona dtaDPE )
        {
            bool rst = false;

            //  Informacion de persona
            dtaDPE.eci_id = Convert.ToInt32( Request["ddlEstadoCivil"].ToString().Trim() );
            dtaDPE.etn_id = Convert.ToInt32( Request["ddlEtnia"].ToString().Trim() );
            dtaDPE.per_emailAlternativo = Request["txtCorreoAlternativo"].ToString().Trim();

            return rst;
        }



        [HttpPost]
        public JsonResult GetDPA( string idType, string idDPA )
        {
            Catalogos ctDPA = new Catalogos();
            JsonResult rstDPA = default(JsonResult);

            switch (idType)
            {
                //  En funcion al pais seleccionado se listan las provincias
                case "ddl_FNPais":
                case "ddl_DUPais":
                    rstDPA = Json(ctDPA.getLstProvincias(idDPA));
                break;

                //  En funcion a la provincia seleccionada se listan las ciudadaes
                case "ddl_FNProvincias":
                case "ddl_DUProvincias":
                    rstDPA = Json(ctDPA.getLstCiudades(idDPA));
                break;

                //  En funcion a la ciudad seleccionada se listan las parroquias
                case "ddl_FNCiudades":
                case "ddl_DUCiudades":
                    rstDPA = Json(ctDPA.getLstParroquias(idDPA));
                break;
            }

            return rstDPA;
        }

        #endregion

        public ActionResult ArchivoMatricula()
        {
            DatosAcademicosEstudiante objDatosAcademico = new DatosAcademicosEstudiante();
            return View("ArchivoMatricula", objDatosAcademico);
        }

        [HttpPost]
        public JsonResult ImpresionArchivoMatricula(string strCodPeriodo) {
            try
            {
                string reportPath = string.Empty;
                reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptArchivoMatricula.rdlc");
                ArchivoMatriculaEstudiante objArchivoMatricula = new ArchivoMatriculaEstudiante(UsuarioActual.CarreraActual.codUsuario.ToString(), UsuarioActual.Cedula.ToString(), strCodPeriodo, "M",UsuarioActual.CarreraActual.Codigo.ToString());
                LocalReport rptMatriculaEstudiante = objArchivoMatricula.getReporteMatricula(reportPath);

                string idTypeFile = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                string deviceInfo = "<DeviceInfo>" +
                                   "   <OutputFormat>" + idTypeFile + "</OutputFormat>" +
                                   "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                renderedBytes = rptMatriculaEstudiante.Render(idTypeFile,
                                                                       deviceInfo,
                                                                       out mimeType,
                                                                       out encoding,
                                                                       out fileNameExtension,
                                                                       out streams,
                                                                       out warnings);
                //  Creo el nombre del archivo
                string strNombreReporte = "Archivo Matriculacion";
                string nameFile = strNombreReporte.Replace(" ", "_") + "_" + UsuarioActual.Cedula.ToString() + ((idTypeFile == "PDF") ? ".pdf" : "");

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
        public JsonResult MostrarEstadisticas(string strCodPeriodoA)
        {
            try
            {
                DatosAcademicosEstudiante objDatosAcademicos = new DatosAcademicosEstudiante(strCodPeriodoA);
                dynamic strEstadistica = objDatosAcademicos.EstadisticaEvaluacionAcumulada();
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