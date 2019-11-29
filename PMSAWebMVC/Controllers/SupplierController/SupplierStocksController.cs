using PMSAWebMVC.Controllers;
using PMSAWebMVC.Models;
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
    public class SupplierStocksController : BaseController
    {
        private PMSAEntities db;
        private string SupplierCode;
        private string SupplierAccount;
        public SupplierStocksController()
        {
            db = new PMSAEntities();
            SupplierCode = "S00001";
            SupplierAccount = "SE00001";
        }
        //進入庫存管理頁面方法
        public ActionResult Index()
        {
            SupplierInfo supplierInfo = db.SupplierInfo.Find(SupplierCode);
            ViewBag.supplierName = supplierInfo.SupplierName;
            ViewBag.supplierCode = SupplierCode;
            return View();
        }
        //dataTable取得顯示資料的方法
        [HttpGet]
        public JsonResult GetSourcelistBySupplierCode(string supplierCode)
        {
            //注意  :   dataTable只接受Enumerable類別 ，所以要加上AsEnumerable()方法
            var query = from sl in db.SourceList.AsEnumerable()
                        where sl.SupplierCode == supplierCode
                        select new SourceList
                        {
                            SourceListID = sl.SourceListID,
                            PartNumber = sl.PartNumber,
                            QtyPerUnit = sl.QtyPerUnit,
                            UnitPrice = sl.UnitPrice,
                            UnitsOnOrder = sl.UnitsOnOrder,
                            UnitsInStock = sl.UnitsInStock
                        };
            var json = new { data = query };
            return Json( json , JsonRequestBehavior.AllowGet);
        }
        //sweetalert2 修改庫存視窗用ajax方法
        [HttpPost]
        public JsonResult UpdateStock([Bind(Include = "UnitsInStock,PartNumber,SourceListOID,SourceListID")] SourceList SourceList)
        {
            if (SourceList.SourceListID ==null) {
                return Json( new { status = "savefail",message="修改失敗" }, JsonRequestBehavior.AllowGet);
            }
            SourceList sourceList= db.SourceList.Find(SourceList.SourceListID);
            sourceList.UnitsInStock = SourceList.UnitsInStock;
            db.Entry(sourceList).State = EntityState.Modified;
            db.SaveChanges();
            return Json( new {status = "saved",message ="修改成功" } ,JsonRequestBehavior.AllowGet);
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
