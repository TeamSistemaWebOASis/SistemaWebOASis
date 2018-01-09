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
    public class EvaluacionRecuperacionModel : Asignatura
    {
        public string jsonEvRecuperacion { get; set; }
        private WSGestorEvaluacion.dtstEvaluacion_Actas _dsEvRecuperacion = new WSGestorEvaluacion.dtstEvaluacion_Actas();


        public EvaluacionRecuperacionModel(string strCodNivel, string strCodAsignatura, string strCodParalelo)
        {
            this._strCodNivel = strCodNivel;
            this._strCodAsignatura = strCodAsignatura;
            this._strCodParalelo = strCodParalelo;
            this._cargarInformacionCarrera();
            this._dsEvRecuperacion = this._CargarNotasEvRecuperacion();

            this.jsonEvRecuperacion = (this._dsEvRecuperacion.Acta.Rows.Count > 0)
                                        ? JsonConvert.SerializeObject(this._dsEvRecuperacion.Acta)
                                        : "";
        }


        private WSGestorEvaluacion.dtstEvaluacion_Actas _CargarNotasEvRecuperacion()
        {
            WSGestorEvaluacion.dtstEvaluacion_Actas rstEvRecuperacion = new WSGestorEvaluacion.dtstEvaluacion_Actas();
            WSGestorEvaluacion.dtstEvaluacion_Actas dsEvRecuperacion = new WSGestorEvaluacion.dtstEvaluacion_Actas();

            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD.ToString());
                ge.set_fUbicacion(this._strUbicacion.ToString());

                rstEvRecuperacion = ge.crearActaArtificialSuspension(   this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                        this._strCodAsignatura,
                                                                        this._strCodNivel,
                                                                        this._strCodParalelo );
                dsEvRecuperacion = (rstEvRecuperacion != null) 
                                        ? rstEvRecuperacion 
                                        : new WSGestorEvaluacion.dtstEvaluacion_Actas();
            }
            catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_CargarNotasEvRecuperacion");
            }

            return dsEvRecuperacion;
        }


        public bool registrarEvaluacionRecuperacion(List<EvaluacionRecuperacion> dtaEvRecuperacion)
        {
            bool rst = false;

            try
            {
                //  Recorro DataTable Acta - registro por registro actualizando la informacion
                if (this._updEvRecuperacion(dtaEvRecuperacion))
                {
                    if (this._guardarEvRecuperacion()){
                        this._dsEvRecuperacion = this._CargarNotasEvRecuperacion();
                        this.jsonEvRecuperacion = (this._dsEvRecuperacion.Acta.Rows.Count > 0)
                                                        ? JsonConvert.SerializeObject(this._dsEvRecuperacion.Acta)
                                                        : "";

                        rst = true;
                    }
                }
            }catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionRecuperacion");
            }

            return rst;
        }

        
        private bool _updEvRecuperacion(List<EvaluacionRecuperacion> dtaEvRecuperacion)
        {
            bool rst = false;

            try
            {
                int numReg = dtaEvRecuperacion.Count;
                if (numReg > 0 && this._dsEvRecuperacion.Acta.Rows.Count > 0){
                    for (int x = 0; x < numReg; x++){
                        if (this._dsEvRecuperacion.Acta.Rows[0]["sintCodMatricula"].ToString() == dtaEvRecuperacion[0].sintCodMatricula.ToString()){
                            this._dsEvRecuperacion.Acta.Rows[x]["bytNota"] = Convert.ToByte(dtaEvRecuperacion[x].bytNota.ToString());
                        }
                    }

                    this._dsEvRecuperacion.AcceptChanges();
                    rst = true;
                }
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_updEvRecuperacion");
            }

            return rst;
        }


        private bool _guardarEvRecuperacion()
        {
            bool rst = false;
            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fUbicacion(this._strUbicacion);
                ge.set_fBaseDatos(this._strNombreBD);

                //  Registro informacion de evaluacion recuperacion
                rst = ge.setActaArtificialSuspension(   this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        this._strCodAsignatura.ToString(),
                                                        this._strCodNivel.ToString(),
                                                        this._strCodParalelo.ToString(),
                                                        this._dsEvRecuperacion,
                                                        this.UsuarioActual.Nombre.ToString());

                //  Actualizar fecha de registro de notas
                ge.ActualizarRegistroFechaIngresoExSuspension(  this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                this._strCodAsignatura.ToString(),
                                                                this._strCodNivel.ToString(),
                                                                this._strCodParalelo.ToString(),
                                                                DateTime.Now);
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_guardarEvFinal");
            }

            return rst;
        }

        
        public string getHTML_EvaluacionRecuperacion()
        {
            string colorRow = "odd";
            string html = string.Empty;

            html += " <tr role='row' class='" + colorRow + "'>";
            html += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            html += " </tr>";

            if (this._dsEvRecuperacion.Acta.Rows.Count > 0)
            {
                html = string.Empty;
                string numMatricula = string.Empty;
                string promedio = string.Empty;
                string numNivel = string.Empty;
                string estadoCumplimiento = string.Empty;

                foreach (DataRow item in this._dsEvRecuperacion.Acta)
                {
                    colorRow = (colorRow == "even") ? "odd" : "even";

                    numMatricula = this._getNumOrdinal(item["bytNumMat"].ToString(), "matricula");
                    numNivel = this._getNumOrdinal(item["strCodNivel"].ToString(), "nivel");
                    estadoCumplimiento = this._getEstadoCumplimiento(   Convert.ToByte(item["Total"].ToString()),
                                                                        item["strCodEquiv"].ToString());

                    html += " <tr id='" + item["strCodigo"] + "' role='row' class='" + colorRow + "'>";
                    html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["No"] + "</td>";
                    html += "     <td style='width: 300px; align-content: center; vertical-align: middle; text-align: left; font-size: 12px;'>" + item["NombreCompleto"].ToString().Trim().ToUpper() + "</td>";
                    html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + numMatricula + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytAcumulado"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytNota"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["Total"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + estadoCumplimiento + "</td>";
                    html += "     <td style='width: 100px;align-content: center; vertical-align: middle; text-align: center;font-size: 12px;'>" + item["strObservaciones"] + "</td>";
                    html += " </tr>";
                }
            }

            return html;
        }


        private string _getEstadoCumplimiento(byte total, string strCodEquivalencia)
        {
            string estCumplimiento = "---";

            try
            {
                //  APROBADO
                if (total >= 28)
                {
                    estCumplimiento = "<span class='label label-success'>" + Language.es_ES.EST_LBL_APROBADO.ToUpper() + "</span>";
                }
                
                //  REPROBADO
                if (total < 28 )
                {
                    estCumplimiento = "<span class='label label-danger'>" + Language.es_ES.LBL_CUMPLIMIENTO_REPROBADO + "</span>"; ;
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




        public string getDtaRptEvRecuperacion(string[] dtaActa, string[] dtaAsignatura, string pathReport, string pathTmp)
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

                if (!string.IsNullOrEmpty(idTypeFile))
                {
                    reportPath = (dtaActa[1] == "pdf" || dtaActa[1] == "xls")
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

                    renderedBytes = rptEvRecuperacion.Render(   reportType,
                                                                deviceInfo,
                                                                out mimeType,
                                                                out encoding,
                                                                out fileNameExtension,
                                                                out streams,
                                                                out warnings);

                    nombreAsignatura = this.getNombreAsignatura();

                    nameFile = Language.es_ES.NF_EV_RECUPERACION + "_" + nombreAsignatura.Replace(" / ", "_").ToUpper() + ((dtaActa[1].ToUpper() == "PDF" || dtaActa[1].ToUpper() == "BLC") ? ".pdf" : ".xls");

                    //  Direcciono la creacion del archivo a una ubicacion temporal
                    string fullPath = Path.Combine(pathTmp, nameFile);

                    //  Creo el archivo en la ubicacion temporal
                    System.IO.File.WriteAllBytes(fullPath, renderedBytes);

                    if (!this.estadoParcialEvRecuperacion()){
                        this.cierreGestionNotasEvRecuperacion();
                    }
                }
            }catch (Exception ex){
                nameFile = "-1";

                Errores err = new Errores();
                err.SetError(ex, "getDtaRptEvFinal");
            }

            return nameFile;
        }


        public bool estadoParcialEvRecuperacion()
        {
            bool ban = true;

            try
            {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                string strParcialActivo = this._evaluacion.getDataEvaluacionActiva().Replace("FN", "");

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


        public void cierreGestionNotasEvRecuperacion()
        {
            try
            {
                string dtaParcial = this._evaluacion.getDataEvaluacionActiva().Replace("FN", "");
                string tpoExamen = string.Empty;

                if (this._dsEvRecuperacion.Acta.Rows.Count > 0){
                    tpoExamen = (dtaParcial == "P") ? "PRI"
                                                        : (dtaParcial == "S") ? "SUS"
                                                                                : "";

                    WSNotasEstudiante.dtstNotasEstudiante dsNE = new WSNotasEstudiante.dtstNotasEstudiante();
                    string strCodPeriodo = this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString();

                    foreach (DataRow item in this._dsEvRecuperacion.Acta){
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
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "cierreGestionNotasEvRecuperacion");
            }
        }



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
            }catch (Exception ex){
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

                rstEvRecuperacion = ge.getImprimirActaExSuspension( this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                    this._strCodAsignatura,
                                                                    this._strCodNivel,
                                                                    this._strCodParalelo);

                dsEvRecuperacion = (rstEvRecuperacion != null)
                                        ? rstEvRecuperacion
                                        : new dtstEvaluacion_ImprimirActas();

            }catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getDtaImpresionEvRecuperacion");
            }

            return dsEvRecuperacion;
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
                this._getDatosMateria(ref strAsignatura,
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
                                                                "ESPOCH"));

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
        
    }
}