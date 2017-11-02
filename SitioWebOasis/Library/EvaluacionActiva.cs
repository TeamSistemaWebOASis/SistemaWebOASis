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
        private int _numDiasFaltantes = default(Int16);

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
                ProxySeguro.GestorAdministracionGeneral gag = new ProxySeguro.GestorAdministracionGeneral();
                gag.CookieContainer = new CookieContainer();
                WSAdministracionGeneral.dtstDatosAdminG_Parametros dsParametros = gag.getDatosParametros();

                if( dsParametros.Parametros_Sistema.Rows.Count > 0){
                    //  Obtengo informacion de los parametros generales del sistema
                    this._drDtaPeriodosEvaluacion = dsParametros.Parametros_Sistema.Select("strCodigo IN('FN1', 'FN2', 'FN3', 'FNP')", "strCodigo");
                }

            } catch (Exception ex) {
                Errores err = new Errores();
                err.SetError(ex, "_getDtaParametro");
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
        private string _getEvaluacionActiva()
        {
            string evaluacionActiva = "NA";
            string evActiva = string.Empty;

            try{
                if (this._drDtaPeriodosEvaluacion.Length > 0){
                    foreach (DataRow item in this._drDtaPeriodosEvaluacion){
                        if (!string.IsNullOrEmpty(item["strValor"].ToString())){
                            DateTime fchItem = Convert.ToDateTime(item["strValor"].ToString());
                            DateTime fchMenorOchoDias = fchItem.Date.AddDays(-8);
                            TimeSpan numDiasDiff = DateTime.Now.Date - fchMenorOchoDias.Date;

                            //  La fecha planificada debe ser menor a la fecha actual "y" 
                            //  la fecha actual menor o igual a ocho dias
                            if (fchMenorOchoDias.CompareTo(DateTime.Now.Date) <= 0 && numDiasDiff.TotalDays <= 8){
                                evaluacionActiva = item["strCodigo"].ToString();
                                break;
                            }
                        }
                    }
                }
            }
            catch(Exception ex){
                evaluacionActiva = "NA";

                Errores err = new Errores();
                err.SetError(ex, "_getDtaParametro");
            }

            return evaluacionActiva;
        }
        

        private void _cargarFchMaximaGestion()
        {
            string dtFchMaximaGestion = default(string);
            try{

                if (this._drDtaPeriodosEvaluacion.Length > 0){
                    foreach (DataRow item in this._drDtaPeriodosEvaluacion){
                        if (this._evaluacionActiva == item["strCodigo"].ToString().Replace("FN", "")){
                            this._fchMaximaGestion = Convert.ToDateTime( item["strValor"].ToString() );
                            TimeSpan numDiasDiff = this._fchMaximaGestion.Date - DateTime.Now.Date;
                            this._numDiasFaltantes = (numDiasDiff.TotalDays > 0)? Convert.ToInt16( numDiasDiff.TotalDays )
                                                                                : 0;

                            break;
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
            string msg = string.Empty;

            if (this._numDiasFaltantes >= 1){
                string msgNumDias = (this._numDiasFaltantes > 1)? "dias"
                                                                : "dia";

                msg = this._fchMaximaGestion.Date.ToString("dd/MM/yyyy") + "( Falta: " + this._numDiasFaltantes.ToString() + " " + msgNumDias + " )";
            }
            else if (this._numDiasFaltantes == 0){
                msg = this._fchMaximaGestion.Date.ToString("dd/MM/yyyy") + "( Ultimo día )";
            }else if (this._numDiasFaltantes < 0) {
                msg = "Gestión cerrada";
            }

            return msg;
        }


        public int getInfoNumDiasFaltantes()
        {
            return this._numDiasFaltantes;
        }


        public bool getActaImpresa( string codAsignatura)
        {
            bool actaImpresa = false;

            try{
                switch (this._evaluacionActiva){
                    //  Parciales 1, 2 y 3
                    case "1":
                    case "2":
                    case "3":
                        actaImpresa = (this._getActaEvAcumulativaImpresa(   codAsignatura, 
                                                                            this._evaluacionActiva)) ? true : false;
                    break;

                    //  Ev. final - Ev. Recuperacion
                    case "EF":
                        actaImpresa = (this._getActaEvFinalImpresa( codAsignatura, 
                                                                    this._evaluacionActiva) ) ? true : false;
                    break;
                }

            }catch (Exception ex) {
                actaImpresa = true;

                Errores err = new Errores();
                err.SetError(ex, "getActaImpresa");
            }

            return actaImpresa;
        }


        private bool _getActaEvAcumulativaImpresa(string codAsignatura, string strCodParcial )
        {
            bool rst = true;
            int numReg = default(int);

            try
            {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                rst = ne.getActaImpresaEvAcumulativo(   this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                        this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        codAsignatura, 
                                                        strCodParcial);
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getActaEvAcumulativaImpresa");
            }

            return rst;
        }


        private bool _getActaEvFinalImpresa(string codAsignatura, string strCodEvaluacion )
        {
            bool rst = true;
            int numReg = default(int);

            try
            {
                ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
                rst = ne.getActaImpresaEvFinalesRecuperacion(   this.UsuarioActual.CarreraActual.Codigo.ToString(),
                                                                this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                                codAsignatura,
                                                                strCodEvaluacion);
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getActaEvAcumulativaImpresa");
            }

            return rst;
        }


    }
}