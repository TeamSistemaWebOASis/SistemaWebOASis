using GestorErrores;
using Microsoft.Reporting.WebForms;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using SitioWebOasis.WSGestorDeReportesMatriculacion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using SitioWebOasis.WSEgresamientoGraduacion;

namespace SitioWebOasis.Models
{
    public class ArchivoMatriculaEstudiante : DatosCarrera
    {
        private string strCodEstud = string.Empty;
        private string strCedula = string.Empty;
        private string strCodPeriodo = string.Empty;
        private string strTipo = string.Empty;
        private string strCodCarrera = string.Empty;
        private string fSexoDirector = string.Empty;
        private string fFirmaDirector = string.Empty;
        private string fFirmaSecretaria = string.Empty;
        private string fTipoCarrera = string.Empty;
        private string fDirectorEscuela = string.Empty;
        private string fSecretariaAcademica = string.Empty;
        private string fDecano = string.Empty;
        private string fVicedecano = string.Empty;

        public ArchivoMatriculaEstudiante(string strCodEstud, string strCedula, string strCodPeriodo, string strTipo, string strCodCarrera)
        {
            this.strCodEstud = strCodEstud;
            this.strCedula = strCedula.Replace("-", "").Trim();
            this.strCodPeriodo = strCodPeriodo;
            this.strTipo = strTipo;
            this.strCodCarrera = strCodCarrera;
        }
        public LocalReport getReporteMatricula(string reportPath)
        {
            LocalReport rptMatricula = new LocalReport();
            try
            {
                GestordeReportes gr = new GestordeReportes();
                gr.CookieContainer = new CookieContainer();
                gr.SetUbicacion(this.strCodCarrera);
                gr.SetCodCarrera(this.strCodCarrera);
                DataSet dtsEstudiante = gr.GetReporteMatricula(this.strCodEstud, this.strCedula, this.strCodPeriodo, this.strTipo);

                if (dtsEstudiante != null && dtsEstudiante.Tables["Estudiante"].Rows.Count > 0)
                {
                    List<ReporteMatriculacionEstudiante> lstMatriculaEst = new List<ReporteMatriculacionEstudiante>();
                    foreach (DataRow item in dtsEstudiante.Tables["Estudiante"].Rows)
                    {
                        ReporteMatriculacionEstudiante objMatriculaEst = new ReporteMatriculacionEstudiante
                        {
                            StrCodigo = Convert.ToString(item["strCodigo"]),
                            StrCedula = Convert.ToString(item["strCedula"]),
                            StrApellidos = Convert.ToString(item["strApellidos"]),
                            StrNombres = Convert.ToString(item["strNombres"]),
                            StrCodPeriodo = Convert.ToString(item["strCodPeriodo"]),
                            StrDescripcionPeriodo = Convert.ToString(item["strDescripcionPeriodo"]),
                            StrCodNivelMatricula = Convert.ToString(item["strDescrNivelMatricula"]),
                            BytNumMatricula = Convert.ToString(item["bytNumMatricula"]),
                            StrObservacionesMatAsignada = Convert.ToString(item["strObservacionesMatAsignada"]),
                            StrObsercacionesMatricula = Convert.ToString(item["strObservacionesMatricula"]),
                            DtFechaMatricula = Convert.ToDateTime(item["dtFechaMatricula"]),
                            StrNivelMateria = Convert.ToString(item["strNivelMateria"]),
                            StrDescrNivelMateria = Convert.ToString(item["strDescrNivelMateria_Asignada"]),
                            StrCodMateria = Convert.ToString(item["strCodMateria"]),
                            StrNombreMateria = Convert.ToString(item["strNombreMateria"]),
                            StrDescrNivelMatricula = Convert.ToString(item["strDescrNivelMatricula"])
                        };
                        lstMatriculaEst.Add(objMatriculaEst);
                    }
                    ReportDataSource rds = new ReportDataSource();
                    rds.Name = "dsArchivoMatricula";
                    rds.Value = lstMatriculaEst;
                    rptMatricula.DataSources.Clear();
                    rptMatricula.DataSources.Add(rds);
                    rptMatricula.ReportPath = reportPath;
                    rptMatricula.SetParameters(_getParametrosGeneralesReporte());
                    rptMatricula.Refresh();
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getReporteMatricula");
            }

            return rptMatricula;
        }
        private IEnumerable<ReportParameter> _getParametrosGeneralesReporte()
        {
            WSInfoCarreras.ParametrosCarrera pc = this._getParametrosCarrera();
            List<ReportParameter> lstPrmRptMatriculaEstudiante = new List<ReportParameter>();

            string facultad = default(string);
            string carrera = default(string);
            string escuela = default(string);
            string strDocente = default(string);
            ObtenerValoresUnidadAcademica();
            try
            {
                ReportParameter prmRptMatriculaEstudiante = new ReportParameter();
                switch (UsuarioActual.CarreraActual.TipoEntidad.ToString())
                {
                    case "CAR":
                        facultad = pc.NombreFacultad;
                        carrera = pc.NombreCarrera;
                        escuela = pc.NombreEscuela;
                        break;
                    case "CAA":
                        facultad = pc.NombreFacultad;
                        carrera = pc.NombreCarrera;
                        escuela = "";
                        break;
                }
                //Director
                if (this.fSexoDirector == "MAS")
                {
                    fFirmaDirector = "DIRECTOR ESCUELA DE " + carrera.Trim().ToUpper();
                }
                else
                {
                    if (this.fSexoDirector == "FEM")
                    {
                        fFirmaDirector = "DIRECTORA ESCUELA DE " + carrera.Trim().ToUpper();
                    }
                    else
                    {
                        fFirmaDirector = "DIRECTOR(A) ESCUELA DE " + carrera.Trim().ToUpper();
                    }
                }
                //Secretaria Académica
                fFirmaSecretaria = "SECRETARIA ACADÉMICA ESCUELA DE " + carrera.Trim().ToUpper();

                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strInstitucion",
                                                                    Language.es_ES.STR_INSTITUCION));

                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strFacultad",
                                                                    facultad));

                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strEscuela",
                                                                    carrera));

                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strCarrera",
                                                                    escuela));

                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strFuente",
                                                                    Language.es_ES.STR_FUENTE_REPORTE));

                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strDirectorEscuela",
                                                                    this.fDirectorEscuela.ToUpper().Trim()));
                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strFirmaDirector",
                                                                    fFirmaDirector.ToUpper().Trim()));
                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strSecretariaAcademica",
                                                                    this.fSecretariaAcademica.ToUpper().Trim()));
                lstPrmRptMatriculaEstudiante.Add(new ReportParameter("strFirmaSecretaria",
                                                                    fFirmaSecretaria.ToUpper().Trim()));
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDatosGeneralesReporte");
            }
            return lstPrmRptMatriculaEstudiante;
        }

        private void ObtenerValoresUnidadAcademica()
        {
            try
            {
                _cargarInformacionCarrera();
                GestorEgresamientoGraduacion gr = new GestorEgresamientoGraduacion();
                gr.CookieContainer = new CookieContainer();
                gr.set_Ubicacion(this.strCodCarrera);
                gr.set_BaseDatos(this._strNombreBD);
                gr.GetDatosPersonalUnidadAcademica(ref fDecano, ref fVicedecano, ref fDirectorEscuela, ref fSecretariaAcademica);
                this.fSexoDirector = gr.GetCodSexoAutoridadUnidadAcademica("DIRE");
                gr.Dispose();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "ObtenerValoresUnidadAcademica");
            }
        }
    }
}