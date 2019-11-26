using PMSAWebMVC.Controllers;
using PMSAWebMVC.Models;
using PMSAWebMVC.ViewModels.PurchaseRequisitions;
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
    public class PurchaseRequisitionsController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: PurchaseRequisitions
        public ActionResult Index()
        {
            var purchaseRequisition = db.PurchaseRequisition.Include(p => p.Employee).Include(p => p.Product).Include(p => p.SignFlow);
            return View(purchaseRequisition.ToList());
        }

        // GET: PurchaseRequisitions/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisition purchaseRequisition = db.PurchaseRequisition.Find(id);
            if (purchaseRequisition == null)
            {
                return HttpNotFound();
            }
            return View(purchaseRequisition);
        }

        // GET: PurchaseRequisitions/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name");
            ViewBag.ProductNumber = new SelectList(db.Product, "ProductNumber", "ProductName");
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID");
            return View();
        }

        // POST: PurchaseRequisitions/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseRequisitionOID,PurchaseRequisitionID,ProductNumber,EmployeeID,PRBeginDate,ProcessStatus,SignStatus,SignFlowOID")] PurchaseRequisition purchaseRequisition)
        {
            int z = 1;
            string x = "", year, month, day;
            year = DateTime.Now.Year.ToString();
            month = DateTime.Now.Month.ToString();
            day = DateTime.Now.Day.ToString();
            x = year + month + day;
            string y = "PR-" + x + "-00" + z.ToString();

            for (int i = 0; i < db.PurchaseRequisition.Count(); i++)
            {
                PurchaseRequisition test = db.PurchaseRequisition.Find(y);
                if (z < 9)
                {
                    if (test != null)
                    {
                        z += 1;
                        y = "PR-" + x + "-00" + z.ToString();
                        test = db.PurchaseRequisition.Find(y);
                    }
                }
                else if (z < 99)
                {
                    if (test != null)
                    {
                        z += 1;
                        y = "PR-" + x + "-0" + z.ToString();
                        test = db.PurchaseRequisition.Find(y);
                    }
                }
                else
                {
                    if (test != null)
                    {
                        z += 1;
                        y = "PR-" + x + "-" + z.ToString();
                        test = db.PurchaseRequisition.Find(y);
                    }
                }


            }
            purchaseRequisition.EmployeeID = "CE00002";
            purchaseRequisition.ProcessStatus = "N";
            purchaseRequisition.SignStatus = "S";
            purchaseRequisition.PurchaseRequisitionID = y;
            //PurchaseRequisitionID請購單編號=PR-yyyyMMdd-3碼流水號(PR-20191023-001)
            if (ModelState.IsValid)
            {
                purchaseRequisition.PRBeginDate = DateTime.Now;
                db.PurchaseRequisition.Add(purchaseRequisition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseRequisition.EmployeeID);
            ViewBag.ProductNumber = new SelectList(db.Product, "ProductNumber", "ProductName", purchaseRequisition.ProductNumber);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseRequisition.SignFlowOID);
            return View(purchaseRequisition);
        }

        // GET: PurchaseRequisitions/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisition purchaseRequisition = db.PurchaseRequisition.Find(id);
            if (purchaseRequisition == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseRequisition.EmployeeID);
            ViewBag.ProductNumber = new SelectList(db.Product, "ProductNumber", "ProductName", purchaseRequisition.ProductNumber);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseRequisition.SignFlowOID);
            return View(purchaseRequisition);
        }

        // POST: PurchaseRequisitions/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseRequisitionOID,PurchaseRequisitionID,ProductNumber,EmployeeID,PRBeginDate,ProcessStatus,SignStatus,SignFlowOID")] PurchaseRequisition purchaseRequisition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseRequisition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseRequisition.EmployeeID);
            ViewBag.ProductNumber = new SelectList(db.Product, "ProductNumber", "ProductName", purchaseRequisition.ProductNumber);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseRequisition.SignFlowOID);
            return View(purchaseRequisition);
        }
        [HttpPost]
        public ActionResult Delete(string id) //請購單刪除
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PurchaseRequisition purchaseRequisition = db.PurchaseRequisition.Find(id);
                if (purchaseRequisition == null)
                {
                    return HttpNotFound();
                }
                db.PurchaseRequisition.Remove(purchaseRequisition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                //return RedirectToAction("Index");
                return Content("<script> alert('刪除失敗');window.location.href='../Index'</script>");
                //return Content("")
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

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>

        private string GetProcessStatus(string ProcessStatus)
        {
            //N = 新增,O = 採購中,C = 結案
            switch (ProcessStatus)
            {
                case "N":
                    return "新增";
                case "C":
                    return "結案";
                case "O":
                    return "採購中";
                default:
                    return "";
            }
        }
        private string GetSignStatus(string SignStatus)
        {
            //S = 簽核中,Y = 同意 ,N = 拒絕
            switch (SignStatus)
            {
                case "S":
                    return "簽核中";
                case "Y":
                    return "同意";
                case "N":
                    return "拒絕";
                default:
                    return "";
            }
        }


        //取得請購單資料集
        public JsonResult GetPurchaseRequisitionsList()
        {
            //請購單可能會無關聯
            Employee emp = User.Identity.GetEmployee();
            var povm = from pur in db.PurchaseRequisition.AsEnumerable() //請購單
                       join pr in db.Product //與產品關聯 
                       on pur.ProductNumber equals pr.ProductNumber 
                       join pp in db.ProductPart //與產品料件關聯
                       on pr.ProductNumber equals pp.ProductNumber 
                       join p in db.Part//與料件關聯
                       on pp.PartNumber equals p.PartNumber 
                       into rels//資料放進rels
                       from rel in rels.DefaultIfEmpty()//
                       group new { pur, pr, pp } by new
                       {
                           pur.PurchaseRequisitionID,//請購單編號
                           pr.ProductName,//產品名稱
                           pur.PRBeginDate,//產生日期
                           pur.ProcessStatus,//處理狀態
                           pur.SignStatus//簽核狀態
                       } into gp
                       orderby gp.Key.PurchaseRequisitionID descending
                       select new PurchaseRequisitionIndexViewModel  //用自己做的ViewModel new出來
                       {
                           PurchaseRequisitionID = gp.Key.PurchaseRequisitionID,
                           ProductName = gp.Key.ProductName,
                           PRBeginDate = gp.Key.PRBeginDate,
                           ProcessStatus = GetProcessStatus(gp.Key.ProcessStatus),//翻譯蒟蒻
                           SignStatus = GetSignStatus(gp.Key.SignStatus)//翻譯蒟蒻
                       };
            return Json(new { data = povm }, JsonRequestBehavior.AllowGet); //傳回資料
        }

        //取得料件資料集
        [HttpGet]
        public JsonResult GetPartList(string id)
        {//data的值=供應商編號 文字=供應商名稱
            var data = Repository.GetPartList(id).Select(p => new { Value = p.PartNumber, Text = p.PartName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //取得請購單明細資料集
        [HttpGet]
        public ActionResult GetPurchaseRequisitionsDtlList(string Name, string partNumber)
        {
            string PrNuber = "";
            var find = from prn in db.Product   //產品名轉產品編號
                       where prn.ProductName == Name
                       select prn;
            foreach (var item in find)
            {
                PrNuber = item.ProductNumber;
            }
          

            Repository rep = new Repository();
            //string c = User.Identity.GetRealName();
            var data = Repository.GetPurchaseRequisitionDtlList(PrNuber, partNumber);
            IList<PurchaseRequisitionDtlItemChecked> resultSet = new List<PurchaseRequisitionDtlItemChecked>();
            foreach (var item in data)
            {
                PurchaseRequisitionDtlItemChecked pod = new PurchaseRequisitionDtlItemChecked
                {
                    ProductNumber = item.ProductNumber,//產品編號
                    PurchaseRequisitionDtlOID = item.PurchaseRequisitionDtlOID,//請購單識別碼
                    Checked = false//確認按鈕
                };
                resultSet.Add(pod);
            }
            PurchaseRequisitionCreateViewModel vm = new PurchaseRequisitionCreateViewModel
            {
                PurchaseRequisitionDtlSetVM = data,
                CheckedResultSetVM = resultSet
            };
            vm.PurchaseRequisitionOID = data.First().PurchaseRequisitionOID;
            return PartialView("_CreatePRItemPartial", vm);//注意
        }

        private void ConfigureViewModel(PurchaseRequisitionCreateViewModel model)
        {
            //參考資料：https://dotnetfiddle.net/PBi075
            IList<ProductItem> Product = Repository.GetProductList();
            model.ProductList = new SelectList(Product, "ProductNameValue", "ProductNameDisplay");
            if (!string.IsNullOrEmpty(model.SelectedProductNume))
            {
                IEnumerable<PartItem> parts = Repository.GetPartList(model.SelectedProductNume);
                model.PartList = new SelectList(parts, "PartNumber", "PartName");
            }
            else
            {
                model.PartList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        public ActionResult Createtest()
        {
            PurchaseRequisitionCreateViewModel model = new PurchaseRequisitionCreateViewModel();
            ConfigureViewModel(model);
            return View(model);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: PurchaseRequisitionDtls
        public ActionResult IndexDtl()
        {
            var purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Include(p => p.Part).Include(p => p.PurchaseRequisition).Include(p => p.SupplierInfo);
            return View(purchaseRequisitionDtl.ToList());
        }

        // GET: PurchaseRequisitionDtls/Details/5
        public ActionResult DetailsDtl(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionDtl purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Find(id);
            if (purchaseRequisitionDtl == null)
            {
                return HttpNotFound();
            }
            return View(purchaseRequisitionDtl);
        }

        // GET: PurchaseRequisitionDtls/Create
        public ActionResult CreateDtl()
        {
            ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName");
            ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "PurchaseRequisitionID");
            ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName");
            PurchaseRequisitionDtl p = new PurchaseRequisitionDtl();
            p.DateRequired = DateTime.Now;
            return View(p);
        }
        // GET: PurchaseRequisitionDtls/Create
        public ActionResult CreateDtl2(string id)
        {
            ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName");
            ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber");
            ///
            ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName");
            //var q = from x in db.SupplierInfo select x.SupplierCode;
            //var q1 = from y in db.PurchaseRequisitionDtl select y.SuggestSupplierCode;
            //var q2 = q.Union(q1);
            //ViewBag.SuggestSupplierCode = new SelectList(q2);
            ///
            ViewBag.PurchaseRequisitionIDD = id;
            PurchaseRequisitionDtl p = new PurchaseRequisitionDtl();
            p.PurchaseRequisitionID = id;
            p.DateRequired = DateTime.Now;
            return View(p);
        }
        //public ActionResult CreateDtl2(string id)
        //{
        //    ViewBag.SourceListID = new SelectList(db.SourceList, "SourceListID", "PartNumber");
        //    ViewBag.SourceListIDD = id;

        //    SourceListDtl s = new SourceListDtl();
        //    s.SourceListID = id;
        //    return View(s);
        //}

        // POST: PurchaseRequisitionDtls/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDtl([Bind(Include = "PurchaseRequisitionDtlOID,PurchaseRequisitionDtlCode,PurchaseRequisitionID,PartNumber,Qty,SuggestSupplierCode,DateRequired")] PurchaseRequisitionDtl purchaseRequisitionDtl)
        {
            int z = 1;
            string y = "", x = purchaseRequisitionDtl.PurchaseRequisitionID;
            y = x + "-00" + z.ToString();
            purchaseRequisitionDtl.PurchaseRequisitionDtlCode = y;
            for (int i = 0; i < db.PurchaseRequisitionDtl.Count(); i++)
            {
                PurchaseRequisitionDtl test = db.PurchaseRequisitionDtl.Find(y);
                if (z < 9)
                {
                    if (test != null)
                    {
                        z += 1;
                        y = x + "-00" + z.ToString();
                        test = db.PurchaseRequisitionDtl.Find(y);
                    }
                }
                else if (z < 99)
                {
                    if (test != null)
                    {
                        z += 1;
                        y = x + "-0" + z.ToString();
                        test = db.PurchaseRequisitionDtl.Find(y);
                    }
                }
                else
                {
                    if (test != null)
                    {
                        z += 1;
                        y = x + "-" + z.ToString();
                        test = db.PurchaseRequisitionDtl.Find(y);
                    }
                }
            }
            purchaseRequisitionDtl.PurchaseRequisitionDtlCode = y;
            //請購單明細代碼 PR-20191016-001-001
            if (ModelState.IsValid)
            {
                db.PurchaseRequisitionDtl.Add(purchaseRequisitionDtl);
                db.SaveChanges();
                return RedirectToAction("IndexDtl");
            }

            ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", purchaseRequisitionDtl.PartNumber);
            ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber", purchaseRequisitionDtl.PurchaseRequisitionID);
            ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseRequisitionDtl.SuggestSupplierCode);
            return View(purchaseRequisitionDtl);
        }


        // POST: PurchaseRequisitionDtls/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDtl2([Bind(Include = "PurchaseRequisitionDtlOID,PurchaseRequisitionDtlCode,PurchaseRequisitionID,PartNumber,Qty,SuggestSupplierCode,DateRequired")] PurchaseRequisitionDtl purchaseRequisitionDtl)
        {
            int z = 1;
            string y = "", x = purchaseRequisitionDtl.PurchaseRequisitionID;
            y = x + "-00" + z.ToString();
            purchaseRequisitionDtl.PurchaseRequisitionDtlCode = y;
            for (int i = 0; i < db.PurchaseRequisitionDtl.Count(); i++)
            {
                PurchaseRequisitionDtl test = db.PurchaseRequisitionDtl.Find(y);
                if (z < 9)
                {
                    if (test != null)
                    {
                        z += 1;
                        y = x + "-00" + z.ToString();
                        test = db.PurchaseRequisitionDtl.Find(y);
                    }
                }
                else if (z < 99)
                {
                    if (test != null)
                    {
                        z += 1;
                        y = x + "-0" + z.ToString();
                        test = db.PurchaseRequisitionDtl.Find(y);
                    }
                }
                else
                {
                    if (test != null)
                    {
                        z += 1;
                        y = x + "-" + z.ToString();
                        test = db.PurchaseRequisitionDtl.Find(y);
                    }
                }
            }
            purchaseRequisitionDtl.PurchaseRequisitionDtlCode = y;
            //請購單明細代碼 PR-20191016-001-001
            if (ModelState.IsValid)
            {
                db.PurchaseRequisitionDtl.Add(purchaseRequisitionDtl);
                db.SaveChanges();
                return RedirectToAction("IndexDtl");
            }

            ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", purchaseRequisitionDtl.PartNumber);
            ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber", purchaseRequisitionDtl.PurchaseRequisitionID);
            ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseRequisitionDtl.SuggestSupplierCode);
            return View(purchaseRequisitionDtl);
        }

        // GET: PurchaseRequisitionDtls/Edit/5
        public ActionResult EditDtl(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionDtl purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Find(id);
            if (purchaseRequisitionDtl == null)
            {
                return HttpNotFound();
            }
            ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", purchaseRequisitionDtl.PartNumber);
            ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber", purchaseRequisitionDtl.PurchaseRequisitionID);
            ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseRequisitionDtl.SuggestSupplierCode);
            return View(purchaseRequisitionDtl);
        }

        // POST: PurchaseRequisitionDtls/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDtl([Bind(Include = "PurchaseRequisitionDtlOID,PurchaseRequisitionDtlCode,PurchaseRequisitionID,PartNumber,Qty,SuggestSupplierCode,DateRequired")] PurchaseRequisitionDtl purchaseRequisitionDtl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseRequisitionDtl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexDtl");
            }
            ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", purchaseRequisitionDtl.PartNumber);
            ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber", purchaseRequisitionDtl.PurchaseRequisitionID);
            ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseRequisitionDtl.SuggestSupplierCode);
            return View(purchaseRequisitionDtl);
        }

        // GET: PurchaseRequisitionDtls/Delete/5
        [HttpPost]
        public ActionResult DeleteDtl(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PurchaseRequisitionDtl purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Find(id);
                if (purchaseRequisitionDtl == null)
                {
                    return HttpNotFound();
                }
                db.PurchaseRequisitionDtl.Remove(purchaseRequisitionDtl);
                db.SaveChanges();
                return RedirectToAction("IndexDtl");
            }
            catch
            {
                return Content("<script> alert('刪除失敗');window.location.href='../Index'</script>");
            }

        }


        //[HttpPost]
        //public ActionResult Delete(string id) //請購單刪除
        //{
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        PurchaseRequisition purchaseRequisition = db.PurchaseRequisition.Find(id);
        //        if (purchaseRequisition == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        db.PurchaseRequisition.Remove(purchaseRequisition);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return Content("<script> alert('刪除失敗');window.location.href='../Index'</script>");
        //    }

        //}
    }
}



