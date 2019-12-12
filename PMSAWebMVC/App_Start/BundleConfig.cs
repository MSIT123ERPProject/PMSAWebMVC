using System.Web;
using System.Web.Optimization;

namespace PMSAWebMVC
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-migrate/jquery-migrate-3.1.0.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好可進行生產時，請使用 https://modernizr.com 的建置工具，只挑選您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                  "~/Scripts/modernizr-*"
                ));

            //共用
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/fontawesome-all.css",
                "~/Content/IconKit/css/iconkit.css",
                 //SB Admin
                 //"~/Content/sbTemplate/sb-admin.css",
                 //Bootstrap
                 //"~/Content/bootstrap.css",
                 "~/Content/MDB/css/bootstrap.css",
                //MDB
                "~/Content/MDB/css/mdb.css",
                "~/Content/MDB/css/bootstrap.theme.css",
                "~/Content/MDB/css/addons/datatables.min.css",
                "~/Content/MDB/css/addons/datatables-select.min.css",
                "~/Content/MDB/css/addons/directives.min.css",
                "~/Content/MDB/css/addons/flag.min.css",
                "~/Content/MDB/css/addons/jquery.zmd.hierarchical-display.min.css",
                "~/Content/MDB/css/addons/rating.min.css",
                "~/Content/MDB/css/modules/animations-extended.css",
                //Site
                "~/Content/Site.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                //Bootstrap
                "~/Scripts/MDB/popper.js",
                //"~/Scripts/bootstrap.js",
                "~/Scripts/MDB/bootstrap.js",
                //SB Admin
                "~/Scripts/jquery.easing.js",
                "~/Scripts/sbTemplate/sb-admin.js",
                //MDB
                "~/Scripts/MDB/mdb.js",
                "~/Scripts/MDB/addons/datatables.min.js",
                "~/Scripts/MDB/addons/datatables-select.min.js",
                "~/Scripts/MDB/addons/directives.min.js",
                "~/Scripts/MDB/addons/flag.min.js",
                "~/Scripts/MDB/addons/imagesloaded.pkgd.min.js",
                "~/Scripts/MDB/addons/jquery.zmd.hierarchical-display.min.js",
                "~/Scripts/MDB/addons/masonry.pkgd.min.js",
                "~/Scripts/MDB/addons/rating.min.js",
                "~/Scripts/MDB/modules/animations-extended.min.js",
                "~/Scripts/MDB/modules/forms-free.min.js",
                "~/Scripts/MDB/modules/scrolling-navbar.min.js",
                "~/Scripts/MDB/modules/treeview.min.js",
                "~/Scripts/MDB/modules/wow.min.js",
                //Site
                "~/Scripts/Site.js"
                ));

            //表單
            bundles.Add(new StyleBundle("~/Content/form").Include(
                //DatetimePicker
                "~/Content/tempusdominus/tempusdominus-bootstrap-4.css",
                //SweetAlert
                "~/Content/sweetAlert/sweetalert2.css",
                //MagicInput
                "~/Content/MagicInput/magic-input.css",
                "~/Content/MagicInput/magic-input.site.css",
                //toastr
                "~/Content/toastr.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/form").Include(
                //DataTables、DatetimePicker
                "~/Scripts/moment-with-locales.js",
                //DatetimePicker
                "~/Scripts/tempusdominus/tempusdominus-bootstrap-4.js",
                "~/Scripts/tempusdominus/tempusdominus-bootstrap-4.zh-TW.js",
                //SweetAlert
                "~/Scripts/sweetAlert/sweetalert2.all.js",
                //toastr
                "~/Scripts/toastr.js"
                ));

            //DataTables
            bundles.Add(new StyleBundle("~/Content/DataTables").Include(
                "~/Content/DataTables/css/datatables.css",
                "~/Content/DataTables/css/dataTables.site.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
                "~/Scripts/DataTables/datatables.js",
                "~/Scripts/DataTables/i18n/jquery.dataTables.zh-TW.js"
                ));

            //HighCharts
            bundles.Add(new ScriptBundle("~/bundles/HighCharts").Include(
                "~/Scripts/highcharts/highcharts.js"
                ));

            //Slick
            bundles.Add(new StyleBundle("~/Content/Slick").Include(
                "~/Content/slick/slick.css",
                "~/Content/slick/slick-theme.css",
                "~/Content/slick/slick-theme.site.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/Slick").Include(
                "~/Scripts/slick/slick.js"
                ));

        }
    }
}