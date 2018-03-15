using GestorErrores;
using Microsoft.Reporting.WebForms;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class NominaEstudiantesModel: Asignatura
    {
        public NominaEstudiantesModel(string strCodAsignatura, string strCodNivel, string strCodParalelo)
        {
            this._strCodAsignatura = strCodAsignatura;
            this._strCodNivel = strCodNivel;
            this._strCodParalelo = strCodParalelo;

            this._cargarInformacionCarrera();
        }


        public string getNameFileRptNominaEstudiantes(string idTypeFile, string pathReport, string pathTmp)
        {
            //  Creo el nombre del archivo
            string nameFile = string.Empty;
            string reportPath = string.Empty;
            string strNameFile = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(idTypeFile))
                {
                    reportPath = Path.Combine(pathReport, "rptListadoMatriculadosMateria.rdlc");
                    LocalReport rptNominaEstudiantesAsignatura = this.getDtaRptNominaEstudiantes(reportPath);

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

                    renderedBytes = rptNominaEstudiantesAsignatura.Render(  reportType,
                                                                            deviceInfo,
                                                                            out mimeType,
                                                                            out encoding,
                                                                            out fileNameExtension,
                                                                            out streams,
                                                                            out warnings);

                    nameFile = Language.es_ES.NF_NOMINA_ESTUDIANTES + "_" + this.getNombreAsignatura().Replace(" / ", "_").ToUpper() + "." + (( idTypeFile.CompareTo("Excel") == 0 ) ? "xls" : "pdf" );

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




        public LocalReport getDtaRptNominaEstudiantes(string reportPath)
        {
            LocalReport rptEvAcumulativa = new LocalReport();

            try{
                DataSet dtaNominaEstudiantes = this._getDtaNominaEstudiantes();
                ReportDataSource rds = new ReportDataSource();
                rds.Value = dtaNominaEstudiantes.Tables[0];
                rds.Name = "dtsListadoMatriculadosMateria";

                rptEvAcumulativa.DataSources.Clear();
                rptEvAcumulativa.DataSources.Add(rds);
                rptEvAcumulativa.ReportPath = reportPath;

                rptEvAcumulativa.SetParameters(this._getParametrosGeneralesReporte());
                rptEvAcumulativa.Refresh();
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "getReporteHorarios");
            }

            return rptEvAcumulativa;
        }



        private DataSet _getDtaNominaEstudiantes()
        {
            DataSet nomina = default(DataSet);
            ProxySeguro.GestorDeReportesMatriculacion grm = new ProxySeguro.GestorDeReportesMatriculacion();
            grm.CookieContainer = new System.Net.CookieContainer();
            grm.SetCodCarrera(this.UsuarioActual.CarreraActual.Codigo);

            try{
                nomina = (DataSet)grm.GetReporteListadoMatriculadosMateriaParalelo(  this._strCodParalelo,
                                                                            this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                            this._strCodNivel,
                                                                            this._strCodAsignatura );
            }
            catch (Exception ex) {
                nomina = default(DataSet);

                Errores err = new Errores();
                err.SetError(ex, "_getDtaNominaEstudiantes");
            }

            grm.Dispose();

            return nomina;
        }




        private IEnumerable<ReportParameter> _getParametrosGeneralesReporte()
        {
            WSInfoCarreras.ParametrosCarrera pc = this._getParametrosCarrera();
            List<ReportParameter> lstPrmRptEvAcumulativa = new List<ReportParameter>();

            string lblFacultad = "FACULTAD:";
            string lblCarrera = "CARRERA:";
            string lblEscuela = "ESCUELA:";

            string facultad = default(string);
            string carrera = default(string);
            string escuela = default(string);

            string strAsignatura = string.Empty;
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
                this._getDatosMateria(  ref strAsignatura,
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
                                                                this._dtstPeriodoVigente.Periodos[0]["strDescripcion"].ToString()));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strLblFacultad",
                                                                lblFacultad));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strLblCarrera",
                                                                lblCarrera));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strLblEscuela",
                                                                lblEscuela));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strAsignatura",
                                                                strAsignatura));

                lstPrmRptEvAcumulativa.Add(new ReportParameter("strCodAsignatura",
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
}