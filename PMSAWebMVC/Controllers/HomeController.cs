
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
        //你原本的建構子不要刪掉
        public HomeController()
        {
        }
        //建構子多載
        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        // 屬性
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult Index()
        {
            ///////////////////////////////
            //多語系功能程式碼
            //檢查 cookies "PMSAWebMVC" 是否存在
            //不存在的話，將當前使用者語言存進cookies
            //HttpCookie c = Request.Cookies["PMSAWebMVC"];
            //if (c == null) {
            //    string culturename = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null;
            //    c.Values.Add("CultureInfo",culturename);
            //    Response.Cookies.Add(c);
            //}
            //多語系功能程式碼
            ////////////////////////////////
            //判斷登入者是誰顯示專屬廠商
            string LoginAccId = User.Identity.GetUserName();
            string LognId = User.Identity.GetUserId();
            //如果 LognId 這人是 Manager
            if (UserManager.IsInRole(LognId, "Supplier"))
            {
                return View("~/Areas/SupplierArea/Views/SupplierHome/Index.cshtml");
            }
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