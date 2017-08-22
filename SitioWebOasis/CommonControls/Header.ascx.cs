namespace SitioWebOasis.CommonControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections;

    using SitioWebOasis.CommonClasses.UI;
    using OAS_SitioWeb.CommonClasses.UI;
    /// <summary>
    ///	Cabecera del Sitio Web.
    /// </summary>
    public abstract class C_Header : System.Web.UI.UserControl
    {
        protected System.Web.UI.WebControls.DataList dtlstTabs;
        protected System.Web.UI.WebControls.Label lblFecha;
        protected ArrayList _ListaVinculos = new ArrayList();
        protected string _SubSite = null;
        protected System.Web.UI.WebControls.Label lblSubSite;
        protected System.Web.UI.WebControls.Label lblSite;
        protected int _SelTab = -1;

        public int SelTab
        {
            get { return this._SelTab; }
            set { this._SelTab = value; }
        }

        public int CountTabs
        {
            get { return this._ListaVinculos.Count; }
        }

        public string SubSite
        {
            get { return this._SubSite; }
            set
            {
                this._SubSite = value;
                this._ListaVinculos.Clear();
                SiteReader reader = SiteReader.ActualSiteReader;
                reader.SubSite = this._SubSite;
                for (int i = 0; i < reader.CountPages; i++)
                {
                    this.AddTab(reader[i]);

                    // seleccionar el tab
                    string strURL = Request.Path;
                    
                    if (strURL == reader[i].FullURL)
                        this._SelTab = i;
                }

            }
        }

        public void AddTab(LinkMenu lnk)
        {
            this._ListaVinculos.Add(lnk);
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            // configurar títulos
            if (!Page.IsPostBack)
            {
                SiteReader reader = SiteReader.ActualSiteReader;
                reader.SubSite = this._SubSite;
                this.lblSite.Text = reader.SiteName;
                this.lblSubSite.Text = reader.SubSiteName;
            }

            // Seleccionar tab
            if (this._SelTab != -1)
            {
                this.dtlstTabs.SelectedIndex = this._SelTab;
            }

            this.EscribirTabs();

            this.EscribirFecha();
        }

        private void EscribirFecha()
        {
            if (!Page.IsPostBack)
                this.lblFecha.Text = DateTime.Now.ToLongDateString();
        }

        private void EscribirTabs()
        {
            if (!Page.IsPostBack)
            {
                this.dtlstTabs.DataSource = this._ListaVinculos;
                this.dtlstTabs.DataBind();
            }
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
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

    }
}
