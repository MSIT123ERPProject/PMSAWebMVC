using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PMSAWebMVC.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoServerCaching();
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }

        protected string cookieName = "PMSAWebMVC";
        protected string cookieKey = "CultureInfo";
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = "";
            HttpCookie cultureCookie = Request.Cookies[cookieName];
            if (cultureCookie != null)
            {
                cultureName = cultureCookie.Values[cookieKey];
            }
            else
            {
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null;
                HttpCookie c = new HttpCookie("PMSAWebMVC");
                c.Values.Add("CultureInfo", cultureName);
                c.Expires.AddDays(30);
                Response.Cookies.Add(c);
            }
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            return base.BeginExecuteCore(callback, state);
        }
    }
}
