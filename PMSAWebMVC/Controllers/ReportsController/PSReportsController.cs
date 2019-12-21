using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers
{
    public class PSReportsController : Controller
    {
        // GET: PSReports
        private PMSAEntities db = new PMSAEntities();
        //採購人員圖表

        //個人每月採購金額
        public ActionResult GetPSMonthSum()
        {
            Employee emp = User.Identity.GetEmployee();
            List<string> empIds = new List<string> { emp.EmployeeID };
            if (emp.Title == "採購主管")
            {
                empIds.AddRange(db.Employee.Where(item => item.ManagerID == emp.EmployeeID)
                    .Select(item => item.EmployeeID).ToList());
            }
            string empId = User.Identity.GetEmployee().EmployeeID;
            var report1 = db.PurchaseOrderDtl.Include("PurchaseOrder").
                          Where(q => "WSRZ".Contains(q.PurchaseOrder.PurchaseOrderStatus) && empIds.Contains(q.PurchaseOrder.EmployeeID)).
                          GroupBy(p => p.PurchaseOrder.CreateDate.Year + "/" + DbFunctions.Right("0" + p.PurchaseOrder.CreateDate.Month, 2)).
                          Select(g => new { name = g.Key, count = g.Sum(q => q.Total) });

            return Json(report1, JsonRequestBehavior.AllowGet);//允許用戶端的HTTP GET請求
        }

        //已採購但未進貨表單
        public JsonResult GetPCE()
        {
            var report = db.PurchaseOrder.
                         GroupBy(p => p.CreateDate.Year + "/" + DbFunctions.Right("0" + p.CreateDate.Month, 2)).
                         Select(g => new
                         {
                             name = g.Key,
                             count = g.Where(w => w.PurchaseOrderStatus == "P").Count(),
                             count1 = g.Where(w => w.PurchaseOrderStatus == "C").Count(),
                             count2 = g.Where(w => "WE".Contains(w.PurchaseOrderStatus)).Count(),
                             count4 = g.Where(w => w.PurchaseOrderStatus == "O").Count()
                         });

            return Json(report, JsonRequestBehavior.AllowGet);
        }

        //已產生採購單但逾期之採購單
        public JsonResult GetO()
        {
            var report = db.PurchaseOrder.
                         GroupBy(p => p.CreateDate.Year + "/" + DbFunctions.Right("0" + p.CreateDate.Month, 2)).
                         Select(g => new
                         {
                             name = g.Key,
                             count = g.Where(w => w.PurchaseOrderStatus == "O").Count()
                         });

            return Json(report, JsonRequestBehavior.AllowGet);
        }

        //幾筆進貨單處於簽核中
        public JsonResult GetSSS()
        {
            var report = db.PurchaseOrderReceiveDtl.Include("PurchaseOrderReceive").
                             Where(w => w.PurchaseOrderReceive.SignStatus == "S").
                             GroupBy(p => p.PurchaseOrderReceive.PurchaseDate.Year + "/" + DbFunctions.Right("0" + p.PurchaseOrderReceive.PurchaseDate.Month, 2)).
                             Select(g => new { name = g.Key, count = g.Count() });

            return Json(report, JsonRequestBehavior.AllowGet);
        }
    }
}