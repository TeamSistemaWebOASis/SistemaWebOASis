using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Library
{
    public class ReporteMatriculacionEstudiante
    {
        private string strCodigo;
        private string strCedula;
        private string strApellidos;
        private string strNombres;
        private string strCodPeriodo;
        private string strDescripcionPeriodo;
        private string strCodNivelMatricula;
        private string strDescrNivelMatricula;
        private string bytNumMatricula;
        private string strObservacionesMatAsignada;
        private string strObsercacionesMatricula;
        private DateTime dtFechaMatricula;
        private string strNivelMateria;
        private string strDescrNivelMateria;
        private string strCodMateria;
        private string strNombreMateria;
        private Boolean blnPeriodoVigente;
        private int intCodMatricula;

        public int IntCodMatricula
        {
            get { return intCodMatricula; }
            set { intCodMatricula = value; }
        }


        public Boolean BlnPeriodoVigente
        {
            get { return blnPeriodoVigente; }
            set { blnPeriodoVigente = value; }
        }


        public string StrNombreMateria
        {
            get { return strNombreMateria; }
            set { strNombreMateria = value; }
        }


        public string StrCodMateria
        {
            get { return strCodMateria; }
            set { strCodMateria = value; }
        }

        public string StrDescrNivelMateria
        {
            get { return strDescrNivelMateria; }
            set { strDescrNivelMateria = value; }
        }


        public string StrNivelMateria
        {
            get { return strNivelMateria; }
            set { strNivelMateria = value; }
        }


        public DateTime DtFechaMatricula
        {
            get { return dtFechaMatricula; }
            set { dtFechaMatricula = value; }
        }

        public string StrObsercacionesMatricula
        {
            get { return strObsercacionesMatricula; }
            set { strObsercacionesMatricula = value; }
        }


        public string StrObservacionesMatAsignada
        {
            get { return strObservacionesMatAsignada; }
            set { strObservacionesMatAsignada = value; }
        }


        public string BytNumMatricula
        {
            get { return bytNumMatricula; }
            set { bytNumMatricula = value; }
        }

        public string StrDescrNivelMatricula
        {
            get { return strDescrNivelMatricula; }
            set { strDescrNivelMatricula = value; }
        }


        public string StrCodNivelMatricula
        {
            get { return strCodNivelMatricula; }
            set { strCodNivelMatricula = value; }
        }

        public string StrDescripcionPeriodo
        {
            get { return strDescripcionPeriodo; }
            set { strDescripcionPeriodo = value; }
        }


        public string StrCodPeriodo
        {
            get { return strCodPeriodo; }
            set { strCodPeriodo = value; }
        }

        public string StrNombres
        {
            get { return strNombres; }
            set { strNombres = value; }
        }



        public string StrApellidos
        {
            get { return strApellidos; }
            set { strApellidos = value; }
        }

        public string StrCedula
        {
            get { return strCedula; }
            set { strCedula = value; }
        }


        public string StrCodigo
        {
            get { return strCodigo; }
            set { strCodigo = value; }
        }

        public List<ReporteMatriculacionEstudiante> listaEstudianteMatricula() {
            List<ReporteMatriculacionEstudiante> objEstMa = new List<ReporteMatriculacionEstudiante>();
            return objEstMa;
        }

    }
}