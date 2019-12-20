using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers
{
    public class WSReportsController : Controller
    {
        // GET: WSReports
        private PMSAEntities db = new PMSAEntities();
        //倉管圖表
        public ActionResult Index()
        {
            return View();
        }

        //最新十筆入庫明細
        public JsonResult GetTenSk()
        {
            var report = db.StockInDtl.Where(w=>w.StockIn.SignStatus == "Y").OrderByDescending(o => o.StockIn.CreateDate).
                         Select(g => new { name = g.PartNumber, count = g.StockInQty }).Take(10);

            return Json(report, JsonRequestBehavior.AllowGet);
        }

        //入庫但有缺件
        public JsonResult GetPORD()
        {
            var report = db.PurchaseOrderReceiveDtl.
                         GroupBy(p => p.PurchaseOrderReceive.PurchaseDate.Year + "/" + p.PurchaseOrderReceive.PurchaseDate.Month).
                         Select(g => new { name = g.Key, count = g.Where(p => p.RejectQty != 0).Count() });

            return Json(report, JsonRequestBehavior.AllowGet);
        }

        //查詢即將到期商品
        public JsonResult GetSInDtl()
        {
            var report = db.StockInDtl.Where(w => w.StockIn.SignStatus == "Y").AsEnumerable().
                         Where(w => w.EXP < DateTime.Now.AddMonths(1)).
                         Select(g => new { name = g.PartNumber, value = g.StockInQty });

            return Json(report, JsonRequestBehavior.AllowGet);
        }
    }
}