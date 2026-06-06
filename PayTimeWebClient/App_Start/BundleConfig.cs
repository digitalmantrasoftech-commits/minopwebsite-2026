using System.Web.Optimization;

namespace PayTimeWebClient
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/mcss").Include(
                "~/assets/plugins/bootstrap-switch/css/bootstrap-switch.min.css",
                "~/assets/plugins/bootstrap-fileinput/bootstrap-fileinput.css"
            ));
            bundles.Add(new StyleBundle("~/bundles/404notfound").Include(
                "~/Content/404notfound.css"
            ));
            bundles.Add(new StyleBundle("~/bundles/layoutcss1").Include(
                "~/assets/plugins/datatables/datatables.min.css",
                "~/assets/plugins/datatables/plugins/bootstrap/datatables.bootstrap.css",
                "~/assets/plugins/bootstrap-datepicker/css/bootstrap-datepicker3.min.css",
                "~/assets/plugins/bootstrap-timepicker/css/bootstrap-timepicker.min.css",
                "~/assets/css/dataTables.checkboxes.css"
            ));
            bundles.Add(new StyleBundle("~/bundles/layoutcss2").Include(
                "~/assets/plugins/bootstrap-daterangepicker/daterangepicker.min.css",
                "~/assets/plugins/fullcalendar/fullcalendar.min.css",
                "~/assets/global/plugins/bootstrap-multiselect/css/bootstrap-multiselect.css",
                "~/assets/global/plugins/select2/css/select2.min.css",
                "~/assets/global/plugins/select2/css/select2-bootstrap.min.css"
            ));
            bundles.Add(new StyleBundle("~/bundles/customindex").Include(
                "~/LandingAssets/css/bootstrap.min.css",
                "~/LandingAssets/css/animate.min.css",
                "~/LandingAssets/css/responsive.css"
            ));
            bundles.Add(new StyleBundle("~/bundles/layoutcss3").Include(
                "~/assets/css/components.min.css",
                "~/assets/css/plugins.min.css",
                "~/assets/layouts/layout/css/layout.min.css",
                "~/assets/layouts/layout/css/custom.css",
                "~/assets/css/minop-saas-redesign.css",
                "~/assets/css/minop-admin-dashboard-redesign.css",
                "~/assets/css/minop-dashboard-unicorn.css",
                "~/Newlayout/css/minop-dashboard-v3.css"
            ));
            bundles.Add(new StyleBundle("~/bundles/landing_common_css").Include(
                "~/AssetsNew/css/bootstrap.css",
                "~/AssetsNew/icons/pe-icon-7-stroke/css/pe-icon-7-stroke.css",
                "~/AssetsNew/icons/font-awesome/css/font-awesome.min.css",
                "~/AssetsNew/icons/icomoon/style.css",
                "~/AssetsNew/css/layout.css",
                "~/AssetsNew/css/header.css",
                "~/AssetsNew/css/footer.css"
            ));
            bundles.Add(new ScriptBundle("~/bundles/landing_page_js_header").Include(
                "~/AssetsNew/js/jquery.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/landing_page_js_footer").Include(
                "~/AssetsNew/js/jquery.js",
                "~/AssetsNew/js/popper.min.js",
                "~/AssetsNew/js/jquery-ui.min.js",
                "~/AssetsNew/js/bootstrap.min.js",
                "~/AssetsNew/js/jquery.fancybox.js",
                "~/AssetsNew/js/wow.js",
                "~/AssetsNew/js/appear.js",
                "~/AssetsNew/js/lazysizes.min.js"
            ));

            //====================landing CSS==================================
            bundles.Add(new ScriptBundle("~/bundles/layoutjs4").Include(
                "~/Scripts/jquery-1.12.4.js",
                "~/Scripts/jquery-ui.js",
                "~/assets/scripts/app.min.js",
                "~/assets/pages/scripts/components-bootstrap-select-splitter.min.js",
                "~/Scripts/jasny-bootstrap.min.js",
                "~/Scripts/respond.js",
                "~/Scripts/html5shiv.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/Util").Include(
                "~/Scripts/AppScript/highcharts.js",
                "~/Scripts/AppScript/highcharts-3d.js",
                "~/Scripts/AppScript/highcharts-more.js",
                "~/Scripts/AppScript/solid-gauge.js",
                "~/Scripts/AppScript/drilldown.js",
                "~/Scripts/AppScript/exporting.js",
                "~/Scripts/AppScript/jquery.knob.js",
                "~/Scripts/AppScript/Utility.js"
            ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"
            ));
            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                "~/assets/plugins/js.cookie.min.js",
                "~/assets/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/assets/plugins/jquery.blockui.min.js",
                "~/assets/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                "~/assets/plugins/bootstrap-fileinput/bootstrap-fileinput.js",
                "~/Newlayout/js/sweet_alert_payroll.js",
                "~/Newlayout/js/minop-dashboard-v3.js"

            ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/assets/plugins/moment.min.js",
                "~/assets/plugins/bootstrap-daterangepicker/daterangepicker.min.js",
                "~/assets/plugins/morris/morris.min.js",
                "~/assets/plugins/morris/raphael-min.js",
                "~/assets/plugins/counterup/jquery.waypoints.min.js",
                "~/assets/plugins/counterup/jquery.counterup.min.js",
                "~/assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.min.js",
                "~/assets/plugins/bootstrap-timepicker/js/bootstrap-timepicker.min.js",
                "~/assets/plugins/fullcalendar/fullcalendar.js",
                "~/assets/plugins/horizontal-timeline/horizontal-timeline.js",
                "~/assets/plugins/flot/jquery.flot.min.js",
                "~/assets/plugins/flot/jquery.flot.resize.min.js",
                "~/assets/plugins/flot/jquery.flot.categories.min.js",
                "~/assets/plugins/jquery-easypiechart/jquery.easypiechart.min.js",
                "~/assets/plugins/jquery.sparkline.min.js",
                "~/assets/global/plugins/bootstrap-maxlength/bootstrap-maxlength.min.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/multiselect").Include(
                "~/assets/pages/scripts/components-bootstrap-multiselect.min.js",
                "~/assets/global/plugins/bootstrap-multiselect/js/bootstrap-multiselect.js",
                "~/assets/plugins/bootstrap-selectsplitter/bootstrap-selectsplitter.min.js",
                "~/assets/pages/scripts/components-select2.min.js",
                "~/assets/global/plugins/select2/js/select2.full.min.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                "~/assets/pages/scripts/components-form-tools.min.js",
                "~/assets/global/scripts/datatable.js",
                "~/Scripts/jquery.dataTables.min.js",
                "~/Scripts/dataTables.jqueryui.min.js",
                "~/assets/plugins/datatables/plugins/bootstrap/datatables.bootstrap.js",
                "~/assets/pages/scripts/table-datatables-managed.js",
                "~/assets/plugins/datatables/dataTables.buttons.min.js",
                "~/assets/plugins/datatables/jszip.min.js",
                "~/assets/plugins/datatables/pdfmake.min.js",
                "~/assets/plugins/datatables/buttons.html5.min.js",
                "~/assets/plugins/datatables/vfs_fonts.js",
                "~/assets/js/dataTables.checkboxes.min.js",
                "~/Newlayout/js/dataTableSorting.js",
                "~/Newlayout/js/jquery.dataTables.Mantra.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/superadminjs").Include(
                "~/assets/plugins/datatables/plugins/bootstrap/datatables.bootstrap.js",
                "~/assets/scripts/app.min.js",
                "~/assets/pages/scripts/components-bootstrap-select-splitter.min.js",
                "~/assets/pages/scripts/table-datatables-managed.js",
                "~/assets/pages/scripts/dashboard.min.js",
                "~/assets/plugins/bootstrap-selectsplitter/bootstrap-selectsplitter.min.js",
                "~/assets/global/plugins/bootstrap-sweetalert/sweetalert.min.js",
                "~/assets/pages/scripts/ui-sweetalert.js"
            ));
        }
    }
}
