using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Models;
using PMSAWebMVC.Services;
using PMSAWebMVC.Utilities.TingHuan;
using PMSAWebMVC.ViewModels.ShipNotices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Areas.SupplierArea.Controllers
{
    public class OrdersController : Controller
    {
        private PMSAEntities db;
        string supplierAccount;
        string supplierCode;
        string POChangedCategoryCodeShipped;
        string RequesterRoleSupplier;
        ShipNoticesUtilities utilities;
        public OrdersController()
        {
            db = new PMSAEntities();
            //supplierCode = "S00001";
            //supplierAccount = "SE00001";
            POChangedCategoryCodeShipped = "S";
            RequesterRoleSupplier = "S";
        }
        /// <summary>
        /// 登入者資料
        /// </summary>
        /// <returns></returns>
        ///  //建構子多載
        public OrdersController(ApplicationUserManager userManager)
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
        /// ///////////////////////////
        // GET: Orders
        public ActionResult Index()
        {
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            //var q = (db.PurchaseOrder.Where(x=>x.SupplierCode==supplierCode).Select(x=>new PurchaseOrder {
            //  SupplierCode=  x.SupplierCode,
            //  PurchaseOrderID = x.PurchaseOrderID,
            //  PurchaseOrderOID = x.PurchaseOrderOID
            //})).AsEnumerable();
            var qpoP = from po in db.PurchaseOrder
                       where po.PurchaseOrderStatus == "P" && po.SupplierCode == supplierCode
                       select new
                       {
                           po.PurchaseOrderID
                       };
            var s = qpoP.ToList();
            List<SelectListItem> orderList = new List<SelectListItem>();
            foreach (var orderID in qpoP)
            {
                SelectListItem order = new SelectListItem()
                {
                    Text = orderID.PurchaseOrderID,
                    Value = orderID.PurchaseOrderID,
                    Selected = false,
                };
                orderList.Add(order);
            }
            OrderSendedToSupplierViewModel orderModel;
            if (orderList.Count() != 0)
            {
                orderModel = new OrderSendedToSupplierViewModel()
                {
                    SupplierCode = supplierCode,
                    orderID = orderList[0].Value,
                    orderList = orderList
                };
                return View(orderModel);
            }

            orderModel = new OrderSendedToSupplierViewModel()
            {
                SupplierCode = supplierCode,
                orderID = "orderID",
                orderList = orderList
            };
            return View(orderModel);
        }
        //Orders/IndexView
        public class OrderSendedToSupplierViewModel
        {
            public string SupplierCode { get; set; }
            public string orderID { get; set; }
            public IEnumerable<SelectListItem> orderList { get; set; }
        }
        //Get the Information of order which was selected 
        public ActionResult GetOrderInfo(string orderID)
        {
            var qpo = (from emp in db.Employee.AsEnumerable()
                       join po in db.PurchaseOrder on emp.EmployeeID equals po.EmployeeID
                       where po.PurchaseOrderID == orderID
                       select new Employee
                       {
                           Name = emp.Name,
                           Mobile = emp.Mobile,
                           Tel = emp.Tel,
                           Email = emp.Email,
                       }
                       ).SingleOrDefault();
            var qSumpod = db.PurchaseOrderDtl.Where(x => x.PurchaseOrderID == orderID).Sum(x => x.Total);
            if (qpo == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            OrderInfoViewModel qTotal = new OrderInfoViewModel
            {
                EmployeeName = qpo.Name,
                EmployeeMobile = qpo.Mobile,
                EmployeeTel = qpo.Tel,
                EmployeeEmail = qpo.Email,
                Total = (decimal)qSumpod
            };

            return PartialView("_IndexOrderInfoPartialView", qTotal);
        }
        //the OrderInfo's ViewModel
        public class OrderInfoViewModel
        {
            [Display(Name = "採購員")]
            public string EmployeeName { get; set; }
            [Display(Name = "手機")]
            public string EmployeeMobile { get; set; }
            [Display(Name = "市話")]
            public string EmployeeTel { get; set; }
            [Display(Name = "電子郵件")]
            public string EmployeeEmail { get; set; }
            [Display(Name = "總金額")]
            public decimal Total { get; set; }
        }
        //GetOrderDtl
        public ActionResult GetOrderDtl(string orderID)
        {
            var qorder = from pod in db.PurchaseOrderDtl
                         where pod.PurchaseOrderID == orderID
                         select new
                         {
                             pod.PurchaseOrderDtlCode,
                             pod.PartName,
                             pod.PartNumber,
                             pod.PurchasedQty,
                             pod.Qty,
                             pod.QtyPerUnit,
                             pod.CommittedArrivalDate,
                         };
            return Json(new { data = qorder }, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetPurchaseOrderS(string supplierCode)
        {
            var qpo = (from po in db.PurchaseOrder
                       where po.PurchaseOrderStatus == "P" && po.SupplierCode == supplierCode
                       select new shipOrderViewModel
                       {
                           PurchaseOrderID = po.PurchaseOrderID,
                           ReceiverName = po.ReceiverName,
                           ReceiverMobile = po.ReceiverMobile,
                           ReceiverTel = po.ReceiverTel,
                           ReceiptAddress = po.ReceiptAddress,
                           PurchaseOrderTotalAmount = 0
                       }).ToList();
            for (int i = 0; i < qpo.Count(); i++)
            {
                string purchaseOrderID = qpo[i].PurchaseOrderID;
                var qorderTotal = db.PurchaseOrderDtl.Where(x => x.PurchaseOrderID == purchaseOrderID).Select(x => x.Total);
                int? orderTotal = 0;
                foreach (int? total in qorderTotal)
                {
                    orderTotal += total;
                }
                qpo[i].PurchaseOrderTotalAmount = (int)orderTotal;
            }
            var json = new { data = qpo };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        //此方法為答交按鈕的方法，此功能為辰哥負責
        public async Task<JsonResult> OrderApply(string orderID)
        {
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            string supplierAccount = supplier.SupplierAccountID;
            string supplierCode = supplier.SupplierCode;

            //供應商答交程式碼
            if (string.IsNullOrWhiteSpace(orderID))
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
            PurchaseOrder orderUpdate = db.PurchaseOrder.Find(orderID);
            if (orderUpdate == null)
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
            orderUpdate.PurchaseOrderStatus = "E";
            ShipNoticesUtilities utilities = new ShipNoticesUtilities();
            if (!utilities.AddAPOChanged(orderUpdate, supplierAccount, supplierCode))
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
            db.Entry(orderUpdate).State = EntityState.Modified;
            db.SaveChanges();
            await SendMailToBuyer(orderUpdate, "已答交", null);
            return Json("success", JsonRequestBehavior.AllowGet);

            var q = from poc in db.POChanged
                        //join po in db.PurchaseOrder on poc.PurchaseOrderID equals po.PurchaseOrderID
                        //into s
                        //from po in s.DefaultIfEmpty()
                    where poc.RequesterRole == "P" && poc.PurchaseOrderID == orderID
                    select new
                    {
                        poc.PurchaseOrderID,
                        poc.RequesterRole,
                    };
            var t = q.ToList();
            if (q.Count() == 0 || q.Count() == null)
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
//<<<<<<< HEAD
            PurchaseOrder order = (from po in db.PurchaseOrder.AsEnumerable()
                                   where po.PurchaseOrderID == orderID
                                   select po).SingleOrDefault();
            if (utilities.AddAPOChanged(order, supplierAccount, supplierCode) == false)
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
//=======
            else
            {
                PurchaseOrder order = (from po in db.PurchaseOrder.AsEnumerable()
                                       where po.PurchaseOrderID == orderID
                                       select po).SingleOrDefault();
                if (utilities.AddAPOChanged(order, supplierAccount, supplierCode) == false)
                {
                    return Json("fail", JsonRequestBehavior.AllowGet);
                }
                //採購單狀態W為雙方答交，供應商未出貨訂單判定應為判斷是否為W
                order.PurchaseOrderStatus = "E";
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
                await SendMailToBuyer(order, "已答交", null);
                return Json("success", JsonRequestBehavior.AllowGet);
//>>>>>>> 802426d1abd7d38d1f6e79d3378d39e1746e0b09
            }
            //採購單狀態W為雙方答交，供應商未出貨訂單判定應為判斷是否為W
            order.PurchaseOrderStatus = "W";
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();
            await SendMailToBuyer(order, "已答交",null);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        //拒絕按鈕
        public async Task<JsonResult> OrderRefuse(string orderID)
        {
            ///////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            string supplierAccount = supplier.SupplierAccountID;
            string supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            ShipNoticesUtilities utilities = new ShipNoticesUtilities();
            PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(orderID);
            if (purchaseOrder == null)
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
            //狀態異動中=C
            purchaseOrder.PurchaseOrderStatus = "C";
            utilities.AddAPOChanged(purchaseOrder, supplierAccount, supplierCode);
            db.Entry(purchaseOrder).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            await SendMailToBuyer(purchaseOrder, "已拒絕",null);
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        //寄信
        public async Task SendMailToBuyer(PurchaseOrder order, string Reply,string OrderDtl)
        {
            ///////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            string supplierAccount = supplier.SupplierAccountID;
            string supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            //string borderColor = "border-color:black";
            //string borderLine = "1";
            //string BuyerName = db.Employee.Find(order.EmployeeID).Name;
            //string shipDtlMail = $"<table style='{borderColor}' border='{borderLine}'>";
            //shipDtlMail += $"<thead><tr><th>{BuyerName}，你好</th></tr></thead>";
            //shipDtlMail += $"<tr><td>訂單編號:{order.PurchaseOrderID}{Reply}</td></tr>";
            //shipDtlMail += "</table>";
            string OrderID = order.PurchaseOrderID;
            string OrderApply = Reply;
            if (OrderDtl == null) {
                OrderDtl = "";
            }
            string SupplierName = db.SupplierInfo.Where(x=>x.SupplierCode==supplierCode).SingleOrDefault().SupplierName;
            string BuyerID = db.Employee.Where(x => x.EmployeeID == order.EmployeeID).SingleOrDefault().EmployeeID;
            string EmployeeName = db.Employee.Find(BuyerID).Name;
            var user = UserManager.Users.Where(x => x.UserName == BuyerID).SingleOrDefault();
            var userId = user.Id;
            // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
            // 傳送包含此連結的電子郵件
            //var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
            //更改密碼要在code之前不然他是拿UpdateSecurityStampAsync 來生code的
            //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Areas\SupplierArea\Views\Shared\SendMailToBuyer.html"));
            // 經測試 gmail 不支援 uri data image 所以用網址傳圖比較保險
            //string img = "https://ci5.googleusercontent.com/proxy/4OJ0k4udeu09Coqzi7ZQRlKXsHTtpTKlg0ungn0aWQAQs2j1tTS6Q6e8E0dZVW2qsbzD1tod84Zbsx62gMgHLFGWigDzFOPv1qBrzhyFIlRYJWSMWH8=s0-d-e1-ft#https://app.flashimail.com/rest/images/5d8108c8e4b0f9c17e91fab7.jpg";
            string MailBody = MembersDBService.getMailBody(tempMail, OrderID,OrderApply, EmployeeName, OrderDtl, SupplierName);
            //寄信
            await UserManager.SendEmailAsync(userId, "供應商訂單答交通知", MailBody);
        }
    }
}