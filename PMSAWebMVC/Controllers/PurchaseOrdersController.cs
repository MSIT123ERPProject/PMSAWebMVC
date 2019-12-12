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

        public JsonResult GetPurchaseOrderList()
        {
            //請購單可能會無關聯
            Employee emp = User.Identity.GetEmployee();
            var povm = from po in db.PurchaseOrder.AsEnumerable()
                       join si in db.SupplierInfo
                       on new { po.SupplierCode, po.EmployeeID } equals new { si.SupplierCode, emp.EmployeeID }
                       join rel in db.PRPORelation
                       on po.PurchaseOrderID equals rel.PurchaseOrderID into rels
                       from rel in rels.DefaultIfEmpty()
                       group new { po, si, rel } by new
                       {
                           po.PurchaseOrderID,
                           po.CreateDate,
                           si.SupplierName,
                           rel.PurchaseRequisitionID,
                           po.PurchaseOrderStatus
                       } into gp
                       orderby gp.Key.PurchaseOrderID descending
                       select new PurchaseOrderIndexViewModel
                       {
                           PurchaseOrderID = gp.Key.PurchaseOrderID,
                           CreateDate = gp.Key.CreateDate,
                           SupplierName = gp.Key.SupplierName,
                           PurchaseRequisitionID = gp.Key.PurchaseRequisitionID,
                           PurchaseOrderStatus = GetStatus(gp.Key.PurchaseOrderStatus)
                       };
            return Json(new { data = povm }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSupplierList(string id)
        {
            Repository rep = new Repository(User.Identity.GetEmployee());
            var data = rep.GetSupplierList(id).Select(s => new { Value = s.SupplierCode, Text = s.SupplierName });
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
            rep.AddPODtlToTableViewModel();
            return PartialView("_CreatePODItemPartial", session.PODItems);
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
                vm = session.PRDItems;
            }
            //已加入一個採購明細，整筆採購單只能是單一供應商的料件
            if (session.Supplier != null)
            {
                var slq = db.SourceList.Where(sl => sl.SupplierCode == session.Supplier.SupplierCode);
                foreach (var sl in slq)
                {
                    foreach (var vmitem in vm)
                    {
                        if (vmitem.PartNumber != sl.PartNumber)
                        {
                            vm.Remove(vmitem);
                            break;
                        }
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

        private void ConfigureViewModel(PurchaseOrderCreateViewModel model)
        {
            //將先前Session中的內容先清空
            if (this.session.PRDItems.Count() > 0)
            {
                session.ResetAllItems();
            }
            //取得請購單資料
            //參考資料：https://dotnetfiddle.net/PBi075
            Repository rep = new Repository(User.Identity.GetEmployee());
            IList<PurchaseRequisitionItem> purchaseRequisitions = rep.GetPurchaseRequisitionList();
            model.PurchaseRequisitionList = new SelectList(purchaseRequisitions, "PurchaseRequisitionIdValue", "PurchaseRequisitionIdDisplay");
            if (!string.IsNullOrEmpty(model.SelectedPurchaseRequisitionID))
            {
                IEnumerable<SupplierItem> suppliers = rep.GetSupplierList(model.SelectedPurchaseRequisitionID);
                model.SupplierList = new SelectList(suppliers, "SupplierCode", "SupplierName");
            }
            else
            {
                model.SupplierList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

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

        // GET: PurchaseOrders/Details/5
        public ActionResult Details(string id)
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

        // GET: PurchaseOrders/Create
        public ActionResult Create()
        {
            PurchaseOrderCreateViewModel model = new PurchaseOrderCreateViewModel();
            ConfigureViewModel(model);
            return View(model);
        }

        // POST: PurchaseOrders/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseOrderOID,PurchaseRequisitionID,CheckedResultSetVM")] PurchaseOrderCreateViewModel model)
        {
            if (model == null || model.CheckedResultSetVM.Count(s => s.Checked) == 0)
            {
                TempData["ErrorMessage"] = "採購細項請至少勾選一項";
                return RedirectToAction("Create");
            }

            //從暫存新增至正式資料表
            DateTime now = DateTime.Now;
            using (PMSAEntities db = new PMSAEntities())
            {
                //新增採購單
                PurchaseOrderTemp pot = db.PurchaseOrderTemp.Find(model.PurchaseOrderOID);
                db.Entry(pot).State = EntityState.Detached;
                string poId = $"PO-{now:yyyyMMdd}-";
                int count = db.PurchaseOrder.Where(i => i.PurchaseOrderID.StartsWith(poId)).Count();
                count++;
                poId = $"{poId}{count:000}";
                PurchaseOrder po = new PurchaseOrder
                {
                    PurchaseOrderID = poId,
                    SupplierCode = pot.SupplierCode,
                    EmployeeID = pot.EmployeeID,
                    CreateDate = now,
                    PurchaseOrderStatus = "N"
                };
                db.PurchaseOrder.Add(po);
                db.SaveChanges();
                //新增採購單明細
                int index = 0;
                foreach (var item in model.CheckedResultSetVM)
                {
                    if (!item.Checked)
                    {
                        continue;
                    }
                    var podt = db.PurchaseOrderDtlTemp.Find(item.PurchaseOrderDtlOID);
                    index++;
                    PurchaseOrderDtl pod = new PurchaseOrderDtl
                    {
                        PurchaseOrderDtlCode = $"{poId}-{index:000}",
                        PurchaseOrderID = poId,
                        PartNumber = podt.PartNumber,
                        PartName = podt.PartName,
                        PartSpec = podt.PartSpec,
                        QtyPerUnit = podt.QtyPerUnit,
                        TotalPartQty = podt.TotalPartQty,
                        OriginalUnitPrice = podt.OriginalUnitPrice,
                        Discount = podt.Discount,
                        PurchaseUnitPrice = podt.PurchaseUnitPrice,
                        Qty = podt.Qty,
                        PurchasedQty = podt.PurchasedQty,
                        GoodsInTransitQty = podt.GoodsInTransitQty,
                        Total = podt.Total,
                        DateRequired = podt.DateRequired,
                        CommittedArrivalDate = podt.CommittedArrivalDate,
                        ShipDate = podt.ShipDate,
                        ArrivedDate = podt.ArrivedDate,
                        SourceListID = podt.SourceListID
                    };
                    db.PurchaseOrderDtl.Add(pod);
                    db.SaveChanges();
                    //請購單與採購單關聯
                    //TODO: 應從暫存取出，目前暫以傳入方式處理
                    PRPORelation rel = new PRPORelation
                    {
                        PurchaseOrderID = poId,
                        PurchaseOrderDtlCode = pod.PurchaseOrderDtlCode,
                        PurchaseRequisitionID = model.PurchaseRequisitionID,
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
                        RequesterID = pot.EmployeeID
                    };
                    db.POChanged.Add(poc);
                    db.SaveChanges();
                    //更新PurchaseOrderDtl.POChangedOID
                    pod.POChangedOID = poc.POChangedOID;
                    db.Entry(pod).Property(podp => podp.POChangedOID).IsModified = true;
                    db.SaveChanges();
                }
                //刪除暫存資料
                var PRPORelationTemps = db.PRPORelationTemp.Where(i => i.PurchaseOrderOID == model.PurchaseOrderOID);
                db.PRPORelationTemp.RemoveRange(PRPORelationTemps);
                db.SaveChanges();
                var PurchaseOrderDtlTemps = db.PurchaseOrderDtlTemp.Where(i => i.PurchaseOrderOID == model.PurchaseOrderOID);
                db.PurchaseOrderDtlTemp.RemoveRange(PurchaseOrderDtlTemps);
                var PurchaseOrderOld = db.PurchaseOrderTemp.Find(model.PurchaseOrderOID);
                db.PurchaseOrderTemp.Remove(PurchaseOrderOld);
            }

            return RedirectToAction("Index");
        }

        // GET: PurchaseOrders/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrder.EmployeeID);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrder.SignFlowOID);
            ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrder.SupplierCode);
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseOrderOID,PurchaseOrderID,SupplierCode,EmployeeID,ReceiverName,ReceiverTel,ReceiverMobile,ReceiptAddress,CreateDate,PurchaseOrderStatus,SignStatus,SignFlowOID")] PurchaseOrder purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseOrder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseOrder.EmployeeID);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseOrder.SignFlowOID);
            ViewBag.SupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseOrder.SupplierCode);
            return View(purchaseOrder);
        }

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
