using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Nigon.WebUI.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js")); 
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/nigon.css"));

            bundles.Add(new StyleBundle("~/Content/cssUi").Include("~/Content/themes/base/jquery-ui.css"));
            bundles.Add(new StyleBundle("~/Content/cssUiAddon").Include("~/Content/themes/base/jquery-ui-timepicker-addon.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryUi").Include("~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryUiAddon").Include("~/Scripts/jquery-ui-timepicker-addon.js"));
            bundles.Add(new ScriptBundle("~/bundles/jsValidation").Include("~/Scripts/jquery.validate.min.js", "~/Scripts/jquery.validate.unobtrusive.min.js", "~/Scripts/additional-methods.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/countDown").Include("~/Scripts/PartialScripts/countDown.js"));
            bundles.Add(new ScriptBundle("~/bundles/dateTimePicker").Include("~/Scripts/PartialScripts/dateTimePicker.js"));
            bundles.Add(new ScriptBundle("~/bundles/scriptsForGalleryFancyBox").Include("~/Scripts/gallery/jquery.fancybox.js"));
            bundles.Add(new ScriptBundle("~/bundles/galleryScript").Include("~/Scripts/PartialScripts/galleryScript.js"));
        }
    }
}