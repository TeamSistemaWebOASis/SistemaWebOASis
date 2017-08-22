using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class EvaluacionesDocenteModel: DatosCarrera
    {
        public string strCodAsignatura{get; set;}

        public string strCodNivel { get; set;}

        public string strCodParalelo { get; set;}

        public string parcialActivo = "3";

        public EvaluacionAcumulativaModel evAcumulativaModel { get; set; }
    }
}