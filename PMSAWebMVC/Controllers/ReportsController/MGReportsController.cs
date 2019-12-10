using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers
{
    public class MGReportsController : Controller
    {
        // GET: MGReports
        private PMSAEntities db = new PMSAEntities();
        //主管圖表
        public ActionResult MGIndex()
        {
            return View();
        }

        //每月採購金額
        public ActionResult GetMonthSum()
        {
            var report1 = db.PurchaseOrderDtl.Include("PurchaseOrder").
                          Where(q => q.PurchaseOrder.PurchaseOrderStatus == "Z").
                          GroupBy(p => p.PurchaseOrder.CreateDate.Year + "/" + p.PurchaseOrder.CreateDate.Month).
                          Select(g => new { name = g.Key, count = g.Sum(q => q.Total) });

            return Json(report1, JsonRequestBehavior.AllowGet);
        }

        //最近十筆新增的貨源清單
        public ActionResult GetSourceList()
        {
            var report1 = db.SourceListDtl.Include("SourceList").OrderByDescending(q => q.CreateDate).
                         Select(g => new { name = g.SourceList.PartNumber, count = g.QtyDemanded, count1 = g.Discount }).
                         Take(10);

            return Json(report1, JsonRequestBehavior.AllowGet);
        }
    }
}