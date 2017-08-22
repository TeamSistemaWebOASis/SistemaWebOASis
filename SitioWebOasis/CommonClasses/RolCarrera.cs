using System;

using System.Collections;
using SitioWebOasis.ProxySeguro;
using System.Data;

namespace SitioWebOasis.CommonClasses.GestionUsuarios
{
	/// <summary>
	/// Representa a un rol de carrera dentro del Sitio Web
	/// </summary>
	[Serializable]
	public class RolCarrera : Rol
	{
		private Carrera _carrera = null;
		
		public RolCarrera(Roles ID, string strNombre, Carrera carrera) : base(ID, strNombre)
		{
			this._carrera = carrera;
		}

		public RolCarrera(string ID, string strNombre, Carrera carrera) : base(ID, strNombre)
		{
			this._carrera = carrera;
		}
		
		public override bool Equals(string rol)
		{
			return rol == this.ID.ToString() + " " + this._carrera.Nombre;
		}

		public bool Equals(Roles ID, string CodCarrera)
		{
			return ((this.ID == ID) && (this._carrera.Codigo == CodCarrera));
		}
        
		public Carrera carrera
		{
			get { return this._carrera; }
		}

		public override string ToString()
		{
			return base.ToString() + ", " + this._carrera.ToString();
		}
	}
}