
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PMSAWebMVC.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {

            //檢查 cookies "PMSAWebMVC" 是否存在
            //不存在的話，將當前使用者語言存進cookies
            //HttpCookie c = Request.Cookies["PMSAWebMVC"];
            //if (c == null) {
            //    string culturename = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null;
            //    c.Values.Add("CultureInfo",culturename);
            //    Response.Cookies.Add(c);
            //}
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Deny()
        {
            return View("~/Views/Shared/Deny.cshtml");
        }

        ////多語系
        ////此時就可以設定使用者的偏好語系於Cookie中，提供後續網站語系識別的依據。
        ////以下在HomeController中建立一個 SetCulture Action 來處理這段事務。
        //public ActionResult SetCulture(string culture, string returnUrl)
        //{
        //    // Validate input 
        //    culture = CultureHelper.GetImplementedCulture(culture);

        //    // Save culture in a cookie 
        //    HttpCookie cookie = Request.Cookies["_culture"];

        //    if (cookie != null)
        //    {
        //        // update cookie value 
        //        cookie.Value = culture;
        //    }
        //    else
        //    {
        //        // create cookie value 
        //        cookie = new HttpCookie("_culture");
        //        cookie.Value = culture;
        //        cookie.Expires = DateTime.Now.AddYears(1);
        //    }

        //    Response.Cookies.Add(cookie);
        //    return RedirectToAction("Index","Home");
        //}

        ////取得語系資料
        ////剛剛我們將語系資訊放置在Cookie中，因此在伺服端接收到Request時就可以查看該Cookie資訊，並且依據該資訊來設定調整目前的CurrentCulture及CurrentUICulture。
        //public class MvcApplication : System.Web.HttpApplication
        //{
        //    protected void Application_Start()
        //    {
        //        AreaRegistration.RegisterAllAreas();
        //        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        //        RouteConfig.RegisterRoutes(RouteTable.Routes);
        //        BundleConfig.RegisterBundles(BundleTable.Bundles);
        //    }

        //    protected void Application_BeginRequest(Object sender, EventArgs e)
        //    {
        //        HttpCookie cultureCookie = Request.Cookies["_culture"];
        //        if (cultureCookie != null)
        //        {
        //            // get culture name
        //            var cultureInfoName = CultureHelper.GetImplementedCulture(cultureCookie.Value);

        //            // set culture
        //            System.Threading.Thread.CurrentThread.CurrentCulture =
        //            new System.Globalization.CultureInfo(cultureInfoName);
        //            System.Threading.Thread.CurrentThread.CurrentUICulture =
        //            new System.Globalization.CultureInfo(cultureInfoName);

        //        }
        //    }
        //}
    }

}