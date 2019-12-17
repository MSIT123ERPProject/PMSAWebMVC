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

        //select
        public ActionResult selectstockinid()
        {
            var datas = db.StockIn.Select(s => new { s.StockInID });
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        // GET: StockInDtls/Details/5
        public ActionResult Detail(int? id)
        {
            var da = db.StockInDtl.Where(w => w.StockInDtlOID == id).Select(s => s.Part.PartName).ToList();
            string partName = da[0];

            var da1 = db.StockInDtl.Where(w => w.StockInDtlOID == id).Select(s => s.PartNumber).ToList();
            string part = da1[0];

            var oid = db.StockInDtl.Where(w => w.StockInDtlOID == id).Select(s => s.StockInID).ToList();
            var oidd = oid[0];
            var purdhid = db.StockIn.Where(w => w.StockInID == oidd).Select(s => s.PurchaseOrderReceiveID).ToList();
            var putdhidd = purdhid[0];
            var purdtl = db.PurchaseOrderReceiveDtl.Where(w => w.PurchaseOrderReceiveID == putdhidd && w.PurchaseOrderDtl.PartNumber == part).Select(s => s.AcceptQty).ToList();
            var puqty = purdtl[0];

            var da2 = db.StockInDtl.Where(w => w.StockInDtlOID == id).Select(s => s.EXP).ToList();
            string date = String.Format("{0:yyyy/MM/dd}", da2[0]);

            var datas = db.StockInDtl.AsEnumerable().Where(w => w.StockInDtlOID == id).
                        Select(s => new
                        {
                            s.StockInID,
                            s.InventoryCode,
                            s.Remark,
                            partName,
                            puqty,
                            s.StockInQty,
                            date
                        });

            return Json(datas, JsonRequestBehavior.AllowGet);
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
        

        // POST: StockInDtls/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Edit( StockInDtl stockInDtl)
        {
            string message = "修改成功!!";
            bool status = true;

            var data = db.Part.Where(w => w.PartName == stockInDtl.PartNumber).Select(s => s.PartNumber).ToList();
            stockInDtl.PartNumber = data[0];

            if (ModelState.IsValid)
            {
                db.Entry(stockInDtl).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = status, message = message, id = db.StockInDtl.Max(x => x.StockInDtlOID) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "修改失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //入庫
        [HttpPost]
        public ActionResult StockInEdit(StockInDtl stockInDtl)
        {
            string message = "入庫成功!!";
            bool status = true;

            var data = db.Part.Where(w => w.PartName == stockInDtl.PartNumber).Select(s => s.PartNumber).ToList();
            stockInDtl.PartNumber = data[0];
            string oid = stockInDtl.InventoryCode;
            //StockInDtlOID: oidd, Y
            //            StockInID:$("#StockInID").val(),Y
            //            InventoryCode: $("#InventoryCode").val(),Y
            //            PartNumber: $("#PartName").val(),Y
            //            StockInQty: $("#StockInQty").val(),Y
            //            Remark: $("#Remark").val(),
            //            EXP: $("#EXP").val()Y

            int senstockin = Convert.ToInt32(stockInDtl.Remark);        //本次入庫數量
            stockInDtl.StockInQty = stockInDtl.StockInQty + senstockin; //原本入庫數量+本次入庫數量

            stockInDtl.Remark = "";

            if (ModelState.IsValid)
            {
                db.Entry(stockInDtl).State = EntityState.Modified;
                db.SaveChanges();
                InventoryDtlEdit(oid, senstockin);
                return Json(new { status = status, message = message, id = db.StockInDtl.Max(x => x.StockInDtlOID) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "入庫失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void InventoryDtlEdit(string oid, int num)
        {
            InventoryDtl inventoryDtl = new InventoryDtl();

            //InventoryCode
            inventoryDtl.InventoryCode = oid;
            //WarehouseCode
            var wcode = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.WarehouseCode).ToList();
            inventoryDtl.WarehouseCode = wcode[0];
            //InventoryCategoryCode
            var iccode = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.InventoryCategoryCode).ToList();
            inventoryDtl.InventoryCategoryCode = iccode[0];
            //SourceListID
            var slid = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.SourceListID).ToList();
            inventoryDtl.SourceListID = slid[0];
            //PartNumber
            var parnum = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.PartNumber).ToList();
            inventoryDtl.PartNumber = parnum[0];
            //UnitsInStock
            var unit = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.UnitsInStock).ToList();
            inventoryDtl.UnitsInStock = unit[0];
            //UnitsOnStockOutOrder
            var unitout = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.UnitsOnStockOutOrder).ToList();
            inventoryDtl.UnitsOnStockOutOrder = unitout[0];
            //UnitsOnStockInOrder 入庫申請數量
            var unitin = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.UnitsOnStockInOrder).ToList();
            inventoryDtl.UnitsOnStockInOrder = unitin[0] + num;
            //SafetyQty
            var safe = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.SafetyQty).ToList();
            inventoryDtl.SafetyQty = safe[0];
            //CreateDate
            var crdate = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.CreateDate).ToList();
            inventoryDtl.CreateDate = crdate[0];
            //CreateEmployeeID
            var cremid = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.CreateEmployeeID).ToList();
            inventoryDtl.CreateEmployeeID = cremid[0];
            //LastModifiedDate
            var lmddate = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.LastModifiedDate).ToList();
            inventoryDtl.LastModifiedDate = lmddate[0];
            //LastModifiedEmployeeID
            var lmemid = db.InventoryDtl.Where(w => w.InventoryCode == oid).Select(s => s.LastModifiedEmployeeID).ToList();
            inventoryDtl.LastModifiedEmployeeID = lmemid[0];

            
            if (ModelState.IsValid)
            {
                db.Entry(inventoryDtl).State = EntityState.Modified;
                db.SaveChanges();
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
