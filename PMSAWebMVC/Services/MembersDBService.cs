using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace PMSAWebMVC.Services
{
    public class MembersDBService
    {
        //給值
        //string callbackUrl = "http://...";
        //string imgUrl = "http://..."
        //string pwd = "xxxxx...";
        //重點!!取得 .html 檔內容
        //string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdEmailTemplate.html"));
        //使用方法把 {xxx} 抽換掉
        //string MailBody = MembersDBService.getMailBody(tempMail, imgUrl, callbackUrl, pwd);
        //寄信
        //await UserManager.SendEmailAsync(user.Id, "重設您的密碼", MailBody);
        /// <summary>
        /// 為了方便閱讀將 email 內容字串寫在 .html 裡。
        /// 為靜態方法 MembersDBService.getMailBody(...傳入字串可以自己寫多載喔)。
        /// 是個為了把變數(ex.網址、圖片網址、程式產生的字串)傳進去 .html 字串的方法。
        /// </summary>
        /// <param name="tempStr">
        /// 重點!!取得 .html 檔內容:
        /// System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdEmailTemplate.html"));
        /// </param>
        /// <param name="imgUrl">
        /// gmail 目前不支援 uri image 圖片請用網址 ex. "http://..."
        /// </param>
        /// <param name="callbackUrl">"http://..."</param>
        /// <param name="pwd">程式產的字串</param>
        /// <returns></returns>
        public static string getMailBody(string tempStr, string imgUrl, string callbackUrl, string pwd)
        {
            tempStr = tempStr.Replace("{imgUrl}", imgUrl);
            tempStr = tempStr.Replace("{callbackUrl}", callbackUrl);
            tempStr = tempStr.Replace("{pwd}", pwd);
            return tempStr;
        }

        public static string getMailBody(string tempStr, string imgUrl, string callbackUrl, string pwd, string SupAccId)
        {
            tempStr = tempStr.Replace("{SupAccId}", SupAccId);
            tempStr = tempStr.Replace("{imgUrl}", imgUrl);
            tempStr = tempStr.Replace("{callbackUrl}", callbackUrl);
            tempStr = tempStr.Replace("{pwd}", pwd);
            return tempStr;
        }
    }
}