using GestorErrores;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SitioWebOasis.WSGestorEvaluacion;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;
namespace SitioWebOasis.Library
{
    public class DescargarActasPA : Asignatura
    {
       

        private WSGestorEvaluacion.dtstEvaluacion_Acumulados _dsEvAcumulativa = new WSGestorEvaluacion.dtstEvaluacion_Acumulados();
        private WSGestorEvaluacion.dtstEvaluacion_Actas _dsEvFinal = new WSGestorEvaluacion.dtstEvaluacion_Actas();
        private WSGestorEvaluacion.dtstEvaluacion_Actas _dsEvRecuperacion = new WSGestorEvaluacion.dtstEvaluacion_Actas();
        string strAsignatura = string.Empty;
        public DescargarActasPA(string strCodCarrera, string strCodPeriodoAcademico, string strCodAsignatura) {
            this._strCodCarrera = strCodCarrera;
            this._strCodPeriodo= strCodPeriodoAcademico;
            var cadena = strCodAsignatura.Split('_');
            this._strCodAsignatura = cadena[0];
            this._strCodNivel = cadena[1];
            this._strCodParalelo = cadena[2];
            this._cargarInformacionCarreraPA(_strCodCarrera);
        }

        public string getActaEvaluacionAcumulativa(string[] dtaActa, string[] dtaAsignatura, string pathReport, string pathTmp) {
            //  Creo el nombre del archivo
            string nameFile = string.Empty;
            string reportPath = string.Empty;
            string nombreAsignatura = string.Empty;

            string idTypeFile = dtaActa[0];
            string strNameFile = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(idTypeFile))
                {
                    reportPath = (dtaActa[0] == "pdf")
                                    ? Path.Combine(pathReport, "rptActaEvaluacionesConNotas.rdlc")
                                    : "";

                    LocalReport rptEvAcumulativa = this.getRptEvAcumulativa(reportPath);

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

                    renderedBytes = rptEvAcumulativa.Render(reportType,
                                                            deviceInfo,
                                                            out mimeType,
                                                            out encoding,
                                                            out fileNameExtension,
                                                            out streams,
                                                            out warnings);

                    nombreAsignatura = this.strAsignatura;
                    nameFile = Language.es_ES.NF_EV_ACUMULATIVA+"_" + nombreAsignatura+".pdf";

                    //  Direcciono la creacion del archivo a una ubicacion temporal
                    string fullPath = Path.Combine(pathTmp, nameFile);

                    //  Creo el archivo en la ubicacion temporal
                    System.IO.File.WriteAllBytes(fullPath, renderedBytes);
                }
            }
            catch (Exception ex)
            {
                nameFile = "-1";

                Errores err = new Errores();
                err.SetError(ex, "_getDtaRptEvAcumulativa");
            }

            return nameFile;

        }
        public string getActaEvaluacionFinal(string[] dtaActa, string[] dtaAsignatura, string pathReport, string pathTmp)
        {
            //  Creo el nombre del archivo
            string nameFile = string.Empty;
            string reportPath = string.Empty;
            string dtaParcial = dtaActa[0];
            string nombreAsignatura = string.Empty;
         

            string idTypeFile = (dtaActa[0] == "pdf")
                                    ? "pdf"
                                    : "";
            string strNameFile = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(idTypeFile))
                {
                    reportPath = (dtaActa[0] == "pdf")
                                    ? Path.Combine(pathReport, "rptActaExPrincipalConNotasR1.rdlc")
                                    : Path.Combine(pathReport, "rptActaExPrincipalSinNotasR1.rdlc");

                    LocalReport rptEvFinal = this.getRptEvFinal(reportPath);

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

                    renderedBytes = rptEvFinal.Render(reportType,
                                                        deviceInfo,
                                                        out mimeType,
                                                        out encoding,
                                                        out fileNameExtension,
                                                        out streams,
                                                        out warnings);

                    nameFile = Language.es_ES.NF_EV_FINAL + "_" + this.strAsignatura + ".pdf";

                    //  Direcciono la creacion del archivo a una ubicacion temporal
                    string fullPath = Path.Combine(pathTmp, nameFile);

                    //  Creo el archivo en la ubicacion temporal
                    System.IO.File.WriteAllBytes(fullPath, renderedBytes);

                    //  verifico si el acta a sido impresa

                }
            }
            catch (Exception ex)
            {
                nameFile = "-1";

                Errores err = new Errores();
                err.SetError(ex, "getDtaRptEvFinal");
            }

            return nameFile;
        }
        public string getActaEvaluacionSuspencion(string[] dtaActa, string[] dtaAsignatura, string pathReport, string pathTmp)
        {
            //  Creo el nombre del archivo
            string nameFile = string.Empty;
            string reportPath = string.Empty;
            //string nombreAsignatura = string.Empty;

            string idTypeFile = (dtaActa[0] == "pdf")
                                    ? "pdf"
                                    : "";
            string strNameFile = string.Empty;

            try
            {

                if (!string.IsNullOrEmpty(idTypeFile))
                {
                    reportPath = (dtaActa[0] == "pdf")
                                    ? Path.Combine(pathReport, "rptActaExSuspensionConNotasR1.rdlc")
                                    : Path.Combine(pathReport, "rptActaExSuspensionSinNotasR1.rdlc");

                    LocalReport rptEvRecuperacion = this.getRptEvRecuperacion(reportPath);

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

                    renderedBytes = rptEvRecuperacion.Render(reportType,
                                                                deviceInfo,
                                                                out mimeType,
                                                                out encoding,
                                                                out fileNameExtension,
                                                                out streams,
                                                                out warnings);
                    nameFile = Language.es_ES.NF_EV_RECUPERACION + "_" + this.strAsignatura + ".pdf";

                    //  Direcciono la creacion del archivo a una ubicacion temporal
                    string fullPath = Path.Combine(pathTmp, nameFile);

                    //  Creo el archivo en la ubicacion temporal
                    System.IO.File.WriteAllBytes(fullPath, renderedBytes);
                }
            }
            catch (Exception ex)
            {
                nameFile = "-1";

                Errores err = new Errores();
                err.SetError(ex, "getDtaRptEvFinal");
            }

            return nameFile;
        }
       
        //Metodos para  Evaluacion Acumulativa
        public LocalReport getRptEvAcumulativa(string reportPath)
        {
            LocalReport rptEvAcumulativa = new LocalReport();

            try
            {
                WSGestorEvaluacion.dtstEvaluacion_ImprimirAcumulados dtaEvAcumulativa = this._getDtaImpresionEvAcumulativa();
                ReportDataSource rds = new ReportDataSource();
                rds.Value = dtaEvAcumulativa.Acta;
                rds.Name = "dtsActasAcumuladas";

                rptEvAcumulativa.DataSources.Clear();
                rptEvAcumulativa.DataSources.Add(rds);
                rptEvAcumulativa.ReportPath = reportPath;

                rptEvAcumulativa.SetParameters(this._getParametrosGeneralesReporte());
                rptEvAcumulativa.Refresh();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getReporteHorarios");
            }

            return rptEvAcumulativa;
        }
        private WSGestorEvaluacion.dtstEvaluacion_ImprimirAcumulados _getDtaImpresionEvAcumulativa()
        {
            WSGestorEvaluacion.dtstEvaluacion_ImprimirAcumulados rstEvAcumulativa = new WSGestorEvaluacion.dtstEvaluacion_ImprimirAcumulados();
            WSGestorEvaluacion.dtstEvaluacion_ImprimirAcumulados dsEvAcumulativa = new WSGestorEvaluacion.dtstEvaluacion_ImprimirAcumulados();

            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD);
                ge.set_fUbicacion(this._strUbicacion);

                rstEvAcumulativa = ge.getImprimirActaEvaluaciones(this._strCodPeriodo,
                                                                    this._strCodAsignatura,
                                                                    this._strCodNivel,
                                                                    this._strCodParalelo);

                dsEvAcumulativa = (rstEvAcumulativa != null)
                                    ? rstEvAcumulativa
                                    : new WSGestorEvaluacion.dtstEvaluacion_ImprimirAcumulados();
            }
            catch (System.Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDtaImpresionEvAcumulativa");
            }

            return dsEvAcumulativa;
        }
        //Metodos para  Evaluacion  Principal
        public LocalReport getRptEvFinal(string reportPath)
        {
            LocalReport rptEvAcumulativa = new LocalReport();

            try
            {
                dtstEvaluacion_ImprimirActas dtaEvFinal = this.getReporteActasExPrincipal();
                ReportDataSource rds = new ReportDataSource();
                rds.Value = dtaEvFinal.Acta;
                rds.Name = "dtsExamenPrincipal";

                rptEvAcumulativa.DataSources.Clear();
                rptEvAcumulativa.DataSources.Add(rds);
                rptEvAcumulativa.ReportPath = reportPath;

                rptEvAcumulativa.SetParameters(this._getParametrosGeneralesReporte());
                rptEvAcumulativa.Refresh();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getReporteHorarios");
            }

            return rptEvAcumulativa;
        }
        public dtstEvaluacion_ImprimirActas getReporteActasExPrincipal()
        {
            //  DataSet
            dtstEvaluacion_ImprimirActas dsActa = new dtstEvaluacion_ImprimirActas();
            dtstEvaluacion_ImprimirActas ds = new dtstEvaluacion_ImprimirActas();

            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD);
                ge.set_fUbicacion(this._strUbicacion);

                ds = ge.getImprimirActaExPrincipal(this._strCodPeriodo,
                                                    this._strCodAsignatura,
                                                    this._strCodNivel,
                                                    this._strCodParalelo);

                dsActa = (ds != null && ds.Acta.Rows.Count > 0)
                            ? ds
                            : new dtstEvaluacion_ImprimirActas();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDtaImpresionEvAcumulativa");
            }

            return dsActa;
        }
        //Metodos para  Evaluacion de Recuperacion o Suspencion
        public LocalReport getRptEvRecuperacion(string reportPath)
        {
            LocalReport rptEvRecuperacion = new LocalReport();

            try
            {
                dtstEvaluacion_ImprimirActas dtaEvRecuperacion = this._getDtaImpresionEvRecuperacion();

                ReportDataSource rds = new ReportDataSource();
                rds.Value = dtaEvRecuperacion.Acta;
                rds.Name = "dtsExamenPrincipal";

                rptEvRecuperacion.DataSources.Clear();
                rptEvRecuperacion.DataSources.Add(rds);
                rptEvRecuperacion.ReportPath = reportPath;

                rptEvRecuperacion.SetParameters(this._getParametrosGeneralesReporte());
                rptEvRecuperacion.Refresh();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getRptEvRecuperacion");
            }

            return rptEvRecuperacion;
        }
        private WSGestorEvaluacion.dtstEvaluacion_ImprimirActas _getDtaImpresionEvRecuperacion()
        {
            dtstEvaluacion_ImprimirActas rstEvRecuperacion = new dtstEvaluacion_ImprimirActas();
            dtstEvaluacion_ImprimirActas dsEvRecuperacion = new dtstEvaluacion_ImprimirActas();

            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD);
                ge.set_fUbicacion(this._strUbicacion);

                rstEvRecuperacion = ge.getImprimirActaExSuspension(this._strCodPeriodo,
                                                                    this._strCodAsignatura,
                                                                    this._strCodNivel,
                                                                    this._strCodParalelo);

                dsEvRecuperacion = (rstEvRecuperacion != null)
                                        ? rstEvRecuperacion
                                        : new dtstEvaluacion_ImprimirActas();

            }
            catch (System.Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDtaImpresionEvRecuperacion");
            }

            return dsEvRecuperacion;
        }
        //Parametros para el reporte
        private IEnumerable<ReportParameter> _getParametrosGeneralesReporte()
        {
            WSInfoCarreras.ParametrosCarrera pc = this._getParametrosCarreraPA(this._strCodCarrera);
            List<ReportParameter> lstPrmRptEvAcumulativa = new List<ReportParameter>();

            string lblFacultad = "FACULTAD:";
            string lblCarrera = "CARRERA:";
            string lblEscuela = "ESCUELA:";

            string facultad = default(string);
            string carrera = default(string);
            string escuela = default(string);

            // string strAsignatura = string.Empty;
            string strNivel = string.Empty;
            string strPeriodo = string.Empty;
            string strDocente = string.Empty;
            string strSistema = string.Empty;
            float numCreditos = default(float);
            byte fHorasTeo = default(byte);
            byte fHorasPra = default(byte);

            try
            {
                ReportParameter prmRptHorarioAcademico = new ReportParameter();
                this._getDatosMateriaPA(ref strAsignatura,
                                        ref strNivel,
                                        ref strPeriodo,
                                        ref strDocente,
                                        ref strSistema,
                                        ref numCreditos,
                                        ref fHorasTeo,
                                        ref fHorasPra);

                switch (UsuarioActual.CarreraActual.TipoEntidad.ToString())
                {
                    case "CAR":
                        facultad = pc.NombreFacultad;
                        carrera = pc.NombreCarrera;
                        escuela = pc.NombreEscuela;
                        break;

                    case "CAA":
                        lblFacultad = "";
                        lblCarrera = "";
                        lblEscuela = "";

                        facultad = pc.NombreFacultad;
                        carrera = pc.NombreCarrera;
                        escuela = "";
                        break;
                }

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strInstitucion",
                                                                Language.es_ES.STR_INSTITUCION));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strPeriodoAcademico",
                                                                strPeriodo));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strLblFacultad",
                                                                lblFacultad));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strLblCarrera",
                                                                lblCarrera));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strLblEscuela",
                                                                lblEscuela));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strAsignatura",
                                                                strAsignatura));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strCodMateria",
                                                                this._strCodAsignatura.ToString().ToUpper()));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strCreditos",
                                                                Convert.ToString(numCreditos)));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strDocente",
                                                                this.UsuarioActual.Nombre.ToString().ToUpper()));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strNivel",
                                                                this._strCodNivel.ToString()));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strParalelo",
                                                                this._strCodParalelo.ToString()));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strFacultad",
                                                                facultad));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strEscuela",
                                                                escuela));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strCarrera",
                                                                carrera));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strURLImagen",
                                                                ""));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strURLImagenDTIC",
                                                                ""));
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDatosGeneralesReporte");
            }

            return lstPrmRptEvAcumulativa;
        }

    }
    public class FileDownload {

        public List<FileInfo> GetFile()
        {
            List<FileInfo> listFiles = new List<FileInfo>();
            string fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Temp");
            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);
            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                listFiles.Add(new FileInfo()
                {
                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName  + @"\" + item.Name,
                    FilePathOrigen= dirInfo.FullName
                });
                i = i + 1;
            }
            return listFiles;
        }

    }
    public class FileInfo
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FilePathOrigen { get; set; }
    } 


}