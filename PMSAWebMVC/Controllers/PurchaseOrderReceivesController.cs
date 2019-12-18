﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PMSAWebMVC.Models;

namespace PMSAWebMVC.Controllers
{
    public class PurchaseOrderReceivesController : Controller
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: PurchaseOrderReceives
        public ActionResult Index()
        {
            var purchaseOrderReceive = db.PurchaseOrderReceive.Include(p => p.Employee).Include(p => p.PurchaseOrder).Include(p => p.SignFlow).Include(p => p.SupplierAccount).Include(p => p.SupplierInfo);
            return View(purchaseOrderReceive);
        }

        //進貨明細
        public ActionResult IndexDtl(string id)
        {
            var purchaseOrderReceiveDtl = db.PurchaseOrderReceiveDtl.Where(w=>w.PurchaseOrderReceiveID == id).Include(p => p.PurchaseOrderDtl).Include(p => p.PurchaseOrderReceive);
            return View(purchaseOrderReceiveDtl);
        }

        // GET: PurchaseOrderReceives/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderReceive purchaseOrderReceive = db.PurchaseOrderReceive.Find(id);
            if (purchaseOrderReceive == null)
            {
                return HttpNotFound();
            }
            var da = db.PurchaseOrderReceive.Where(w => w.PurchaseOrderReceiveID == id).Select(s => s.SignStatus).ToList();
            StockInsController stock = new StockInsController();
            string datastust = stock.stut(da[0]);

            var dat = db.PurchaseOrderReceive.Where(w => w.PurchaseOrderReceiveID == id).Select(s => s.PurchaseDate).ToList();
            var date = dat[0].ToShortDateString();

            var emid = db.PurchaseOrderReceive.Where(w => w.PurchaseOrderReceiveID == id).Select(s => new { s.PurchaseEmployeeID, s.Employee.Name }).ToList();
            var emid1 = emid[0].PurchaseEmployeeID + "-" + emid[0].Name;

            var supid = db.PurchaseOrderReceive.Where(w => w.PurchaseOrderReceiveID == id).Select(s => new { s.SupplierAccountID, s.SupplierAccount.ContactName }).ToList();
            var supid1 = supid[0].SupplierAccountID + "-" + supid[0].ContactName;

            var scode = db.PurchaseOrderReceive.Where(w => w.PurchaseOrderReceiveID == id).Select(s => new { s.SupplierCode, s.SupplierInfo.SupplierName }).ToList();
            var scode1 = scode[0].SupplierCode + "-" + scode[0].SupplierName;

            var datas = db.PurchaseOrderReceive.AsEnumerable().Where(w => w.PurchaseOrderReceiveID == id).
                        Select(s => new
                        {
                            s.PurchaseOrderReceiveID,
                            s.PurchaseOrderID,
                            date,
                            emid1,
                            scode1,
                            supid1,
                            datastust
                        });

            return Json(datas, JsonRequestBehavior.AllowGet);
        }
        
        // create
        DateTime now = DateTime.Now;
        public void Create(string shipid)
        {
            PurchaseOrderReceive purchaseOrderReceive = new PurchaseOrderReceive();
            //PurchaseOrderReceiveID
            string porId = $"POR-{now:yyyyMMdd}-";
            int count = db.PurchaseOrderReceive.Where(x => x.PurchaseOrderReceiveID.StartsWith(porId)).Count();
            count++;
            porId = $"{porId}{count:000}";
            purchaseOrderReceive.PurchaseOrderReceiveID = porId;
            //PurchaseOrderID
            var poid = db.ShipNotice.Where(w => w.ShipNoticeID == shipid).Select(s => s.PurchaseOrderID).ToList();
            var poid1 = poid[0];
            purchaseOrderReceive.PurchaseOrderID = poid1;
            //PurchaseDate
            purchaseOrderReceive.PurchaseDate = now;
            //PurchaseEmployeeID
            var pemid = db.ShipNotice.Where(w => w.ShipNoticeID == shipid).Select(s => s.EmployeeID).ToList();
            string pemid1 = pemid[0];
            purchaseOrderReceive.PurchaseEmployeeID = pemid1;
            //SupplierCode
            var supcode = db.ShipNotice.Where(w => w.ShipNoticeID == shipid).Select(s => s.SupplierAccount.SupplierCode).ToList();
            string supcode1 = supcode[0];
            purchaseOrderReceive.SupplierCode = supcode1;
            //SupplierAccountID
            var supid = db.ShipNotice.Where(w => w.ShipNoticeID == shipid).Select(s => s.SupplierAccountID).ToList();
            string supid1 = supid[0];
            purchaseOrderReceive.SupplierAccountID = supid1;
            //SignStatus
            purchaseOrderReceive.SignStatus = "S";
            if (ModelState.IsValid)
            {
                db.PurchaseOrderReceive.Add(purchaseOrderReceive);
                db.SaveChanges();
                //呼叫新增進貨單明細方法
                PurchaseOrderReceiveDtlsController purchaseOrderReceiveDtlsController = new PurchaseOrderReceiveDtlsController();
                purchaseOrderReceiveDtlsController.Create(shipid, porId);
            }
        }

        // GET: PurchaseOrderReceives/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PurchaseOrderReceive purchaseOrderReceive = db.PurchaseOrderReceive.Find(id);
        //    if (purchaseOrderReceive == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.PurchaseEmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrderReceive.PurchaseEmployeeID);
        //    ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrder, "PurchaseOrderID", "SupplierCode", purchaseOrderReceive.PurchaseOrderID);
        //    ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrderReceive.SignFlowOID);
        //    ViewBag.SupplierAccountID = new SelectList(db.SupplierAccount, "SupplierAccountID", "ContactName", purchaseOrderReceive.SupplierAccountID);
        //    ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrderReceive.SupplierCode);
        //    return View(purchaseOrderReceive);
        //}

        //// POST: PurchaseOrderReceives/Edit/5
        //// 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        //// 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "PurchaseOrderReceiveOID,PurchaseOrderReceiveID,PurchaseOrderID,PurchaseDate,PurchaseEmployeeID,SupplierCode,SupplierAccountID,SignStatus,SignFlowOID")] PurchaseOrderReceive purchaseOrderReceive)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(purchaseOrderReceive).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.PurchaseEmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrderReceive.PurchaseEmployeeID);
        //    ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrder, "PurchaseOrderID", "SupplierCode", purchaseOrderReceive.PurchaseOrderID);
        //    ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrderReceive.SignFlowOID);
        //    ViewBag.SupplierAccountID = new SelectList(db.SupplierAccount, "SupplierAccountID", "ContactName", purchaseOrderReceive.SupplierAccountID);
        //    ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrderReceive.SupplierCode);
        //    return View(purchaseOrderReceive);
        //}
        

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
