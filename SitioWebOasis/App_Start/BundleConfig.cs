using System.Web;
using System.Web.Optimization;

namespace SitioWebOasis
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //  HOJAS DE ESTILO ("StyleBundle")
            bundles.Add(new StyleBundle("~/Content/css").Include(   "~/Content/css/bootstrap.min.css",
                                                                    "~/Content/css/font-awesome.min.css",
                                                                    "~/Content/css/main.css",
                                                                    "~/Content/css/my-custom-styles.css",
                                                                    "~/Content/css/skins/slategray.css",
                                                                    "~/Content/css/ui.jqgrid-bootstrap.css", 
                                                                    "~/Content/css/HoldOn.min.css"));

            //  ARCHIVOS JavaScript("ScriptBundle")
            bundles.Add(new ScriptBundle("~/bundles/js").Include(   "~/Content/js/es-ES.js",
                                                                    "~/Content/js/jquery/jquery-2.1.0.min.js",
                                                                    "~/Content/js/bootstrap/bootstrap.min.js",
                                                                    "~/Content/js/modernizr/modernizr.js",
                                                                    "~/Content/js/jquery-slimscroll/jquery.slimscroll.min.js",
                                                                    "~/Content/js/king-common.js",
                                                                    "~/Content/js/bootstrap-tour/bootstrap-tour.custom.js",
                                                                    "~/Content/js/king-chart-stat-transparent.js",
                                                                    "~/Content/js/king-chart-stat.js",
                                                                    "~/Content/js/king-components-transparent.js",
                                                                    "~/Content/js/king-components.js",
                                                                    "~/Content/js/king-elements.js",
                                                                    "~/Content/js/king-form-layouts.js",
                                                                    "~/Content/js/king-page-transparent.js",
                                                                    "~/Content/js/king-page.js",
                                                                    "~/Content/js/bootstrap-multiselect/bootstrap-multiselect.js",
                                                                    "~/Content/js/jquery-plugin/jquery-maskedinput/jquery.masked-input.min.js",
                                                                    "~/Content/js/jquery-validation/jquery-validation-v1.16.0-min.js",
                                                                    "~/Content/js/jquery.redirect.js",
                                                                    "~/Content/js/jqgrid/jquery.jqGrid.min.js",
                                                                    "~/Content/js/jqgrid/i18n/grid.locale-en.js",
                                                                    "~/Content/js/jqgrid/jquery.jqGrid.fluid.js",
                                                                    "~/Content/js/HoldOn.min.js",
                                                                    "~/Content/js/jquery-sparkline/jquery.sparkline.min.js"));
        }
    }
}
