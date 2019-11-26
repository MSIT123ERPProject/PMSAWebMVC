using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.TingHuan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers.SupplierController
{
    public class SupplierHomePageController : Controller
    {
        private PMSAEntities db;
        string supplierAccount;
        string supplierCode;
        string POChangedCategoryCodeShipped;
        string RequesterRoleSupplier;
        ShipNoticesUtilities utilities = new ShipNoticesUtilities();
        public SupplierHomePageController()
        {

            db = new PMSAEntities();
            supplierCode = "S00001";
            supplierAccount = "SE00001";
            POChangedCategoryCodeShipped = "S";
            RequesterRoleSupplier = "S";
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
            return View();
        }
        public ActionResult GetStockData()
        {
            var q = from sl in db.SourceList
                    join pt in db.Part on
                    sl.PartNumber equals pt.PartNumber
                    where sl.SupplierCode == supplierCode /*&& (sl.UnitsInStock <= sl.SafetyQty)*/
                    select new
                    {
                        sl.PartNumber,
                        sl.SafetyQty,
                        sl.UnitsInStock,
                        sl.UnitsOnOrder,
                        pt.PartName
                    };

            var s = q.ToList();
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPartTotalPricePercentage()
        {
            //這裡LINQ語法需要更改一下，同一個料件要加總起來，並必須顯示出所有的料件
            var q = from pod in db.PurchaseOrderDtl
                    join sl in db.SourceList
                    on pod.SourceListID equals sl.SourceListID
                    where  sl.SupplierCode == supplierCode
                    select new
                    {
                        pod.PartName,
                        pod.PartNumber,
                        pod.QtyPerUnit,
                        pod.Qty,
                        sl.UnitPrice,
                        sl.SourceListID
                    };
            List<partTotalPriceViewModel> list = new List<partTotalPriceViewModel>();
            foreach (var data in q)
            {
                partTotalPriceViewModel temp = new partTotalPriceViewModel();
                temp.ToalPrice = data.Qty * data.UnitPrice * data.QtyPerUnit;
                temp.PartNumber = data.PartNumber;
                temp.PartName = data.PartName;
                list.Add(temp);
            }
            double total = 0;
            for (int i = 0; i < list.Count(); i++)
            {
                total += list[i].ToalPrice;
            }
            for (int i = 0; i < list.Count(); i++)
            {
               double p =  list[i].ToalPrice / total*100;
                p = Math.Round(p, 1);
                list[i].Percentage = p;
            }
            return Json(list,JsonRequestBehavior.AllowGet );
        }

        public class partTotalPriceViewModel
        {
            public string PartName { get; set; }
            public string PartNumber { get; set; }
            public int ToalPrice { get; set; }
            public double Percentage { get; set; }
        }
    }
}