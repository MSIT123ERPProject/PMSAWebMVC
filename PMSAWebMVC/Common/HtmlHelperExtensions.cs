using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace PMSAWebMVC
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// 產生一個字串
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks>
        /// Example:@Html.OutputText("Language".Toi18n())
        /// </remarks>
        public static MvcHtmlString OutputText(this System.Web.Mvc.HtmlHelper helper, string text)
        {
            return new MvcHtmlString(text);
        }
    }
}