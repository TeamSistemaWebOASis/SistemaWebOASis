using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace SitioWebOasis.Library
{
    public class HorarioClaseDocente
    {
        private string strCodHora;
        private string strDescripcionHora;
        private string strInicioFinHora;
        private string strLunes;
        private string strMartes;
        private string strMiercoles;
        private string strJueves;
        private string strViernes;
        private string strSabado;
        private string strDomingo;

        public string StrDomingo
        {
            get { return strDomingo; }
            set { strDomingo = value; }
        }


        public string StrSabado
        {
            get { return strSabado; }
            set { strSabado = value; }
        }


        public string StrViernes
        {
            get { return strViernes; }
            set { strViernes = value; }
        }


        public string StrJueves
        {
            get { return strJueves; }
            set { strJueves = value; }
        }


        public string StrMiercoles
        {
            get { return strMiercoles; }
            set { strMiercoles = value; }
        }

        public string StrMartes
        {
            get { return strMartes; }
            set { strMartes = value; }
        }


        public string StrLunes
        {
            get { return strLunes; }
            set { strLunes = value; }
        }

        public string StrInicioFinHora
        {
            get { return strInicioFinHora; }
            set { strInicioFinHora = value; }
        }

        public string StrDescripcionHora
        {
            get { return strDescripcionHora; }
            set { strDescripcionHora = value; }
        }


        public string StrCodHora
        {
            get { return strCodHora; }
            set { strCodHora = value; }
        }
        public List<HorarioClaseDocente> LstHorario()
        {
            List<HorarioClaseDocente> lst = new List<HorarioClaseDocente>();
            return lst;
        }


    }
}