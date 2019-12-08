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
    public class StockInDtlsController : Controller
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: StockInDtls
        public ActionResult Index()
        {
            var stockInDtl = db.StockInDtl.Include(s => s.InventoryDtl).Include(s => s.Part).Include(s => s.StockIn);
            return View(stockInDtl.ToList());
        }

        // GET: StockInDtls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockInDtl stockInDtl = db.StockInDtl.Find(id);
            if (stockInDtl == null)
            {
                return HttpNotFound();
            }
            return View(stockInDtl);
        }

        

        // POST: StockInDtls/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        public void Create(string stockInid)
        {
            StockInDtl stockInDtl = new StockInDtl();

            stockInDtl.StockInID = stockInid;

            var porid = db.StockIn.Where(w1 => w1.StockInID == stockInid).Select(s => s.PurchaseOrderReceiveID).ToList();
            //庫存編號
            string si = porid[0];
            var code = db.PurchaseOrderReceiveDtl.Where(w => w.PurchaseOrderReceiveID == si).Select(s => s.PurchaseOrderDtlCode);
            var sz = code.ToList();
            foreach (var item in sz)
            {
                //1.倉庫必須先有資料才能有入庫明細，不然抓不到庫存編號
                //2.必須選擇所要進入的倉庫是哪個，因為一個料件可能存放不同倉庫
                //3.等於不能自動產生
                var sourcelist = db.PurchaseOrderDtl.Where(w => w.PurchaseOrderDtlCode == item).Select(s => s.SourceListID).ToList();
                string list = sourcelist[0];
                var codee = db.InventoryDtl.Where(w => w.SourceListID == list).Select(s => s.InventoryCode).ToList();
                stockInDtl.InventoryCode = codee[0];
                var num = db.PurchaseOrderDtl.Where(w => w.PurchaseOrderDtlCode == item).Select(s => s.PartNumber).ToList();
                stockInDtl.PartNumber = num[0];
                stockInDtl.StockInQty = 0;

                if (ModelState.IsValid)
                {
                    db.StockInDtl.Add(stockInDtl);
                    db.SaveChanges();
                }
            }
        }

        // GET: StockInDtls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockInDtl stockInDtl = db.StockInDtl.Find(id);
            if (stockInDtl == null)
            {
                return HttpNotFound();
            }
            ViewBag.InventoryCode = new SelectList(db.InventoryDtl, "InventoryCode", "WarehouseCode", stockInDtl.InventoryCode);
            ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", stockInDtl.PartNumber);
            ViewBag.StockInID = new SelectList(db.StockIn, "StockInID", "PurchaseOrderReceiveID", stockInDtl.StockInID);
            return View(stockInDtl);
        }

        // POST: StockInDtls/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockInDtlOID,StockInID,InventoryCode,PartNumber,StockInQty,EXP,Remark")] StockInDtl stockInDtl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockInDtl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InventoryCode = new SelectList(db.InventoryDtl, "InventoryCode", "WarehouseCode", stockInDtl.InventoryCode);
            ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", stockInDtl.PartNumber);
            ViewBag.StockInID = new SelectList(db.StockIn, "StockInID", "PurchaseOrderReceiveID", stockInDtl.StockInID);
            return View(stockInDtl);
        }

        // GET: StockInDtls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockInDtl stockInDtl = db.StockInDtl.Find(id);
            if (stockInDtl == null)
            {
                return HttpNotFound();
            }
            return View(stockInDtl);
        }

        // POST: StockInDtls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockInDtl stockInDtl = db.StockInDtl.Find(id);
            db.StockInDtl.Remove(stockInDtl);
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
