using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Library
{
    public class EvaluacionRecuperacion
    {
        public int No { get; set; }

        public string NombreCompleto { get; set; }

        public int Total { get; set; }

        public double bytAcumulado { get; set; }

        public double bytAsistencia { get; set; }

        public double bytNota { get; set; }

        public double bytNumMat { get; set; }

        public short sintCodMatricula { get; set; }

        public string strCodEquiv { get; set; }

        public string strCodMateria { get; set; }

        public string strCodNivel { get; set; }

        public string strCodParalelo { get; set; }

        public string strCodPeriodo { get; set; }

        public string strCodTipoExamen { get; set; }

        public string strCodigo { get; set; }

        public string strObservaciones { get; set; }
        
    }
}