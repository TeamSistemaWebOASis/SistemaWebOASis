using GestorErrores;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class EvaluacionesDocenteModel: Asignatura
    {
        public EvaluacionAcumulativaModel evAcumulativaModel { get; set; }


        public EvaluacionFinalModel evFinalModel { get; set; }


        public EvaluacionRecuperacionModel evRecuperacionModel { get; set; }


        public string strCodAsignatura { get { return this._strCodAsignatura; } }

        public string strCodNivel { get { return this._strCodNivel; } }

        public string strCodParalelo { get { return this._strCodParalelo; } }

        public string getDtaEvaluacionActiva { get { return this._evaluacion.getDataEvaluacionActiva(); } }

        public EvaluacionAcumulativaModel evAcumulativa;

        public EvaluacionFinalModel evFinal;

        public EvaluacionRecuperacionModel evRecuperacion;

        public EvaluacionesDocenteModel(string strCodNivel, string strCodAsignatura, string strCodParalelo)
        {
            this._strCodNivel = strCodNivel;
            this._strCodAsignatura = strCodAsignatura;
            this._strCodParalelo = strCodParalelo;

            //  modelo - EVALUACION ACUMULATIVA
            this.evAcumulativa = new EvaluacionAcumulativaModel(strCodNivel,
                                                                strCodAsignatura,
                                                                strCodParalelo);

            //  modelo - EVALUACION FINAL
            this.evFinal = new EvaluacionFinalModel(strCodNivel,
                                                    strCodAsignatura,
                                                    strCodParalelo);

            //  modelo - EVALUACION RECUPERACION
            this.evRecuperacion = new EvaluacionRecuperacionModel(  strCodNivel,
                                                                    strCodAsignatura,
                                                                    strCodParalelo );
        }
        
    }
}