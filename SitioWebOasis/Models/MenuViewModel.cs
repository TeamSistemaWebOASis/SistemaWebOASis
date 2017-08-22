using SitioWebOasis.CommonClasses.GestionUsuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class MenuViewModel
    {
        public int MenuID { get; set; }

        public string Title { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public bool IsAction { get; set; }

        public string Link { get; set; }

        public string Class { get; set; }

        public string icon { get; set; }

        public List<MenuViewModel> SubMenu { get; set; }
    }
}