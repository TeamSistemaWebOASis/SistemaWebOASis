using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Library
{
    public class HorarioExamenDocente
    {
        private string strCodMateria;
        private string strCodParalelo;
        private string strCodNivel;
        private string strDescripcionNivel;
        private string strNombreMateria;
        private string dtFechaExPrinc;
        private string dtFechaExSusp;
        private string strCedula;
        private string strKeyMateria;

        public string StrKeyMateria
        {
            get { return strKeyMateria; }
            set { strKeyMateria = value; }
        }
        
        public string StrCedula
        {
            get { return strCedula; }
            set { strCedula = value; }
        }


        public string DtFechaExSusp
        {
            get { return dtFechaExSusp; }
            set { dtFechaExSusp = value; }
        }


        public string DtFechaExPrinc
        {
            get { return dtFechaExPrinc; }
            set { dtFechaExPrinc = value; }
        }


        public string StrNombreMateria
        {
            get { return strNombreMateria; }
            set { strNombreMateria = value; }
        }


        public string StrDescripcionNivel
        {
            get { return strDescripcionNivel; }
            set { strDescripcionNivel = value; }
        }


        public string StrCodNivel
        {
            get { return strCodNivel; }
            set { strCodNivel = value; }
        }


        public string StrCodParalelo
        {
            get { return strCodParalelo; }
            set { strCodParalelo = value; }
        }


        public string StrCodMateria
        {
            get { return strCodMateria; }
            set { strCodMateria = value; }
        }
        public  List<HorarioExamenDocente> lstHorarioEx()
        {
            List<HorarioExamenDocente> lstHorarioExamen = new List<HorarioExamenDocente>();
            return lstHorarioExamen;
        }

    }
}