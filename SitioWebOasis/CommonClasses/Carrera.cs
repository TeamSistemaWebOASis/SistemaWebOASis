using System;

namespace SitioWebOasis.CommonClasses.GestionUsuarios
{
	/// <summary>
	/// Representa una Carrera específica
	/// dentro de la ESPOCH.
	/// </summary>
	[Serializable]
	public class Carrera
	{
		private string _Codigo = null;
		private string _Nombre = null;
        private string _tipoEntidad = null;
        private string _strSede = null;
        private string _codUsuario = null;
        private string _strCodPeriodo = null;
        private string _strDescripcionPeriodo = null;


        public Carrera(string strCodigo, string strNombre, string strTpoEntidad, string strSede, string strCodUsuario, string strCodPeriodo = "", string strDescripcionPeriodo = "")
		{
			this._Codigo = strCodigo;
			this._Nombre = strNombre;
            this._tipoEntidad = strTpoEntidad;
            this._strSede = strSede;
            this._codUsuario = strCodUsuario;
            this._strCodPeriodo = strCodPeriodo;
            this._strDescripcionPeriodo = strDescripcionPeriodo;
        }

        public string Codigo
		{
			get { return this._Codigo; }
		}

		public string Nombre
		{
			get { return this._Nombre; }
		}

		public string TipoEntidad
		{
			get { return this._tipoEntidad; }
		}

        public string SedeCarrera
        {
            get { return this._strSede; }
        }

        public string codUsuario
        {
            get { return this._codUsuario; }
        }

		public override string ToString()
		{
			return this._Nombre;
		}

        public string strCodPeriodo
        {
            get { return this._strCodPeriodo; }
        }


        public string strDescripcionPeriodo
        {
            get { return this._strDescripcionPeriodo; }
        }

    }
}
