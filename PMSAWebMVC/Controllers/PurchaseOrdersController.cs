using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.YaChen;
using PMSAWebMVC.ViewModels;
using PMSAWebMVC.ViewModels.PurchaseOrders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers
{
    public class PurchaseOrdersController : BaseController
    {
        private PMSAEntities db;
        PurchaseOrderCreateSession session;

        public PurchaseOrdersController()
        {
            db = new PMSAEntities();
            db.Database.Log = Console.Write;
            session = PurchaseOrderCreateSession.Current;
        }

        // GET: PurchaseOrders
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 主畫面採購單資料
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPurchaseOrderListViewModel()
        {
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            var vm = rep.GetPurchaseOrderListViewModel();
            return PartialView("_IndexPODItemPartial", vm);
        }

        /// <summary>
        /// 送出至供應商畫面
        /// </summary>
        /// <param name="id">採購單編號 PurchaseOrderID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SendToSupplier(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            POSendToSupplierViewModel.SendToSupplierViewModel vm = rep.GetPOSendToSupplierViewModel(id);
            if (vm == null)
            {
                return HttpNotFound();
            }
            return View(vm);
        }

        /// <summary>
        /// 答交供應商畫面
        /// </summary>
        /// <param name="id">採購單編號 PurchaseOrderID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OrderCommitments(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            POSendToSupplierViewModel.SendToSupplierViewModel vm = rep.GetPOSendToSupplierViewModel(id);
            if (vm == null)
            {
                return HttpNotFound();
            }
            return View(vm);
        }

        /// <summary>
        /// 答交供應商畫面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("OrderCommitments")]
        [ValidateAntiForgeryToken]
        public ActionResult OrderCommitmentsPost([Bind(Include = "POItem")] POSendToSupplierViewModel.SendToSupplierViewModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            rep.UpdatePOStatus(model.POItem.PurchaseOrderID, "W");
            return Json(new { message = "答交成功", status = "success" });
        }

        /// <summary>
        /// 送出至供應商畫面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("SendToSupplier")]
        [ValidateAntiForgeryToken]
        public ActionResult SendToSupplierPost([Bind(Include = "POItem")] POSendToSupplierViewModel.SendToSupplierViewModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            rep.UpdatePOStatus(model.POItem.PurchaseOrderID, "P");
            return Json(new { message = "送出至供應商成功", status = "success" });
        }

        //取得供應商資料集
        [HttpGet]
        public JsonResult GetSupplierList(string id)
        {//data的值=供應商編號 文字=供應商名稱
            Repository rep = new Repository(User.Identity.GetEmployee());
            var data = rep.GetSupplierList(id).Select(s => new { Value = s.SupplierCode, Text = s.SupplierName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //取得倉庫資料集
        [HttpGet]
        public JsonResult GetWarehouseInfoList()
        {
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            var data = db.WarehouseInfo.Select(item => new
            {
                item.WarehouseCode,
                item.Employee.Name,
                item.Address,
                item.Tel
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得請購單基本資料
        /// </summary>
        /// <param name="id">請購單編號</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPRInfoViewModel(string id)
        {
            Repository rep = new Repository(User.Identity.GetEmployee());
            var vm = rep.GetPRInfoViewModel(id);
            return PartialView("_CreatePRInfoPartial", vm);
        }

        /// <summary>
        /// 取得請購明細資料
        /// </summary>
        /// <param name="id">請購單編號</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPRDtlInfoViewModel(string id)
        {
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            var vm = rep.GetPRDtlInfoViewModel(id);
            return PartialView("_CreatePODtlInfoPartial", vm);
        }

        /// <summary>
        /// 取得新增時的採購明細資料
        /// </summary>
        /// <param name="id">請購單編號</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPODtlItemViewModel(string id)
        {
            session.PODItemEditting = null;
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            var data = rep.GetPODtlItemInitViewModel(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得新增時編輯後的採購明細資料
        /// </summary>
        /// <param name="id">請購單編號</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPODtlUpdateItemViewModel(int qty, DateTime dateRequired, string sourceList, string modalPrdCode, string mode)
        {
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            var data = rep.GetPODtlUpdateItemViewModel(qty, dateRequired, sourceList, modalPrdCode, mode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 加入新增的採購明細資料到表格
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddPODtlToTableViewModel()
        {
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            var vm = rep.AddPODtlToTableViewModel();
            //計算金額總計
            ViewBag.Aggregate = vm.Sum(item => item.Total).ToString("C0");
            return PartialView("_CreatePODItemPartial", vm);
        }

        /// <summary>
        /// 刪除採購明細
        /// </summary>
        /// <param name="id">請購單明細編號</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DeletePODtlItem(string id)
        {
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            rep.DeletePODtlItem(id);
            return Json(new { message = "刪除成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得請購明細表
        /// </summary>
        /// <param name="id">請購單編號</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPRDtlTableViewModel(string id)
        {
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            IList<PRDtlTableViewModel> vm = null;
            PurchaseRequisition pr = null;
            if (session.PRDItems == null || session.PRDItems.Count == 0)
            {
                vm = rep.GetPRDtlTableViewModel(id);
                pr = rep.GetPurchaseRequisition(id);
                //存入Session
                session.PRDItems = vm;
                session.PRItems.Add(pr);
            }
            else
            {
                vm = session.PRDItems.ToList();
            }
            //已加入一個採購明細，整筆採購單只能是單一供應商的料件
            if (session.Supplier != null)
            {
                var slq = db.SourceList.Where(sl => sl.SupplierCode == session.Supplier.SupplierCode).ToList();
                bool same = false;
                for (int i = vm.Count() - 1; i >= 0; i--)
                {
                    same = false;
                    foreach (var sl in slq)
                    {
                        if (vm[i].PartNumber == sl.PartNumber)
                        {
                            same = true;
                            break;
                        }
                    }
                    if (same == false)
                    {
                        vm.Remove(vm[i]);
                    }
                }
            }
            //將加入採購單的明細排除
            if (session.PODItems != null && session.PODItems.Count > 0)
            {
                foreach (var pod in session.PODItems)
                {
                    foreach (var vmitem in vm)
                    {
                        if (vmitem.PurchaseRequisitionDtlCode == pod.PurchaseRequisitionDtlCode)
                        {
                            vm.Remove(vmitem);
                            break;
                        }
                    }
                }
            }

            return PartialView("_CreatePRDtlTablePartial", vm);
        }

        /// <summary>
        /// 取得請購明細表關連之貨源清單
        /// </summary>
        /// <param name="id">料件編號</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPOCSourceListViewModel(string id)
        {
            Repository rep = new Repository(User.Identity.GetEmployee());
            var vm = rep.GetPOCSourceListViewModel(id);
            return PartialView("_CreateSLItemPartial", vm);
        }

        /// <summary>
        /// 取得供應商資訊
        /// </summary>
        /// <param name="id">料件編號</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSUPInfoViewModel()
        {
            Repository rep = new Repository(User.Identity.GetEmployee());
            if (session.Supplier == null)
            {
                return new EmptyResult();
            }
            var vm = new SUPInfoViewModel
            {
                SupplierName = session.Supplier.SupplierInfo.SupplierName,
                ContactName = session.Supplier.ContactName,
                Email = session.Supplier.Email,
                Tel = session.Supplier.Tel
            };
            return PartialView("_CreateSUPInfoPartial", vm);
        }

        [HttpGet]
        public ActionResult GetPurchaseOrderDtlList(string id, string supplierCode)
        {
            Repository rep = new Repository(User.Identity.GetEmployee());
            string c = User.Identity.GetRealName();
            var data = rep.GetPurchaseOrderDtlList(id, supplierCode);
            IList<PurchaseOrderDtlItemChecked> resultSet = new List<PurchaseOrderDtlItemChecked>();
            foreach (var item in data)
            {
                PurchaseOrderDtlItemChecked pod = new PurchaseOrderDtlItemChecked
                {
                    PurchaseRequisitionDtlCode = item.PurchaseRequisitionDtlCode,
                    PurchaseOrderDtlOID = item.PurchaseOrderDtlOID,
                    Checked = false
                };
                resultSet.Add(pod);
            }
            PurchaseOrderCreateViewModel vm = new PurchaseOrderCreateViewModel
            {
                PurchaseOrderDtlSetVM = data,
                CheckedResultSetVM = resultSet
            };
            vm.PurchaseOrderOID = data.First().PurchaseOrderOID;
            return PartialView("_CreatePODItemPartial", vm);
        }

        private void ConfigureCreateViewModel(PurchaseOrderCreateViewModel model)
        {
            //將先前Session中的內容先清空
            if (this.session.PRDItems.Count() > 0)
            {
                session.ResetAllItems();
            }
            //取得請購單資料
            //參考資料：https://dotnetfiddle.net/PBi075
            Repository rep = new Repository(User.Identity.GetEmployee());
            IList<ViewModels.PurchaseOrders.SelectListItem> purchaseRequisitions = rep.GetPurchaseRequisitionList();
            model.PurchaseRequisitionList = new SelectList(purchaseRequisitions, "Value", "Display");
            IList<System.Web.Mvc.SelectListItem> warehouseInfos = rep.GetWarehouseInfoList();
            model.WarehouseInfoList = new SelectList(warehouseInfos, "Value", "Text");
        }

        // GET: PurchaseOrders/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Repository rep = new Repository(User.Identity.GetEmployee(), db);
            POSendToSupplierViewModel.SendToSupplierViewModel vm = rep.GetPOSendToSupplierViewModel(id);
            if (vm == null)
            {
                return HttpNotFound();
            }
            return View(vm);
        }

        // GET: PurchaseOrders/Create
        public ActionResult Create()
        {
            PurchaseOrderCreateViewModel model = new PurchaseOrderCreateViewModel();
            ConfigureCreateViewModel(model);
            return View(model);
        }

        // POST: PurchaseOrders/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "POInfoItem")] PurchaseOrderCreateViewModel model)
        {
            StringBuilder sb = new StringBuilder();
            if (session.PODItems.Count == 0)
            {
                sb.Append("採購明細為必填").Append(Environment.NewLine);
            }
            if (string.IsNullOrWhiteSpace(model.POInfoItem.ReceiverName))
            {
                sb.Append("收貨人姓名 為必填").Append(Environment.NewLine);
            }
            if (string.IsNullOrWhiteSpace(model.POInfoItem.ReceiptAddress))
            {
                sb.Append("收貨地址 為必填").Append(Environment.NewLine);
            }
            if (string.IsNullOrWhiteSpace(model.POInfoItem.ReceiverTel) && string.IsNullOrWhiteSpace(model.POInfoItem.ReceiverMobile))
            {
                sb.Append("收貨人市話 及 收貨人手機 需擇一填寫").Append(Environment.NewLine);
            }
            if (sb.Length > 0)
            {
                return Json(new { message = sb.ToString(), status = "warning" });
            }

            //建立設定資料
            IList<PurchaseOrderDtlItem> pods = session.PODItems;
            Employee emp = User.Identity.GetEmployee();
            SupplierAccount sa = session.Supplier;
            PurchaseRequisition pr = session.PRItems.First();
            DateTime now = DateTime.Now;

            //寫入資料庫
            using (PMSAEntities db = new PMSAEntities())
            {
                //新增採購單
                string poId = $"PO-{now:yyyyMMdd}-";
                int count = db.PurchaseOrder.Where(i => i.PurchaseOrderID.StartsWith(poId)).Count();
                count++;
                poId = $"{poId}{count:000}";
                PurchaseOrder po = new PurchaseOrder
                {
                    PurchaseOrderID = poId,
                    SupplierCode = sa.SupplierCode,
                    EmployeeID = emp.EmployeeID,
                    CreateDate = now,
                    PurchaseOrderStatus = "N",
                    ReceiverName = model.POInfoItem.ReceiverName,
                    ReceiptAddress = model.POInfoItem.ReceiptAddress,
                    ReceiverMobile = model.POInfoItem.ReceiverMobile,
                    ReceiverTel = model.POInfoItem.ReceiverTel
                };
                db.PurchaseOrder.Add(po);
                db.SaveChanges();

                //新增採購單明細
                int index = 0;
                foreach (var item in pods)
                {
                    index++;
                    PurchaseOrderDtl pod = new PurchaseOrderDtl
                    {
                        PurchaseOrderDtlCode = $"{poId}-{index:000}",
                        PurchaseOrderID = poId,
                        PartNumber = item.PartNumber,
                        PartName = item.PartName,
                        PartSpec = item.PartSpec,
                        QtyPerUnit = item.QtyPerUnit,
                        TotalPartQty = item.TotalPartQty,
                        TotalSourceListQty = item.TotalSourceListQty,
                        OriginalUnitPrice = item.OriginalUnitPrice,
                        Discount = item.Discount,
                        PurchaseUnitPrice = item.PurchaseUnitPrice,
                        Qty = item.Qty,
                        PurchasedQty = item.PurchasedQty,
                        GoodsInTransitQty = item.GoodsInTransitQty,
                        Total = item.Total,
                        DateRequired = item.DateRequired,
                        CommittedArrivalDate = item.CommittedArrivalDate,
                        ShipDate = item.ShipDate,
                        ArrivedDate = item.ArrivedDate,
                        SourceListID = item.SourceListID,
                        LastModifiedAccountID = emp.EmployeeID
                    };
                    db.PurchaseOrderDtl.Add(pod);
                    db.SaveChanges();

                    //請購單與採購單關聯
                    PRPORelation rel = new PRPORelation
                    {
                        PurchaseOrderID = poId,
                        PurchaseOrderDtlCode = pod.PurchaseOrderDtlCode,
                        PurchaseRequisitionID = pr.PurchaseRequisitionID,
                        PurchaseRequisitionDtlCode = item.PurchaseRequisitionDtlCode,
                    };
                    db.PRPORelation.Add(rel);
                    db.SaveChanges();

                    //採購單異動總表     
                    POChanged poc = new POChanged
                    {
                        PurchaseOrderID = poId,
                        POChangedCategoryCode = "N",
                        RequestDate = now,
                        RequesterRole = "P",
                        RequesterID = emp.EmployeeID,
                        PurchaseOrderDtlCode = pod.PurchaseOrderDtlCode,
                        Qty = pod.Qty,
                        DateRequired = pod.DateRequired
                    };
                    db.POChanged.Add(poc);
                    db.SaveChanges();

                    //更新PurchaseOrderDtl.POChangedOID
                    pod.POChangedOID = poc.POChangedOID;
                    db.Entry(pod).Property(podp => podp.POChangedOID).IsModified = true;
                    db.SaveChanges();
                }
                //清空Session
                session.ResetAllItems();
            }

            return Json(new { message = "新增採購單成功", status = "success" });
        }

        //// GET: PurchaseOrders/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(id);
        //    if (purchaseOrder == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrder.EmployeeID);
        //    ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrder.SignFlowOID);
        //    ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrder.SupplierCode);
        //    return View(purchaseOrder);
        //}

        //// POST: PurchaseOrders/Edit/5
        //// 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        //// 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "PurchaseOrderOID,PurchaseOrderID,SupplierCode,EmployeeID,ReceiverName,ReceiverTel,ReceiverMobile,ReceiptAddress,CreateDate,PurchaseOrderStatus,SignStatus,SignFlowOID")] PurchaseOrder purchaseOrder)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(purchaseOrder).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrder.EmployeeID);
        //    ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrder.SignFlowOID);
        //    ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrder.SupplierCode);
        //    return View(purchaseOrder);
        //}

        // GET: PurchaseOrders/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PurchaseOrder purchaseOrder = db.PurchaseOrder.Find(id);
            db.PurchaseOrder.Remove(purchaseOrder);
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
