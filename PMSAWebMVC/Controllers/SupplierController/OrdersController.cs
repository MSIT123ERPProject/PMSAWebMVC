using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.TingHuan;
using PMSAWebMVC.ViewModels.ShipNotices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers.SupplierController
{
    public class OrdersController : Controller
    {
        private PMSAEntities db;
        string supplierAccount;
        string supplierCode;
        string POChangedCategoryCodeShipped;
        string RequesterRoleSupplier;
        ShipNoticesUtilities utilities = new ShipNoticesUtilities();
        public OrdersController()
        {
            //   SupplierAccount supplier = User.Identity.GetSupplierAccount();
            db = new PMSAEntities();
            supplierCode = "S00001";
            // supplierAccount = supplier.SupplierAccountID;
            supplierAccount = "SE00001";
            POChangedCategoryCodeShipped = "S";
            RequesterRoleSupplier = "S";
        }
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetPurchaseOrderS()
        {
            var qpo = from po in db.PurchaseOrder
                      where po.PurchaseOrderStatus == "P" && po.SupplierCode==supplierCode
                      select new shipOrderViewModel
                      {
                          PurchaseOrderID= po.PurchaseOrderID,
                          ReceiverName= po.ReceiverName,
                          ReceiverMobile=po.ReceiverMobile,
                          ReceiverTel=po.ReceiverTel,
                          ReceiptAddress=po.ReceiptAddress,
                          PurchaseOrderTotalAmount =0
                      };
            //計算每筆訂單總金額
            foreach ( var order in qpo ) {
               var qorderTotal= db.PurchaseOrderDtl.Where(x => x.PurchaseOrderID == order.PurchaseOrderID).Select(x=>x.Total);
                int?  orderTotal= 0;
                foreach ( var total in qorderTotal ) {
                    orderTotal += total;
                }
                order.PurchaseOrderTotalAmount = (int)orderTotal;
            }
            var json = new {data= qpo.ToList() } ;
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}