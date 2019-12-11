using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PMSAWebMVC.Models;

namespace PMSAWebMVC.Controllers
{
    public class InventoryDtlsController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: InventoryDtls
        public ActionResult Index()
        {
            var inventoryDtl = db.InventoryDtl.Include(i => i.Employee).Include(i => i.Employee1).Include(i => i.InventoryCategory).Include(i => i.Part).Include(i => i.SourceList).Include(i => i.WarehouseInfo);

            return View(inventoryDtl);
        }

        //dropdownlist
        public ActionResult whousecodelist()
        {
            var datas = db.WarehouseInfo.Select(s => new { s.WarehouseCode, s.WarehouseName });
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult inventorycategorycode()
        {
            var datas = db.InventoryCategory.Select(s => new { s.InventoryCategoryCode, s.InventoryCategoryName });
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        //因為不同倉庫可能存放相同料號的庫存，所以在此用倉庫來判斷哪些新增過
        public ActionResult sourcelistID(string id)
        {
            var da1 = db.InventoryDtl.Where(w => w.WarehouseCode == id).Select(s => s.SourceListID);
            var da2 = db.SourceList.Select(s => s.SourceListID);
            var datas = da2.Except(da1);
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        // GET: InventoryDtls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryDtl inventoryDtl = db.InventoryDtl.Find(id);
            if (inventoryDtl == null)
            {
                return HttpNotFound();
            }

            var datas = db.InventoryDtl.AsEnumerable().Where(w => w.InventoryCode == id).
                        Select(s => new
                        {
                            s.InventoryCode,
                            s.WarehouseInfo.WarehouseName,
                            s.InventoryCategory.InventoryCategoryName,
                            s.SourceListID,
                            s.Part.PartName,
                            s.UnitsOnStockOutOrder,
                            s.UnitsOnStockInOrder,
                            s.SafetyQty,
                            s.UnitsInStock,
                            crname = s.CreateEmployeeID,
                            CreateDate = s.CreateDate.ToString(),
                            laname = s.LastModifiedEmployeeID,
                            LastModifiedDate = s.LastModifiedDate.ToString()
                        });

            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        // POST: InventoryDtls/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Create(InventoryDtl inventoryDtl)
        {
            string message = "新增成功!!";
            bool status = true;

            if (inventoryDtl.SourceListID != null)
            {
                int z = 1;
                string y = inventoryDtl.WarehouseCode + "-" + inventoryDtl.InventoryCategoryCode + "-" + inventoryDtl.SourceListID + "-S0000" + z.ToString();

                for (int i = 0; i < db.InventoryDtl.Count(); i++)
                {
                    InventoryDtl test = new InventoryDtl();
                    if (z < 9)
                    {
                        if (test != null)
                        {
                            z += 1;
                            y = inventoryDtl.WarehouseCode + "-" + inventoryDtl.InventoryCategoryCode + "-" + inventoryDtl.SourceListID + "-S0000" + z.ToString();
                            test = db.InventoryDtl.Find(y);
                        }
                    }
                    else if (z < 99)
                    {
                        if (test != null)
                        {
                            z += 1;
                            y = inventoryDtl.WarehouseCode + "-" + inventoryDtl.InventoryCategoryCode + "-" + inventoryDtl.SourceListID + "-S000" + z.ToString();
                            test = db.InventoryDtl.Find(y);
                        }
                    }
                    else if (z < 999)
                    {
                        if (test != null)
                        {
                            z += 1;
                            y = inventoryDtl.WarehouseCode + "-" + inventoryDtl.InventoryCategoryCode + "-" + inventoryDtl.SourceListID + "-S00" + z.ToString();
                            test = db.InventoryDtl.Find(y);
                        }
                    }
                    else if (z < 9999)
                    {
                        if (test != null)
                        {
                            z += 1;
                            y = inventoryDtl.WarehouseCode + "-" + inventoryDtl.InventoryCategoryCode + "-" + inventoryDtl.SourceListID + "-S0" + z.ToString();
                            test = db.InventoryDtl.Find(y);
                        }
                    }
                    else
                    {
                        if (test != null)
                        {
                            z += 1;
                            y = inventoryDtl.WarehouseCode + "-" + inventoryDtl.InventoryCategoryCode + "-" + inventoryDtl.SourceListID + "-S" + z.ToString();
                            test = db.InventoryDtl.Find(y);
                        }
                    }

                }

                inventoryDtl.InventoryCode = y;

                var q = from p in db.SourceList
                        where p.SourceListID == inventoryDtl.SourceListID
                        select p.PartNumber;

                var p1 = q.ToList();

                var qe = from p in db.SourceList
                         select p.SourceListID;
                var q1 = from p in db.InventoryDtl
                         select p.SourceListID;
                var q2 = qe.Except(q1);

                int ui = inventoryDtl.UnitsInStock;
                var qt = from p in db.SourceList
                         where p.SourceListID == inventoryDtl.SourceListID
                         select p.QtyPerUnit;
                var pp = qt.ToList();
                int qty = ui * Convert.ToInt32(pp[0].ToString());

                if (inventoryDtl.SafetyQty == null)
                {
                    inventoryDtl.SafetyQty = 0;
                }

                var use = User.Identity.GetEmployee();

                if (ModelState.IsValid)
                {
                    inventoryDtl.UnitsInStock = qty;
                    inventoryDtl.PartNumber = p1[0].ToString();
                    inventoryDtl.CreateEmployeeID = use.EmployeeID;
                    inventoryDtl.LastModifiedEmployeeID = use.EmployeeID;
                    inventoryDtl.UnitsOnStockInOrder = 0;
                    inventoryDtl.UnitsOnStockOutOrder = 0;
                    inventoryDtl.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    inventoryDtl.LastModifiedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    db.InventoryDtl.Add(inventoryDtl);
                    db.SaveChanges();

                    var ii = db.InventoryDtl.Max(x => x.InventoryDtlOID);

                    var inventorycode = db.InventoryDtl.Where(w => w.InventoryDtlOID == ii).Select(s => s.InventoryCode);
                    var partnumber = db.InventoryDtl.Where(w => w.InventoryDtlOID == ii).Select(s => s.Part.PartName);
                    var unitsonstockinorder = db.InventoryDtl.Where(w => w.InventoryDtlOID == ii).Select(s => s.UnitsOnStockInOrder);
                    var unitsonstockoutorder = db.InventoryDtl.Where(w => w.InventoryDtlOID == ii).Select(s => s.UnitsOnStockOutOrder);
                    var unit = db.InventoryDtl.Where(w => w.InventoryDtlOID == ii).Select(s => s.UnitsInStock);

                    var inv = inventorycode.ToList();
                    var c = inv[0].ToString();

                    var pat = partnumber.ToList();
                    var pa = pat[0].ToString();

                    var uio = unitsonstockinorder.ToList();
                    var uis = uio[0].ToString();

                    var uoo = unitsonstockoutorder.ToList();
                    var uos = uoo[0].ToString();

                    var uin = unit.ToList();
                    var uii = uin[0].ToString();

                    return Json(new { status = status, message = message, id = db.InventoryDtl.Max(x => x.InventoryDtlOID),
                                      c, pa, uis, uos, uii }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    message = "新增失敗!!";
                    status = false;
                    return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                message = "新增失敗!!目前已沒有新的貨源清單編號可新增庫存!!!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        //修改
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(InventoryDtl inventoryDtl)
        {
            string message = "修改成功!!";
            bool status = true;

            var q = from p in db.InventoryDtl
                    where p.WarehouseCode == inventoryDtl.WarehouseCode
                    select p.WarehouseCode;
            var p0 = q.ToList();
            var q1 = db.InventoryDtl.Where(w => w.InventoryCategoryCode == inventoryDtl.InventoryCategoryCode).Select(s => s.InventoryCategoryCode);
            var p1 = q1.ToList();
            var q2 = db.InventoryDtl.Where(w => w.Part.PartName == inventoryDtl.PartNumber).Select(s => s.PartNumber);
            var p2 = q2.ToList();

            int qty = 0;
            var num = from p in db.InventoryDtl
                      where p.InventoryCode == inventoryDtl.InventoryCode
                      select p.UnitsInStock;
            var num1 = num.ToList();
            if (Convert.ToInt32(num1[0].ToString()) != inventoryDtl.UnitsInStock)
            {
                int ui = inventoryDtl.UnitsInStock;
                var qt = from p in db.SourceList
                         where p.SourceListID == inventoryDtl.SourceListID
                         select p.QtyPerUnit;
                var pp = qt.ToList();
                qty = ui * pp[0];
            }
            else
            {
                qty = inventoryDtl.UnitsInStock;
            }
            var use = User.Identity.GetEmployee();
            var unit = db.InventoryDtl.Where(w => w.InventoryCode == inventoryDtl.InventoryCode).Select(s => s.UnitsInStock);
            var uin = unit.ToList();
            var uii = uin[0].ToString();
            if (ModelState.IsValid)
            {
                inventoryDtl.UnitsInStock = qty;
                inventoryDtl.InventoryCategoryCode = p1[0].ToString();
                inventoryDtl.WarehouseCode = p0[0].ToString();
                inventoryDtl.PartNumber = p2[0].ToString();
                inventoryDtl.UnitsOnStockInOrder = 0;
                inventoryDtl.UnitsOnStockOutOrder = 0;
                inventoryDtl.LastModifiedEmployeeID = use.EmployeeID;
                inventoryDtl.CreateDate = Convert.ToDateTime(inventoryDtl.CreateDate.ToShortDateString());
                inventoryDtl.LastModifiedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                db.Entry(inventoryDtl).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = status, message = message, id = db.InventoryDtl.Max(x => x.InventoryDtlOID), qty }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "修改失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StockInEdit(string id)
        {
            InventoryDtl inventoryDtl = new InventoryDtl();

            //InventoryCode
            inventoryDtl.InventoryCode = id;
            //WarehouseCode
            var wcode = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.WarehouseCode).ToList();
            inventoryDtl.WarehouseCode = wcode[0];
            //InventoryCategoryCode
            var iccode = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.InventoryCategoryCode).ToList();
            inventoryDtl.InventoryCategoryCode = iccode[0];
            //SourceListID
            var slid = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.SourceListID).ToList();
            inventoryDtl.SourceListID = slid[0];
            //PartNumber
            var parnum = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.PartNumber).ToList();
            inventoryDtl.PartNumber = parnum[0];
            //UnitsInStock  庫存數量
            var unit = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.UnitsInStock).ToList();
            var unitin = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.UnitsOnStockInOrder).ToList();
            inventoryDtl.UnitsInStock = unit[0] + unitin[0];
            int qty = inventoryDtl.UnitsInStock;
            //UnitsOnStockOutOrder
            var unitout = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.UnitsOnStockOutOrder).ToList();
            inventoryDtl.UnitsOnStockOutOrder = unitout[0];
            //UnitsOnStockInOrder 入庫申請數量
            inventoryDtl.UnitsOnStockInOrder = 0;
            //SafetyQty
            var safe = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.SafetyQty).ToList();
            inventoryDtl.SafetyQty = safe[0];
            //CreateDate
            var crdate = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.CreateDate).ToList();
            inventoryDtl.CreateDate = crdate[0];
            //CreateEmployeeID
            var cremid = db.InventoryDtl.Where(w => w.InventoryCode == id).Select(s => s.CreateEmployeeID).ToList();
            inventoryDtl.CreateEmployeeID = cremid[0];
            //LastModifiedDate
            inventoryDtl.LastModifiedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            //LastModifiedEmployeeID
            var use = User.Identity.GetEmployee();
            inventoryDtl.LastModifiedEmployeeID = use.EmployeeID;


            if (ModelState.IsValid)
            {
                db.Entry(inventoryDtl).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { qty, id = db.InventoryDtl.Max(x => x.InventoryDtlOID) }, JsonRequestBehavior.AllowGet);
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
