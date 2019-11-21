using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace PMSAWebMVC
{
    public static class StringExtensions
    {
        //依語系產生i18n
        public static string Toi18n(this string value)
        {
            return Resources.AppResource.ResourceManager.GetString(value);
        }

        public static string CultureName(this string value)
        {
            //存取瀏覽器設定
            return Thread.CurrentThread.CurrentUICulture.Name;
        }

        public static string Toi18n(this string nameSpace, string value)
        {
            string resKey = (string.IsNullOrEmpty(nameSpace) ? string.Empty : nameSpace + ".") + value;
            return Resources.AppResource.ResourceManager.GetString(resKey);
        }
    }
}