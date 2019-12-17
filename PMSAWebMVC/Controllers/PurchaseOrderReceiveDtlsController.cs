using System;
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
    public class PurchaseOrderReceiveDtlsController : Controller
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: PurchaseOrderReceiveDtls
        public ActionResult Index()
        {
            var purchaseOrderReceiveDtl = db.PurchaseOrderReceiveDtl.Include(p => p.PurchaseOrderDtl).Include(p => p.PurchaseOrderReceive);
            return View(purchaseOrderReceiveDtl.ToList());
        }

        // GET: PurchaseOrderReceiveDtls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderReceiveDtl purchaseOrderReceiveDtl = db.PurchaseOrderReceiveDtl.Find(id);
            if (purchaseOrderReceiveDtl == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrderReceiveDtl);
        }

        // GET: PurchaseOrderReceiveDtls/Create
        public ActionResult Create()
        {
            ViewBag.PurchaseOrderDtlCode = new SelectList(db.PurchaseOrderDtl, "PurchaseOrderDtlCode", "PurchaseOrderID");
            ViewBag.PurchaseOrderReceiveID = new SelectList(db.PurchaseOrderReceive, "PurchaseOrderReceiveID", "PurchaseOrderID");
            return View();
        }

        // POST: PurchaseOrderReceiveDtls/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseOrderReceiveDtlOID,PurchaseOrderReceiveDtlCode,PurchaseOrderReceiveID,PurchaseOrderDtlCode,PurchaseQty,PurchaseAmount,RejectQty,AcceptQty,RejectReason,Remark")] PurchaseOrderReceiveDtl purchaseOrderReceiveDtl)
        {
            if (ModelState.IsValid)
            {
                db.PurchaseOrderReceiveDtl.Add(purchaseOrderReceiveDtl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PurchaseOrderDtlCode = new SelectList(db.PurchaseOrderDtl, "PurchaseOrderDtlCode", "PurchaseOrderID", purchaseOrderReceiveDtl.PurchaseOrderDtlCode);
            ViewBag.PurchaseOrderReceiveID = new SelectList(db.PurchaseOrderReceive, "PurchaseOrderReceiveID", "PurchaseOrderID", purchaseOrderReceiveDtl.PurchaseOrderReceiveID);
            return View(purchaseOrderReceiveDtl);
        }

        // GET: PurchaseOrderReceiveDtls/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderReceiveDtl purchaseOrderReceiveDtl = db.PurchaseOrderReceiveDtl.Find(id);
            if (purchaseOrderReceiveDtl == null)
            {
                return HttpNotFound();
            }
            ViewBag.PurchaseOrderDtlCode = new SelectList(db.PurchaseOrderDtl, "PurchaseOrderDtlCode", "PurchaseOrderID", purchaseOrderReceiveDtl.PurchaseOrderDtlCode);
            ViewBag.PurchaseOrderReceiveID = new SelectList(db.PurchaseOrderReceive, "PurchaseOrderReceiveID", "PurchaseOrderID", purchaseOrderReceiveDtl.PurchaseOrderReceiveID);
            return View(purchaseOrderReceiveDtl);
        }

        // POST: PurchaseOrderReceiveDtls/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseOrderReceiveDtlOID,PurchaseOrderReceiveDtlCode,PurchaseOrderReceiveID,PurchaseOrderDtlCode,PurchaseQty,PurchaseAmount,RejectQty,AcceptQty,RejectReason,Remark")] PurchaseOrderReceiveDtl purchaseOrderReceiveDtl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseOrderReceiveDtl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PurchaseOrderDtlCode = new SelectList(db.PurchaseOrderDtl, "PurchaseOrderDtlCode", "PurchaseOrderID", purchaseOrderReceiveDtl.PurchaseOrderDtlCode);
            ViewBag.PurchaseOrderReceiveID = new SelectList(db.PurchaseOrderReceive, "PurchaseOrderReceiveID", "PurchaseOrderID", purchaseOrderReceiveDtl.PurchaseOrderReceiveID);
            return View(purchaseOrderReceiveDtl);
        }

        // GET: PurchaseOrderReceiveDtls/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderReceiveDtl purchaseOrderReceiveDtl = db.PurchaseOrderReceiveDtl.Find(id);
            if (purchaseOrderReceiveDtl == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrderReceiveDtl);
        }

        // POST: PurchaseOrderReceiveDtls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PurchaseOrderReceiveDtl purchaseOrderReceiveDtl = db.PurchaseOrderReceiveDtl.Find(id);
            db.PurchaseOrderReceiveDtl.Remove(purchaseOrderReceiveDtl);
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
