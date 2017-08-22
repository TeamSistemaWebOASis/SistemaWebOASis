using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Library
{
    public class EvaluacionAcumulativa
    {
        public string No { get; set; }

        public string sintCodMatricula { get; set; }

        public string Nombre { get; set; }

        public string Nivel { get; set; }

        public double bytNota1 { get; set; }

        public double bytNota2 { get; set; }

        public double bytNota3 { get; set; }

        public int total { get; set; }

        public int bytAsistencia { get; set; }

        public string observaciones { get; set; }
    }
}