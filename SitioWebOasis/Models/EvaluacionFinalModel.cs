using GestorErrores;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SitioWebOasis.Library;
using SitioWebOasis.WSGestorEvaluacion;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace SitioWebOasis.Models
{
    public class EvaluacionFinalModel: Asignatura
    {
        public string jsonEvFinal { get; set; }
        private WSGestorEvaluacion.dtstEvaluacion_Actas _dsEvFinal = new WSGestorEvaluacion.dtstEvaluacion_Actas();


        public EvaluacionFinalModel(string strCodNivel, string strCodAsignatura, string strCodParalelo)
        {
            this._strCodAsignatura = strCodAsignatura;
            this._strCodNivel = strCodNivel;
            this._strCodParalelo = strCodParalelo;
            this._cargarInformacionCarrera();
            this._dsEvFinal = this._CargarNotasEvFinal();

            this.jsonEvFinal = (this._dsEvFinal.Acta.Rows.Count > 0)
                                    ? JsonConvert.SerializeObject(this._dsEvFinal.Acta)
                                    : "";
        }


        private WSGestorEvaluacion.dtstEvaluacion_Actas _CargarNotasEvFinal()
        {
            WSGestorEvaluacion.dtstEvaluacion_Actas dsEvFinal = new WSGestorEvaluacion.dtstEvaluacion_Actas();
            WSGestorEvaluacion.dtstEvaluacion_Actas rstEvFinal = new WSGestorEvaluacion.dtstEvaluacion_Actas();

            try
            {
                ProxySeguro.GestorEvaluacion gev = new ProxySeguro.GestorEvaluacion();
                gev.CookieContainer = new CookieContainer();
                gev.set_fBaseDatos(this._strNombreBD.ToString());
                gev.set_fUbicacion(this._strUbicacion.ToString());
                
                rstEvFinal = gev.crearActaArtificialPrincipalR1(    this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                    this._strCodAsignatura, 
                                                                    this._strCodNivel,
                                                                    this._strCodParalelo);

                dsEvFinal = (rstEvFinal != null)? rstEvFinal 
                                                : new WSGestorEvaluacion.dtstEvaluacion_Actas();
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_CargarNotasEvFinal");
            }

            return dsEvFinal;
        }


        public bool registrarEvaluacionFinal(List<EvaluacionFinal> dtaEvFinal)
        {
            bool rst = false;

            try{
                //  Recorro DataTable Acta - registro por registro actualizando la informacion
                if (this._updEvAcumulativa(dtaEvFinal)){
                    if (this._guardarEvFinal()){
                        this._dsEvFinal = this._CargarNotasEvFinal();
                        this.jsonEvFinal = (this._dsEvFinal.Acta.Rows.Count > 0)
                                                ? JsonConvert.SerializeObject(this._dsEvFinal.Acta)
                                                : "";

                        rst = true;
                    }                    
                }
            }catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionAcumulativa");
            }

            return rst;
        }
        

        private bool _updEvAcumulativa(List<EvaluacionFinal> dtaEvFinal)
        {
            bool rst = false;

            try{
                int numReg = dtaEvFinal.Count;
                if ( numReg > 0 && this._dsEvFinal.Acta.Rows.Count > 0 ){
                    for (int x = 0; x < numReg; x++){
                        if (this._dsEvFinal.Acta.Rows[0]["sintCodMatricula"].ToString() == dtaEvFinal[0].sintCodMatricula.ToString()){
                            this._dsEvFinal.Acta.Rows[x]["bytNota"] = Convert.ToByte(dtaEvFinal[x].bytNota.ToString());
                        }
                    }

                    this._dsEvFinal.AcceptChanges();
                    rst = true;
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_updEvEstudiantes");
            }

            return rst;
        }


        private bool _guardarEvFinal()
        {
            bool rst = false;
            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fUbicacion(this._strUbicacion);
                ge.set_fBaseDatos(this._strNombreBD);

                //  Registro informacion de evaluacion final
                rst = ge.setActaArtificialPrincipal(this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                    this._strCodAsignatura.ToString(),
                                                    this._strCodNivel.ToString(),
                                                    this._strCodParalelo.ToString(), 
                                                    this._dsEvFinal,
                                                    this.UsuarioActual.Nombre.ToString());

                //  Actualizar fecha de registro de notas
                ge.ActualizarRegistroFechaIngresoExPrincipal(   this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                this._strCodAsignatura.ToString(),
                                                                this._strCodNivel.ToString(),
                                                                this._strCodParalelo.ToString(),
                                                                DateTime.Now);
            }
            catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_guardarEvFinal");
            }

            return rst;
        }


        public string getHTML_EvaluacionFinal()
        {
            string colorRow = "odd";
            string html = string.Empty;

            html += " <tr role='row' class='" + colorRow + "'>";
            html += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            html += " </tr>";

            if (this._dsEvFinal.Acta.Rows.Count > 0)
            {
                html = string.Empty;
                string numMatricula = string.Empty;
                string promedio = string.Empty;
                string numNivel = string.Empty;
                string estadoCumplimiento = string.Empty;

                foreach (DataRow item in this._dsEvFinal.Acta)
                {
                    colorRow = (colorRow == "even") ? "odd" : "even";

                    numMatricula = this._getNumOrdinal(item["bytNumMat"].ToString(), "matricula");
                    numNivel = this._getNumOrdinal(item["strCodNivel"].ToString(), "nivel");
                    estadoCumplimiento = this._getEstadoCumplimiento(   Convert.ToByte(item["Total"].ToString()),
                                                                        Convert.ToByte(item["bytAsistencia"].ToString()),
                                                                        item["strCodEquiv"].ToString());

                    html += " <tr id='" + item["strCodigo"] + "' role='row' class='" + colorRow + "'>";
                    html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["No"] + "</td>";
                    html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["strCodigo"] + "</td>";
                    html += "     <td style='width: 300px; align-content: center; vertical-align: middle; text-align: left; font-size: 12px;'>" + item["NombreCompleto"].ToString().Trim().ToUpper() + "</td>";
                    html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + numMatricula + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytAcumulado"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytAsistencia"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytNota"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["Total"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + estadoCumplimiento + "</td>";
                    html += "     <td style='width: 100px;align-content: center; vertical-align: middle; text-align: center;font-size: 12px;'>" + item["strObservaciones"] + "</td>";
                    html += " </tr>";
                }
            }

            return html;
        }


        private string _getEstadoCumplimiento( byte total, byte asistencia, string strCodEquivalencia )
        {
            string estCumplimiento = "---";

            try{
                //  APROBADO
                if ( total >= 28 && asistencia >= 70){
                    estCumplimiento = "<span class='label label-success'>" + Language.es_ES.EST_LBL_APROBADO.ToUpper() + "</span>";
                }

                //  EVALUACION RECUPERACION
                if (total >= 16 && total < 28 && asistencia >= 70){
                    estCumplimiento = "<span class='label label-warning'>" + Language.es_ES.EST_LBL_EV_RECUPERACION + "</span>"; ;
                }

                //  REPROBADO - FALTAS
                if (asistencia < 70 && strCodEquivalencia == "R"){
                    estCumplimiento = "<span class='label label-danger'>" + Language.es_ES.LBL_CUMPLIMIENTO_REPROBADO_FALTAS + "</span>"; ;
                }

                //  REPROBADO
                if ( total < 16 && asistencia >= 70 ){
                    estCumplimiento = "<span class='label label-danger'>" + Language.es_ES.LBL_CUMPLIMIENTO_REPROBADO + "</span>"; ;
                }

                //  EXONERADO
                if (total >= 28 && asistencia >= 70 && strCodEquivalencia == "E"){
                    estCumplimiento = "<span class='label label-success'>" + Language.es_ES.LBL_CUMPLIMIENTO_EXONERADO + "</span>"; ;
                }
            }
            catch (Exception ex)
            {
                estCumplimiento = "---";
                Errores err = new Errores();
                err.SetError(ex, "_guardarEvAcumulativa");
            }

            return estCumplimiento;
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
                
                ds = ge.getImprimirActaExPrincipal( this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                    this._strCodAsignatura,
                                                    this._strCodNivel,
                                                    this._strCodParalelo );

                dsActa = (ds != null && ds.Acta.Rows.Count > 0) 
                            ? ds
                            : new dtstEvaluacion_ImprimirActas();
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getDtaImpresionEvAcumulativa");
            }

            return dsActa;
        }


        public string getDtaRptEvFinal(string[] dtaActa, string[] dtaAsignatura, string pathReport, string pathTmp)
        {
            //  Creo el nombre del archivo
            string nameFile = string.Empty;
            string reportPath = string.Empty;
            string dtaParcial = dtaActa[0];
            string nombreAsignatura = string.Empty;

            string idTypeFile = (dtaActa[1] == "pdf" || dtaActa[1] == "blc")
                                    ? "pdf"
                                    : (dtaActa[1] == "xls") ? "Excel"
                                                            : "";
            string strNameFile = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(idTypeFile)){
                    reportPath = (dtaActa[1] == "pdf" || dtaActa[1] == "xls")
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

                    renderedBytes = rptEvFinal.Render(  reportType,
                                                        deviceInfo,
                                                        out mimeType,
                                                        out encoding,
                                                        out fileNameExtension,
                                                        out streams,
                                                        out warnings);

                    nombreAsignatura = this.getNombreAsignatura();
                    nameFile = Language.es_ES.NF_EV_FINAL + "_" + nombreAsignatura.Replace(" / ", "_").ToUpper() + ((dtaActa[1].ToUpper() == "PDF" || dtaActa[1].ToUpper() == "BLC") ? ".pdf" : ".xls");

                    //  Direcciono la creacion del archivo a una ubicacion temporal
                    string fullPath = Path.Combine(pathTmp, nameFile);

                    //  Creo el archivo en la ubicacion temporal
                    System.IO.File.WriteAllBytes(fullPath, renderedBytes);

                    if (!this.estadoParcialEvFinal()){
                        this.cierreGestionNotasEvFinal();
                    }
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

                switch (this.UsuarioActual.CarreraActual.TipoEntidad.ToString())
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

                lstPrmRptEvAcumulativa.Add(new ReportParameter( "strInstitucion",
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


        public bool estadoParcialEvFinal()
        {
            bool ban = true;

            try{
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                string strParcialActivo = this._evaluacion.getDataEvaluacionActiva().Replace("FN", "");
                //string strTipoExamen = (strParcialActivo == "P") 
                //                            ? "PRI" 
                //                            : (strParcialActivo == "S") ? "SUS" : "";

                //  true: acta impresa / false: acta NO impresa
                ban = ne.getActaImpresaEvFinalesRecuperacion(   UsuarioActual.CarreraActual.Codigo.ToString(),
                                                                this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                this._strCodAsignatura,
                                                                this._strCodParalelo,
                                                                strParcialActivo);
            }
            catch (Exception ex)
            {
                ban = true;
                Errores err = new Errores();
                err.SetError(ex, "parcialActivo");
            }

            return ban;
        }


        public void cierreGestionNotasEvFinal()
        {
            try{
                string dtaParcial = this._evaluacion.getDataEvaluacionActiva().Replace("FN", "");
                string tpoExamen = string.Empty;
                
                if (this._dsEvFinal.Acta.Rows.Count > 0){
                    tpoExamen = ( dtaParcial == "P" )   ? "PRI" 
                                                        : ( dtaParcial == "S" ) ? "SUS" 
                                                                                : "";

                    

                    WSNotasEstudiante.dtstNotasEstudiante dsNE = new WSNotasEstudiante.dtstNotasEstudiante();
                    string strCodPeriodo = this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString();

                    foreach (DataRow item in this._dsEvFinal.Acta){
                        DataRow drEvFR = dsNE.EvFinalRecuperacion.NewRow();
                        drEvFR.BeginEdit();

                        drEvFR["sintCodMatricula"] = item["sintCodMatricula"].ToString();
                        drEvFR["strCodPeriodo"] = strCodPeriodo;
                        drEvFR["strCodMateria"] = this._strCodAsignatura;
                        drEvFR["strCodTipoExamen"] = tpoExamen;
                        drEvFR["boolSus"] = 0;
                        drEvFR["strObservacion"] = "";

                        dsNE.Tables["EvFinalRecuperacion"].Rows.Add(drEvFR);
                        drEvFR.EndEdit();
                    }

                    ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                    ne.registrarDatosEvFinalesRecuperacion( this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                            dsNE);

                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "cierreGestionNotasParcial");
            }
        }
    }
}