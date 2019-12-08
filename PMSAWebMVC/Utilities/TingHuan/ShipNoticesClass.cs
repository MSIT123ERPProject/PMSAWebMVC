using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Controllers;
using PMSAWebMVC.Models;
using PMSAWebMVC.Services;
using PMSAWebMVC.ViewModels.ShipNotices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Utilities.TingHuan
{
    public class ShipNoticesUtilities : BaseController/*, IIdentityMessageService*/
    {
        private PMSAEntities db = new PMSAEntities();
        /// <summary>
        /// 寄信功能想寫在這失敗了
        /// </summary>
        /// <param name="requesterRole"></param>
        /// <param name="purchaseOrderID"></param>
        /// <returns></returns>
        public ShipNoticesUtilities()
        {
        }
        public ShipNoticesUtilities(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
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
        public async Task SendMailToBuyer(List<OrderDtlForMail> shipNotice)
        {
            string shipDtlMail = $"<table><tr><td>{shipNotice.FirstOrDefault().ShipNoticeID}</td></tr>";
            foreach (var snd in shipNotice)
            {
                DateTime shipdate = (DateTime)snd.ShipDate;
                shipDtlMail += "<tr>";
                shipDtlMail += $"<td>{snd.ShipNoticeID}</td><td>{snd.PartNumber}</td><td>{snd.PartName}</td><td>{snd.ShipQty}</td><td>{shipdate.ToString("yyyy/MM/dd")}</td>";
                shipDtlMail += "</tr>";
            }
            shipDtlMail += "</table>";
            //先找到你要寄信的人(這邊用供應商帳號找)，並儲存 user.Id
            //這裡的值在資料庫的 dbo.AppUsers table
            var user = UserManager.Users.Where(x => x.UserName == "SE00001").SingleOrDefault();
            //user.Id 等等寄信方法第一個參數會用到
            var userId = user.Id;
            //string shipDtlMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\ShipNotices\..."));
            //信裡要用的變數
            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //string SupAccID = user.UserName;

            //寄信
            await UserManager.SendEmailAsync(userId, "商品出貨通知", shipDtlMail);
        }
        ////////////////////////////////////////////////////////
        //找出該purchaseOrderID最新一筆異動資料
        public int FindPOChangedOID(string requesterRole, string purchaseOrderID)
        {
            var poc = db.POChanged.Where(x => (x.RequesterRole == requesterRole) && (x.PurchaseOrderID == purchaseOrderID));
            DateTime dt = poc.FirstOrDefault().RequestDate;
            int pOChangedOID = poc.FirstOrDefault().POChangedOID;
            foreach (var pocD in poc)
            {
                if (pocD.RequestDate > dt)
                {
                    dt = pocD.RequestDate;
                    pOChangedOID = pocD.POChangedOID;
                }
            }
            return pOChangedOID;
        }
        //找出該purchaseOrderDtlCode最新一筆異動資料
        public int FindPOChangedOIDByDtlCode(string requesterRole, string purchaseOrderDtlCode)
        {
            var poc = db.POChanged.Where(x => (x.RequesterRole == requesterRole) && (x.PurchaseOrderDtlCode == purchaseOrderDtlCode));
            DateTime dt = poc.FirstOrDefault().RequestDate;
            int pOChangedOID = poc.FirstOrDefault().POChangedOID;
            foreach (var pocD in poc)
            {
                if (pocD.RequestDate > dt)
                {
                    dt = pocD.RequestDate;
                    pOChangedOID = pocD.POChangedOID;
                }
            }
            return pOChangedOID;
        }

        //寄送出貨通知明細給公司採購
        public async Task sendShipDtlMailToBuyer(List<int> shipDtlList)
        {
            string tempMail = "";
            string img = "";
            string callbackUrl = "";
            string pwd = "";
            string MailBody = MembersDBService.getMailBody(tempMail, img, callbackUrl, pwd);

            //寄信
            //await UserManager.SendEmailAsync(user.Id, "重設您的密碼", MailBody);

            //var user = await UserManager.Users.Where(x => x.UserName.Contains("C") && x.UserName == EmpId).SingleOrDefaultAsync();
            //// 傳送包含此連結的電子郵件
            //var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
            //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdEmailTemplate.html"));
            //// 經測試 gmail 不支援 uri data image 所以用網址傳圖比較保險
            //string img = "https://ci5.googleusercontent.com/proxy/4OJ0k4udeu09Coqzi7ZQRlKXsHTtpTKlg0ungn0aWQAQs2j1tTS6Q6e8E0dZVW2qsbzD1tod84Zbsx62gMgHLFGWigDzFOPv1qBrzhyFIlRYJWSMWH8=s0-d-e1-ft#https://app.flashimail.com/rest/images/5d8108c8e4b0f9c17e91fab7.jpg";
            //string pwd = generateFirstPwd();
            //string MailBody = MembersDBService.getMailBody(tempMail, img, callbackUrl, pwd);
            ////emp table user table
            ////重設資料庫該 user 密碼 並 hash 存入 db
            ////重設db密碼
            ////1.重設 user 密碼
            //await UserManager.UpdateSecurityStampAsync(user.Id);
            //user.PasswordHash = UserManager.PasswordHasher.HashPassword(pwd);
            //user.LastPasswordChangedDate = null;

            //var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();
            //emp.PasswordHash = user.PasswordHash;

            ////寄信
            //await UserManager.SendEmailAsync(user.Id, "重設您的密碼", MailBody);

            ////3.更新db寄信相關欄位
            ////SendLetterDate
            //emp.SendLetterDate = DateTime.Now;
            ////SendLetterStatus
            //emp.SendLetterStatus = "S";

            ////更新狀態欄位 user emo table
            //await AccStatusReset(EmpId);
        }
        //把訂單狀態換成文字敘述的方法
        public string GetStatus(string purchaseOrderStatus)
        {
            //N = 新增,P = 送出,C = 異動中,E = 答交,D = 整筆訂單取消,S = 出貨,R = 點交,O = 逾期,Z = 結案
            switch (purchaseOrderStatus)
            {
                case "N":
                    return "新增";
                case "P":
                    return "送出";
                case "C":
                    return "異動中";
                case "E":
                    return "答交";
                case "D":
                    return "取消";
                case "S":
                    return "出貨";
                case "R":
                    return "點交";
                case "O":
                    return "逾期";
                case "Z":
                    return "結案";
                default:
                    return "";
            }

        }


    }
}