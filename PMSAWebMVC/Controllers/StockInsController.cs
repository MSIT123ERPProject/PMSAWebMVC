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
            return View(stockIn);
        }

        // POST: StockIns/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        DateTime now = DateTime.Now;
        public void Create(string id)
        {
            StockIn stockIn = new StockIn();
            //string message = "新增成功!!";
            //bool status = true;
            stockIn.PurchaseOrderReceiveID = id;
            string stockinid = "";
            if (stockIn.PurchaseOrderReceiveID != null)
            {
                int z = 1;
                string y = $"IN-{now:yyyyMMdd}-";  //前面的值

                for (int i = 0; i < db.StockIn.Count(); i++)
                {
                    StockIn test = new StockIn();
                    if (z < 9)
                    {
                        if (test != null)
                        {
                            z += 1;
                            y = y + "00" + z.ToString();
                            test = db.StockIn.Find(y);
                        }
                    }
                    else if (z < 99)
                    {
                        if (test != null)
                        {
                            z += 1;
                            y = y + "0" + z.ToString();
                            test = db.StockIn.Find(y);
                        }
                    }
                    else
                    {
                        if (test != null)
                        {
                            z += 1;
                            y = y + z.ToString();
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
        }


        // GET: StockIns/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.CreateEmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", stockIn.CreateEmployeeID);
            ViewBag.PurchaseOrderReceiveID = new SelectList(db.PurchaseOrderReceive, "PurchaseOrderReceiveID", "PurchaseOrderID", stockIn.PurchaseOrderReceiveID);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", stockIn.SignFlowOID);
            return View(stockIn);
        }

        // POST: StockIns/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockInOID,StockInID,PurchaseOrderReceiveID,Remark,AddStockDate,CreateEmployeeID,CreateDate,SignStatus,SignFlowOID")] StockIn stockIn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockIn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreateEmployeeID = new SelectList(db.Employee, "EmployeeID", "Name", stockIn.CreateEmployeeID);
            ViewBag.PurchaseOrderReceiveID = new SelectList(db.PurchaseOrderReceive, "PurchaseOrderReceiveID", "PurchaseOrderID", stockIn.PurchaseOrderReceiveID);
            ViewBag.SignFlowOID = new SelectList(db.SignFlow, "SignFlowOID", "OriginatorID", stockIn.SignFlowOID);
            return View(stockIn);
        }

        // GET: StockIns/Delete/5
        public ActionResult Delete(string id)
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
            return View(stockIn);
        }

        // POST: StockIns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            StockIn stockIn = db.StockIn.Find(id);
            db.StockIn.Remove(stockIn);
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
