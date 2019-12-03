using PMSAWebMVC.Controllers;
using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.TingHuan;
using PMSAWebMVC.ViewModels.ShipNotices;
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
        string POChangedCategoryCodeShipped;
        string RequesterRoleSupplier;
        ShipNoticesUtilities utilities = new ShipNoticesUtilities();
        public ShipNoticesController()
        {
         //   SupplierAccount supplier = User.Identity.GetSupplierAccount();
            db = new PMSAEntities();
            supplierCode = "S00001";
           // supplierAccount = supplier.SupplierAccountID;
            supplierAccount = "SE00001";
            POChangedCategoryCodeShipped = "S";
            RequesterRoleSupplier = "S";
        }
        /// 出貨管理首頁//////////////////////////////////////////////////
        public ActionResult Index()
        {
            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"];
            }
            else
            {
                ViewBag.message = "你好";
            }
            return View();
        }
        //此方法為幫助INDEX的DATATABLE查訂單資料
        public JsonResult GetPurchaseOrderList(string PurchaseOrderStatus)
        {
            string status = PurchaseOrderStatus;
            var query = from po in db.PurchaseOrder.AsEnumerable()
                        where (po.PurchaseOrderStatus == status && po.SupplierCode == supplierCode)
                        select new shipOrderViewModel
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
        public ActionResult UnshipOrderDtl([Bind(Include = "PurchaseOrderID")]shipOrderViewModel purchaseOrder)
        {

            var q = from po in db.PurchaseOrder
                    where po.PurchaseOrderID == purchaseOrder.PurchaseOrderID
                    select new shipOrderViewModel
                    {
                        PurchaseOrderID = po.PurchaseOrderID,
                        PurchaseOrderStatus = po.PurchaseOrderStatus
                    };
            var query = q.First();
            return View(query);
        }
        //檢視未出貨訂單明細的datatable的AJAX取資料方法
        public JsonResult GetPurchaseOrderDtl(string purchaseOrderID)
        {
            //這裡面要傳過去的欄位本來還要有其他表的，但是因為join時會出現datareader尚未關閉的錯誤
            //所以先放棄，亞辰說要另外寫一個方法計算總金額，不然會發生衝突
            var q =
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
                };
            //注意dataTable的資料繫結一定要這樣寫，這樣另一邊column的DATA屬性才能繫結的到
            var s = new { data = q };
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ////出貨明細檢視並勾選完畢後進入此方法(出貨按鈕)
        //要修改該採購單明細的實際出貨日期(ShipDate)，並新增資料到出貨明細
        //採購單明細要一一檢查庫存是否足夠，不足則告知是哪筆訂單明細不足，並取消動作回原頁面
        //如果有全部出貨則修改採購單狀態為已出貨，如果沒有?
        //如果只有部分出貨，採購單狀態不修改，但是需要新增一筆異動
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult shipCheckDtl(shipOrderViewModel unshipOrderDtl)
        {
            string message = "";
            //建立一個LIST用來接住所有的OrderDtlItemChecked
            IList<OrderDtlItemChecked> OrderDtlChecked = unshipOrderDtl.orderDtlItemCheckeds;
            //用來存放確定有要出貨的LIST(有勾選)
            List<PurchaseOrderDtl> orderDtls = new List<PurchaseOrderDtl>();
            //檢查是否有勾選出貨，true為有勾，有則放進orderDtls
            foreach (var dtl in OrderDtlChecked)
            {
                if (dtl.Checked)
                {
                    PurchaseOrderDtl purchaseOrderDtl = db.PurchaseOrderDtl.Find(dtl.PurchaseOrderDtlCode);
                    orderDtls.Add(purchaseOrderDtl);
                }
            }
            DateTime now = DateTime.Now;
            List<SourceList> sourceLists = new List<SourceList>(); //這個LIST目前沒有用
            //檢查庫存是否足夠，不足則顯示庫存不足的訊息，足夠則扣掉該或源清單庫存
            //並新增該採購單明細實際出貨日期，新增出貨明細//
            foreach (var dtl in orderDtls)
            {
                SourceList sourceList = db.SourceList.Find(dtl.SourceListID);
                if (sourceList.UnitsInStock < dtl.Qty)
                {
                    //這裡要return 錯誤訊息，並且回到原頁面
                    TempData["message"] = "<script>Swal.fire({  icon: 'error',  title: 'Oops...',  text: '庫存不足!',  footer: '<a href>Why do I have this issue?</a>'})</script>";
                    message = "庫存不足!";
                    // return Json(new { PurchaseOrderID = unshipOrderDtl.PurchaseOrderID, message = message }, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("UnshipOrderDtl", "ShipNotices", new { PurchaseOrderID = unshipOrderDtl.PurchaseOrderID, message = message });
                }
                //扣除該料件貨源清單的庫存以及訂單數量
                //出貨數量要在這裡檢查，先檢查出貨明細裡面的shipQty比對是否小於同一個採購單明細的Qty，
                //是的話，扣除該料件貨源清單的庫存以及訂單數量並且更新shipQty
                if (db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault() != null)
                {
                    ShipNoticeDtl snd = db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault();
                    int orderQty = db.PurchaseOrderDtl.Find(dtl.PurchaseOrderDtlCode).Qty;
                    if (orderQty > snd.ShipQty)
                    {
                        sourceList.UnitsInStock = sourceList.UnitsInStock - dtl.Qty;
                        snd.ShipQty += dtl.Qty;
                        db.Entry(snd).State = EntityState.Modified;
                    }
                }
                else
                {
                    sourceList.UnitsInStock = sourceList.UnitsInStock - dtl.Qty;
                    if (sourceList.UnitsOnOrder < dtl.Qty)
                    {
                        sourceList.UnitsOnOrder = 0;
                    }
                    else
                    {
                        sourceList.UnitsOnOrder = sourceList.UnitsOnOrder - dtl.Qty;
                    }
                    sourceLists.Add(sourceList);
                }
                //新增出貨通知 應該在這 先檢查是否有該筆出貨通知(因為有可能分開出貨，所以同筆訂單後出貨的就不用在增加出貨通知，只要增加出貨明細即可)
                if (db.ShipNotice.Where(x => x.PurchaseOrderID == unshipOrderDtl.PurchaseOrderID).FirstOrDefault() == null)
                {
                    //新增出貨通知//感覺應該要在外面再加一個迴圈做出貨通知以及出貨明細
                    ShipNotice shipNotice = new ShipNotice();
                    string snId = $"SN-{now:yyyyMMdd}-";
                    int count = db.ShipNotice.Where(x => x.ShipNoticeID.StartsWith(snId)).Count();
                    count++;
                    snId = $"{snId}{count:000}";
                    shipNotice.ShipNoticeID = snId;
                    shipNotice.PurchaseOrderID = unshipOrderDtl.PurchaseOrderID;
                    shipNotice.ShipDate = now;
                    shipNotice.EmployeeID = db.PurchaseOrder.Find(unshipOrderDtl.PurchaseOrderID).EmployeeID;
                    shipNotice.CompanyCode = db.Employee.Find(shipNotice.EmployeeID).CompanyCode;
                    shipNotice.SupplierAccountID = supplierAccount;
                    db.ShipNotice.Add(shipNotice);
                    //先把新增的出貨通知資料存進資料庫
                    db.SaveChanges();
                }
                //檢查是否有該出貨明細，沒有則新增出貨明細
                if (db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault() == null)
                {
                    //新增出貨明細 保存期限先不填 
                    ShipNoticeDtl shipNoticeDtl = new ShipNoticeDtl();
                    shipNoticeDtl.ShipNoticeID = db.ShipNotice.Where(x => x.PurchaseOrderID == unshipOrderDtl.PurchaseOrderID).FirstOrDefault().ShipNoticeID;
                    shipNoticeDtl.PurchaseOrderDtlCode = dtl.PurchaseOrderDtlCode;
                    shipNoticeDtl.ShipQty = dtl.Qty;
                    //金額為數量*單價*折扣*批量
                    shipNoticeDtl.ShipAmount = Convert.ToInt32(dtl.Qty * dtl.PurchaseUnitPrice * (1 - dtl.Discount) * dtl.QtyPerUnit);
                    //把新出貨明細資料加進資料庫
                    db.ShipNoticeDtl.Add(shipNoticeDtl);
                }
                //不管是採購單明細或是採購單有異動都要新增採購單異動總表
                //新增採購單異動總表(明細)
                POChanged pOChanged = new POChanged();
                pOChanged.PurchaseOrderID = unshipOrderDtl.PurchaseOrderID;
                pOChanged.POChangedCategoryCode = POChangedCategoryCodeShipped;
                pOChanged.RequestDate = now;
                pOChanged.DateRequired = dtl.DateRequired;
                pOChanged.RequesterRole = RequesterRoleSupplier;
                pOChanged.RequesterID = supplierAccount;
                pOChanged.PurchaseOrderDtlCode = dtl.PurchaseOrderDtlCode;
                pOChanged.Qty = db.PurchaseOrderDtl.Find(dtl.PurchaseOrderDtlCode).Qty;
                db.POChanged.Add(pOChanged);
                db.SaveChanges();
                //新增採購單明細出貨日期欄位以及POchangedOID欄位
                dtl.ShipDate = now;
                //更新採購單明細POChangedOID欄位
                //找出最新一筆採購單異動資料且是供應商的
                dtl.POChangedOID = utilities.FindPOChangedOIDByDtlCode(RequesterRoleSupplier, dtl.PurchaseOrderDtlCode);

                //把資料庫中的每筆訂單明細以及貨源清單資料狀態改為追蹤
                db.Entry(dtl).State = EntityState.Modified;
                db.Entry(sourceList).State = EntityState.Modified;
            }
            //存進資料庫
            db.SaveChanges();
            //檢查該筆訂單所有產品是否都已經出貨，如果是，將該筆採購單狀態改為已出貨"S"
            //預設先當作都已出貨
            bool poCheck = true;
            var q = from pod in db.PurchaseOrderDtl
                    where pod.PurchaseOrderID == unshipOrderDtl.PurchaseOrderID
                    select pod;
            foreach (var pod in q)
            {
                if (pod.ShipDate == null)
                {
                    //找到未出貨產品，代表尚未全部出貨
                    poCheck = false;
                }
            }
            //確認是否已全部出貨，如果是，修改採購單狀態為已出貨(S)並新增一筆採購單異動資料
            //採購單明細的POChangedOID欄位也要更新
            if (poCheck)
            {
                //改採購單狀態
                db.PurchaseOrder.Find(unshipOrderDtl.PurchaseOrderID).PurchaseOrderStatus = POChangedCategoryCodeShipped;
                //新增採購單異動總表
                POChanged pOChanged = new POChanged();
                pOChanged.PurchaseOrderID = unshipOrderDtl.PurchaseOrderID;
                pOChanged.POChangedCategoryCode = POChangedCategoryCodeShipped;
                pOChanged.RequestDate = now;
                pOChanged.RequesterRole = RequesterRoleSupplier;
                pOChanged.RequesterID = supplierAccount;
                db.POChanged.Add(pOChanged);
                db.SaveChanges();
                //然後把找出來的採購單異動總表最新的POChangedOID更新至採購單明細POChangedOID欄位中
                var podQueryForPOChangedOID = from pod in db.PurchaseOrderDtl
                                              where pod.PurchaseOrderID == unshipOrderDtl.PurchaseOrderID
                                              select pod;
                int pOChangedOID = utilities.FindPOChangedOID(RequesterRoleSupplier, unshipOrderDtl.PurchaseOrderID);
                foreach (var pod in podQueryForPOChangedOID)
                {
                    pod.POChangedOID = pOChangedOID;
                    db.Entry(pod).State = EntityState.Modified;
                }
                db.SaveChanges();
                // TempData["message"] = "<script>Swal.fire({position: 'top-end',icon: 'success',title: ' 已全部出貨',showConfirmButton: false,timer: 1500})</script>";
                message = "已全部出貨";
            }
            //成功回原頁面
            //TempData["message"] = "<script>Swal.fire({position: 'top-end',icon: 'success',title: '出貨處理成功，庫存已扣除',showConfirmButton: false,timer: 1500})</script>";
            TempData["message"] = "出貨處理成功，庫存已扣除";
            if (message == "")
            {
                message = "出貨處理成功，庫存已扣除";
            }
            //return Json(new { PurchaseOrderID = unshipOrderDtl.PurchaseOrderID, message = message },JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index", "ShipNotices", new { PurchaseOrderID = unshipOrderDtl.PurchaseOrderID, message = message });
        }
        //出貨按鈕ACTION結束在這
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UnshipOrderDtl的patialView方法
        /// 改用PARTIALVIEW寫寫看
        /// </summary>
        /// <returns></returns>
        //回傳PATIALVIEW給UnShipOrderDtl.cshtml
        //這裡應該要檢查庫存不足，和已出貨的明細，並直接顯示在第一欄
        public ActionResult GetPurchaseOrderDtlPatialView(shipOrderViewModel unshipOrderDtlViewModel)
        {
            var q = from pod in db.PurchaseOrderDtl.AsEnumerable()
                    join sl in db.SourceList on pod.SourceListID equals sl.SourceListID
                    where pod.PurchaseOrderID == unshipOrderDtlViewModel.PurchaseOrderID
                    select new
                    {
                        pod.PurchaseOrderID,
                        pod.PurchaseOrderDtlOID,
                        pod.PurchaseOrderDtlCode,
                        pod.Qty,
                        sl.UnitsInStock
                    };
            IList<OrderDtlItemChecked> odc = new List<OrderDtlItemChecked>();
            foreach (var item in q)
            {
                OrderDtlItemChecked orderDtlItemChecked = new OrderDtlItemChecked();
                orderDtlItemChecked.PurchaseOrderDtlOID = item.PurchaseOrderDtlOID;
                orderDtlItemChecked.PurchaseOrderDtlCode = item.PurchaseOrderDtlCode;
                //顯示庫存是否足夠
                if (item.UnitsInStock >= item.Qty)
                {
                    orderDtlItemChecked.IsEnough = true;
                }
                else
                {
                    orderDtlItemChecked.IsEnough = false;
                }
                //預設為沒有勾選
                orderDtlItemChecked.Checked = false;
                odc.Add(orderDtlItemChecked);
            }

            IEnumerable<OrderDtlItem> od = null;

            var queryOrderitem = from pod in db.PurchaseOrderDtl
                                 join sl in db.SourceList
                                 on pod.SourceListID equals sl.SourceListID
                                 where pod.PurchaseOrderID == unshipOrderDtlViewModel.PurchaseOrderID
                                 select new OrderDtlItem
                                 {
                                     PurchaseOrderDtlOID = pod.PurchaseOrderDtlOID,
                                     PurchaseOrderDtlCode = pod.PurchaseOrderDtlCode,
                                     PartName = pod.PartName,
                                     PartNumber = pod.PartNumber,
                                     QtyPerUnit = pod.QtyPerUnit,
                                     TotalPartQty = pod.TotalPartQty,
                                     Qty = pod.Qty,
                                     PurchaseQty = pod.Qty,
                                     SourceListID = pod.SourceListID,
                                     CommittedArrivalDate = pod.CommittedArrivalDate,
                                     ShipDate = pod.ShipDate,
                                     DateRequired = pod.DateRequired,
                                     UnitsInStock = sl.UnitsInStock
                                 };
            od = queryOrderitem.ToList();
            foreach (var orderdtl in od)
            {
                if (orderdtl.ShipDate == null)
                {
                    orderdtl.Unship = true;
                }
                else
                {
                    orderdtl.Unship = false;
                }
            }
            shipOrderViewModel uodvm = new shipOrderViewModel()
            {
                PurchaseOrderID = unshipOrderDtlViewModel.PurchaseOrderID,
                orderDtlItems = od,
                orderDtlItemCheckeds = odc
            };

            return PartialView("_GetPurchaseOrderDtlPatialView", uodvm);
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

        //此方法須改寫
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
        /// <summary>
        ///  查詢頁面的顯示紙TABLE的用法
        /// </summary>
        /// <param name="disposing"></param>
        [HttpGet]
        public ActionResult ChildTableForOrderDtl(string purchaseOrderID)
        {
            PurchaseOrder po = db.PurchaseOrder.Find(purchaseOrderID);
            IEnumerable<OrderDtlItem> orderDtls = from pod in db.PurchaseOrderDtl
                                                  where pod.PurchaseOrderID == purchaseOrderID
                                                  select new OrderDtlItem
                                                  {
                                                      PurchaseOrderDtlOID = pod.PurchaseOrderDtlOID,
                                                      PurchaseOrderDtlCode = pod.PurchaseOrderDtlCode,
                                                      PartName = pod.PartName,
                                                      Qty = pod.Qty,
                                                      QtyPerUnit = pod.QtyPerUnit,
                                                      SourceListID = pod.SourceListID,
                                                      CommittedArrivalDate = pod.CommittedArrivalDate,
                                                      ShipDate = pod.ShipDate
                                                  };
            return Json(orderDtls, JsonRequestBehavior.AllowGet);
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
