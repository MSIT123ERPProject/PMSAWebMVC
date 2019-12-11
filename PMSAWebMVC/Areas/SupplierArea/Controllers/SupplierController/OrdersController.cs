using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.TingHuan;
using PMSAWebMVC.ViewModels.ShipNotices;
using System;
using System.Collections.Generic;
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
            var qpo = (from po in db.PurchaseOrder
                       where po.SupplierCode == supplierCode
                       select new
                       {
                           SupplierCode = po.SupplierCode,
                           PurchaseOrderID = po.PurchaseOrderID,
                           PurchaseOrderOID = po.PurchaseOrderOID
                       }).ToList().Select( x=>new PurchaseOrder {
                           SupplierCode = x.SupplierCode,
                           PurchaseOrderID = x.PurchaseOrderID,
                           PurchaseOrderOID = x.PurchaseOrderOID
                       });
            //var s = q.ToList();
            var d = qpo.ToList();
            return View(qpo);
        }
        public JsonResult GetPurchaseOrderS( string supplierCode )
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
        public ActionResult OrderApply()
        {
            //供應商答交程式碼
            return View();
        }
    }
}