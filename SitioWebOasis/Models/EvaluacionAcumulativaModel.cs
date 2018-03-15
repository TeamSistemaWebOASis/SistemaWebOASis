using GestorErrores;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAS_Seguridad.Cliente;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Web.Helpers;

namespace SitioWebOasis.Models
{
    public class EvaluacionAcumulativaModel: Asignatura
    {
        public string jsonEvAcumulativa { get; set; }
        private string _dtaEvAcumulativa = string.Empty;
        private WSGestorEvaluacion.dtstEvaluacion_Acumulados _dsEvAcumulativa = new WSGestorEvaluacion.dtstEvaluacion_Acumulados();
        //  DatosAcademicosDocente _dad;

        public EvaluacionAcumulativaModel( string strCodNivel, string strCodAsignatura, string strCodParalelo )
        {
            this._strCodAsignatura = strCodAsignatura;
            this._strCodNivel = strCodNivel;
            this._strCodParalelo = strCodParalelo;
            this._cargarInformacionCarrera();

            this._dsEvAcumulativa = this._CargarNotasEvAcumulativa();

            this.jsonEvAcumulativa = (this._dsEvAcumulativa.Acta.Rows.Count > 0)
                                        ? JsonConvert.SerializeObject(this._dsEvAcumulativa.Acta)
                                        : "";
        }

        private WSGestorEvaluacion.dtstEvaluacion_Acumulados _CargarNotasEvAcumulativa()
        {
            WSGestorEvaluacion.dtstEvaluacion_Acumulados rstEvAcumulativa = new WSGestorEvaluacion.dtstEvaluacion_Acumulados();
            WSGestorEvaluacion.dtstEvaluacion_Acumulados dsEvAcumulativa = new WSGestorEvaluacion.dtstEvaluacion_Acumulados();

            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD);
                ge.set_fUbicacion(this._strUbicacion);

                rstEvAcumulativa = ge.crearActaArtificialEvaluaciones(  this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                        this._strCodAsignatura,
                                                                        this._strCodNivel,
                                                                        this._strCodParalelo);

                dsEvAcumulativa = (rstEvAcumulativa != null)
                                        ? rstEvAcumulativa
                                        : new WSGestorEvaluacion.dtstEvaluacion_Acumulados();

            }
            catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getAsignaturasDocente");
            }

            return dsEvAcumulativa;
        }
        
        
        public string getHTML_EvaluacionAcumulativa()
        {
            string colorRow = "odd";
            string html = string.Empty;

            html += " <tr role='row' class='" + colorRow + "'>";
            html += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            html += " </tr>";

            if (this._dsEvAcumulativa.Acta.Rows.Count > 0)
            {
                html = string.Empty;
                string numMatricula = string.Empty;
                string promedio = string.Empty;
                string numNivel = string.Empty;
                string estadoCumplimiento = string.Empty;
                string parcialActivo = this._evaluacion.getDataEvaluacionActiva();

                foreach (DataRow item in this._dsEvAcumulativa.Acta)
                {
                    colorRow = (colorRow == "even") ? "odd" : "even";

                    numMatricula = this._getNumOrdinal(item["bytNumMat"].ToString(), "matricula");
                    numNivel = this._getNumOrdinal(item["strCodNivel"].ToString(), "nivel");
                    estadoCumplimiento = (parcialActivo == "3" || parcialActivo == "P")
                                            ? this._getEstadoCumplimiento(item["Total"].ToString(), item["bytAsistencia"].ToString())
                                            : "---";

                    html += " <tr id='" + item["strCodigo"] + "' role='row' class='" + colorRow + "'>";
                    html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["No"] + "</td>";
                    html += "     <td style='width: 30px; align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["strCodigo"] + "</td>";
                    html += "     <td style='width: 300px; align-content: center; vertical-align: middle; text-align: left; font-size: 12px;'>" + item["NombreEstudiante"].ToString().Trim().ToUpper() + "</td>";
                    html += "     <td style='width: 30px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + numMatricula + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytNota1"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytNota2"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytNota3"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["Total"] + "</td>";
                    html += "     <td style='width: 40px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + item["bytAsistencia"] + "</td>";
                    html += "     <td style='width: 50px;align-content: center; vertical-align: middle; text-align: center; font-size: 12px;'>" + estadoCumplimiento + "</td>";
                    html += "     <td style='width: 100px;align-content: center; vertical-align: middle; text-align: center;font-size: 12px;'>" + item["strObservaciones"] + "</td>";
                    html += " </tr>";
                }
            }

            return html;
        }


        public bool registrarEvaluacionAcumulativa(List<EvaluacionAcumulativa> dtaEvAcumulativa )
        {
            bool rst = false;

            try{
                //  Recorro DataTable Acta - registro por registro actualizando la informacion
                if( this._updEvAcumulativa(dtaEvAcumulativa)){
                    rst = this._guardarEvAcumulativa();
                    this._dsEvAcumulativa = this._CargarNotasEvAcumulativa();

                    this.jsonEvAcumulativa = (this._dsEvAcumulativa.Acta.Rows.Count > 0)
                                                ? JsonConvert.SerializeObject(this._dsEvAcumulativa.Acta)
                                                : "";
                }
            }
            catch (System.Exception ex)
            {
                rst = false;

                Errores err = new Errores();
                err.SetError(ex, "registrarEvaluacionAcumulativa");
            }

            return rst;
        }


        private bool _updEvAcumulativa(List<EvaluacionAcumulativa> dtaEvAcumulativa)
        {
            bool rst = false;
            byte nota = default(byte);
            string parcialActivo = this._evaluacion.getDataEvaluacionActiva();

            try{
                int numReg = dtaEvAcumulativa.Count;
                if (numReg > 0 && this._dsEvAcumulativa.Acta.Rows.Count > 0)
                {
                    for (int x = 0; x < numReg; x++)
                    {
                        if (this._dsEvAcumulativa.Acta.Rows[0]["sintCodMatricula"].ToString() == dtaEvAcumulativa[0].sintCodMatricula.ToString()){
                            this._dsEvAcumulativa.Acta.Rows[x]["bytAsistencia"] = Convert.ToByte(dtaEvAcumulativa[x].bytAsistencia.ToString());
                            switch (parcialActivo){
                                case "1": nota = Convert.ToByte(dtaEvAcumulativa[x].bytNota1.ToString()); break;
                                case "2": nota = Convert.ToByte(dtaEvAcumulativa[x].bytNota2.ToString()); break;
                                case "3": nota = Convert.ToByte(dtaEvAcumulativa[x].bytNota3.ToString()); break;
                            }

                            this._dsEvAcumulativa.Acta.Rows[x]["bytNota" + parcialActivo.Replace("FN", "")] = nota;
                        }
                    }

                    this._dsEvAcumulativa.AcceptChanges();
                    rst = true;
                }
            }
            catch(Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "_updEvEstudiantes");
            }

            return rst;
        }


        private bool _guardarEvAcumulativa()
        {
            bool rst = false;
            bool regEvAcumulado = false;
            bool regFchRegistro = false;

            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fUbicacion(this._strUbicacion);
                ge.set_fBaseDatos(this._strNombreBD);

                //  Registro de acta
                regEvAcumulado = ge.setActaEvaluaciones(this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        this._strCodAsignatura.ToString(),
                                                        this._strCodNivel.ToString(),
                                                        this._strCodParalelo.ToString(),
                                                        this._dsEvAcumulativa,
                                                        this.UsuarioActual.Nombre.ToString());

                //  Registro de fecha de ingreso de evaluaciones
                regFchRegistro = ge.ActualizarRegistroFechaIngresoEvaluaciones(this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                                this._strCodAsignatura.ToString(),
                                                                                this._strCodNivel.ToString(),
                                                                                this._strCodParalelo.ToString(),
                                                                                DateTime.Now);

                rst = (regEvAcumulado && regFchRegistro) ? true
                                                        : false;
            }
            catch (System.Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_guardarEvAcumulativa");
            }

            return rst;
        }


        private string _getEstadoCumplimiento(string totalAcumulado, string porcientoAsistencia)
        {
            string estCumplimiento = "---";
            int ta = Convert.ToInt16(totalAcumulado);
            int pa = Convert.ToInt16(porcientoAsistencia);

            try
            {
                //  EXONERADO
                if( ta >= 25 && pa >= 70 ){
                    estCumplimiento = "<span class='label label-success'>"+ Language.es_ES.LBL_CUMPLIMIENTO_EXONERADO +"</span>";
                }

                //  EVALUACION FINAL
                if (ta >= 12 && ta < 25 && pa >= 70){
                    estCumplimiento = "<span class='label label-default'>"+ Language.es_ES.LBL_CUMPLIMIENTO_EV_FINAL +"</span>"; ;
                }

                //  REPROBADO
                if (ta < 12 && pa >= 70){
                    estCumplimiento = "<span class='label label-danger'>" + Language.es_ES.LBL_CUMPLIMIENTO_REPROBADO + "</span>"; ;
                }

                //  REPROBADO - FALTAS
                if (pa < 70){
                    estCumplimiento = "<span class='label label-danger'>" + Language.es_ES.LBL_CUMPLIMIENTO_REPROBADO_FALTAS + "</span>"; ;
                }
            }catch(Exception ex){
                estCumplimiento = "---";
                Errores err = new Errores();
                err.SetError(ex, "_guardarEvAcumulativa");
            }

            return estCumplimiento;
        }
        

        public LocalReport getRptEvAcumulativa(string reportPath)
        {
            LocalReport rptEvAcumulativa = new LocalReport();

            try{
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

            try{
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD);
                ge.set_fUbicacion(this._strUbicacion);

                rstEvAcumulativa = ge.getImprimirActaEvaluaciones(this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
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

                lstPrmRptEvAcumulativa.Add(new ReportParameter( "strInstitucion",
                                                                Language.es_ES.STR_INSTITUCION) );

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
            catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getDatosGeneralesReporte");
            }

            return lstPrmRptEvAcumulativa;
        }


        public bool estadoParcialEvAcumulativa()
        {
            bool ban = true;

            try {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                string strParcialActivo = this._evaluacion.getDataEvaluacionActiva().Replace("FN", "");

                //  true: acta impresa / false: acta NO impresa
                ban = ne.getActaImpresaEvAcumulativo(   UsuarioActual.CarreraActual.Codigo.ToString(),
                                                        this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        this._strCodAsignatura,
                                                        this._strCodParalelo,
                                                        strParcialActivo );
            } catch (Exception ex) {
                ban = true;
                Errores err = new Errores();
                err.SetError(ex, "parcialActivo");
            }

            return ban;
        }


        public void cierreGestionNotasParcial()
        {
            string parcial1 = "0";
            string parcial2 = "0";
            string parcial3 = "0";

            try{
                string dtaParcial = this._evaluacion.getDataEvaluacionActiva().Replace("FN", "");

                if (this._dsEvAcumulativa.Acta.Rows.Count > 0)
                {
                    switch (dtaParcial)
                    {
                        case "1":
                            parcial1 = "1";
                            break;

                        case "2":
                            parcial1 = "1";
                            parcial2 = "1";
                            break;

                        case "3":
                            parcial1 = "1";
                            parcial2 = "1";
                            parcial3 = "1";
                            break;
                    }

                    ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                    int numRegEA = ne.getNumRegistrosEvAcumulativo(this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                                    this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                    this._strCodAsignatura.ToString());

                    if (dtaParcial == "1"){
                        if (numRegEA == 0){
                            this._addCerrarGestionParcial(parcial1, parcial2, parcial3);
                        }else{
                            this._updCerrarGestionParcial(parcial1, parcial2, parcial3);
                        }
                    }
                    else if (dtaParcial == "2" || dtaParcial == "3"){
                        if (numRegEA == 0){
                            this._addCerrarGestionParcial(parcial1, parcial2, parcial3);
                        }else{
                            this._updCerrarGestionParcial(parcial1, parcial2, parcial3);
                        }
                    }
                }
            }
            catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "cierreGestionNotasParcial");
            }
        }


        private bool _addCerrarGestionParcial(string parcial1, string parcial2, string parcial3)
        {
            bool rst = true;

            try
            {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                foreach (DataRow item in this._dsEvAcumulativa.Acta){
                    ne.CerrarGestionNotasParcial(   this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                    Convert.ToInt16(item["sintCodMatricula"].ToString()),
                                                    this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                    this._strCodAsignatura,
                                                    Convert.ToByte(parcial1),
                                                    Convert.ToByte(parcial2),
                                                    Convert.ToByte(parcial3),
                                                    "Cierre por Sitio Web");
                }
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_addCerrarGestionParcial");
            }

            return rst;
        }


        private bool _updCerrarGestionParcial( string parcial1, string parcial2, string parcial3)
        {
            bool rst = false;

            try
            {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();

                foreach (DataRow item in this._dsEvAcumulativa.Acta){
                    ne.updCierreGestionNotasParcial(this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                    Convert.ToInt16(item["sintCodMatricula"].ToString()),
                                                    this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                    this._strCodAsignatura,
                                                    Convert.ToByte(parcial1),
                                                    Convert.ToByte(parcial2),
                                                    Convert.ToByte(parcial3),
                                                    "Cierre por Sitio Web");
                }

                rst = true;
            }catch(Exception ex){
                rst = false;

                Errores err = new Errores();
                err.SetError(ex, "_addCerrarGestionParcial");
            }

            return rst;
        }


        public string getDtaRptEvAcumulativa(string[] dtaActa, string[] dtaAsignatura, string pathReport, string pathTmp)
        {
            //  Creo el nombre del archivo
            string nameFile = string.Empty;
            string reportPath = string.Empty;
            string dtaParcial = dtaActa[0];
            string nombreAsignatura = string.Empty;

            string idTypeFile = (dtaActa[1] == "pdf" || dtaActa[1] == "blc" )
                                    ? "pdf"
                                    : (dtaActa[1] == "xls") ? "Excel"
                                                            : "rar";
            string strNameFile = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(idTypeFile)){
                    reportPath = (dtaActa[1] == "pdf" || dtaActa[1] == "xls")
                                    ? Path.Combine(pathReport, "rptActaEvaluacionesConNotas.rdlc")
                                    : Path.Combine(pathReport, "rptActaEvaluacionesSinNotas.rdlc");

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

                    nombreAsignatura = this.getNombreAsignatura();

                    nombreAsignatura = this.getNombreAsignatura();
                    nameFile = Language.es_ES.NF_EV_ACUMULATIVA + "_" + nombreAsignatura.Replace(" / ", "_").ToUpper() + ((dtaActa[1].ToUpper() == "PDF" || dtaActa[1].ToUpper() == "BLC") ? ".pdf" : ".xls");

                    //  Direcciono la creacion del archivo a una ubicacion temporal
                    string fullPath = Path.Combine(pathTmp, nameFile);

                    //  Creo el archivo en la ubicacion temporal
                    System.IO.File.WriteAllBytes(fullPath, renderedBytes);

                    if (!this.estadoParcialEvAcumulativa()){
                        this.cierreGestionNotasParcial();
                    }
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
    }
}