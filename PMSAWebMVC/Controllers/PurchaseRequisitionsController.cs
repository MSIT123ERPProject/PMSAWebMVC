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

     

     

        //// GET: PurchaseRequisitions/Create
        //public ActionResult Create()
        //{
        //    ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name");
        //    ViewBag.ProductNumber = new SelectList(db.Product, "ProductNumber", "ProductName");
        //    ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID");
        //    return View();
        //}

        //// POST: PurchaseRequisitions/Create
        //// 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        //// 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "PurchaseRequisitionOID,PurchaseRequisitionID,ProductNumber,EmployeeID,PRBeginDate,ProcessStatus,SignStatus,SignFlowOID")] PurchaseRequisition purchaseRequisition)
        //{
        //    int z = 1;
        //    string x = "", year, month, day;
        //    year = DateTime.Now.Year.ToString();
        //    month = DateTime.Now.Month.ToString();
        //    day = DateTime.Now.Day.ToString();
        //    x = year + month + day;
        //    string y = "PR-" + x + "-00" + z.ToString();

        //    for (int i = 0; i < db.PurchaseRequisition.Count(); i++)
        //    {
        //        PurchaseRequisition test = db.PurchaseRequisition.Find(y);
        //        if (z < 9)
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = "PR-" + x + "-00" + z.ToString();
        //                test = db.PurchaseRequisition.Find(y);
        //            }
        //        }
        //        else if (z < 99)
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = "PR-" + x + "-0" + z.ToString();
        //                test = db.PurchaseRequisition.Find(y);
        //            }
        //        }
        //        else
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = "PR-" + x + "-" + z.ToString();
        //                test = db.PurchaseRequisition.Find(y);
        //            }
        //        }


        //    }
        //    purchaseRequisition.EmployeeID = "CE00002";
        //    purchaseRequisition.ProcessStatus = "N";
        //    purchaseRequisition.SignStatus = "S";
        //    purchaseRequisition.PurchaseRequisitionID = y;
        //    //PurchaseRequisitionID請購單編號=PR-yyyyMMdd-3碼流水號(PR-20191023-001)
        //    if (ModelState.IsValid)
        //    {
        //        purchaseRequisition.PRBeginDate = DateTime.Now;
        //        db.PurchaseRequisition.Add(purchaseRequisition);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseRequisition.EmployeeID);
        //    ViewBag.ProductNumber = new SelectList(db.Product, "ProductNumber", "ProductName", purchaseRequisition.ProductNumber);
        //    ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseRequisition.SignFlowOID);
        //    return View(purchaseRequisition);
        //}

        


        

        // POST: PurchaseRequisitions/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "PurchaseRequisitionOID,PurchaseRequisitionID,ProductNumber,EmployeeID,PRBeginDate,ProcessStatus,SignStatus,SignFlowOID")] PurchaseRequisition purchaseRequisition)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(purchaseRequisition).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseRequisition.EmployeeID);
        //    ViewBag.ProductNumber = new SelectList(db.Product, "ProductNumber", "ProductName", purchaseRequisition.ProductNumber);
        //    ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseRequisition.SignFlowOID);
        //    return View(purchaseRequisition);
        //}
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
        //        //return RedirectToAction("Index");
        //        return Content("<script> alert('刪除失敗');window.location.href='../Index'</script>");
        //        //return Content("")
        //    }

        //}


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
                    return "請購中";
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

        // GET: PurchaseRequisitions請購頁面
        public ActionResult Index()
        {
            var user = User.Identity.GetEmployee();
            var purchaseRequisition = db.PurchaseRequisition.Include(p => p.Employee).Include(p => p.Product).Include(p => p.SignFlow).Where(p => p.EmployeeID == user.EmployeeID);
            foreach (var data in purchaseRequisition)
            {
                data.ProcessStatus = GetProcessStatus(data.ProcessStatus);
                data.SignStatus = GetSignStatus(data.SignStatus);
            }
            return View(purchaseRequisition.ToList());
        }

        // GET: PurchaseRequisitions/Edit/5 修改
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

        //檢視
        public ActionResult Detail(string id) 
        {
            var user = User.Identity.GetEmployee();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisition purchaseRequisition = db.PurchaseRequisition.Find(id);
            if (purchaseRequisition == null)
            {
                return HttpNotFound();
            }
            // p.PRBeginDate, p.ProcessStatus, p.SignStatus
            //, ProcessStatus= GetProcessStatus(p.ProcessStatus), SignStatus = GetSignStatus(p.SignStatus)
            //var datas = db.PurchaseRequisition.Where(p => (p.EmployeeID == user.EmployeeID && p.PurchaseRequisitionID == id))
            //    .Select(p => new { p.PurchaseRequisitionID, p.ProductNumber, p.EmployeeID, p.PRBeginDate,
            //        p.ProcessStatus,p.SignStatus
            //    });
            var datas = from p in db.PurchaseRequisition.AsEnumerable()
                        where p.EmployeeID == user.EmployeeID && p.PurchaseRequisitionID==id
                        select new /*PurchaseRequisitionIndexViewModel*/
                        {
                            PurchaseRequisitionID=p.PurchaseRequisitionID,
                            ProductNumber=p.ProductNumber,
                            EmployeeID=p.EmployeeID,
                            PRBeginDate=p.PRBeginDate.ToString("yyyy/MM/dd"),
                            //ProcessStatus = GetProcessStatus(p.ProcessStatus),
                            //SignStatus = GetSignStatus(p.SignStatus)
                            ProcessStatus = p.ProcessStatus,
                            SignStatus = p.SignStatus
                        };
            var da=datas.ToList();
            return Json(datas, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public JsonResult GetSupplierList(string id)
        {//data的值=供應商編號 文字=供應商名稱
            var data = Repository.GetSupplierList(id).Select(s => new { Value = s.SupplierCode, Text = s.SupplierName });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        ////取得請購單明細資料集
        //[HttpGet]
        //public ActionResult GetPurchaseRequisitionsDtlList(string Name, string partNumber)
        //{
        //    string PrNuber = "";
        //    var find = from prn in db.Product   //產品名轉產品編號
        //               where prn.ProductName == Name
        //               select prn;
        //    foreach (var item in find)
        //    {
        //        PrNuber = item.ProductNumber;
        //    }
          

        //    Repository rep = new Repository();
        //    //string c = User.Identity.GetRealName();
        //    var data = Repository.GetPurchaseRequisitionDtlList(PrNuber, partNumber);
        //    IList<PurchaseRequisitionDtlItemChecked> resultSet = new List<PurchaseRequisitionDtlItemChecked>();
        //    foreach (var item in data)
        //    {
        //        PurchaseRequisitionDtlItemChecked pod = new PurchaseRequisitionDtlItemChecked
        //        {
        //            ProductNumber = item.ProductNumber,//產品編號
        //            PurchaseRequisitionDtlOID = item.PurchaseRequisitionDtlOID,//請購單識別碼
        //            Checked = false//確認按鈕
        //        };
        //        resultSet.Add(pod);
        //    }
        //    PurchaseRequisitionCreateViewModel vm = new PurchaseRequisitionCreateViewModel
        //    {
        //        PurchaseRequisitionDtlSetVM = data,
        //        CheckedResultSetVM = resultSet
        //    };
        //    vm.PurchaseRequisitionOID = data.First().PurchaseRequisitionOID;
        //    return PartialView("_CreatePRItemPartial", vm);//注意
        //}

        ////新增寫進資料庫
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Createtest([Bind(Include = "PurchaseRequisitionOID,PurchaseRequisitionID,ProductNumber,EmployeeID,PRBeginDate,ProcessStatus,SignStatus,SignFlowOID")] PurchaseRequisition purchaseRequisition)
        //{
        //    int z = 1;
        //    string x = "", year, month, day;
        //    year = DateTime.Now.Year.ToString();
        //    month = DateTime.Now.Month.ToString();
        //    day = DateTime.Now.Day.ToString();
        //    x = year + month + day;
        //    string y = "PR-" + x + "-00" + z.ToString();

        //    for (int i = 0; i < db.PurchaseRequisition.Count(); i++)
        //    {
        //        PurchaseRequisition test = db.PurchaseRequisition.Find(y);
        //        if (z < 9)
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = "PR-" + x + "-00" + z.ToString();
        //                test = db.PurchaseRequisition.Find(y);
        //            }
        //        }
        //        else if (z < 99)
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = "PR-" + x + "-0" + z.ToString();
        //                test = db.PurchaseRequisition.Find(y);
        //            }
        //        }
        //        else
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = "PR-" + x + "-" + z.ToString();
        //                test = db.PurchaseRequisition.Find(y);
        //            }
        //        }


        //    }
            
        //    purchaseRequisition.EmployeeID = "CE00002";
        //    purchaseRequisition.ProcessStatus = "N";
        //    purchaseRequisition.SignStatus = "S";
        //    purchaseRequisition.PurchaseRequisitionID = y;
        //    //PurchaseRequisitionID請購單編號=PR-yyyyMMdd-3碼流水號(PR-20191023-001)
        //    if (ModelState.IsValid)
        //    {
        //        purchaseRequisition.PRBeginDate = DateTime.Now;
        //        db.PurchaseRequisition.Add(purchaseRequisition);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.EmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", purchaseRequisition.EmployeeID);
        //    ViewBag.ProductNumber = new SelectList(db.Product, "ProductNumber", "ProductName", purchaseRequisition.ProductNumber);
        //    ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", purchaseRequisition.SignFlowOID);
        //    return View(purchaseRequisition);
        //}

        //取得請購單明細資料集並且將資料寫入暫存表中
        [HttpGet]
        public ActionResult GetPurchaseRequisitionsDtlTab(System.DateTime dateRequired, int qty,string supplierCode, string productName,string partNumber)
        {
            if (qty == 0||supplierCode==""||productName==""||partNumber=="")
            {
                return Json(new { a = 1 }, JsonRequestBehavior.AllowGet);
            }
            PurchaseRequisitionTemp prt = new PurchaseRequisitionTemp();
            PurchaseRequisitionDtlTemp prdt = new PurchaseRequisitionDtlTemp();


            string SuName = "";
            var find = from su in db.SupplierInfo   //供應商編號轉名稱
                       where su.SupplierCode == supplierCode
                       select su;
            foreach (var item in find)
            {
                SuName = item.SupplierName;
            }

            string PtName = "";
            var find1 = from pr in db.Part   //料件編號轉料件名稱
                       where pr.PartNumber == partNumber
                       select pr;
            foreach (var item in find1)
            {
                PtName = item.PartName;
            }

            string PrNumber = "";
            var find2 = from pr in db.Product   //供應商編號轉名稱
                       where pr.ProductName == productName
                       select pr;
            foreach (var item in find2)
            {
                PrNumber = item.ProductNumber;
            }

            PurchaseRequisitionTemp purchaseRequisitionTemp = new PurchaseRequisitionTemp(); //new請購單暫存表實體
            purchaseRequisitionTemp.ProductNumber = PrNumber;
            var user = User.Identity.GetEmployee();
            purchaseRequisitionTemp.EmployeeID = user.EmployeeID;
            purchaseRequisitionTemp.ProcessStatus = "N";
            purchaseRequisitionTemp.SignStatus = "S";
            //purchaseRequisitionTemp.PurchaseRequisitionID = y;
            var prtdata1 = from prtt1 in db.PurchaseRequisitionTemp   //取得全部暫存表
                          select prtt1;
            var prtdata2 = prtdata1.ToList();
            if (prtdata2.Count() == 0)
            {
                if (ModelState.IsValid)//丟資料庫
                {
                    purchaseRequisitionTemp.PRBeginDate = DateTime.Now;
                    db.PurchaseRequisitionTemp.Add(purchaseRequisitionTemp);

                    db.SaveChanges();
                }
            }
        

            PurchaseRequisitionDtlTemp purchaseRequisitionDtlTemp = new PurchaseRequisitionDtlTemp();//new請購單明細暫存表
            int OID;
            var prtdata = from prtt in db.PurchaseRequisitionTemp   //取得全部暫存表
                          select prtt;
            var find4 = prtdata.Where(f => f.EmployeeID == purchaseRequisitionTemp.EmployeeID);//判斷採購人員
            OID = find4.Max(f=>f.PurchaseRequisitionOID); //取得最新請購單暫存識別碼
            purchaseRequisitionDtlTemp.PurchaseRequisitionOID = OID;
            purchaseRequisitionDtlTemp.PartNumber = partNumber;
            purchaseRequisitionDtlTemp.Qty = qty;
            purchaseRequisitionDtlTemp.SuggestSupplierCode = supplierCode;
            purchaseRequisitionDtlTemp.DateRequired = dateRequired;
            if (ModelState.IsValid)//丟資料庫
            {
                db.PurchaseRequisitionDtlTemp.Add(purchaseRequisitionDtlTemp);
                db.SaveChanges();
            }

            
            var data = Repository.GetPurchaseRequisitionDtlList(purchaseRequisitionTemp.EmployeeID);
            PurchaseRequisitionCreateViewModel vm = new PurchaseRequisitionCreateViewModel
            {
                PurchaseRequisitionDtlSetVM = data, //採購單明細設定模型
               
            };
            vm.PurchaseRequisitionOID = data.First().PurchaseRequisitionOID;//採購單識別碼



            return PartialView("_CreatePRItemPartial",vm);//注意
        }
        [HttpGet] //站存表資料寫入請購單
        public ActionResult CreatePR()
        {
            
            var user = User.Identity.GetEmployee();

            var prtdata = from prt in db.PurchaseRequisitionTemp   //取得暫存表
                          where prt.EmployeeID == user.EmployeeID
                          select prt;
            var prtdata2 = prtdata.ToList();
            if (prtdata2.Count() > 0)
            {
                PurchaseRequisition purchaseRequisition = new PurchaseRequisition();//請購單
                
                List<PurchaseRequisitionDtl> purchaseRequisitionDtllist = new List<PurchaseRequisitionDtl>();//請購單明細陣列

                int z = 1; //建立請購單編號
                string x = "", year, month, day;
                year = DateTime.Now.Year.ToString();
                month = DateTime.Now.Month.ToString();
                day = DateTime.Now.Day.ToString();
                if (DateTime.Now.Day < 10) { day = "0" + day; }
                if (DateTime.Now.Month < 10) { month = "0" + month; }
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
                purchaseRequisition.PurchaseRequisitionID = y;  //寫入請購單
                purchaseRequisition.ProductNumber = prtdata2[0].ProductNumber;
                purchaseRequisition.EmployeeID = prtdata2[0].EmployeeID;
                purchaseRequisition.PRBeginDate = DateTime.Now;
                purchaseRequisition.ProcessStatus = "N";
                purchaseRequisition.SignStatus = "S";


                var prdtdata = from prdt in db.PurchaseRequisitionDtlTemp   //取得請購明細暫存表
                               where prdt.PurchaseRequisitionTemp.EmployeeID== user.EmployeeID
                               select prdt;
                var prdtdata2 = prdtdata.ToList();
                int ii = 1;  
                foreach (var prdt in prdtdata2)//寫入請購單明細
                {
                    PurchaseRequisitionDtl purchaseRequisitionDtl = new PurchaseRequisitionDtl();//請購單明細
                    string iii="";
                    if (ii < 10) {  iii = y + "-" + "00" + ii; }
                    else if (ii < 100) {  iii = y + "-" + "0" + ii; }
                    else  {  iii = y + "-" + ii; }
                    
                    purchaseRequisitionDtl.PurchaseRequisitionDtlCode =iii ;
                    purchaseRequisitionDtl.PurchaseRequisitionID = y;
                    purchaseRequisitionDtl.PartNumber = prdt.PartNumber;
                    purchaseRequisitionDtl.Qty = prdt.Qty;
                    purchaseRequisitionDtl.SuggestSupplierCode = prdt.SuggestSupplierCode;
                    purchaseRequisitionDtl.DateRequired = prdt.DateRequired;
                    purchaseRequisitionDtllist.Add(purchaseRequisitionDtl);
                    db.PurchaseRequisitionDtlTemp.Remove(prdt);
                    ii += 1;
                    
                }
                purchaseRequisitionDtllist.ToList();
                
                if (ModelState.IsValid)//丟資料庫
                {
                   
                    db.PurchaseRequisition.Add(purchaseRequisition);
                    foreach (var p in purchaseRequisitionDtllist)
                    {
                        db.PurchaseRequisitionDtl.Add(p);
                    }
                db.SaveChanges();
                }

                return RedirectToAction("Index");
              
            }
            else
            {
                return Json(new { a = 1 }, JsonRequestBehavior.AllowGet);
            }

            
        }
        public ActionResult load() //請購單暫存明細刪除
        {
            var user = User.Identity.GetEmployee();
            var data = Repository.GetPurchaseRequisitionDtlList(user.EmployeeID);
            PurchaseRequisitionCreateViewModel vm = new PurchaseRequisitionCreateViewModel
            {
                PurchaseRequisitionDtlSetVM = data, //採購單明細設定模型

            };
            if (data.Count()>0)
            {
                vm.PurchaseRequisitionOID = data.First().PurchaseRequisitionOID;//採購單識別碼
                return PartialView("_CreatePRItemPartial", vm);//注意
            }
            return Json(new { a = 1 },JsonRequestBehavior.AllowGet);

        }
        //下拉選單
        public ActionResult getProcessStatus()
        {
            var ProcessStatus = db.PurchaseRequisition.Select(p => new
            {
                
                p.ProcessStatus,
            }).Distinct();
            return Json(ProcessStatus, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getSignStatus()
        {
            var SignStatus = db.PurchaseRequisition.Select(p => new
            {
                p.SignStatus
            }).Distinct();
            return Json(SignStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Deletetest(string id) //請購單暫存明細刪除
        {
            //try
            //{
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PurchaseRequisitionDtlTemp purchaseRequisitionDtlTemp = db.PurchaseRequisitionDtlTemp.Find(int.Parse(id));
                if (purchaseRequisitionDtlTemp == null)
                {
                    return HttpNotFound();
                }
                db.PurchaseRequisitionDtlTemp.Remove(purchaseRequisitionDtlTemp);
                db.SaveChanges();

                return RedirectToAction("Createtest");
            //}
            //catch
            //{
            //    //return RedirectToAction("Index");
            //    return Content("<script> alert('刪除失敗');window.location.href='../Index'</script>");
            //    //return Content("")
            //}

        }
        private void ConfigureViewModel(PurchaseRequisitionCreateViewModel model)
        {
            //參考資料：https://dotnetfiddle.net/PBi075
            IList<ProductItem> Product = Repository.GetProductList();
            model.ProductList = new SelectList(Product, "ProductNameValue", "ProductNameDisplay");
            if (!string.IsNullOrEmpty(model.SelectedProductName))
            {
                IEnumerable<PartItem> parts = Repository.GetPartList(model.SelectedProductName);
                model.PartList = new SelectList(parts, "PartNumber", "PartName");
            }
            else
            {
                model.PartList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        //修改請購單
        [HttpPost]
        [AuthorizeDeny(Roles = "Manager")]
        public ActionResult Edit(PurchaseRequisition purchaseRequisition)
        {
            string message = "修改成功!!";
            bool status = true;

            if (ModelState.IsValid)
            {
                db.Entry(purchaseRequisition).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = status, message = message, id = db.PurchaseRequisition.Max(x => x.PurchaseRequisitionID) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "修改失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Createtest()
        {
            PurchaseRequisitionCreateViewModel model = new PurchaseRequisitionCreateViewModel();
            model.DateRequired= DateTime.Now;
            ConfigureViewModel(model);
            return View(model);
        }
        //請購明細顯示
        public ActionResult IndexDtl(string id)
        {
            var purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Include(p => p.Part).Include(p => p.PurchaseRequisition).Include(p => p.SupplierInfo).Where(p=>p.PurchaseRequisitionID==id);
            return View(purchaseRequisitionDtl.ToList());
        }
        //請購明細檢視
        public ActionResult DetailDtl(string id)
        {
            var user = User.Identity.GetEmployee();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseRequisitionDtl purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Find(id);
            if (purchaseRequisitionDtl == null)
            {
                return HttpNotFound();
            }
            var datas = from p in db.PurchaseRequisitionDtl.AsEnumerable()
                        where p.PurchaseRequisition.EmployeeID == user.EmployeeID && p.PurchaseRequisitionDtlCode == id
                        select new /*PurchaseRequisitionIndexViewModel*/
                        {
                            PurchaseRequisitionDtlCode = p.PurchaseRequisitionDtlCode,
                            PurchaseRequisitionID = p.PurchaseRequisitionID,
                            PartNumber = p.PartNumber,
                            Qty = p.Qty,
                            SuggestSupplierCode=p.SuggestSupplierCode,
                            DateRequired = p.DateRequired.ToString("yyyy/MM/dd"),

                        };
            var da = datas.ToList();
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        //下拉選單
        public ActionResult getsuggestSupplierCode()
        {
            //var supplierInfo = db.SourceList.Where(s=>s.PartNumber== partNumber).Select(c => new
            //{
            //    c.SupplierInfo.SupplierCode,
            //    c.SupplierInfo.SupplierName
            //});
            var supplierInfo = db.SourceList.Select(c => new
            {
                c.SupplierInfo.SupplierCode,
                c.SupplierInfo.SupplierName
            });
            var sup = supplierInfo.ToList();
            return Json(supplierInfo, JsonRequestBehavior.AllowGet);
        }

        //修改請購單明細
        [HttpPost]
        [AuthorizeDeny(Roles = "Manager")]
        public ActionResult EditDtl(PurchaseRequisitionDtl purchaseRequisitionDtl)
        {
            string message = "修改成功!!";
            bool status = true;

            if (ModelState.IsValid)
            {
                db.Entry(purchaseRequisitionDtl).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = status, message = message, id = db.PurchaseRequisitionDtl.Max(x => x.PurchaseRequisitionDtlCode) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "修改失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: PurchaseRequisitionDtls
        //public ActionResult IndexDtl()
        //{
        //    var purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Include(p => p.Part).Include(p => p.PurchaseRequisition).Include(p => p.SupplierInfo);
        //    return View(purchaseRequisitionDtl.ToList());
        //}

        // GET: PurchaseRequisitionDtls/Details/5
        //public ActionResult DetailsDtl(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PurchaseRequisitionDtl purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Find(id);
        //    if (purchaseRequisitionDtl == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(purchaseRequisitionDtl);
        //}

        //// GET: PurchaseRequisitionDtls/Create
        //public ActionResult CreateDtl()
        //{
        //    ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName");
        //    ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "PurchaseRequisitionID");
        //    ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName");
        //    PurchaseRequisitionDtl p = new PurchaseRequisitionDtl();
        //    p.DateRequired = DateTime.Now;
        //    return View(p);
        //}
        //// GET: PurchaseRequisitionDtls/Create
        //public ActionResult CreateDtl2(string id)
        //{
        //    ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName");
        //    ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber");
        //    ///
        //    ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName");
        //    //var q = from x in db.SupplierInfo select x.SupplierCode;
        //    //var q1 = from y in db.PurchaseRequisitionDtl select y.SuggestSupplierCode;
        //    //var q2 = q.Union(q1);
        //    //ViewBag.SuggestSupplierCode = new SelectList(q2);
        //    ///
        //    ViewBag.PurchaseRequisitionIDD = id;
        //    PurchaseRequisitionDtl p = new PurchaseRequisitionDtl();
        //    p.PurchaseRequisitionID = id;
        //    p.DateRequired = DateTime.Now;
        //    return View(p);
        //}
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateDtl([Bind(Include = "PurchaseRequisitionDtlOID,PurchaseRequisitionDtlCode,PurchaseRequisitionID,PartNumber,Qty,SuggestSupplierCode,DateRequired")] PurchaseRequisitionDtl purchaseRequisitionDtl)
        //{
        //    int z = 1;
        //    string y = "", x = purchaseRequisitionDtl.PurchaseRequisitionID;
        //    y = x + "-00" + z.ToString();
        //    purchaseRequisitionDtl.PurchaseRequisitionDtlCode = y;
        //    for (int i = 0; i < db.PurchaseRequisitionDtl.Count(); i++)
        //    {
        //        PurchaseRequisitionDtl test = db.PurchaseRequisitionDtl.Find(y);
        //        if (z < 9)
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = x + "-00" + z.ToString();
        //                test = db.PurchaseRequisitionDtl.Find(y);
        //            }
        //        }
        //        else if (z < 99)
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = x + "-0" + z.ToString();
        //                test = db.PurchaseRequisitionDtl.Find(y);
        //            }
        //        }
        //        else
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = x + "-" + z.ToString();
        //                test = db.PurchaseRequisitionDtl.Find(y);
        //            }
        //        }
        //    }
        //    purchaseRequisitionDtl.PurchaseRequisitionDtlCode = y;
        //    //請購單明細代碼 PR-20191016-001-001
        //    if (ModelState.IsValid)
        //    {
        //        db.PurchaseRequisitionDtl.Add(purchaseRequisitionDtl);
        //        db.SaveChanges();
        //        return RedirectToAction("IndexDtl");
        //    }

        //    ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", purchaseRequisitionDtl.PartNumber);
        //    ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber", purchaseRequisitionDtl.PurchaseRequisitionID);
        //    ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseRequisitionDtl.SuggestSupplierCode);
        //    return View(purchaseRequisitionDtl);
        //}


        //// POST: PurchaseRequisitionDtls/Create
        //// 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        //// 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateDtl2([Bind(Include = "PurchaseRequisitionDtlOID,PurchaseRequisitionDtlCode,PurchaseRequisitionID,PartNumber,Qty,SuggestSupplierCode,DateRequired")] PurchaseRequisitionDtl purchaseRequisitionDtl)
        //{
        //    int z = 1;
        //    string y = "", x = purchaseRequisitionDtl.PurchaseRequisitionID;
        //    y = x + "-00" + z.ToString();
        //    purchaseRequisitionDtl.PurchaseRequisitionDtlCode = y;
        //    for (int i = 0; i < db.PurchaseRequisitionDtl.Count(); i++)
        //    {
        //        PurchaseRequisitionDtl test = db.PurchaseRequisitionDtl.Find(y);
        //        if (z < 9)
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = x + "-00" + z.ToString();
        //                test = db.PurchaseRequisitionDtl.Find(y);
        //            }
        //        }
        //        else if (z < 99)
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = x + "-0" + z.ToString();
        //                test = db.PurchaseRequisitionDtl.Find(y);
        //            }
        //        }
        //        else
        //        {
        //            if (test != null)
        //            {
        //                z += 1;
        //                y = x + "-" + z.ToString();
        //                test = db.PurchaseRequisitionDtl.Find(y);
        //            }
        //        }
        //    }
        //    purchaseRequisitionDtl.PurchaseRequisitionDtlCode = y;
        //    //請購單明細代碼 PR-20191016-001-001
        //    if (ModelState.IsValid)
        //    {
        //        db.PurchaseRequisitionDtl.Add(purchaseRequisitionDtl);
        //        db.SaveChanges();
        //        return RedirectToAction("IndexDtl");
        //    }

        //    ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", purchaseRequisitionDtl.PartNumber);
        //    ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber", purchaseRequisitionDtl.PurchaseRequisitionID);
        //    ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseRequisitionDtl.SuggestSupplierCode);
        //    return View(purchaseRequisitionDtl);
        //}

        //// GET: PurchaseRequisitionDtls/Edit/5
        //public ActionResult EditDtl(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PurchaseRequisitionDtl purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Find(id);
        //    if (purchaseRequisitionDtl == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", purchaseRequisitionDtl.PartNumber);
        //    ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber", purchaseRequisitionDtl.PurchaseRequisitionID);
        //    ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseRequisitionDtl.SuggestSupplierCode);
        //    return View(purchaseRequisitionDtl);
        //}

        //// POST: PurchaseRequisitionDtls/Edit/5
        //// 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        //// 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditDtl([Bind(Include = "PurchaseRequisitionDtlOID,PurchaseRequisitionDtlCode,PurchaseRequisitionID,PartNumber,Qty,SuggestSupplierCode,DateRequired")] PurchaseRequisitionDtl purchaseRequisitionDtl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(purchaseRequisitionDtl).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("IndexDtl");
        //    }
        //    ViewBag.PartNumber = new SelectList(db.Part, "PartNumber", "PartName", purchaseRequisitionDtl.PartNumber);
        //    ViewBag.PurchaseRequisitionID = new SelectList(db.PurchaseRequisition, "PurchaseRequisitionID", "ProductNumber", purchaseRequisitionDtl.PurchaseRequisitionID);
        //    ViewBag.SuggestSupplierCode = new SelectList(db.SupplierInfo, "SupplierCode", "SupplierName", purchaseRequisitionDtl.SuggestSupplierCode);
        //    return View(purchaseRequisitionDtl);
        //}

        //// GET: PurchaseRequisitionDtls/Delete/5
        //[HttpPost]
        //public ActionResult DeleteDtl(string id)
        //{
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        PurchaseRequisitionDtl purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Find(id);
        //        if (purchaseRequisitionDtl == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        db.PurchaseRequisitionDtl.Remove(purchaseRequisitionDtl);
        //        db.SaveChanges();
        //        return RedirectToAction("IndexDtl");
        //    }
        //    catch
        //    {
        //        return Content("<script> alert('刪除失敗');window.location.href='../Index'</script>");
        //    }

        //}


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



