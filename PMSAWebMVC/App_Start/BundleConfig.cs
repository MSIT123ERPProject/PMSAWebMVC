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
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好可進行生產時，請使用 https://modernizr.com 的建置工具，只挑選您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            //共用
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/fontawesome-all.css",
                 //Bootstrap
                 //"~/Content/bootstrap.css",
                 "~/Content/MDB/css/bootstrap.css",                
                //MDB
                "~/Content/MDB/css/mdb.css",
                "~/Content/MDB/css/addons/datatables.min.css",
                "~/Content/MDB/css/addons/datatables-select.min.css",
                "~/Content/MDB/css/addons/directives.min.css",
                "~/Content/MDB/css/addons/flag.min.css",
                "~/Content/MDB/css/addons/jquery.zmd.hierarchical-display.min.css",
                "~/Content/MDB/css/addons/rating.min.css",
                "~/Content/MDB/css/modules/animations-extended.css",
                //SB Admin
                //"~/Content/sbTemplate/sb-admin.css",
                //Site
                "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                //Bootstrap
                "~/Scripts/MDB/popper.js",
                //"~/Scripts/bootstrap.js",
                "~/Scripts/MDB/bootstrap.js",
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
                //SB Admin
                "~/Scripts/jquery.easing.js",
                "~/Scripts/sbTemplate/sb-admin.js",
                //Site
                "~/Scripts/Site.js"
                ));

            //表單
            bundles.Add(new StyleBundle("~/Content/form").Include(
                //DatetimePicker
                "~/Content/tempusdominus/tempusdominus-bootstrap-4.css",
                 //pickadate
                 "~/Content/pickadate/themes/classic.css",
                 "~/Content/pickadate/themes/classic.date.css",
                 "~/Content/pickadate/themes/classic.time.css",
                //SweetAlert
                "~/Content/sweetAlert/sweetalert2.css",
                //MagicInput
                "~/Content/MagicInput/magic-input.css",
                "~/Content/MagicInput/magic-input.site.css"
                ));
            bundles.Add(new ScriptBundle("~/bundles/form").Include(
                //DataTables、DatetimePicker
                "~/Scripts/moment-with-locales.js",
                //DatetimePicker
                "~/Scripts/tempusdominus/tempusdominus-bootstrap-4.js",
                "~/Scripts/tempusdominus/tempusdominus-bootstrap-4.zh-TW.js",
                 //pickadate
                 "~/Scripts/pickadate/picker.js",
                 "~/Scripts/pickadate/picker.date.js",
                 "~/Scripts/pickadate/picker.time.js",
                 "~/Scripts/pickadate/translations/zh_TW.js",
                //SweetAlert
                "~/Scripts/sweetAlert/sweetalert2.all.js"
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

            //toastr
            bundles.Add(new ScriptBundle("~/bundles/toastrcss").Include(
                "~/Content/toastr.min.css"
                ));
            bundles.Add(new ScriptBundle("~/bundles/toastrjs").Include(
                "~/Scripts/toastr.min.js"
                ));
            
        }
    }
}