using System;

using System.Collections;
using SitioWebOasis.ProxySeguro;
using System.Data;
using SitioWebOasis.CommonClasses.GestionUsuarios;

namespace OAS_SitioWeb.CommonClasses.GestionUsuarios
{
	/// <summary>
	/// Representa a un rol de facultad dentro del Sitio Web
	/// </summary>
	[Serializable]
	public class RolFacultad : Rol
	{
		private Facultad _facultad = null;
		
		public RolFacultad(Roles ID, string strNombre, Facultad facultad) : base(ID, strNombre)
		{
			this._facultad = facultad;
		}

		public RolFacultad(string ID, string strNombre, Facultad facultad) : base(ID, strNombre)
		{
			this._facultad = facultad;
		}

		public override bool Equals(string rol)
		{
			return rol == this.ID.ToString() + " " + this._facultad.Nombre;
		}

		public bool Equals(Roles ID, string CodFacultad)
		{
			return ((this.ID == ID) && (this._facultad.Codigo == CodFacultad));
		}

		public Facultad facultad
		{
			get { return this._facultad; }
		}

		public override string ToString()
		{
			return base.ToString() + ", " + this._facultad.ToString();
		}
	}
}
