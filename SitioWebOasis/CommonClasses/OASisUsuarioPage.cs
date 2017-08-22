using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using SitioWebOasis.CommonClasses.GestionUsuarios;

namespace SitioWebOasis.CommonClasses.UI
{
	/// <summary>
	/// Clase base para una página en la cual se desea tener acceso
	/// al objeto Usuario almacenado en sesión a través de la porpiedad
	/// UsuarioActual
	/// </summary>
	public class OASisUsuarioPage : OASisPage
	{
    protected override void OnInit(EventArgs e)
		{
			this.VerificarExistenciaDeUsuario();
			base.OnInit(e);
		}

		/// <summary>
		/// Retorna una instancia de la clase usuario que representa
		/// al usuario actual (autenticado o no) que está accediendo
		/// a la aplicación Web
		/// </summary>
		protected virtual Usuario UsuarioActual
		{
			get	{ return this.Session["UsuarioActual"] as Usuario; }
		}

		private void VerificarExistenciaDeUsuario()
		{
			if (this.UsuarioActual == null)
				throw new Exception("No se tiene acceso al objeto Usuario");
		}

	}
}