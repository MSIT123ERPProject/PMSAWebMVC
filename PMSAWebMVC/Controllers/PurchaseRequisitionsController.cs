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
    [Authorize(Roles = "Buyer, Manager, ProductionControl")]
    public class PurchaseRequisitionsController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();



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

        public string GetProcessStatus(string ProcessStatus)
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
        public string GetSignStatus(string SignStatus)
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
            IEnumerable < PurchaseRequisition > purchaseRequisition = null;
            if (user.EmployeeID == "CE00005")
            {
                 purchaseRequisition = db.PurchaseRequisition.Include(p => p.Employee).Include(p => p.Product).Include(p => p.SignFlow);
                
            }
            else
            {
                purchaseRequisition = db.PurchaseRequisition.Include(p => p.Employee).Include(p => p.Product).Include(p => p.SignFlow).Where(p => p.EmployeeID == user.EmployeeID);
            }
            
            foreach (var data in purchaseRequisition)
            {
                data.ProcessStatus = GetProcessStatus(data.ProcessStatus);
                data.SignStatus = GetSignStatus(data.SignStatus);
            }
            ViewBag.userEmployeeID = user.EmployeeID;
            return View(purchaseRequisition.ToList());
        }


        //檢視
        public ActionResult Detail(string id ) 
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

            var datas = from p in db.PurchaseRequisition.AsEnumerable()
                        where p.PurchaseRequisitionID == id
                        select new /*PurchaseRequisitionIndexViewModel*/
                        {
                            PurchaseRequisitionID = p.PurchaseRequisitionID,
                            ProductNumber = p.ProductNumber,
                            EmployeeID = p.EmployeeID,
                            PRBeginDate = p.PRBeginDate.ToString("yyyy/MM/dd"),
                            //ProcessStatus = GetProcessStatus(p.ProcessStatus),
                            //SignStatus = GetSignStatus(p.SignStatus)
                            ProcessStatus = p.ProcessStatus,
                            SignStatus = p.SignStatus,
                            UserEmployeeID = user.EmployeeID
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
        
        //簽核用 取得請購單資料
        [HttpGet]
        public ActionResult GetPurchaseRequisitionsConfirm(string purchaseRequisitionID)
        {
            IEnumerable<PurchaseRequisitionDtlItem> data = null;
            var prConfirm = from pr in db.PurchaseRequisition
                            join prd in db.PurchaseRequisitionDtl
                            on pr.PurchaseRequisitionID equals prd.PurchaseRequisitionID

                            where pr.PurchaseRequisitionID == purchaseRequisitionID
                            select new PurchaseRequisitionDtlItem
                            {
                                PurchaseRequisitionOID = pr.PurchaseRequisitionOID,

                                EmployeeID = pr.EmployeeID,
                                PRBeginDate = pr.PRBeginDate,
                                PartNumber = prd.PartNumber,
                                PartName = prd.Part.PartName,
                                PurchaseRequisitionID = pr.PurchaseRequisitionID,
                                EmployeeName = pr.Employee.Name,
                                ProcessStatus=pr.ProcessStatus,

                               Qty = prd.Qty,
                               SupplierName = prd.SupplierInfo.SupplierName,
                               DateRequired = prd.DateRequired,
                               ProductNumber = pr.ProductNumber,
                               ProductName = pr.Product.ProductName,
                               PurchaseRequisitionDtlOID = prd.PurchaseRequisitionDtlOID
                           };
            data = prConfirm.ToList();

            PurchaseRequisitionConfirmViewModel vm = new PurchaseRequisitionConfirmViewModel
            {
                PurchaseRequisitionDtlSetVM = data, //採購單明細設定模型

            };
            vm.PurchaseRequisitionOID = data.First().PurchaseRequisitionOID;//採購單識別碼
            ViewBag.ProductName = data.First().ProductName; ViewBag.PRBeginDate = data.First().PRBeginDate.ToString("yyyy/MM/dd"); ViewBag.EmployeeName = data.First().EmployeeName;
            ViewBag.PurchaseRequisitionOID = data.First().PurchaseRequisitionOID; ViewBag.ProcessStatus = GetProcessStatus(data.First().ProcessStatus); ViewBag.PurchaseRequisitionID = data.First().PurchaseRequisitionID;

            return PartialView("_Confirm", vm);//注意
        }


        //簽核用 同意
        [HttpGet]
        public ActionResult ConfirmY(string purchaseRequisitionID)
        {
            PurchaseRequisition purchaseRequisition = db.PurchaseRequisition.Find(purchaseRequisitionID);
           purchaseRequisition.SignStatus = "Y";
            if (ModelState.IsValid)//丟資料庫
            {
                db.Entry(purchaseRequisition).State = EntityState.Modified;
                db.SaveChanges();
            }
            PurchaseRequisitionConfirmViewModel model = new PurchaseRequisitionConfirmViewModel();
            ConfirmModel(model);
            return View("Confirm", model);
        }
        //簽核用 拒絕
        [HttpGet]
        public ActionResult ConfirmN(string purchaseRequisitionID)
        {
            PurchaseRequisition purchaseRequisition = db.PurchaseRequisition.Find(purchaseRequisitionID);
            purchaseRequisition.SignStatus = "N";
            if (ModelState.IsValid)//丟資料庫
            {
                db.Entry(purchaseRequisition).State = EntityState.Modified;
                db.SaveChanges();
            }
            PurchaseRequisitionConfirmViewModel model = new PurchaseRequisitionConfirmViewModel();
            ConfirmModel(model);
            return View("Confirm", model);
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
            var d = data.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

     

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
            var x=find4.ToList();
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
            var depr = prtdata2.First().PurchaseRequisitionOID;
            PurchaseRequisitionTemp de = db.PurchaseRequisitionTemp.Find(depr);
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





                /////////////////////////////簽核
                SignFlow signFlow = new SignFlow();
                var d = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                var dd = Convert.ToDateTime(d);

                signFlow.OriginatorID = user.EmployeeID;//放資料
                signFlow.SignBeginDate = dd;
                signFlow.SignEvent = "PR";
                signFlow.SignStatusCode = "S";

                if (ModelState.IsValid)//丟資料庫
                {

                    db.SignFlow.Add(signFlow);
                 
                    db.SaveChanges();
                }
                var sf = db.SignFlow.Where(s => s.OriginatorID == user.EmployeeID && s.SignBeginDate == dd).SingleOrDefault();

                SignFlowDtl signFlowDtl = new SignFlowDtl();
                signFlowDtl.SignFlowOID = sf.SignFlowOID;
                signFlowDtl.ApprovingOfficerID = user.ManagerID;
                signFlowDtl.SignStatusCode = "S";



                /////////////////////////////////

                purchaseRequisition.SignFlowOID = sf.SignFlowOID;
                //請購單取得簽核識別碼



                if (ModelState.IsValid)//丟資料庫
                {
                    db.SignFlowDtl.Add(signFlowDtl);
                    db.PurchaseRequisition.Add(purchaseRequisition);
                    foreach (var p in purchaseRequisitionDtllist)
                    {
                        db.PurchaseRequisitionDtl.Add(p);
                       
                    }
                    db.PurchaseRequisitionTemp.Remove(de);
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
            model.ProductList = new SelectList(Product, "ProductNameDisplay", "ProductNameValue");
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
        private void ConfirmModel(PurchaseRequisitionConfirmViewModel model)
        {
            //參考資料：https://dotnetfiddle.net/PBi075
            IList<PurchaseRequisitionItem> purchaseRequisition = Repository.GetPurchaseRequisitionList();
            model.PurchaseRequisitionList = new SelectList(purchaseRequisition, "PurchaseRequisitionIDDisplay", "PurchaseRequisitionIDValue");
            
        }

        //修改請購單
        [HttpPost]
        //[AuthorizeDeny(Roles = "Manager")]
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

        //請購單新增畫面
        public ActionResult Createtest()
        {
            PurchaseRequisitionCreateViewModel model = new PurchaseRequisitionCreateViewModel();
            model.DateRequired= DateTime.Now;
            ConfigureViewModel(model);
            return View(model);
        }

        //請購單簽核畫面
        public ActionResult Confirm()
        {
            PurchaseRequisitionConfirmViewModel model = new PurchaseRequisitionConfirmViewModel();
            ConfirmModel(model);
            return View(model);
        }

        //請購明細顯示
        public ActionResult IndexDtl(string id)
        {
            var purchaseRequisitionDtl = db.PurchaseRequisitionDtl.Include(p => p.Part).Include(p => p.PurchaseRequisition).Include(p => p.SupplierInfo).Where(p=>p.PurchaseRequisitionID==id);
            return View(purchaseRequisitionDtl.ToList());
        }
        //請購明細檢視
        public ActionResult DetailDtl(string id/*, string partNumber*/)
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
                            SuggestSupplierName=p.SupplierInfo.SupplierName,
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
            var supplierInfo = db.SupplierInfo.Select(c => new
            {
                c.SupplierCode,
                c.SupplierName
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
        ////簽核用頁面切換
        //public ActionResult Sign(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Repository rep = new Repository(User.Identity.GetEmployee(), db);
        //    PurchaseRequisitionSendToSupplierViewModel.SendToSupplierViewModel vm = rep.GetPOSendToSupplierViewModel(id);
        //    if (vm == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(vm);
        //}


        //簽核用 取得請購單資料
        [HttpGet]
        public ActionResult Sign(string purchaseRequisitionID)
        {
            IEnumerable<PurchaseRequisitionDtlItem> data = null;
            var prConfirm = from pr in db.PurchaseRequisition
                            join prd in db.PurchaseRequisitionDtl
                            on pr.PurchaseRequisitionID equals prd.PurchaseRequisitionID
                            join  sf in db.SignFlow
                            on pr.SignFlowOID equals sf.SignFlowOID
                            join sfd in db.SignFlowDtl
                            on sf.SignFlowOID equals sfd.SignFlowOID
                            where pr.PurchaseRequisitionID == purchaseRequisitionID
                            select new PurchaseRequisitionDtlItem
                            {
                                PurchaseRequisitionOID = pr.PurchaseRequisitionOID,

                                EmployeeID = pr.EmployeeID, //請購單
                                PRBeginDate = pr.PRBeginDate,
                                PartNumber = prd.PartNumber,
                                PartName = prd.Part.PartName,
                                PurchaseRequisitionID = pr.PurchaseRequisitionID,
                                EmployeeName = pr.Employee.Name,
                                ProcessStatus = pr.ProcessStatus,
                                SuggestSupplierCode=prd.SuggestSupplierCode,

                                Qty = prd.Qty,
                                SupplierName = prd.SupplierInfo.SupplierName, //明細
                                DateRequired = prd.DateRequired,
                                ProductNumber = pr.ProductNumber,
                                ProductName = pr.Product.ProductName,
                                PurchaseRequisitionDtlOID = prd.PurchaseRequisitionDtlOID,
                                PurchaseRequisitionDtlCode = prd.PurchaseRequisitionDtlCode,


                                SignFlowOID=sf.SignFlowOID,
                                OriginatorID=sf.OriginatorID,
                                SignBeginDate=sf.SignBeginDate,
                                SignEvent=sf.SignEvent,
                                SignStatusCode=sf.SignStatusCode,


                                SignFlowDtlOID=sfd.SignFlowDtlOID,
                                ApprovingOfficerName=sfd.Employee.Name,
                                SignOpinion = sfd.SignOpinion,
                                SignDate = sfd.SignDate,
                                SignPassword=sfd.Employee.PasswordHash


                            };
            data = prConfirm.ToList();

            PurchaseRequisitionConfirmViewModel vm = new PurchaseRequisitionConfirmViewModel
            {
                PurchaseRequisitionDtlSetVM = data, //採購單明細設定模型

            };
            vm.PurchaseRequisitionOID = data.First().PurchaseRequisitionOID;//採購單識別碼
            ViewBag.ProductName = data.First().ProductName; ViewBag.PRBeginDate = data.First().PRBeginDate.ToString("yyyy/MM/dd"); ViewBag.EmployeeName = data.First().EmployeeName;
            ViewBag.PurchaseRequisitionOID = data.First().PurchaseRequisitionOID; ViewBag.ProcessStatus = GetProcessStatus(data.First().ProcessStatus); ViewBag.PurchaseRequisitionID = data.First().PurchaseRequisitionID;
            ViewBag.ApprovingOfficerName = data.First().ApprovingOfficerName; ViewBag.SignPassword = data.First().SignPassword;
            ViewBag.SignFlowOID = data.First().SignFlowOID; ViewBag.SignFlowDtlOID = data.First().SignFlowDtlOID;
            return View(vm);//注意
        }



        [HttpPost]
        //[AuthorizeDeny(Roles = "Manager")]
        public ActionResult Sign1(PurchaseRequisitionDtlItem purchaseRequisitionDtlItem)
        {
            
            PurchaseRequisition purchaseRequisition = db.PurchaseRequisition.Find(purchaseRequisitionDtlItem.PurchaseRequisitionID);
            SignFlow signFlow = db.SignFlow.Find(purchaseRequisitionDtlItem.SignFlowOID);
            SignFlowDtl signFlowDtl = db.SignFlowDtl.Find(purchaseRequisitionDtlItem.SignFlowDtlOID);
            purchaseRequisition.SignStatus = purchaseRequisitionDtlItem.SignStatus;
            signFlow.SignStatusCode= purchaseRequisitionDtlItem.SignStatus;
            signFlowDtl.SignStatusCode = purchaseRequisitionDtlItem.SignStatus;

            string message = "修改成功!!";
            bool status = true;

            if (ModelState.IsValid)
            {
                db.Entry(purchaseRequisition).State = EntityState.Modified;
                db.Entry(signFlow).State = EntityState.Modified;
                db.Entry(signFlowDtl).State = EntityState.Modified;
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
    }
}



