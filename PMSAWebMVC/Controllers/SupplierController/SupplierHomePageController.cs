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
                    where sl.SupplierCode == supplierCode
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
                double p = list[i].ToalPrice / total * 100;
                p = Math.Round(p, 1);
                list[i].Percentage = p;
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public class partTotalPriceViewModel
        {
            public string PartName { get; set; }
            public string PartNumber { get; set; }
            public int ToalPrice { get; set; }
            public double Percentage { get; set; }
        }
        //第三張表的方法要寫未出貨的數量以及需求日期
        //已出貨料件的數量以及出貨日期
        //用LINECHART呈現即可
        //或是BUBBLECHART
        [HttpGet]
        public ActionResult GetPartQtyByShipStatus()
        {
            //2019 11/26 22:57 這裡要改寫
            var qpodUnship = from pod in db.PurchaseOrderDtl.AsEnumerable()
                             //join pod in db.PurchaseOrderDtl
                             //on po.PurchaseOrderID equals pod.PurchaseOrderID
                             where /*po.SupplierCode == supplierCode &&*/ pod.ShipDate == null 
                             select new OrderPartQty
                             {
                                 name = pod.PartName,
                                 value = pod.Qty,
                             };
            var qpodShipped = from po in db.PurchaseOrder.AsEnumerable()
                              join pod in db.PurchaseOrderDtl
                              on po.PurchaseOrderID equals pod.PurchaseOrderID
                              where po.SupplierCode == supplierCode && pod.ShipDate != null
                              select new OrderPartQty
                              {
                                  name = pod.PartName,
                                  value= pod.Qty,
                              };
            List<OrderPartQty> orderPartQtyUnship = qpodUnship.ToList();
            List<OrderPartQty> orderPartQtyShipped = qpodShipped.ToList();
            IList<OrderPart> orderParts = new List<OrderPart>();
            orderParts.Add(new OrderPart{ShipStatus="Unship",OrderPartQties=orderPartQtyUnship  });
            orderParts.Add(new OrderPart { ShipStatus = "Shipped", OrderPartQties = orderPartQtyShipped });
            var json = orderParts;
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        //需要兩個ViewModel來幫助生成BubbleChart
        public class OrderPart
        {
            //ShipStatus是用來讓VIEW判斷是否已出貨
            public string ShipStatus { get; set; }
            public List<OrderPartQty> OrderPartQties { get; set; }
        }
        public class OrderPartQty
        {
            public string name { get; set; }
            public int value { get; set; }
            //public DateTime? ShipDate { get; set; }
            //public DateTime? DateRequired { get; set; }
        }
    }
}