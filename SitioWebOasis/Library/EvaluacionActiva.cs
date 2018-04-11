using GestorErrores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using OAS_Seguridad.Cliente;

namespace SitioWebOasis.Library
{
    public class EvaluacionActiva: DatosCarrera
    {
        private string _codCarrera = string.Empty;
        private string _codAsignatura = string.Empty;

        private string _parcialUno = string.Empty;
        private string _parcialDos = string.Empty;
        private string _parcialTres = string.Empty;
        private string _evFinal = string.Empty;
        private string _evRecuperacion = string.Empty;

        private string _prmParcialUno = "FN1";
        private string _prmParcialDos = "FN2";
        private string _prmParcialTres = "FN3";
        private string _prmParcialPrincipal = "FNP";

        private WSGestorDeReportesMatriculacion.dtstCursosDocente _dtstCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();
        private DataRow[] _drDtaPeriodosEvaluacion = default(DataRow[]);
        private string _evaluacionActiva = default(string);
        private DateTime _fchMaximaGestion = default(DateTime);
        private int _numDiasFaltantes = Convert.ToInt16("-1");

        public EvaluacionActiva()
        {
            this._cargaDatosPeriodosEvaluaciones();
            
            this._dtstPeriodoVigente = this._dataPeriodoAcademicoVigente();
            this._evaluacionActiva = this._getEvaluacionActiva().Replace("FN", "");
            this._cargarFchMaximaGestion();
        }
        

        private void _cargaDatosPeriodosEvaluaciones()
        {
            try {
                this._cargarInformacionCarrera();
                ProxySeguro.GestorDefinicionPeriodo gdp = new ProxySeguro.GestorDefinicionPeriodo();
                gdp.CookieContainer = new CookieContainer();
                gdp.set_fBaseDatos(this._strNombreBD);
                gdp.set_fUbicacion(this._strUbicacion);
                WSGestorDefinicionPeriodoCarrera.dtstDatosAdminC_ParametrosCarrera dsParametrosCarrera = gdp.getParametrosCarreraFechas();

                if (dsParametrosCarrera.Parametros_Carrera.Rows.Count > 0){
                    //  Obtengo informacion de los parametros generales del sistema
                    this._drDtaPeriodosEvaluacion = dsParametrosCarrera.Parametros_Carrera.Select("strCodigo IN('FN1', 'FN2', 'FN3', 'FNP', 'FNS')", "strCodigo");
                }

            } catch (Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "_cargaDatosPeriodosEvaluaciones");
            }
        }

        /// <summary>
        ///     Retorma informacion de la evaluacion activa en funcion a los parametros generales del sistema 
        ///     en un determinado periodo academico.
        /// 
        ///     Se considera una "evaluacion activa" a la evaluacion cuya fecha tope de evaluacion es menor a 8 dias 
        ///     en comparacion a la fecha actual
        /// 
        /// </summary>
        /// <returns> Retorna la evaluacion vigente, en caso de no existir ninguna evaluacion activa retorna un valor cero (0) </returns>
        /// 
        private string _getEvaluacionActiva()
        {
            string evaluacionActiva = "";
            string evActiva = string.Empty;
            DateTime fchItem = default(DateTime);
            DateTime fchMenorOchoDias = default(DateTime);
            TimeSpan numDiasDiff = default(TimeSpan);

            try
            {
                string[] dtaCarreraEspecial = this._getDtaCarreraEspecial();

                if (dtaCarreraEspecial[0].Split(',').Contains(this.UsuarioActual.CarreraActual.Codigo.ToString())){
                    fchItem = Convert.ToDateTime(dtaCarreraEspecial[1]);
                    fchMenorOchoDias = fchItem.Date.AddDays(-8);
                    numDiasDiff = DateTime.Now.Date - fchMenorOchoDias.Date;

                    if (fchMenorOchoDias.CompareTo(DateTime.Now.Date) <= 0 && numDiasDiff.TotalDays <= 8){
                        evaluacionActiva = dtaCarreraEspecial[2];
                    }
                }else{
                    if (this._drDtaPeriodosEvaluacion.Length > 0){
                        foreach (DataRow item in this._drDtaPeriodosEvaluacion){
                            if (!string.IsNullOrEmpty(item["strValor"].ToString())){
                                fchItem = Convert.ToDateTime(item["strValor"].ToString());
                                fchMenorOchoDias = (item["strCodigo"].ToString().CompareTo("FNP") != 0)  
                                                        ? fchItem.Date.AddDays(-8) 
                                                        : fchItem.Date.AddDays(-15);

                                numDiasDiff = DateTime.Now.Date - fchMenorOchoDias.Date;

                                //  La fecha planificada debe ser menor a la fecha actual "y" 
                                //  la fecha actual menor o igual a ocho dias
                                if (item["strCodigo"].ToString().CompareTo("FNP") != 0 && fchMenorOchoDias.CompareTo(DateTime.Now.Date) <= 0 && numDiasDiff.TotalDays <= 8){
                                    evaluacionActiva = item["strCodigo"].ToString();
                                    break;
                                }else if (item["strCodigo"].ToString().CompareTo("FNP") == 0 && fchMenorOchoDias.CompareTo(DateTime.Now.Date) <= 0 && numDiasDiff.TotalDays <= 15){
                                    //  La fecha planificada debe ser menor a la fecha actual "y" 
                                    //  la fecha actual menor o igual a quince (15) dias
                                    evaluacionActiva = item["strCodigo"].ToString();
                                    break;
                                }

                            }
                        }
                    }
                }
            }catch(Exception ex){
                evaluacionActiva = "";
                Errores err = new Errores();
                err.SetError(ex, "_getEvaluacionActiva");
            }

            //  return evaluacionActiva;
            return "FN1";
        }

        private string[] _getDtaCarreraEspecial()
        {
            string[] dtaCarreraEspecial = new string[3];

            try{
                var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                dtaCarreraEspecial[0] = appSettings.Get("carreraEspecial").ToString();
                dtaCarreraEspecial[1] = appSettings.Get("ceFchMaxGestion").ToString();
                dtaCarreraEspecial[2] = appSettings.Get("ceParcialActivo").ToString();                
            }
            catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getDtaCarreraEspecial");
            }

            return dtaCarreraEspecial;
        }

        private void _cargarFchMaximaGestion()
        {
            DateTime fchItem = default(DateTime);
            DateTime fchMenorOchoDias = default(DateTime);
            TimeSpan numDiasDiff = default(TimeSpan);

            try
            {
                string[] dtaCarreraEspecial = this._getDtaCarreraEspecial();

                if (dtaCarreraEspecial[0].Split(',').Contains(this.UsuarioActual.CarreraActual.Codigo.ToString()))
                {
                    this._fchMaximaGestion = Convert.ToDateTime(dtaCarreraEspecial[1]);
                    numDiasDiff = this._fchMaximaGestion.Date - DateTime.Now.Date;
                    this._numDiasFaltantes = (numDiasDiff.TotalDays > 0)? Convert.ToInt16(numDiasDiff.TotalDays)
                                                                        : 0;
                }else{
                    if (this._drDtaPeriodosEvaluacion.Length > 0){
                        foreach (DataRow item in this._drDtaPeriodosEvaluacion){
                            if (this._evaluacionActiva == item["strCodigo"].ToString().Replace("FN", "")){
                                this._fchMaximaGestion = Convert.ToDateTime(item["strValor"].ToString());
                                numDiasDiff = this._fchMaximaGestion.Date - DateTime.Now.Date;
                                this._numDiasFaltantes = (numDiasDiff.TotalDays > 0)? Convert.ToInt16(numDiasDiff.TotalDays)
                                                                                    : 0;

                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "_getDtaParametro");
            }

        }


        public string getDataEvaluacionActiva()
        {
            return this._evaluacionActiva;
        }

        public string getInfoEvaluacionActiva()
        {
            string msg = "Gestión cerrada";
            
            if (this._numDiasFaltantes >= 1){
                string msgNumDias = (this._numDiasFaltantes > 1)? "dias"
                                                                : "dia";

                msg = this._fchMaximaGestion.Date.ToString("dd/MM/yyyy") + "( Falta: " + this._numDiasFaltantes.ToString() + " " + msgNumDias + " )";
            }
            else if (this._numDiasFaltantes == 0){
                msg = this._fchMaximaGestion.Date.ToString("dd/MM/yyyy") + "( Ultimo día )";
            }
            else if (this._numDiasFaltantes < 0){
                msg = "Gestión cerrada";
            }

            return msg;
        }


        public int getInfoNumDiasFaltantes()
        {
            return this._numDiasFaltantes;
        }


        public DataRow getProximoParcial()
        {
            DataRow drProximoParcial = default(DataRow);

            try{
                if (this._drDtaPeriodosEvaluacion.Length > 0){
                    foreach (DataRow item in this._drDtaPeriodosEvaluacion){
                        if (DateTime.Now.Date.CompareTo(Convert.ToDateTime(item["strValor"].ToString())) < 0){
                            drProximoParcial = item;
                            break;
                        }
                    }
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "getProximoParcial");
            }

            return drProximoParcial;
        }


        public string getFchInicioEvaluacion(string dtaTpoEvaluacion)
        {
            string strFchInicio = default(string);

            try{
                if (this._drDtaPeriodosEvaluacion.Length > 0){
                    foreach(DataRow item in this._drDtaPeriodosEvaluacion){
                        if ( item["strCodigo"].ToString().Replace("FN", "").CompareTo(dtaTpoEvaluacion.Replace("FN", "")) == 0){
                            DateTime dtFchInicio = Convert.ToDateTime(item["strValor"].ToString()).Date.AddDays(-8);
                            strFchInicio = dtFchInicio.ToString("dd/MM/yyyy");
                            break;
                        }
                    }
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "getFchInicioEvaluacion");
            }

            return strFchInicio;
        }



        public bool getActaImpresa( string codAsignatura, string strCodParalelo)
        {
            bool actaImpresa = false;

            try{
                switch (this._evaluacionActiva){
                    //  Parciales 1, 2 y 3
                    case "1":
                    case "2":
                    case "3":
                        actaImpresa = this._getActaEvAcumulativaImpresa(codAsignatura,
                                                                        strCodParalelo, 
                                                                        this._evaluacionActiva);
                    break;

                    //  Ev. final - Ev. recuperacion
                    case "P":
                    case "S":
                        actaImpresa = this._getActaEvFinalImpresa(  codAsignatura,
                                                                    strCodParalelo,
                                                                    this._evaluacionActiva);
                    break;
                }

            }catch (Exception ex) {
                actaImpresa = true;

                Errores err = new Errores();
                err.SetError(ex, "getActaImpresa");
            }

            return actaImpresa;
        }


        private bool _getActaEvAcumulativaImpresa(string codAsignatura, string strCodParalelo, string strCodParcial )
        {
            bool rst = true;
            int numReg = default(int);

            try
            {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                rst = ne.getActaImpresaEvAcumulativo(   this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                        this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        codAsignatura,
                                                        strCodParalelo,
                                                        strCodParcial);
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getActaEvAcumulativaImpresa");
            }

            return rst;
        }


        private bool _getActaEvFinalImpresa(string codAsignatura, string strCodParalelo, string strTpoExamen )
        {
            bool rst = true;
            int numReg = default(int);

            try{
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();                
                rst = ne.getActaImpresaEvFinalesRecuperacion(   this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                                this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                codAsignatura,
                                                                strCodParalelo, 
                                                                strTpoExamen );
            }
            catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getActaEvAcumulativaImpresa");
            }

            return rst;
        }


    }
}