using System;

using System.Collections;
using System.Data;
using SitioWebOasis.CommonClasses.UI;
using OAS_SitioWeb.CommonClasses.GestionUsuarios;
using System.Collections.Generic;
using GestorErrores;

namespace SitioWebOasis.CommonClasses.GestionUsuarios
{
    /// <summary>
    /// Representa a un Usuario de la Aplicación
    /// Web del Sistema Académico.
    /// </summary>
    [Serializable]
	public class Usuario
	{
		private string _Login = null;
		private string _Nombre = null;
		private string _Cedula = null;
        private DateTime _dtInicioSession = default(DateTime);
        List<Carrera> lstCarrerasUP = new List<Carrera>();

		private Rol[] _roles = null;
		private Rol _RolActual = null;
		
		public Usuario()
		{
			this._roles = new Rol[1];
			this._roles[0] = new Rol(Roles.PublicoGeneral,"Público General");
			this._RolActual = this._roles[0];
        }

		public Rol[] roles
		{
			get { return this._roles; }
		}
		
		private int GetNumRoles(SitioWebOasis.WSSeguridad.dtstUsuario dsUsuario)
		{
			int cont = 0;
			DataTable dtRoles = dsUsuario.Tables["roles"];
			foreach (DataRow dr in dtRoles.Rows)
			{
				DataRow[] drRows = dr.GetChildRows("RolesCarreras");
				if (drRows.Length != 0) // rol de carrera
				{
					cont += drRows.Length;
				}
				else // rol convencional
				{
					cont++;
				}
			}

			return cont;
		}
		
        private void _lstUltimosPeriodoVigenteCarrera(SitioWebOasis.WSSeguridad.dtstUsuario dsUsuario)
        {
            try{
                if (dsUsuario.UltimosPeriodos.Rows.Count > 0){
                    foreach (DataRow item in dsUsuario.UltimosPeriodos){
                        Carrera objCarreras = new Carrera(item["strCodCarrera"].ToString(),
                                                            "",
                                                            "",
                                                            "",
                                                            "",
                                                            item["strCodPeriodo"].ToString(), 
                                                            item["strDescripcionPeriodo"].ToString());

                        lstCarrerasUP.Add(objCarreras);
                    }
                }
            }catch(Exception ex){
                GestorErrores.Errores err = new GestorErrores.Errores();
                err.SetError(ex, "CarreraEnPeriodoVigente");
            }
        }

		public Usuario(SitioWebOasis.WSSeguridad.dtstUsuario dsUsuario)
		{
			DataRow drUsuario = dsUsuario.Tables["Usuarios"].Rows[0];
			this._Login = drUsuario["strLogin"].ToString();
			this._Nombre = drUsuario["strNombre"].ToString();
			this._Cedula = drUsuario["strCedula"].ToString();
            this._dtInicioSession = DateTime.Now;
            this._lstUltimosPeriodoVigenteCarrera(dsUsuario);
      
			DataRow[] drRoles = drUsuario.GetChildRows("UsuariosRoles");

			this._roles = new Rol[this.GetNumRoles(dsUsuario)];

			int i = 0;
			foreach (DataRow drRol in drRoles)
			{
				DataRow[] drCarreras = drRol.GetChildRows("RolesCarreras");
				DataRow[] drFacultades = drRol.GetChildRows("RolesFacultades");
				Rol nuevoRol = null;
				
				if (drCarreras.Length != 0) // crear roles de carrera
				{
					foreach (DataRow drCarrera in drCarreras)
					{
						nuevoRol = new RolCarrera(drRol["strIDRol"].ToString(),drRol["strNombreRol"].ToString(),this.CrearCarrera(drCarrera));
						this._roles[i] = nuevoRol;
						if (i == 0) // asignación del rol actual
							this._RolActual = nuevoRol;
						i++;
					}
				}
				else if (drFacultades.Length != 0) // crear roles de facultad
				{
					foreach (DataRow drFacultad in drFacultades)
					{
						nuevoRol = new RolFacultad(drRol["strIDRol"].ToString(),drRol["strNombreRol"].ToString(),this.CrearFacultad(drFacultad));
						this._roles[i] = nuevoRol;
						if (i == 0) // asignación del rol actual
							this._RolActual = nuevoRol;
						i++;
					}
				}
				else // crear rol convencional
				{
					nuevoRol = new Rol(drRol["strIDRol"].ToString(),drRol["strNombreRol"].ToString());
					this._roles[i] = nuevoRol;
					if (i == 0) // asignación del rol actual
						this._RolActual = nuevoRol;
					i++;
				}
			}
		}

		public Rol[] GetRolesCarrera()
		{
			Rol[] roles = new Rol[this._roles.Length];
			int i = 0;

			foreach (Rol rol in this.roles)
				if (rol is RolCarrera)
					roles[i++] = rol;

			if (i != 0)
			{
				Rol[] retRoles = new Rol[i];
				Array.Copy(roles,0,retRoles,0,i);
				return retRoles;
			}
			else
				return null;
		}

		public Rol[] GetRolesFacultad()
		{
			Rol[] roles = new Rol[this._roles.Length];
			int i = 0;

			foreach (Rol rol in this.roles)
				if (rol is RolFacultad)
					roles[i++] = rol;

			if (i != 0)
			{
				Rol[] retRoles = new Rol[i];
				Array.Copy(roles,0,retRoles,0,i);
				return retRoles;
			}
			else
				return null;
		}

		public Rol[] GetRolesCarrera(Roles rol)
		{
			Rol[] RolesCarrera = this.GetRolesCarrera();
			if (RolesCarrera != null)
			{
				Rol[] roles = new Rol[RolesCarrera.Length];
				int i = 0;
				foreach (Rol r in RolesCarrera)
					if (r.ID == rol)
						roles[i++] = r;
        
				Rol[] retRoles = new Rol[i];
				Array.Copy(roles,0,retRoles,0,i);
				return retRoles;
			}
			else
				return null;
		}

		public Rol[] GetRolesFacultad(Roles rol)
		{
			Rol[] RolesFacultad = this.GetRolesFacultad();
			if (RolesFacultad != null)
			{
				Rol[] roles = new Rol[RolesFacultad.Length];
				int i = 0;
				foreach (Rol r in RolesFacultad)
					if (r.ID == rol)
						roles[i++] = r;
        
				Rol[] retRoles = new Rol[i];
				Array.Copy(roles,0,retRoles,0,i);
				return retRoles;
			}
			else
				return null;
		}

		private Carrera CrearCarrera(DataRow drCarrera)
		{
			return new Carrera( drCarrera["strCodigo"].ToString(),
                                drCarrera["strNombre"].ToString(), 
                                drCarrera["strCodTipoEntidad"].ToString(),
                                drCarrera["strSede"].ToString(),
                                drCarrera["strCodUsuario"].ToString());
		}

		private Facultad CrearFacultad(DataRow drFacultad)
		{
			return new Facultad(drFacultad["strCodigo"].ToString(),drFacultad["strNombre"].ToString());
		}

		public string Login
		{
			get { return this._Login; }
		}

		public string Cedula
		{
			get { return this._Cedula; }
		}

		public string Nombre
		{
			get { return this._Nombre; }
		}

        public int getIdleTime()
        {
            var minutes = DateTime.Now - this._dtInicioSession;
            return Convert.ToInt32( minutes.TotalMinutes );
        }

        public void resetSessionTime()
        {
            this._dtInicioSession = DateTime.Now;
        }
		
		private bool BuscarRol(Roles r)
		{
			foreach (Rol rol in this._roles)
				if (rol.Equals(r))
					return true;
			return false;
		}
        

		public Rol RolActual
		{
			get { return this._RolActual; }
			set { this._RolActual = value; }
		}

		public void SetRolCarreraActual(Roles IDRol, string strCodCarrera)
		{
			foreach (Rol r in this.roles)
				if (r is RolCarrera)
				{
					RolCarrera rol = (RolCarrera)r;
					if (rol.Equals(IDRol,strCodCarrera))
					{
						this._RolActual = rol;
						return;
					}
				}
		}

		public void SetRolFacultadActual(Roles IDRol, string strCodFacultad)
		{
			foreach (Rol r in this.roles)
				if (r is RolFacultad)
				{
					RolFacultad rol = (RolFacultad)r;
					if (rol.Equals(IDRol,strCodFacultad))
					{
						this._RolActual = rol;
						return;
					}
				}
		}

		public Carrera CarreraActual
		{
			get
			{
				if ((this._RolActual != null) && (this._RolActual is RolCarrera))
					return ((RolCarrera)this._RolActual).carrera;
				else
					return null;
			}
		}

		public Facultad FacultadActual
		{
			get
			{
				if ((this._RolActual != null) && (this._RolActual is RolFacultad))
					return ((RolFacultad)this._RolActual).facultad;
				else
					return null;
			}
		}

		public bool TieneElRolDe(Roles rol)
		{
			return this.BuscarRol(rol);
		}
        
		public int NumRolesGlobales
		{
			get 
			{
				return (this._roles.Length - this.NumRolesDeCarrera);
			}
		}

		public int NumRolesDeCarrera
		{
			get
			{
				Rol[] roles = this.GetRolesCarrera();
				if (roles != null)
					return roles.Length;
				else
					return 0;
			}
		}

		public int NumRolesDeFacultad
		{
			get
			{
				Rol[] roles = this.GetRolesFacultad();
				if (roles != null)
					return roles.Length;
				else
					return 0;
			}
		}

		public int GetNumRolesDeCarrera(Roles TipoRol)
		{
			int cont = 0;
			foreach (Rol r in this._roles)
				if ((r is RolCarrera) && (r.ID == TipoRol))
					cont++;
			return cont;
		}

		public bool TieneRolGlobal()
		{
			return (this.NumRolesGlobales > 0);
		}

		public bool TieneRolDeCarrera()
		{
			return (this.NumRolesDeCarrera > 0);
		}

		public bool TieneRolDeFacultad()
		{
			return (this.NumRolesDeFacultad > 0);
		}

		public ArrayList GetCarreras(Roles rol)
		{
			ArrayList listaCarreras = new ArrayList();
			foreach (Rol r in this._roles)
			{
				if ((r is RolCarrera) && (r.ID == rol)){
					RolCarrera rolc = (RolCarrera)r;
                    listaCarreras.Add(rolc.carrera);
				}
			}

			return listaCarreras;
		}

		public ArrayList GetFacultades(Roles rol)
		{
			ArrayList listaFacultades = new ArrayList();
			foreach (Rol r in this._roles)
			{
				if ((r is RolFacultad) && (r.ID == rol))
				{
					RolFacultad rolf = (RolFacultad)r;
					listaFacultades.Add(rolf.facultad);
				}
			}
			
            return listaFacultades;
		}


        public Carrera getUltimoPeriodoVigenteCarrera( string strCodCarrera )
        {
            Carrera dtaCarreraPeriodoVigente = null;
            try{
                if( this.lstCarrerasUP.Count > 0){
                    foreach( Carrera item in this.lstCarrerasUP){
                        if (item.Codigo == strCodCarrera) {
                            dtaCarreraPeriodoVigente = item;
                            break;
                        }
                    }
                }
            }catch (Exception ex) {
                dtaCarreraPeriodoVigente = null;
                Errores err = new Errores();
                err.SetError(ex, "getCarreraPeriodoVigente");
            }

            return dtaCarreraPeriodoVigente;
        }


		/// <summary>
		/// Determina los sitios a los que tiene acceso un usuario específico.
		/// Modificar en caso de que aumenten los sitios.
		/// </summary>
		/// <param name="usr">Usuario</param>
		/// <returns>ArrayList lleno con objetos LinkMenu que representan las páginas principales
		/// de los sitios hacia los cuales puede acceder el usuario.</returns>
		/*private ArrayList SubSitiosALosQueTieneAcceso(Usuario usr)
		{
			ArrayList list = new ArrayList();
			
			if (usr.TieneElRolDe(Roles.AdministradorGlobal))
				list.Add(new LinkMenu("Sitio de Administración General","Administracion/AdministradoresHome.aspx"));
			if (usr.TieneElRolDe(Roles.Aspirante))
				list.Add(new LinkMenu("Sitio para aspirantes","Aspirantes/AspirantesHome.aspx"));
			if (usr.TieneElRolDe(Roles.DirectivoCarrera))
				list.Add(new LinkMenu("Sitio para Directivos de Carrera","DirectivosCarrera/DirectivosCarreraHome.aspx"));
			if (usr.TieneElRolDe(Roles.DirectivoFacultad))
				list.Add(new LinkMenu("Sitio para Directivos de Facultad","DirectivosFacultad/DirectivosFacultadHome.aspx"));
			if (usr.TieneElRolDe(Roles.DirectivoInstitucion))
				list.Add(new LinkMenu("Sitio para Directivos Institucionales","DirectivosInstitucion/DirectivosInstitucionHome.aspx"));
			if (usr.TieneElRolDe(Roles.Docente))
				list.Add(new LinkMenu("Sitio para Docentes","Docentes/DocentesHome.aspx"));
			if (usr.TieneElRolDe(Roles.Estudiante))
				list.Add(new LinkMenu("Sitio para Estudiantes","Estudiantes/EstudiantesHome.aspx"));

			list.Add(new LinkMenu("Sitio Principal","Default.aspx"));
			
			return list;
		}*/

		/// <summary>
		/// Determina los sitios a los que tiene acceso un usuario específico.
		/// Nota: Modificar en caso de que aumenten los sitios.
		/// </summary>
		/// <returns>ArrayList lleno con objetos LinkMenu que representan las páginas principales
		/// de los sitios hacia los cuales puede acceder el usuario.</returns>
		public ArrayList SubSitiosALosQueTieneAcceso()
		{
			ArrayList list = new ArrayList();
			
			if (this.TieneElRolDe(Roles.AdministradorGlobal))
				list.Add(new LinkMenu("Sitio de Administración General","Administracion/AdministradoresHome.aspx"));
			if (this.TieneElRolDe(Roles.Aspirante))
				list.Add(new LinkMenu("Sitio para aspirantes","Aspirantes/AspirantesHome.aspx"));
			if (this.TieneElRolDe(Roles.DirectivoCarrera))
				list.Add(new LinkMenu("Sitio para Directivos de Carrera","DirectivosCarrera/DirectivosCarreraHome.aspx"));
			if (this.TieneElRolDe(Roles.DirectivoFacultad))
				list.Add(new LinkMenu("Sitio para Directivos de Facultad","DirectivosFacultad/DirectivosFacultadHome.aspx"));
			if (this.TieneElRolDe(Roles.DirectivoInstitucion))
				list.Add(new LinkMenu("Sitio para Directivos Institucionales","DirectivosInstitucion/DirectivosInstitucionHome.aspx"));
			if (this.TieneElRolDe(Roles.Docentes))
				list.Add(new LinkMenu("Sitio para Docentes","Docentes/DocentesHome.aspx"));
			if (this.TieneElRolDe(Roles.Estudiantes))
				list.Add(new LinkMenu("Sitio para Estudiantes","Estudiantes/EstudiantesHome.aspx"));

			list.Add(new LinkMenu("Sitio Principal","Default.aspx"));
			
			return list;
		}	
	}
}
