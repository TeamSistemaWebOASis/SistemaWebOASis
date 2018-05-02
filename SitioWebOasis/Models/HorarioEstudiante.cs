using GestorErrores;
using Microsoft.Reporting.WebForms;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.WSDatosUsuario;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SitioWebOasis.Models
{
    public class HorarioEstudiante
    {
        private WSInfoCarreras.dtstPeriodoVigente _dsPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();
        private string _ultimoPeriodoEstudiante = string.Empty;
        private WSDatosUsuario.dtstDatosCursosCarrera _dtCursosCarrera = new dtstDatosCursosCarrera();

        private string _strCodParalelo = string.Empty;
        private string _strCodNivel = string.Empty;
        /// <summary>
        ///     Construtor de la clase HorarioEstudiante
        /// </summary>
        /// <param name="strCursoParalelo"> 
        ///     Identificador del Curso y paralelo a mostrar el horario, 
        ///     el valor pre determinado es "-2" el cual indica que es el horario 
        ///     del estudiante en la carrera.
        /// </param>
        public HorarioEstudiante( string strCursoParalelo = "-2" )
        {
            _dsPeriodoVigente = _getPeriodoVigenteCarrera();
            _dtCursosCarrera = _getCursosCarrera();
            _ultimoPeriodoEstudiante = _getUltimoPeriodoEstudiante();
            _strCodNivel = "-2";

            if ( !string.IsNullOrEmpty(strCursoParalelo) && strCursoParalelo != "-2"){
                string[] codNivelParalelo = strCursoParalelo.Split('|');
                _strCodNivel = codNivelParalelo[0];
                _strCodParalelo = codNivelParalelo[1];
            }
        }


        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        private string _getUltimoPeriodoEstudiante()
        {
            string ultimoPeriodo = string.Empty;

            try
            {
                ProxySeguro.DatosUsuario du = new ProxySeguro.DatosUsuario();
                ultimoPeriodo = du.getUltimoPeriodoEstudiante(  this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                                this.UsuarioActual.Cedula.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getUltimoPeriodoEstudiante");
            }

            return ultimoPeriodo;
        }


        private WSInfoCarreras.dtstPeriodoVigente _getPeriodoVigenteCarrera()
        {
            WSInfoCarreras.dtstPeriodoVigente dsPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();

            try
            {
                ProxySeguro.InfoCarreras ic = new ProxySeguro.InfoCarreras();
                dsPeriodoVigente = ic.GetPeriodoVigenteCarrera(UsuarioActual.CarreraActual.Codigo.ToString());
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getPeriodoVigenteCarrera");
            }

            return dsPeriodoVigente;
        }


        private WSDatosUsuario.dtstDatosCursosCarrera _getCursosCarrera()
        {
            WSDatosUsuario.dtstDatosCursosCarrera dsCC = new dtstDatosCursosCarrera();

            try
            {
                ProxySeguro.DatosUsuario du = new ProxySeguro.DatosUsuario();
                dsCC = du.getCursosCarrera( UsuarioActual.CarreraActual.Codigo.ToString(),
                                            _dsPeriodoVigente.Periodos.Rows[0]["strCodigo"].ToString());
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getCursosCarrera");
            }

            return dsCC;
        }


        private WSGestorDeReportesMatriculacion.dtstHorario _horarioEstudiante()
        {
            WSGestorDeReportesMatriculacion.dtstHorario dsHorario = new WSGestorDeReportesMatriculacion.dtstHorario();
            WSGestorDeReportesMatriculacion.dtstHorario rstServicioWeb = new WSGestorDeReportesMatriculacion.dtstHorario();

            try
            {
                if (!string.IsNullOrEmpty(_ultimoPeriodoEstudiante))
                {
                    ProxySeguro.GestorDeReportesMatriculacion rm = new ProxySeguro.GestorDeReportesMatriculacion();
                    rm.CookieContainer = new System.Net.CookieContainer();
                    rm.SetCodCarrera(UsuarioActual.CarreraActual.Codigo.ToString());

                    //  El servicio web retorna no registra la excepcion y retorna null
                    rstServicioWeb = rm.GetReporteHorarioEstudiante(UsuarioActual.Cedula.ToString(),
                                                                    UsuarioActual.CarreraActual.Codigo.ToString(),
                                                                    _ultimoPeriodoEstudiante);

                    dsHorario = (rstServicioWeb != null)
                                    ? this._limpiarHorarioCurso( rstServicioWeb )
                                    : new WSGestorDeReportesMatriculacion.dtstHorario();
                }
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_horarioEstudiante");
            }

            return dsHorario;
        }


        public List<System.Web.Mvc.SelectListItem> getLstCursosCarrera()
        {
            List<System.Web.Mvc.SelectListItem> lstCursosCarrera = new List<System.Web.Mvc.SelectListItem>();
            System.Web.Mvc.SelectListItem curso = new System.Web.Mvc.SelectListItem();

            if (_dtCursosCarrera.CursosCarrera.Rows.Count > 0){
                if (UsuarioActual.RolActual.ID.ToString().CompareTo("Estudiantes") == 0){
                    curso = new System.Web.Mvc.SelectListItem();
                    curso.Value = "-2";
                    curso.Text = Language.es_ES.STR_MI_HORARIO.ToString();
                    curso.Selected = true;

                    lstCursosCarrera.Add(curso);
                }

                foreach (DataRow item in _dtCursosCarrera.CursosCarrera){
                    curso = new System.Web.Mvc.SelectListItem();
                    curso.Value = item["strCodNivelParalelo"].ToString();
                    curso.Text = item["strDescripcionNivel"].ToString();

                    if ( _strCodNivel + "|" + _strCodParalelo == item["strCodNivelParalelo"].ToString()){
                        curso.Selected = true;
                    }

                    lstCursosCarrera.Add(curso);
                }
            }

            return lstCursosCarrera;
        }
     

        private WSGestorDeReportesMatriculacion.dtstHorario _horarioCursoParalelo(string strCodNivel, string strCodParalelo)
        {
            WSGestorDeReportesMatriculacion.dtstHorario hcp = new WSGestorDeReportesMatriculacion.dtstHorario();
            WSGestorDeReportesMatriculacion.dtstHorario dsHorarioCurso = new WSGestorDeReportesMatriculacion.dtstHorario();

            try
            {
                ProxySeguro.GestorDeReportesMatriculacion rm = new ProxySeguro.GestorDeReportesMatriculacion();
                rm.CookieContainer = new System.Net.CookieContainer();
                rm.SetCodCarrera(UsuarioActual.CarreraActual.Codigo.ToString());

                hcp = rm.GetReporteHorarioCurso(strCodParalelo,
                                                _dsPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                strCodNivel);

                dsHorarioCurso = this._limpiarHorarioCurso(hcp);
            }
            catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_horarioCursoParalelo");
            }

            return dsHorarioCurso;
        }


        private WSGestorDeReportesMatriculacion.dtstHorario _limpiarHorarioCurso(WSGestorDeReportesMatriculacion.dtstHorario hcp)
        {
            try
            {
                foreach (DataRow item in hcp.Horario){
                    if( string.IsNullOrEmpty( item["strLunes"].ToString() )  
                        && string.IsNullOrEmpty(item["strMartes"].ToString())
                        && string.IsNullOrEmpty(item["strMiercoles"].ToString())
                        && string.IsNullOrEmpty(item["strJueves"].ToString())
                        && string.IsNullOrEmpty(item["strViernes"].ToString())
                        && string.IsNullOrEmpty(item["strSabado"].ToString())
                        && string.IsNullOrEmpty(item["strDomingo"].ToString() ) ){
                        item.Delete();
                    }
                }

                hcp.AcceptChanges();
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_limpiarHorarioCurso");
            }

            return hcp;
        }


        public string HTMLHorarioCarrera()
        {
            string rst = string.Empty;

            rst += " <tr role='row'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS.ToUpper() + "</td>";
            rst += " </tr>";

            try{
                WSGestorDeReportesMatriculacion.dtstHorario he = (_strCodNivel == "-2")
                                                                    ? _horarioEstudiante()
                                                                    : _horarioCursoParalelo(_strCodNivel, _strCodParalelo);

                if ( he != null && he.Horario.Rows.Count > 0){
                    rst = string.Empty;
                    foreach( DataRow item in he.Horario){
                        rst += " <tr role='row' class='even'>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strDescripcionHora"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strInicioFinHora"].ToString() + "</ td >";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strLunes"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strMartes"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strMiercoles"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strJueves"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strViernes"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strSabado"].ToString() + "</td>";
                        rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strDomingo"].ToString() + "</td>";
                        rst += " </tr>";
                    }
                }
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "HTMLHorarioCarrera");
            }

            return rst;
        }


        public LocalReport getReporteHorarios( string reportPath )
        {
            LocalReport rptHorarioAcademico = new LocalReport();

            try
            {
                WSGestorDeReportesMatriculacion.dtstHorario he = ( _strCodNivel == "-2" )
                                                                    ? _horarioEstudiante()
                                                                    : _horarioCursoParalelo(_strCodNivel, _strCodParalelo);

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "dsHorarioAcademico";
                rds.Value = he.Horario;

                rptHorarioAcademico.DataSources.Clear();
                rptHorarioAcademico.DataSources.Add(rds);
                rptHorarioAcademico.ReportPath = reportPath;
                
                rptHorarioAcademico.SetParameters(_getParametrosGeneralesReporte());
                rptHorarioAcademico.Refresh();
            }
            catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getReporteHorarios");
            }

            return rptHorarioAcademico;
        }


        public LocalReport getReporteHorariosExamenes(string reportPath)
        {
            LocalReport rptHorarioExEst = new LocalReport();

            try
            {
                WSGestorDeReportesEvaluacion.dtstHorarioExamenes dsHorariosExamenes = this._getHorarioExamenes();

                ReportDataSource rds = new ReportDataSource();
                rds.Name = "dsHorarioExamenes";
                rds.Value = dsHorariosExamenes.Materias;

                rptHorarioExEst.DataSources.Clear();
                rptHorarioExEst.DataSources.Add(rds);
                rptHorarioExEst.ReportPath = reportPath;

                rptHorarioExEst.SetParameters(this._getParametrosGeneralesReporte());
                rptHorarioExEst.Refresh();
            }
            catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getReporteHorariosExamenes");
            }

            return rptHorarioExEst;
        }


        private WSGestorDeReportesEvaluacion.dtstHorarioExamenes _getHorarioExamenes()
        {
            WSGestorDeReportesEvaluacion.dtstHorarioExamenes dsConsultaHorarioExamenes = new WSGestorDeReportesEvaluacion.dtstHorarioExamenes();
            WSGestorDeReportesEvaluacion.dtstHorarioExamenes dsHorarioExamenes = new WSGestorDeReportesEvaluacion.dtstHorarioExamenes();

            try{
                ProxySeguro.GestorDeReportesEvaluacion re = new ProxySeguro.GestorDeReportesEvaluacion();
                re.CookieContainer = new System.Net.CookieContainer();
                re.set_CodCarrera( UsuarioActual.CarreraActual.Codigo.ToString() );

                dsHorarioExamenes = re.GetHorarioExamenesEstudiante(_dsPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                    UsuarioActual.Cedula.ToString());
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getHorarioExamenes");
            }

            return dsHorarioExamenes;
        }


        private IEnumerable<ReportParameter> _getParametrosGeneralesReporte()
        {
            WSInfoCarreras.ParametrosCarrera pc = this._getParametrosCarrera();
            List<ReportParameter> lstPrmRptHorarioAcademico = new List<ReportParameter>();

            string lblFacultad = "FACULTAD:";
            string lblCarrera = "CARRERA:";
            string lblEscuela = "ESCUELA:";

            string facultad = default(string);
            string carrera = default(string);
            string escuela = default(string);
            string strCurso = default(string);

            try
            {
                ReportParameter prmRptHorarioAcademico = new ReportParameter();

                lstPrmRptHorarioAcademico.Add(  new ReportParameter("strPeriodoAcademico",
                                                _dsPeriodoVigente.Periodos[0]["strDescripcion"].ToString().ToUpper()));
                
                strCurso = ( this._strCodNivel != "-2" )
                                ? this._getDescripcionCurso( this._strCodNivel, this._strCodParalelo )
                                : "";

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
                
                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strInstitucion",
                                                                    Language.es_ES.STR_INSTITUCION));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblHorarioAcademico",
                                                                    Language.es_ES.STR_HORARIO_ACADEMICO));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblHorarioExamenes",
                                                                    Language.es_ES.STR_HORARIO_EXAMENES));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblPeriodoAcademico",
                                                                    Language.es_ES.STR_PERIODO_ACADEMICO));


                if(this._strCodNivel == "-2"){
                    lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblCedula",
                                                                        "Cedula"));

                    lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblNombres",
                                                                        "Nombres"));

                    lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strCedula",
                                                                        UsuarioActual.Cedula.ToString()));

                    lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strNombresUsuario",
                                                                        UsuarioActual.Nombre.ToString()));
                }else{
                    lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblCedula",
                                                                        string.Empty));

                    lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblNombres",
                                                                        string.Empty));

                    lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strCedula",
                                                                        string.Empty));

                    lstPrmRptHorarioAcademico.Add(new ReportParameter("strNombresUsuario",
                                                                        string.Empty));
                }

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblFacultad",
                                                                    lblFacultad));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblCarrera",
                                                                    lblCarrera));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strLblEscuela",
                                                                    lblEscuela));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strFacultad",
                                                                    facultad));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strEscuela",
                                                                    carrera));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strCarrera",
                                                                    escuela));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strCurso",
                                                                    strCurso));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strFuente",
                                                                    Language.es_ES.STR_FUENTE_REPORTE));
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDatosGeneralesReporte");
            }

            return lstPrmRptHorarioAcademico;
        }



        private string _getDescripcionCurso(string strCodNivel, string strCodParalelo )
        {
            string rst = default(string);

            try
            {
                dtstDatosCursosCarrera dsCursosCarrera = this._dtCursosCarrera;
                DataRow[] rstFiltro = dsCursosCarrera.CursosCarrera.Select("strCodParalelo = '"+ strCodParalelo + "' AND strCodNivel = '"+ strCodNivel + "'");

                rst = (rstFiltro.Length > 0)
                        ? rstFiltro[0]["strDescripcionNivel"].ToString()
                        : "";
            }
            catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDescripcionCurso");
            }

            return rst;
        }


        private List<ReportParameter> _getDatosGeneralesReporte()
        {
            WSInfoCarreras.ParametrosCarrera pc = this._getParametrosCarrera();
            List<ReportParameter> lstPrmRptHorarioAcademico = new List<ReportParameter>();

            string lblFacultad = "FACULTAD:";
            string lblCarrera = "CARRERA:";
            string lblEscuela = "ESCUELA:";

            string facultad = default(string);
            string carrera = default(string);
            string escuela = default(string);

            try
            {
                ReportParameter prmRptHorarioAcademico = new ReportParameter();

                lstPrmRptHorarioAcademico.Add(  new ReportParameter("strPeriodoAcademico",
                                                _dsPeriodoVigente.Periodos[0]["strCodigo"].ToString().ToUpper()));

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


                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblFacultad",
                                                                    lblFacultad));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblCarrera",
                                                                    lblCarrera));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strLblEscuela",
                                                                    lblEscuela));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strFacultad",
                                                                    facultad));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strEscuela",
                                                                    carrera));

                lstPrmRptHorarioAcademico.Add(new ReportParameter("strCarrera",
                                                                    escuela));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strCurso",
                                                                    "PRUEBA CURSO"));

                lstPrmRptHorarioAcademico.Add(new ReportParameter(  "strFuente",
                                                                    "ESPOCH - DTIC"));
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getDatosGeneralesReporte");
            }

            return lstPrmRptHorarioAcademico;
        }


        private WSInfoCarreras.ParametrosCarrera _getParametrosCarrera()
        {
            WSInfoCarreras.ParametrosCarrera pc = new WSInfoCarreras.ParametrosCarrera();
            try
            {
                WSInfoCarreras.InfoCarreras ic = new WSInfoCarreras.InfoCarreras();
                pc = ic.GetParametrosCarrera(this.UsuarioActual.CarreraActual.Codigo.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getParametrosCarrera");
            }

            return pc;
        }

    }
}