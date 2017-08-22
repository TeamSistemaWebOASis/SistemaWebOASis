using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using SitioWebOasis.CommonClasses.UI;
using GestorErrores;

namespace SitioWebOasis.CommonControls
{
	//[ControlBuilderAttribute(typeof(CustomParseControlBuilder))]
	/// <summary>
	/// Menú lateral del sitio web
	/// </summary>
	public abstract class C_Menu : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataList dtlstLinks;
		protected ArrayList _ListaVinculos = new ArrayList();
		protected string _Titulo = null;
		protected bool _LinkHomeVisible = true;

		public int SelectionID
		{
			get
			{
				if (this.ViewState["SelectionID"] != null){
					return (int)this.ViewState["SelectionID"];
                }else{
                    return -1;
                }
			}
			set { this.ViewState["SelectionID"] = value; }
		}

		
		private void SeleccionarLink()
		{
			try
			{
				Session["mensaje"]="";
				Session["mensaje1"]="";
			}
			catch (Exception e)
			{
                Errores err = new Errores();
                err.SetError(e, "SeleccionarLink");
            }

            //  Indice seleccionado
            if (this.SelectionID >= 0) 
				this.dtlstLinks.SelectedIndex = this.SelectionID;
			else // buscar el URL actual en la lista de vínculos
			{
				int i = 0;
				foreach (LinkMenu lnk in this._ListaVinculos)
				{
					if (lnk.FullURLSinQueryString == Request.Path)
					{
						this.dtlstLinks.SelectedIndex = i;
						break;
					}
					i++;
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			
		}

		public string Titulo
		{
			get { return this._Titulo; }
			set { this._Titulo = value; }
		}

		public bool LinkHomeVisible
		{
			get { return this._LinkHomeVisible; }
			set { this._LinkHomeVisible = value; }
		}

		public void AddLink(LinkMenu lnk)
		{
			this._ListaVinculos.Add(lnk);			
		}

		private void dtlstLinks_DataBinding(object sender, System.EventArgs e)
		{
			this.dtlstLinks.DataSource = this._ListaVinculos;
		}

		private void dtlstLinks_SelectedIndexChanged(object sender, System.EventArgs e)
		{

		}



        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //  this.dtlstLinks.DataBinding += new System.EventHandler(this.dtlstLinks_DataBinding);
            //  this.dtlstLinks.SelectedIndexChanged += new System.EventHandler(this.dtlstLinks_SelectedIndexChanged);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion
    }
}
