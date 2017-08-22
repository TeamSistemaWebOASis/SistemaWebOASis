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

using SitioWebOasis.CommonControls;

namespace SitioWebOasis.CommonClasses.UI
{
	/// <summary>
	/// Clase base para la mayoría de páginas de este Sitio Web, permite
	/// que la página pertenezca a un subsitio específico
	/// </summary>
	public class OASisPage : System.Web.UI.Page
	{
		protected C_Header ucCabeceraGeneral;
		protected C_Menu ucMenu;
			
		/// <summary>
		/// Retorna el SubSitio de la cabecera
		/// determinando qué título y vínculos (tabs) aparecerán en la misma
		/// </summary>
		protected string SubSite
		{
			get { return this.ucCabeceraGeneral.SubSite; }
			//set { this.ucCabeceraGeneral.SubSite = value; }
		}
        

		protected override void OnInit(EventArgs e)
		{
			//this.VerificarExistenciaDeUsuario();
			base.OnInit(e);
		}

		protected virtual void CargarLinksEnMenu()
		{
			// será sobreecrito en clases derivadas para cargar items en los menus
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			/* los links del menú deben ser cargados después de que se hayan procesado
			 los eventos de los controles de la página porque el menú puede cambiar
			 dinámicamente (por ejemplo al cambiar de carrera) */
			this.CargarLinksEnMenu();
		} 
	}
}