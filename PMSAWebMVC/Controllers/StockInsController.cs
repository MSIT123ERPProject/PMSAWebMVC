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
    public class StockInsController : Controller
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: StockIns
        public ActionResult Index()
        {
            var stockIn = db.StockIn.Include(s => s.Employee).Include(s => s.PurchaseOrderReceive).Include(s => s.SignFlow);
            return View(stockIn.ToList());
        }

        //入庫明細
        public ActionResult IndexDtl(string id)
        {
            var stockInDtl = db.StockInDtl.Where(w=>w.StockInID == id).Include(s => s.InventoryDtl).Include(s => s.Part).Include(s => s.StockIn);
            return View(stockInDtl.ToList());
        }

        // GET: StockIns/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockIn stockIn = db.StockIn.Find(id);
            if (stockIn == null)
            {
                return HttpNotFound();
            }
            var da = db.StockIn.Where(w => w.StockInID == id).Select(s => s.SignStatus).ToList();
            string datastust = stut(da[0]);
            var dat = db.StockIn.Where(w => w.StockInID == id).Select(s => s.CreateDate).ToList();
            var date = dat[0].ToShortDateString();

            var datas = db.StockIn.AsEnumerable().Where(w => w.StockInID == id).
                        Select(s => new
                        {
                            s.StockInID,
                            s.PurchaseOrderReceiveID,
                            s.Remark,
                            s.AddStockDate,
                            s.CreateEmployeeID,
                            date,
                            datastust
                        });

            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        public string stut(string ss)
        {
            string sstus1 = "";
            switch (ss)
            {
                case "Y":
                    sstus1 = "同意";
                    break;
                case "N":
                    sstus1 = "拒絕";
                    break;
                case "S":
                    sstus1 = "簽核中";
                    break;
            }
            return sstus1;
        }

        //判斷多少進貨單尚未變成入庫單
        public string date()
        {
            string word = "";
            var q = db.PurchaseOrderReceive.Select(s => s.PurchaseOrderReceiveID);
            var q1 = db.StockIn.Select(s => s.PurchaseOrderReceiveID);
            var data1 = q.Except(q1);
            var data2 = data1.ToList();
            if(data2.Count() == 0)
            {
                return word = "false";
            }
            else
            {
                return word = data2[0];
            }
        }
        // POST: StockIns/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        DateTime now = DateTime.Now;
        public ActionResult Create()
        {
            string id = date();
            string message = "新增成功!!";
            bool status1 = true;
            if (id == "false")
            {
                message = "目前沒有新的進貨單可以入庫了!!";
                status1 = false;
                return Json(new
                {
                    status = status1,
                    message = message
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                StockIn stockIn = new StockIn();
                //string message = "新增成功!!";
                //bool status = true;
                stockIn.PurchaseOrderReceiveID = id;
                string stockinid = "";
                if (stockIn.PurchaseOrderReceiveID != null)
                {
                    int z = 1;
                    string y = $"IN-{now:yyyyMMdd}-00{z.ToString()}";  //前面的值

                    for (int i = 0; i < db.StockIn.Count(); i++)
                    {
                        StockIn test = new StockIn();
                        if (z < 9)
                        {
                            if (test != null)
                            {
                                z += 1;
                                y = $"IN-{now:yyyyMMdd}-00{z.ToString()}";
                                test = db.StockIn.Find(y);
                            }
                        }
                        else if (z < 99)
                        {
                            if (test != null)
                            {
                                z += 1;
                                y = $"IN-{now:yyyyMMdd}-0{z.ToString()}";
                                test = db.StockIn.Find(y);
                            }
                        }
                        else
                        {
                            if (test != null)
                            {
                                z += 1;
                                y = $"IN-{now:yyyyMMdd}-{z.ToString()}";
                                test = db.StockIn.Find(y);
                            }
                        }
                    }
                    stockIn.StockInID = y;  //入庫單號
                    stockinid = stockIn.StockInID;
                    //進貨單號會直接抓回傳過來的值
                    //備註預設空值
                    //加庫存日期可先為空值，加入時再修改日期
                    var use = User.Identity.GetEmployee();
                    stockIn.CreateEmployeeID = use.EmployeeID; //建檔人員直接抓登入者就好
                                                               //建檔日期
                    stockIn.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    //簽核狀態跟簽核流程總表識別碼都是直接抓進貨總表
                    var status = db.PurchaseOrderReceive.Where(w => w.PurchaseOrderReceiveID == stockIn.PurchaseOrderReceiveID).Select(s => s.SignStatus);
                    var atus = status.ToList();
                    stockIn.SignStatus = atus[0];

                    if (ModelState.IsValid)
                    {
                        db.StockIn.Add(stockIn);
                        db.SaveChanges();
                        
                    }
                    //------------------------------------------------------------------------------------------
                    //入庫明細
                    //------------------------------------------------------------------------------------------
                    //在此呼叫stockInDtl的新增方法
                    StockInDtlsController stockInDtls = new StockInDtlsController();
                    stockInDtls.Create(stockinid);
                    
                }
                var oid = db.StockIn.Max(m => m.StockInOID);
                var inid = db.StockIn.Where(w => w.StockInOID == oid).Select(s => s.StockInID).ToList();
                string inid1 = inid[0];
                var orid = db.StockIn.Where(w => w.StockInOID == oid).Select(s => s.PurchaseOrderReceiveID).ToList();
                string orid1 = orid[0];
                var sstus = db.StockIn.Where(w => w.StockInOID == oid).Select(s => s.SignStatus).ToList();
                string sstus1 = stut(sstus[0]);

                var crdate = db.StockIn.Where(w => w.StockInOID == oid).Select(s => s.CreateDate).ToList();
                string crdate1 = crdate[0].ToShortDateString();
                return Json(new
                {
                    status = status1,
                    message = message,
                    id = db.StockIn.Max(x => x.StockInOID),
                    inid1,orid1,sstus1,crdate1
                }, JsonRequestBehavior.AllowGet);
            }
            
        }

        

        // POST: StockIns/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Edit(StockIn stockIn)
        {
            string message = "修改成功!!";
            bool status = true;
            
            switch (stockIn.SignStatus)
            {
                case "同意":
                    stockIn.SignStatus = "Y";
                    break;
                case "拒絕":
                    stockIn.SignStatus = "N";
                    break;
                case "簽核中":
                    stockIn.SignStatus = "S";
                    break;
            }

            if (ModelState.IsValid)
            {
                db.Entry(stockIn).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = status, message = message, id = db.StockIn.Max(x => x.StockInOID) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "修改失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
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
