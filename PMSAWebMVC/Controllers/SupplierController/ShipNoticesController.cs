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
    public class ShipNoticesController : BaseController
    {
        /// <summary>
        /// 這個控制器的方法裡面的參數 string id 一律為 採購單編號(purchaseOrderID)
        /// </summary>
        private PMSAEntities db;
        string supplierAccount;
        string supplierCode;
        public ShipNoticesController()
        {
            db = new PMSAEntities();
            supplierCode = "S00001";
            supplierAccount = "SE00001";
        }
        //此方法為幫助INDEX的DATATABLE查訂單資料
        public JsonResult GetPurchaseOrderList(string PurchaseOrderStatus)
        {
            string status = PurchaseOrderStatus;

            var query = from po in db.PurchaseOrder.AsEnumerable()
                        where (po.PurchaseOrderStatus == status && po.SupplierCode == supplierCode)
                        select new PurchaseOrder
                        {
                            PurchaseOrderStatus = po.PurchaseOrderStatus,
                            PurchaseOrderID = po.PurchaseOrderID,
                            ReceiverName = po.ReceiverName,
                            ReceiverTel = po.ReceiverTel,
                            ReceiverMobile = po.ReceiverMobile,
                            ReceiptAddress = po.ReceiptAddress
                        };
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        //檢視未出貨訂單明細，並要可以勾選要出貨的明細，檢視該採購單所有的產品，並可以選擇出貨那些產品
        public ActionResult UnshipOrderDtl([Bind(Include = "PurchaseOrderID")]PurchaseOrder purchaseOrder)
        {
            return View(purchaseOrder);
        }
        //檢視未出貨訂單明細的datatable的AJAX取資料方法
        public JsonResult GetPurchaseOrderDtl(string purchaseOrderID)
        {
            //這裡面要傳過去的欄位本來還要有其他表的，但是因為join時會出現datareader尚未關閉的錯誤
            //所以先放棄，亞辰說要另外寫一個方法計算總金額，不然會發生衝突
            var q = (
                from pod in db.PurchaseOrderDtl.AsEnumerable()
                where pod.PurchaseOrderID == purchaseOrderID
                select new
                {
                    pod.PurchaseOrderID,
                    pod.PurchaseOrderDtlCode,
                    pod.PartName,
                    pod.Qty,
                    pod.QtyPerUnit,
                    pod.CommittedArrivalDate,
                });
            //注意dataTable的資料繫結一定要這樣寫，這樣另一邊的DATA屬性才能繫結的到
            var s = new { data = q };
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        //出貨明細檢視並勾選完畢後進入此方法
        //要修改該採購單明細的實際出貨日期(ShipDate)，並新增資料到出貨明細
        //採購單明細要一一檢查庫存是否足夠，不足則告知是哪筆訂單明細不足，並取消動作回原頁面
        //如果有全部出貨則修改採購單狀態為已出貨，如果沒有?
        //如果只有部分出貨，採購單狀態???????
        public ActionResult shipCheckDtl(  )
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        //按下檢視後進入此方法
        public ActionResult Edit([Bind(Include = "id")] string id)
        {
            PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound("purchaseOrder is null");
            }
            if (purchaseOrder.PurchaseOrderStatus == "E")
            {
                return RedirectToAction("shipCheck", "ShipNotices", new { id });
            }
            else if (purchaseOrder.PurchaseOrderStatus == "S")
            {
                return RedirectToAction("shipNoticeDisplay", "ShipNotices", new { id });
            }
            return HttpNotFound("Not Found");
        }
        /// <summary>
        /// 此功能為亞辰負責
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //判斷如果是未答交的訂單的方法
        public ActionResult purchaseOrderSended(string id)
        {
            PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound("purchaseOrder Not Found or id is null");
            }
            PurchaseOrder purchaseOrderViewModel = new PurchaseOrder();
            //purchaseOrderViewModel.failMessage = Convert.ToString(TempData["failMessage"]);
            purchaseOrderViewModel.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
            purchaseOrderViewModel.ReceiverName = purchaseOrder.ReceiverName;
            purchaseOrderViewModel.ReceiverTel = purchaseOrder.ReceiverTel;
            purchaseOrderViewModel.ReceiverMobile = purchaseOrder.ReceiverMobile;
            purchaseOrderViewModel.ReceiptAddress = purchaseOrder.ReceiptAddress;
            return View(purchaseOrderViewModel);
        }
        //======================================================================================
        /// <summary>
        ///  出貨確認********************更改寫法這裡之後可能不會用，因為出貨要改為個別出貨
        /// </summary>
        /// <param name="shipNotice"></param>
        /// <returns></returns>
        //出貨確認Controller，要修改採購單狀態、以及貨源清單庫存數量
        public ActionResult shipCheck(string id)
        {
            PurchaseOrder po = db.PurchaseOrder.Find(id);
            var query = from nn in db.PurchaseOrderDtl where nn.PurchaseOrderID == id select nn;
            int amount = 0;
            foreach (var y in query)
            {
                amount = amount + (int)y.Total;
            }
            ViewBag.amount = amount;
            return View(po);
        }
        //按下確定出貨之後，ajax呼叫此方法修改貨源清單庫存數量及訂單數量並且新增一筆出貨通知
        //出貨通知明細部分還沒有做
        [HttpGet]
        public ActionResult shipChecked(string id)
        {
            string purchaseOrderID = id;
            if (purchaseOrderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //修改貨源清單庫存數量              
            var podquery = from pod in db.PurchaseOrderDtl
                           join sl in db.SourceList on pod.SourceListID equals sl.SourceListID
                           where pod.PurchaseOrderID == purchaseOrderID
                           select new { pod.Qty, sl.UnitsInStock, pod.SourceListID, pod.PurchaseOrderID, pod.PurchaseOrderDtlCode, sl.UnitsOnOrder };

            List<string> sourceListsTemp = new List<string>();
            List<string> purchaseOrderDtlTemp = new List<string>();
            foreach (var x in podquery)
            {
                if (x.UnitsInStock >= x.Qty)
                {
                    //將貨源清單庫存數量以及訂單數量扣除該採購單料件請購數量並存回資料庫
                    sourceListsTemp.Add(x.SourceListID);
                    purchaseOrderDtlTemp.Add(x.PurchaseOrderDtlCode);

                    //SourceList sourceList = db.SourceList.Find(x.SourceListID);
                    //PurchaseOrderDtl purchaseOrderDtl = db.PurchaseOrderDtl.Find(x.PurchaseOrderDtlCode);
                    //sourceList.UnitsInStock = sourceList.UnitsInStock - purchaseOrderDtl.Qty;
                    //sourceList.UnitsOnOrder = sourceList.UnitsOnOrder - purchaseOrderDtl.Qty;
                    //db.Entry(sourceList).State = EntityState.Modified;
                }
                else
                {
                    return Json("<script>Swal.fire('庫存不足')</script>", JsonRequestBehavior.AllowGet);
                }
            }
            for (int i = 0; i < sourceListsTemp.Count(); i++)
            {
                SourceList sourceList = db.SourceList.Find(sourceListsTemp[i]);
                PurchaseOrderDtl purchaseOrderDtl = db.PurchaseOrderDtl.Find(purchaseOrderDtlTemp[i]);
                sourceList.UnitsInStock = sourceList.UnitsInStock - purchaseOrderDtl.Qty;
                sourceList.UnitsOnOrder = sourceList.UnitsOnOrder - purchaseOrderDtl.Qty;
                //如果訂單書量小於零，則讓他為零
                if (sourceList.UnitsOnOrder < 0)
                {
                    sourceList.UnitsOnOrder = 0;
                }
                db.Entry(sourceList).State = EntityState.Modified;
            }
            //修改採購單狀態
            PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(purchaseOrderID);
            purchaseOrder.PurchaseOrderStatus = "S";
            db.Entry(purchaseOrder).State = EntityState.Modified;
            //新增出貨通知
            DateTime now = DateTime.Now;
            ShipNotice shipNotice = new ShipNotice();
            string snId = $"SN-{now:yyyyMMdd}-";
            int count = db.ShipNotice.Where(x => x.ShipNoticeID.StartsWith(snId)).Count();
            count++;
            snId = $"{snId}{count:000}";
            shipNotice.ShipNoticeID = snId;
            shipNotice.PurchaseOrderID = purchaseOrderID;
            shipNotice.ShipDate = now;
            shipNotice.EmployeeID = purchaseOrder.EmployeeID;
            var compCode = db.Employee.Where(x => x.EmployeeID == purchaseOrder.EmployeeID).First();
            shipNotice.CompanyCode = compCode.CompanyCode;
            shipNotice.SupplierAccountID = supplierAccount;
            //檢查出貨通知表裡面是否有該訂單ID，如果有，顯示該筆訂單已出貨
            ShipNotice ship = db.ShipNotice.Where(x => x.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
            if (ship != null)
            {
                return Json("<script>Swal.fire('此筆訂單已出貨')</script>", JsonRequestBehavior.AllowGet);
            }
            //存進資料庫 
            purchaseOrder.ShipNotice.Add(shipNotice);
            db.SaveChanges();
            return Json("<script>Swal.fire('出貨成功')</script>", JsonRequestBehavior.AllowGet);
        }

        //顯示出貨通知資訊
        public ActionResult shipNoticeDisplay(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder po = db.PurchaseOrder.Find(id);
            if (po == null)
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
            ShipNotice sn = db.ShipNotice.Where(x => x.PurchaseOrderID == id).FirstOrDefault();
            ViewBag.shipNoticeID = sn.ShipNoticeID;
            ViewBag.shipDate = sn.ShipDate;
            return View(po);
        }

        //把訂單狀態換成文字敘述的方法
        private string GetStatus(string purchaseOrderStatus)
        {
            //N = 新增,P = 送出,C = 異動中,E = 答交,D = 整筆訂單取消,S = 出貨,R = 點交,O = 逾期,Z = 結案
            switch (purchaseOrderStatus)
            {
                case "N":
                    return "新增";
                case "P":
                    return "送出";
                case "C":
                    return "異動中";
                case "E":
                    return "答交";
                case "D":
                    return "取消";
                case "S":
                    return "出貨";
                case "R":
                    return "點交";
                case "O":
                    return "逾期";
                case "Z":
                    return "結案";
                default:
                    return "";
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
