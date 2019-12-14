using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Filter
{
    public class MyExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            //將錯誤記到記事本
            string s = "訊息:" + filterContext.Exception.Message + " 種類: " + filterContext.Exception.GetType().ToString() + " 資源: " + filterContext.Exception.Source;
            StreamWriter sw = File.AppendText((filterContext.RequestContext.HttpContext.Request.PhysicalApplicationPath) + "\\ErrorLog.txt");
            sw.WriteLine(s);
            sw.Close();
            //將 ExceptionHandled = true 跳過錯誤
            filterContext.ExceptionHandled = true;
            // 重新導到登入後首頁
            filterContext.Result = new RedirectResult("~/Home/Index");
        }
    }
}