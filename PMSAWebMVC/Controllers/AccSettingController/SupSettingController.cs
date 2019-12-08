using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers.AccSettingController
{
    public class SupSettingController : BaseController
    {
        // GET: SupSetting
        public ActionResult Index()
        {
            return View();
        }

        // POST: SupSetting/Create
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            return RedirectToAction("Index");
        }
    }
}