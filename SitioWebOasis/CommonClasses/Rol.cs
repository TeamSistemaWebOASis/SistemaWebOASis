using System;

namespace SitioWebOasis.CommonClasses.GestionUsuarios
{
	public enum Roles {PublicoGeneral,Aspirante,Estudiantes,Docentes,DirectivoInstitucion,DirectivoFacultad,DirectivoCarrera,AdministradorGlobal};
	
	/// <summary>
	/// Representa un rol global dentro del Sitio Web.
	/// </summary>
	[Serializable]
	public class Rol
	{

		private Roles _ID = Roles.PublicoGeneral;
		private string _Nombre = "Público General";

		public Rol()
		{
		}

		public Rol(Roles ID, string strNombre)
		{
      this._ID = ID;
			this._Nombre = strNombre;
		}

		public Rol(string ID, string strNombre)
		{
      this._Nombre = strNombre;
			Roles r = Roles.PublicoGeneral;
			switch (ID)
			{
				case "ASP":
					r = Roles.Aspirante;
					break;
				case "EST":
					r = Roles.Estudiantes;
					break;
				case "DOC":
					r = Roles.Docentes;
					break;
				case "DIRINST":
					r = Roles.DirectivoInstitucion;
					break;
				case "DIRFAC":
					r = Roles.DirectivoFacultad;
					break;
				case "DIRCAR":
					r = Roles.DirectivoCarrera;
					break;
				case "ADMINGLO":
					r = Roles.AdministradorGlobal;
					break;
			}
			this._ID = r;
		}

		public bool Equals(Roles r)
		{
			return r.Equals(this._ID);
		}

		public virtual bool Equals(string rol)
		{
			return rol == this._ID.ToString();
		}

		public Roles ID
		{
			get { return this._ID; }
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
