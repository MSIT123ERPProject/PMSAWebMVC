using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers.AccSettingController
{
    public class BuyerSettingController : BaseController
    {
        // GET: Buyer
        public ActionResult Index()
        {
            return View();
        }

        // POST: Buyer/Create
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}