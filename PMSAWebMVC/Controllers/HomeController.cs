
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

    }

}