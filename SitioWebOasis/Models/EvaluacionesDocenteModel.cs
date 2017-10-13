using GestorErrores;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class EvaluacionesDocenteModel: DatosAcademicosDocente
    {
        public string strCodAsignatura{get; set;}


        public string strCodNivel { get; set;}


        public string strCodParalelo { get; set;}
        

        public EvaluacionesDocenteModel(){}


        public EvaluacionAcumulativaModel evAcumulativaModel { get; set; }


        public EvaluacionFinalModel evFinalModel { get; set;}


        public EvaluacionRecuperacionModel evRecuperacionModel { get; set; }

    }
}