using GestorErrores;
using System;

namespace SitioWebOasis.CommonClasses.UI
{
    /// <summary>
    /// Representa un hipervínculo.
    /// </summary>
    public class LinkMenu
    {
        public LinkMenu(string strLname, string strURL)
        {
            this._LinkName = strLname;
            this._LinkURL = strURL;
        }
        private string _LinkName = null;
        private string _LinkURL = null;

        /// <summary>
        /// Nombre del vínculo
        /// </summary>
        public string LinkName
        {
            get { return this._LinkName; }
        }

        /// <summary>
        /// URL completo del vínculo, incluyendo a cualquier querystring
        /// </summary>
        public string FullURL
        {
            get
            {
                if (this._LinkURL != ""){
                    string path = System.Web.HttpContext.Current.Request.Url.Host + "/" + this._LinkURL;
                    return path;
                }else{
                    return "";
                }
            }
        }

        /// <summary>
        /// URL completo del vínculo excluyendo al QueryString
        /// </summary>
        public string FullURLSinQueryString
        {
            get
            {
                char[] chr = new char[] { '?' };
                string[] str = this.FullURL.Split(chr);
                if (str != null)
                    return str[0];
                else
                    return this.FullURL;
            }
        }

        /// <summary>
        /// Crea un hipervínculo HTML
        /// </summary>
        /// <returns></returns>
        public string GetHTMLLink()
        {
            //  return "<a href=\"" + this.FullURL + "\">" + this._LinkName + "</a>";
            return "<a href=\"" + this._LinkURL + "\">" + this._LinkName + "</a>";
        }




        public string getLinkURL()
        {
            return this._LinkURL;
        }


    }
}
