using System;

namespace SitioWebOasis.CommonClasses.GestionUsuarios
{
	/// <summary>
	/// Representa una Facultad específica
	/// dentro de la ESPOCH.
	/// </summary>
	[Serializable]
	public class Facultad
	{

		private string _Codigo = null;
		private string _Nombre = null;
		    
		public Facultad(string strCodigo, string strNombre)
		{
			this._Codigo = strCodigo;
			this._Nombre = strNombre;
		}

		public string Codigo
		{
			get { return this._Codigo; }
		}

		public string Nombre
		{
			get { return this._Nombre; }
		}

		public override string ToString()
		{
			return this._Nombre;
		}

	}
}
