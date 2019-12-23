using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Controllers;
using PMSAWebMVC.Models;
using PMSAWebMVC.Services;
using PMSAWebMVC.Utilities.TingHuan;
using PMSAWebMVC.ViewModels.ShipNotices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace PMSAWebMVC.Areas.SupplierArea.Controllers
{
    [Authorize(Roles = "Supplier")]
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
        ShipNoticesUtilities utilities;
        public ShipNoticesController()
        {
            db = new PMSAEntities();
            utilities = new ShipNoticesUtilities();
            //supplierCode = "S00001";
            //supplierAccount = "SE00001";
            POChangedCategoryCodeShipped = "S";
            RequesterRoleSupplier = "S";
        }
        //建構子多載
        public ShipNoticesController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        // 屬性
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        /// 出貨管理首頁//////////////////////////////////////////////////
        public ActionResult Index()
        {
            ////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"];
            }
            else
            {
               // ViewBag.message = "你好";
            }
            return View();
        }
        //==============================
        //第一個下拉式選單的方法
        [HttpGet]
        public ActionResult GetOrderbyStatus(string status)
        {
            ////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            var qpo = from po in db.PurchaseOrder
                      where po.PurchaseOrderStatus == status && po.SupplierCode == supplierCode
                      select new GetOrderbyStatusViewModel
                      {
                          value = po.PurchaseOrderID,
                          text = po.PurchaseOrderID,
                      };
            if ( qpo.Count() ==0 ) {
                return Json(false,JsonRequestBehavior.AllowGet);
            }
            return Json(qpo, JsonRequestBehavior.AllowGet);
        }
        //給第二個下拉式選單使用的資料
        public class GetOrderbyStatusViewModel
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        //==========================================
        //此方法為幫助INDEX的DATATABLE查訂單資料
        public JsonResult GetPurchaseOrderList(string PurchaseOrderStatus)
        {
            ////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            ShipNoticesUtilities utilities = new ShipNoticesUtilities();
            string status = PurchaseOrderStatus;
            var query = (from po in db.PurchaseOrder.AsEnumerable()
                         where (po.PurchaseOrderStatus == status && po.SupplierCode == supplierCode)
                         select new shipOrderViewModel
                         {
                             PurchaseOrderStatus = po.PurchaseOrderStatus,
                             PurchaseOrderID = po.PurchaseOrderID,
                             ReceiverName = po.ReceiverName,
                             ReceiverTel = po.ReceiverTel,
                             ReceiverMobile = po.ReceiverMobile,
                             ReceiptAddress = po.ReceiptAddress,
                         }).ToList();
            for (int i = 0; i < query.Count(); i++)
            {
                query[i].PurchaseOrderStatusDisplay = utilities.GetStatus(query[i].PurchaseOrderStatus);
            }
            var c = query.ToList();
            List<shipOrderViewModel> qlist = query.ToList();
            for (int i = 0; i < qlist.Count(); i++)
            {
                //ShipNoticesUtilities utilities = new ShipNoticesUtilities();
                string d = utilities.GetStatus(qlist[i].PurchaseOrderStatus);
                qlist[i].PurchaseOrderStatusDisplay = d;
            }
            var s = query.ToList();
            return Json(new { data = qlist }, JsonRequestBehavior.AllowGet);
        }
        //檢視未出貨訂單明細，並要可以勾選要出貨的明細，檢視該採購單所有的產品，並可以選擇出貨那些產品
        public async Task<ActionResult> UnshipOrderDtl([Bind(Include = "PurchaseOrderID")]shipOrderViewModel purchaseOrder)
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
        public async Task<ActionResult> shipCheckDtl(shipOrderViewModel unshipOrderDtl)
        {
            string shipnoticesid = "";//為了進貨單而設立的變數
            ////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            string message = "";
            //此LIST要用來存放出貨明細ID 用來寄送電子郵件給公司採購員
            List<string> shipDtlList = new List<string>();
            List<int> shipDtlListQty = new List<int>();
            string shipNoticeID = "";
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
            //檢查是否至少一個被勾選，如沒有則跳回去UnshipOrderDtl頁面
            if (orderDtls.Count() == 0)
            {
                TempData["message"] = "請選擇欲出貨商品!";
                message = "請選擇欲出貨商品!";
                return RedirectToAction("Index", "ShipNotices", new { PurchaseOrderID = unshipOrderDtl.PurchaseOrderID, message = message });
            }
            //檢查出貨
            for (int i = 0; i < unshipOrderDtl.orderDtlItemCheckeds.Count(); i++)
            {
                if (unshipOrderDtl.orderDtlItemCheckeds[i].Qty == 0) {
                    TempData["message"] = "出貨數量不得為零!";
                    message = "出貨數量不得為零!";
                    return RedirectToAction("Index", "ShipNotices", new { PurchaseOrderID = unshipOrderDtl.PurchaseOrderID, message = message });
                };
            }
            DateTime now = DateTime.Now;
            //檢查庫存是否足夠，不足則顯示庫存不足的訊息，足夠則扣掉該或源清單庫存
            //並新增該採購單明細實際出貨日期，新增出貨明細//
            foreach (var dtl in orderDtls)
            {
                SourceList sourceList = db.SourceList.Find(dtl.SourceListID);
                if (dtl.Qty==0 ||sourceList.UnitsInStock < unshipOrderDtl.orderDtlItemCheckeds.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault().Qty)
                {
                    //這裡要return 錯誤訊息，並且回到原頁面
                    TempData["message"] = "出貨數量不得為零!";
                    message = "出貨數量不得為零!";
                    // return Json(new { PurchaseOrderID = unshipOrderDtl.PurchaseOrderID, message = message }, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index", "ShipNotices", new { PurchaseOrderID = unshipOrderDtl.PurchaseOrderID, message = message });
                }
                //扣除該料件貨源清單的庫存以及訂單數量
                //出貨數量要在這裡檢查，先檢查出貨明細裡面的shipQty比對是否小於同一個採購單明細的Qty，
                //是的話，扣除該料件貨源清單的庫存以及訂單數量並且更新shipQty
                if (db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault() != null)
                {
                    ShipNoticeDtl snd = db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault();
                    int orderQty = dtl.Qty;
                    if (orderQty > snd.ShipQty || (unshipOrderDtl.orderDtlItemCheckeds.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault().Qty + snd.ShipQty) < orderQty)
                    {
                        sourceList.UnitsInStock = sourceList.UnitsInStock - unshipOrderDtl.orderDtlItemCheckeds.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault().Qty;
                    }
                }
                else
                {
                    sourceList.UnitsInStock = sourceList.UnitsInStock - unshipOrderDtl.orderDtlItemCheckeds.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault().Qty;
                    if (sourceList.UnitsOnOrder < dtl.Qty)
                    {
                        sourceList.UnitsOnOrder = 0;
                    }
                    else
                    {
                        sourceList.UnitsOnOrder = sourceList.UnitsOnOrder - dtl.Qty;
                    }
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
                    shipnoticesid = snId; //將出貨ID存入變數中
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
                    shipnoticesid = shipNoticeDtl.ShipNoticeID; //將出貨ID存入變數中
                    shipNoticeDtl.PurchaseOrderDtlCode = dtl.PurchaseOrderDtlCode;
                    shipNoticeDtl.ShipQty = unshipOrderDtl.orderDtlItemCheckeds.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault().Qty;
                    //金額為數量*單價*折扣*批量
                    shipNoticeDtl.ShipAmount = Convert.ToInt32(shipNoticeDtl.ShipQty * dtl.PurchaseUnitPrice * (1 - dtl.Discount) * dtl.QtyPerUnit);
                    //把新出貨明細資料加進資料庫
                    db.ShipNoticeDtl.Add(shipNoticeDtl);
                    //存進出貨明細OID給寄送電子郵件用，改成存採購單編號CODE，因為OID會有新增先後順序的問題
                    shipDtlList.Add(dtl.PurchaseOrderDtlCode);
                    shipDtlListQty.Add(unshipOrderDtl.orderDtlItemCheckeds.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault().Qty);
                }
                //有的話，則去修改出貨明細表的出貨數量和出貨金額
                else
                {
                    ShipNoticeDtl snd = db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault();
                    snd.ShipQty += unshipOrderDtl.orderDtlItemCheckeds.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault().Qty;
                    snd.ShipAmount = Convert.ToInt32(snd.ShipQty * dtl.PurchaseUnitPrice * (1 - dtl.Discount) * dtl.QtyPerUnit);
                    db.Entry(snd).State = EntityState.Modified;
                    //存進出貨明細OID給寄送電子郵件用，改成存採購單編號CODE，因為OID會有新增先後順序的問題
                    shipDtlList.Add(dtl.PurchaseOrderDtlCode);
                    shipDtlListQty.Add(unshipOrderDtl.orderDtlItemCheckeds.Where(x => x.PurchaseOrderDtlCode == dtl.PurchaseOrderDtlCode).FirstOrDefault().Qty);
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
                else
                {
                    //如果有出貨過，檢查出貨數量是否跟採購單採購數量一致
                    ShipNoticeDtl snd = db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == pod.PurchaseOrderDtlCode).SingleOrDefault();
                    if (snd != null && pod.Qty > snd.ShipQty)
                    {
                        poCheck = false;
                    }
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
            List<OrderDtlForMail> odm = orderDtlForMails(shipDtlList, shipDtlListQty);
            await SendMailToBuyer(odm);

            //呼叫新增進貨單方法
            PurchaseOrderReceivesController purchaseOrderReceivesController = new PurchaseOrderReceivesController();
            purchaseOrderReceivesController.Create(shipnoticesid);

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
                        pod.PurchasedQty,
                        sl.UnitsInStock
                    };
            IList<OrderDtlItemChecked> odc = new List<OrderDtlItemChecked>();
            foreach (var item in q)
            {
                OrderDtlItemChecked orderDtlItemChecked = new OrderDtlItemChecked();
                orderDtlItemChecked.PurchaseOrderDtlOID = item.PurchaseOrderDtlOID;
                orderDtlItemChecked.PurchaseOrderDtlCode = item.PurchaseOrderDtlCode;
                int shipQty = 0;
                if (db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == item.PurchaseOrderDtlCode).SingleOrDefault() != null)
                {
                    shipQty = db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == item.PurchaseOrderDtlCode).SingleOrDefault().ShipQty;
                }
                orderDtlItemChecked.Qty = item.Qty - shipQty;
                //顯示庫存是否足夠
                if (item.UnitsInStock >= orderDtlItemChecked.Qty)
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
                                     PurchaseQty = pod.Qty,
                                     SourceListID = pod.SourceListID,
                                     CommittedArrivalDate = pod.CommittedArrivalDate,
                                     ShipDate = pod.ShipDate,
                                     DateRequired = pod.DateRequired,
                                     UnitsInStock = sl.UnitsInStock
                                 };
            od = queryOrderitem.ToList();
            //檢查是否有出貨過，有的話要檢查是否出貨明細SHIPQTY是否和採購單QTY是否相同
            //相同的話，代表該商品已全部出貨完畢，Unship = true，否則仍然有需要出貨的數量，Unship = false
            foreach (var orderdtl in od)
            {
                if (orderdtl.ShipDate == null)
                {
                    orderdtl.Unship = true;
                    orderdtl.ShipQty = 0;
                }
                else
                {
                    ShipNoticeDtl snd = db.ShipNoticeDtl.Where(x => x.PurchaseOrderDtlCode == orderdtl.PurchaseOrderDtlCode).SingleOrDefault();
                    if (snd != null && snd.ShipQty < orderdtl.PurchaseQty)
                    {
                        orderdtl.Unship = true;
                        orderdtl.ShipQty = snd.ShipQty;
                    }
                    else
                    {
                        orderdtl.Unship = false;
                        orderdtl.ShipQty = orderdtl.PurchaseQty;
                    }
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
        //用來查詢已出貨商品明細(用採購單編號)，並且回傳一份清單給寄信用的方法用
        //這裡回傳的清單應該要是出貨數量而不是已出貨數量
        public List<OrderDtlForMail> orderDtlForMails(List<string> shipDtlList, List<int> shipDtlListQty)
        {
            List<OrderDtlForMail> odm = new List<OrderDtlForMail>();
            for (int i = 0; i < shipDtlList.Count(); i++)
            {
                string code = shipDtlList[i];
                var q = (from snd in db.ShipNoticeDtl
                         join pod in db.PurchaseOrderDtl
                         on snd.PurchaseOrderDtlCode equals pod.PurchaseOrderDtlCode
                         where snd.PurchaseOrderDtlCode == code
                         select new OrderDtlForMail
                         {
                             ShipNoticeID = snd.ShipNoticeID,
                             supplierCode = supplierCode,
                             PartName = pod.PartName,
                             PartNumber = pod.PartNumber,
                             ShipDate = pod.ShipDate,
                             //ShipQty = snd.ShipQty,
                             ShipNoticeOID = snd.ShipNoticeDtlOID
                         }).SingleOrDefault();
                q.ShipQty = shipDtlListQty[i];
                odm.Add(q);
            }
            return odm;
        }
        //SendEmail
        public async Task SendMailToBuyer(List<OrderDtlForMail> shipNotice)
        {
            ///////////////////////////////////////////////////
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            string supplierAccount = supplier.SupplierAccountID;
            string supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            string supAccID = supplierAccount;
            //string borderColor = "border-color:black";
            //string borderLine = "1";
            //string shipDtlMail = $"<h2>出貨單號:{shipNotice.FirstOrDefault().ShipNoticeID}</h2>";
            //shipDtlMail += $"<table style={borderColor} border={borderLine}><thead><tr><th>出貨商品明細流水號</th><th>料件編號</th><th>料件名稱</th><th>出貨數量</th><th>出貨日期</th></tr><thead><tbody>";
            //foreach (var snd in shipNotice)
            //{
            //    DateTime shipDate = (DateTime)snd.ShipDate;
            //    shipDtlMail += "<tr>";
            //    shipDtlMail += $"<td>{snd.ShipNoticeOID}</td><td>{snd.PartNumber}</td><td>{snd.PartName}</td><td>{snd.ShipQty}</td><td>{shipDate.ToString("yyyy/MM/dd")}</td>";
            //    shipDtlMail += "</tr>";
            //}
            //shipDtlMail += "</tbody></table>";  
            string snId = shipNotice[0].ShipNoticeID;
            PurchaseOrder order = db.PurchaseOrder.Find(db.ShipNotice.Find(snId).PurchaseOrderID);
            string OrderID = order.PurchaseOrderID;
            string OrderApply = "已出貨";
            string borderSize = "1";
            // borderColor = "border:'1'";
            //borderColor += "sans-serif;margin: 20px auto; background-color: #eee; padding-bottom: 20px;";
            string OrderDtl = $"<tr><td colspan='5'><h2>出貨明細</h2></br><h2>出貨單號:{snId}</h2></td></tr>";
            OrderDtl += "<tr><td>出貨明細編號</td><td>料件編號</td><td>料件名稱</td><td>出貨數量</td><td>出貨日期</td></tr>";
            foreach (var snd in shipNotice)
            {
                DateTime shipDate = (DateTime)snd.ShipDate;
                OrderDtl += $"<tr>";
                OrderDtl += $"<td>{snd.ShipNoticeOID}</td><td>{snd.PartNumber}</td><td>{snd.PartName}</td><td>{snd.ShipQty}</td><td>{shipDate.ToString("yyyy/MM/dd")}</td>";
                OrderDtl += "</tr>";
            }
            string SupplierName = db.SupplierInfo.Where(x => x.SupplierCode == supplierCode).SingleOrDefault().SupplierName;
            string BuyerID = db.Employee.Where(x => x.EmployeeID == order.EmployeeID).SingleOrDefault().EmployeeID;

            string emp = db.PurchaseOrder.Find(db.ShipNotice.Find(snId).PurchaseOrderID).EmployeeID;
            string EmployeeName = db.Employee.Find(emp).Name;
            var user = UserManager.Users.Where(x => x.UserName == emp).SingleOrDefault();
            var userId = user.Id;
            string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Areas\SupplierArea\Views\Shared\SendMailToBuyer.html"));
            string MailBody = MembersDBService.getMailBody(tempMail, OrderID, OrderApply, EmployeeName, OrderDtl, SupplierName);
            //寄信
            await UserManager.SendEmailAsync(userId, "商品出貨通知", MailBody);
            //  bool a =    UserManager.SendEmailAsync(userId, "商品出貨通知", shipDtlMail).IsCompleted;
        }

        //按下檢視後進入此方法
        //public ActionResult Edit([Bind(Include = "id")] string id)
        //{
        //    PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(id);
        //    if (purchaseOrder == null)
        //    {
        //        return HttpNotFound("purchaseOrder is null");
        //    }
        //    if (purchaseOrder.PurchaseOrderStatus == "E")
        //    {
        //        return RedirectToAction("shipCheck", "ShipNotices", new { id });
        //    }
        //    else if (purchaseOrder.PurchaseOrderStatus == "S")
        //    {
        //        return RedirectToAction("shipNoticeDisplay", "ShipNotices", new { id });
        //    }
        //    return HttpNotFound("Not Found");
        //}
        /// <summary>
        /// 此功能為亞辰負責
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //判斷如果是未答交的訂單的方法
        //public ActionResult purchaseOrderSended(string id)
        //{
        //    PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(id);
        //    if (purchaseOrder == null)
        //    {
        //        return HttpNotFound("purchaseOrder Not Found or id is null");
        //    }
        //    PurchaseOrder purchaseOrderViewModel = new PurchaseOrder();
        //    //purchaseOrderViewModel.failMessage = Convert.ToString(TempData["failMessage"]);
        //    purchaseOrderViewModel.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
        //    purchaseOrderViewModel.ReceiverName = purchaseOrder.ReceiverName;
        //    purchaseOrderViewModel.ReceiverTel = purchaseOrder.ReceiverTel;
        //    purchaseOrderViewModel.ReceiverMobile = purchaseOrder.ReceiverMobile;
        //    purchaseOrderViewModel.ReceiptAddress = purchaseOrder.ReceiptAddress;
        //    return View(purchaseOrderViewModel);
        //}
        //======================================================================================

        //此方法須改寫
        //顯示出貨通知資訊
        //public ActionResult shipNoticeDisplay(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PurchaseOrder po = db.PurchaseOrder.Find(id);
        //    if (po == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var query = from n in db.PurchaseOrderDtl where n.PurchaseOrderID == id select n;
        //    int amount = 0;
        //    foreach (var x in query)
        //    {
        //        amount = amount + (int)x.Total;
        //    }
        //    ViewBag.amount = amount;
        //    ShipNotice sn = db.ShipNotice.Where(x => x.PurchaseOrderID == id).FirstOrDefault();
        //    ViewBag.shipNoticeID = sn.ShipNoticeID;
        //    ViewBag.shipDate = sn.ShipDate;
        //    return View(po);
        //}
        /// <summary>
        ///  查詢頁面的顯示紙TABLE的用法，目前沒有用到
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
                                                      QtyPerUnit = pod.QtyPerUnit,
                                                      SourceListID = pod.SourceListID,
                                                      CommittedArrivalDate = pod.CommittedArrivalDate,
                                                      ShipDate = pod.ShipDate
                                                  };
            return Json(orderDtls, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// disposing
        /// </summary>
        /// <param name="disposing"></param>
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
