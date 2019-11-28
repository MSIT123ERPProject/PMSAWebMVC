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

        //下拉選單
        public ActionResult getemployeeid()
        {
            var employees = db.Employee.Select(c => new
            {
                c.EmployeeID,
                c.Name
            });

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        //新增
        [HttpPost]
        [AuthorizeDeny(Roles = "Manager")]
        public ActionResult Create(WarehouseInfo warehouseInfo)
        {
            string message = "新增成功!!";
            bool status = true;

            if (ModelState.IsValid)
            {
                db.WarehouseInfo.Add(warehouseInfo);
                db.SaveChanges();
                return Json(new { status = status, message = message, id = db.WarehouseInfo.Max(x => x.WarehouseInfoOID) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "新增失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //刪除
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

        //檢視
        public ActionResult Detail(string id)
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
            var datas = db.WarehouseInfo.Where(w => w.WarehouseCode == id).
                       Select(g => new { g.WarehouseInfoOID, g.WarehouseCode, g.WarehouseName, g.Address, g.EmployeeID, g.Tel, g.Remark });

            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        //修改
        [HttpPost]
        [AuthorizeDeny(Roles = "Manager")]
        public ActionResult Edit(WarehouseInfo warehouseInfo)
        {
            string message = "修改成功!!";
            bool status = true;

            if (ModelState.IsValid)
            {
                db.Entry(warehouseInfo).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = status, message = message, id = db.WarehouseInfo.Max(x => x.WarehouseInfoOID) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "修改失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
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
