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
    public class PurchaseOrderReceivesController : Controller
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: PurchaseOrderReceives
        public ActionResult Index()
        {
            var purchaseOrderReceive = db.PurchaseOrderReceive.Include(p => p.Employee).Include(p => p.PurchaseOrder).Include(p => p.SignFlow).Include(p => p.SupplierAccount).Include(p => p.SupplierInfo);
            return View(purchaseOrderReceive.ToList());
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
            return View(purchaseOrderReceive);
        }

        // GET: PurchaseOrderReceives/Create
        public ActionResult Create()
        {
            ViewBag.PurchaseEmployeeID = new SelectList(db.Employee, "EmployeeID", "Name");
            ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrder, "PurchaseOrderID", "SupplierCode");
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID");
            ViewBag.SupplierAccountID = new SelectList(db.SupplierAccount, "SupplierAccountID", "ContactName");
            ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName");
            return View();
        }

        // POST: PurchaseOrderReceives/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseOrderReceiveOID,PurchaseOrderReceiveID,PurchaseOrderID,PurchaseDate,PurchaseEmployeeID,SupplierCode,SupplierAccountID,SignStatus,SignFlowOID")] PurchaseOrderReceive purchaseOrderReceive)
        {
            if (ModelState.IsValid)
            {
                db.PurchaseOrderReceive.Add(purchaseOrderReceive);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PurchaseEmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrderReceive.PurchaseEmployeeID);
            ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrder, "PurchaseOrderID", "SupplierCode", purchaseOrderReceive.PurchaseOrderID);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrderReceive.SignFlowOID);
            ViewBag.SupplierAccountID = new SelectList(db.SupplierAccount, "SupplierAccountID", "ContactName", purchaseOrderReceive.SupplierAccountID);
            ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrderReceive.SupplierCode);
            return View(purchaseOrderReceive);
        }

        // GET: PurchaseOrderReceives/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.PurchaseEmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrderReceive.PurchaseEmployeeID);
            ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrder, "PurchaseOrderID", "SupplierCode", purchaseOrderReceive.PurchaseOrderID);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrderReceive.SignFlowOID);
            ViewBag.SupplierAccountID = new SelectList(db.SupplierAccount, "SupplierAccountID", "ContactName", purchaseOrderReceive.SupplierAccountID);
            ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrderReceive.SupplierCode);
            return View(purchaseOrderReceive);
        }

        // POST: PurchaseOrderReceives/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseOrderReceiveOID,PurchaseOrderReceiveID,PurchaseOrderID,PurchaseDate,PurchaseEmployeeID,SupplierCode,SupplierAccountID,SignStatus,SignFlowOID")] PurchaseOrderReceive purchaseOrderReceive)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseOrderReceive).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PurchaseEmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrderReceive.PurchaseEmployeeID);
            ViewBag.PurchaseOrderID = new SelectList(db.PurchaseOrder, "PurchaseOrderID", "SupplierCode", purchaseOrderReceive.PurchaseOrderID);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrderReceive.SignFlowOID);
            ViewBag.SupplierAccountID = new SelectList(db.SupplierAccount, "SupplierAccountID", "ContactName", purchaseOrderReceive.SupplierAccountID);
            ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrderReceive.SupplierCode);
            return View(purchaseOrderReceive);
        }

        // GET: PurchaseOrderReceives/Delete/5
        public ActionResult Delete(string id)
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
            return View(purchaseOrderReceive);
        }

        // POST: PurchaseOrderReceives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PurchaseOrderReceive purchaseOrderReceive = db.PurchaseOrderReceive.Find(id);
            db.PurchaseOrderReceive.Remove(purchaseOrderReceive);
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
