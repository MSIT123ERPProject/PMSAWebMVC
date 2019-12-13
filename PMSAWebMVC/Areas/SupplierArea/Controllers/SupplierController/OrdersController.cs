using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.TingHuan;
using PMSAWebMVC.ViewModels.ShipNotices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
                           PurchaseOrderID = po.PurchaseOrderID
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
            [Display(Name="採購員")]
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
        public JsonResult GetPurchaseOrderS(string supplierCode)
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
        public ActionResult OrderApply(string orderID)
        {
            ///////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            string supplierAccount = supplier.SupplierAccountID;
            string supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            ShipNoticesUtilities utilities = new ShipNoticesUtilities();

            //供應商答交程式碼

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
            PurchaseOrder order = (from po in db.PurchaseOrder.AsEnumerable()
                                   where po.PurchaseOrderID == orderID
                                   select po).SingleOrDefault();
            if (utilities.AddAPOChanged(order, supplierAccount, supplierCode) == false)
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
            //採購單狀態W為雙方答交，供應商未出貨訂單判定應為判斷是否為W
            order.PurchaseOrderStatus = "W";
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        //拒絕按鈕
        public ActionResult OrderRefuse(string orderID) {
            ///////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            string supplierAccount = supplier.SupplierAccountID;
            string supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            ShipNoticesUtilities utilities = new ShipNoticesUtilities();
            PurchaseOrder purchaseOrder= db.PurchaseOrder.Find(orderID);
            //狀態異動中=C
            purchaseOrder.PurchaseOrderStatus = "C";
            utilities.AddAPOChanged(purchaseOrder,supplierAccount,supplierCode);
            db.Entry(purchaseOrder).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json("success",JsonRequestBehavior.AllowGet);
        }
    }
}