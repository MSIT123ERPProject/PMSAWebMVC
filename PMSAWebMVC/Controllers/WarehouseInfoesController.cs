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
    [Authorize(Roles = "Warehouse")]
    public class WarehouseInfoesController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: WarehouseInfoes
        public ActionResult Index()
        {
            var warehouseInfo = db.WarehouseInfo.Include(w => w.Employee);
            return View(warehouseInfo);
        }


        // GET: WarehouseInfoes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseInfo warehouseInfo = db.WarehouseInfo.Find(id);
            if (warehouseInfo == null)
            {
                return HttpNotFound();
            }
            return View(warehouseInfo);
        }

        // GET: WarehouseInfoes/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name");
            return View();
        }

        // POST: WarehouseInfoes/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeDeny(Roles = "Manager")]
        public ActionResult Create([Bind(Include = "WarehouseInfoOID,WarehouseCode,WarehouseName,Address,EmployeeID,Tel,Remark")] WarehouseInfo warehouseInfo)
        {
            if (ModelState.IsValid)
            {
                db.WarehouseInfo.Add(warehouseInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", warehouseInfo.EmployeeID);
            return View(warehouseInfo);
        }

        // GET: WarehouseInfoes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseInfo warehouseInfo = db.WarehouseInfo.Find(id);
            if (warehouseInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", warehouseInfo.EmployeeID);
            return View(warehouseInfo);
        }

        // POST: WarehouseInfoes/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeDeny(Roles = "Manager")]
        public ActionResult Edit([Bind(Include = "WarehouseInfoOID,WarehouseCode,WarehouseName,Address,EmployeeID,Tel,Remark")] WarehouseInfo warehouseInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warehouseInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", warehouseInfo.EmployeeID);
            return View(warehouseInfo);
        }

        // GET: WarehouseInfoes/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    WarehouseInfo warehouseInfo = db.WarehouseInfo.Find(id);
        //    if (warehouseInfo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(warehouseInfo);
        //}

        [HttpPost]
        [AuthorizeDeny(Roles = "Manager")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseInfo warehouseInfo = db.WarehouseInfo.Find(id);
            if (warehouseInfo == null)
            {
                return HttpNotFound();
            }
            db.WarehouseInfo.Remove(warehouseInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: WarehouseInfoes/Delete/5
        //[HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    WarehouseInfo warehouseInfo = db.WarehouseInfo.Find(id);
        //    db.WarehouseInfo.Remove(warehouseInfo);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
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
