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
        /// <summary>
        ///寄信方法 
        /// </summary>
        /// <param name="shipNotice"></param>
        /// <returns></returns>
        public async Task SendMailToBuyer(List<OrderDtlForMail> shipNotice)
        {
            //出貨明細通知的TABLE要改一下
            string borderColor = "border-color:black";
            string borderLine ="1";
            string shipDtlMail = "<table" + $" style={borderColor}" +$" border={borderLine}" +">";
             shipDtlMail += $"<tr><td>{shipNotice.FirstOrDefault().ShipNoticeID}</td></tr>";
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
        public async Task SendMailToBuyer(PurchaseOrder order)
        {
            string borderColor = "border-color:black";
            string borderLine = "1";
            string shipDtlMail = "<table" + $" style={borderColor}" + $" border={borderLine}" + ">";
            shipDtlMail += $"<thead><tr><th>{order.Employee}，你好</th></tr></thead>";
            shipDtlMail += $"<tr><td>訂單編號:{order.PurchaseOrderID}已出貨</td></tr>";
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
            await UserManager.SendEmailAsync(userId, "供應商訂單答交通知", shipDtlMail);
        }

        public bool AddAPOChanged(PurchaseOrder purchaseOrder, string supplierAccount, string supplierCode)
        {
            POChanged pO = new POChanged();
            pO.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
            pO.POChangedCategoryCode = purchaseOrder.PurchaseOrderStatus;
            pO.RequestDate = DateTime.Now;
            pO.RequesterRole = "S";
            pO.RequesterID = supplierAccount;
            db.POChanged.Add(pO);
            try
            {
                db.SaveChanges();
                return true;
            }
            catch {
                return false;
            }
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