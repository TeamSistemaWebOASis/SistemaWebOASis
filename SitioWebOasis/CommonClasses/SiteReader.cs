using SitioWebOasis.CommonClasses.UI;
using System;
using System.Data;
using System.Web;
using System.Web.Caching;

namespace OAS_SitioWeb.CommonClasses.UI
{
	/// <summary>
	/// Descendiente de DataSet que provee acceso hacia la estructura
	/// de datos del archivo Sitemap.xml.
	/// </summary>
	public class SiteReader : DataSet
	{
		private string _SubSite = null;
		protected static string _XMLConfigFile = System.Configuration.ConfigurationManager.AppSettings["SubSitesConfigFile"].ToString();

        public string SubSite
		{
			get { return this._SubSite; }
			set { this._SubSite = value; }
		}

		public static SiteReader ActualSiteReader
		{
			get
			{

			if (HttpContext.Current.Cache.Get("SiteReader") == null)
				{
					CargarSiteReaderEnCache();
				}

				return (SiteReader)HttpContext.Current.Cache.Get("SiteReader");
			}
		}

		private static void CargarSiteReaderEnCache()
		{
			string strFileName = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath) + "\\" + _XMLConfigFile;
			SiteReader reader = new SiteReader(strFileName);
			HttpContext.Current.Cache.Insert("SiteReader",reader,new CacheDependency(strFileName),Cache.NoAbsoluteExpiration,Cache.NoSlidingExpiration,CacheItemPriority.BelowNormal,new CacheItemRemovedCallback(SiteReader.ReCargarConfigFile));
		}

		//callback
		private static void ReCargarConfigFile(string key, object value, CacheItemRemovedReason reason)
		{
			CargarSiteReaderEnCache();
		}


		public SiteReader(string strXMLSiteConfigFile)
		{
			this.ReadXml(strXMLSiteConfigFile,XmlReadMode.InferSchema);
		}

		private DataRow GetSubSite()
		{
			DataTable site = this.Tables["site"];
			DataRow drSite = site.Rows[0];

			DataRow[] subsites = drSite.GetChildRows("site_subsite");
			foreach (DataRow drSubSite in subsites)
				if (drSubSite["id"].ToString() == this._SubSite)
					return drSubSite;
			return null;
		}

		public LinkMenu this[int index]
		{
			get
			{
				DataRow drSubSite = this.GetSubSite();
				DataRow[] pages = drSubSite.GetChildRows("subsite_page");
				DataRow drPage = pages[index];
				string strLnk = drPage["href"].ToString();
				return new LinkMenu(drPage["name"].ToString(),strLnk);
			}
		}

		public int CountPages
		{
			get 
			{
				DataRow drSubSite = this.GetSubSite();
				DataRow[] pages = drSubSite.GetChildRows("subsite_page");
        return pages.Length;
			}
		}

		public string SiteName
		{
			get 
			{
				return this.Tables["site"].Rows[0]["name"].ToString();
			}
		}

		public string SubSiteName
		{
			get 
			{
				DataRow dr = this.GetSubSite();
				if (dr != null)
          return dr["name"].ToString();
				else
					return null;
			}
		}


	}
}
