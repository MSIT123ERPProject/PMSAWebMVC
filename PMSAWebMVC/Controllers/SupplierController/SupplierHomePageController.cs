using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers.SupplierController
{
    public class SupplierHomePageController : Controller
    {
        PMSAEntities db;
        public SupplierHomePageController()
        {
            db = new PMSAEntities();

        }
        // GET: SupplierHomePage
        //供應商首頁
        public ActionResult Index()
        {
            return View();
        }
        //highChart
        public ActionResult SupplierHomePage()
        {
            var q = from sl in db.SourceList
                    select new
                    {
                        sl.PartNumber,
                        sl.SafetyQty
                    };
            return Json(q,JsonRequestBehavior.AllowGet);
        }
    }
}