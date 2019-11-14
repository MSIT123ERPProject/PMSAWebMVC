using PMSAWebMVC.Models;
using PMSAWebMVC.ViewModels.ShipNotices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers
{
    public class ShipNoticesController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();

        //GET: ShipNotices
        //public ActionResult DisplayPurchaseOrderStatus(PurchaseOrder purchaseOrder)
        //{
        //    var q = from n in db.POChangedCategory select n;
        //    SelectList selectLists = new SelectList();
        //    var s = q.Where(x=>(x.POChangedCategoryCode == "P" || x.POChangedCategoryCode=="E"|| x.POChangedCategoryCode=="S"));
        //    return selectLists( q,s);
        //}

        //目前沒功能
        public ActionResult IndexUnshipped(PurchaseOrder purchaseOrder)
        {

            if (purchaseOrder != null && purchaseOrder.PurchaseOrderID == "P")
            {
                IQueryable<PurchaseOrder> po = db.PurchaseOrder.Where(n => n.PurchaseOrderID == "P");

                return View(po);
            }
            return View();
        }

        public ActionResult Index(PurchaseOrder purchaseOrder)
        {
            string statusSended = "P";
            string statusApplied = "E";
            string statusShipped = "S";
            string SupplierCode = db.SupplierAccount.Find("SE00001").SupplierCode;
            purchaseOrder = db.PurchaseOrder.Find(purchaseOrder.PurchaseOrderID);
            if (purchaseOrder != null && (purchaseOrder.PurchaseOrderStatus == statusSended || purchaseOrder.PurchaseOrderStatus == statusApplied || purchaseOrder.PurchaseOrderStatus == statusShipped))
            {
                var p = from n in db.PurchaseOrder
                        where n.PurchaseOrderID == purchaseOrder.PurchaseOrderID && n.SupplierCode == SupplierCode
                        select n;
                var Order = db.PurchaseOrder.Where(n => n.PurchaseOrderID == purchaseOrder.PurchaseOrderID && n.SupplierCode == SupplierCode);
                return View(Order);
            }
            else
            {
                if (purchaseOrder != null)
                {
                    ViewBag.failMessage = $"<script>Swal.fire('{ PMSAWebMVC.Resources.AppResource.noData }');</script>";
                }
                //不知道為甚麼不能丟空的VIEW()
                //var q = from n in db.PurchaseOrder where n.PurchaseOrderOID == null select n;
                //return View(q);

                //===========================預設為顯示已答交的訂單
                var query = from n in db.PurchaseOrder
                            where (n.PurchaseOrderStatus == statusSended && n.SupplierCode == SupplierCode)
                            select n;
                var q = db.PurchaseOrder.Where(n => n.PurchaseOrderStatus == statusSended && n.SupplierCode == SupplierCode);
                //ViewBag.PurchaseOrderStatus = new SelectList(,);
                return View(query);
            }

        }

        //[HttpPost]
        //public ActionResult purchaseOrderStatus()
        //{
        //    return View();
        //}

        // GET: ShipNotices/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder shipNotice = db.PurchaseOrder.Find(id);
            if (shipNotice == null)
            {
                return HttpNotFound();
            }
            var query = from n in db.PurchaseOrderDtl where n.PurchaseOrderID == id select n;
            int amount = 0;
            foreach (var x in query)
            {
                amount = amount + (int)x.Total;
            }
            ViewBag.amount = amount;
            return View(shipNotice);
        }

        // GET: ShipNotices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShipNotices/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ShipNoticeOID,ShipNoticeID,PurchaseOrderID,ShipDate,EmployeeID,CompanyCode,SupplierAccountID")] ShipNotice shipNotice)
        {
            if (ModelState.IsValid)
            {
                db.ShipNotice.Add(shipNotice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shipNotice);
        }

        // GET: ShipNotices/Edit/5
        public ActionResult Edit(string id)
        {
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }
            var query = from n in db.PurchaseOrderDtl where n.PurchaseOrderID == id select n;
            int amount = 0;
            foreach (var x in query)
            {
                amount = amount + (int)x.Total;
            }
            purchaseOrderViewModel.failMessage = Convert.ToString(TempData["failMessage"]);
            purchaseOrderViewModel.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
            purchaseOrderViewModel.ReceiverName = purchaseOrder.ReceiverName;
            purchaseOrderViewModel.ReceiverTel = purchaseOrder.ReceiverTel;
            purchaseOrderViewModel.ReceiverMobile = purchaseOrder.ReceiverMobile;
            purchaseOrderViewModel.ReceiptAddress = purchaseOrder.ReceiptAddress;

            ViewBag.failMessage = Convert.ToString(TempData["failMessage"]);
            ViewBag.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
            ViewBag.ReceiverName = purchaseOrder.ReceiverName;
            ViewBag.ReceiverTel = purchaseOrder.ReceiverTel;
            ViewBag.ReceiverMobile = purchaseOrder.ReceiverMobile;
            ViewBag.ReceiptAddress = purchaseOrder.ReceiptAddress;
            ViewBag.amount = amount;
            return View(purchaseOrderViewModel);
        }

        // POST: ShipNotices/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ShipNoticeOID,ShipNoticeID,PurchaseOrderID,ShipDate,EmployeeID,CompanyCode,SupplierAccountID")] ShipNotice shipNotice)
        {
            PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(shipNotice.PurchaseOrderID);
            TempData.Add("purchaseOrderID", shipNotice.PurchaseOrderID);
            ViewBag.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
            ViewBag.ReceiverName = purchaseOrder.ReceiverName;
            ViewBag.ReceiverTel = purchaseOrder.ReceiverTel;
            ViewBag.ReceiverMobile = purchaseOrder.ReceiverMobile;
            ViewBag.ReceiptAddress = purchaseOrder.ReceiptAddress;
            //if (ModelState.IsValid)
            //{
            //    db.Entry(shipNotice).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            return View(shipNotice.PurchaseOrderID);
        }

        //出貨確認Controller，要修改採購單狀態、以及貨源清單庫存數量
        [HttpPost, ActionName("shipCheck")]
        public ActionResult shipCheck(string purchaseOrderID)
        {
            if (purchaseOrderID != null)
            {
                //修改貨源清單庫存數量              
                var podquery = from pod in db.PurchaseOrderDtl
                               join sl in db.SourceList on pod.SourceListID equals sl.SourceListID
                               where pod.PurchaseOrderID == purchaseOrderID
                               select new { pod.TotalPartQty, sl.UnitsInStock, pod.SourceListID, pod.PurchaseOrderID, pod.PurchaseOrderDtlCode };
                foreach (var x in podquery)
                {
                    if (x.UnitsInStock >= x.TotalPartQty)
                    {
                        SourceList sourceList = db.SourceList.Find(x.SourceListID);
                        PurchaseOrderDtl purchaseOrderDtl = db.PurchaseOrderDtl.Find(x.PurchaseOrderDtlCode);
                        sourceList.UnitsInStock = sourceList.UnitsInStock - purchaseOrderDtl.TotalPartQty;
                        db.Entry(sourceList).State = EntityState.Modified;
                    }
                    else
                    {
                        PurchaseOrder po = db.PurchaseOrder.Find(purchaseOrderID);
                        TempData.Add("failmessage", $"<script>Swal.fire('{PMSAWebMVC.Resources.AppResource.NoStock}')</script>");
                        var query = from nn in db.PurchaseOrderDtl where nn.PurchaseOrderID == purchaseOrderID select nn;
                        int amount = 0;
                        foreach (var y in query)
                        {
                            amount = amount + (int)y.Total;
                        }
                        ViewBag.amount = amount;
                        //return View(po);
                        return RedirectToAction("Edit", "ShipNotices", new { id = purchaseOrderID });
                    }
                }
                //===================================
                //舊寫法
                //var n = from pod in db.PurchaseOrderDtl where pod.PurchaseOrderID == purchaseOrderID select pod;
                //var s = from sl in db.SourceList select sl;
                //List<string> sourceListTemp = new List<string>();
                //List<string> PurchaseOrderDtlCodeTemp = new List<string>();
                //foreach (var i in n)
                //{
                //    foreach (var j in s)
                //    {
                //        if (i.SourceListID == j.SourceListID)
                //        {
                //            sourceListTemp.Add(j.SourceListID);
                //            PurchaseOrderDtlCodeTemp.Add(i.PurchaseOrderDtlCode);
                //        }
                //    }
                //}
                //for (int i = 0; i < sourceListTemp.Count(); i++)
                //{
                //    SourceList sourceList = db.SourceList.Find(sourceListTemp[i]);
                //    PurchaseOrderDtl purchaseOrderDtl = db.PurchaseOrderDtl.Find(PurchaseOrderDtlCodeTemp[i]);
                //    int temp = sourceList.UnitsInStock - purchaseOrderDtl.TotalPartQty;
                //    if (temp >= 0)
                //    {
                //        sourceList.UnitsInStock = temp;


                //    }
                //    else
                //    {
                //        TempData.Add("failMessage", $"<script>Swal.fire('{PMSAWebMVC.Resources.AppResource.NoStock}')</script>");
                //        PurchaseOrder po = db.PurchaseOrder.Find(purchaseOrderID);
                //        var query = from nn in db.PurchaseOrderDtl where nn.PurchaseOrderID == purchaseOrderID select nn;
                //        int amount = 0;
                //        foreach (var x in query)
                //        {
                //            amount = amount + (int)x.Total;
                //        }
                //        ViewBag.amount = amount;
                //        return RedirectToAction("Edit", "ShipNotices", new { id = purchaseOrderID });
                //    }
                //    db.Entry(sourceList).State = EntityState.Modified;
                //    //db.Entry(purchaseOrderDtl).State = EntityState.Modified;
                //    // db.SaveChanges();
                //}
                //修改採購單狀態
                PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(purchaseOrderID);
                purchaseOrder.PurchaseOrderStatus = "S";
                db.Entry(purchaseOrder).State = EntityState.Modified;
                //存進資料庫
                db.SaveChanges();
                //return View(purchaseOrder);
                return RedirectToAction("Index", "ShipNotices", new { controller = "Index", action = "ShipNotices", id = purchaseOrder.PurchaseOrderID });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: ShipNotices/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShipNotice shipNotice = db.ShipNotice.Find(id);
            if (shipNotice == null)
            {
                return HttpNotFound();
            }
            return View(shipNotice);
        }

        // POST: ShipNotices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ShipNotice shipNotice = db.ShipNotice.Find(id);
            db.ShipNotice.Remove(shipNotice);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
