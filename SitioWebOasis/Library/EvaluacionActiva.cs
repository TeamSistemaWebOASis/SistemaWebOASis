using GestorErrores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

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

        public EvaluacionActiva() { }
        

        public EvaluacionActiva( string idCarrera, string codAsignatura )
        {
            this._codCarrera = idCarrera;
            this._codAsignatura = codAsignatura;
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
        public string getDtaEvaluacionActiva()
        {
            string dtaEvaluacionActiva = "NA";
            string evActiva = string.Empty;

            try
            {
                ProxySeguro.GestorAdministracionGeneral gag = new ProxySeguro.GestorAdministracionGeneral();
                WSAdministracionGeneral.dtstDatosAdminG_Parametros dsParametros = gag.getDatosParametros();

                //  Obtengo informacion de los parametros generales del sistema
                DataRow[] drParametros = dsParametros.Parametros_Sistema.Select("strCodigo IN('FN1', 'FN2', 'FN3', 'FNP')", "strCodigo");

                foreach(DataRow item in drParametros){
                    if( !string.IsNullOrEmpty( item["strValor"].ToString() ) ){
                        DateTime fchItem = Convert.ToDateTime(item["strValor"].ToString());
                        DateTime fchMenorOchoDias = fchItem.Date.AddDays(-8);
                        TimeSpan numDiasDiff = DateTime.Now.Date - fchMenorOchoDias.Date;

                        //  La fecha planificada debe ser menor a la fecha actual "y" 
                        //  la fecha actual menor o igual a ocho dias
                        if(fchMenorOchoDias.CompareTo(DateTime.Now.Date) <= 0 && numDiasDiff.TotalDays <= 8 ) { 
                            switch (item["strCodigo"].ToString()) {
                                case "FN1": dtaEvaluacionActiva = "1"; break;
                                case "FN2": dtaEvaluacionActiva = "2"; break;
                                case "FN3": dtaEvaluacionActiva = "3"; break;
                                case "FNP": dtaEvaluacionActiva = "EF"; break;
                            }

                            break;
                        }
                    }
                }
            }
            catch(Exception ex){
                dtaEvaluacionActiva = string.Empty;

                Errores err = new Errores();
                err.SetError(ex, "_getDtaParametro");
            }


            //  return dtaEvaluacionActiva;
            return "ER";
        }



        public string getActaImpresa( string codAsignatura, string evActiva)
        {
            string numActa = "NA";

            try{
                switch (evActiva)
                {
                    //  Parciales 1, 2 y 3
                    case "1":
                    case "2":
                    case "3":
                        numActa = (this._getActaEvAcumulativaImpresa(codAsignatura, evActiva)) ? "NA" : evActiva;
                    break;

                    //  Ev. final - Ev. Recuperacion
                    case "EF":
                        numActa = (this._getActaEvFinalImpresa(codAsignatura, evActiva) ) ? "NA" : evActiva;
                    break;
                }

            }catch (Exception ex) {
                numActa = "NA";
                Errores err = new Errores();
                err.SetError(ex, "getActaImpresa");
            }

            return numActa;
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