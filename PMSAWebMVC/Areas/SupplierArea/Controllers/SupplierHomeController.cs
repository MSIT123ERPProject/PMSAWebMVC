using Microsoft.SqlServer.Server;
using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.TingHuan;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Areas.SupplierArea.Controllers
{
    public class SupplierHomeController : Controller
    {
        private PMSAEntities db;
        string supplierAccount;
        string supplierCode;
        string POChangedCategoryCodeShipped;
        string RequesterRoleSupplier;
        ShipNoticesUtilities utilities;
        public SupplierHomeController()
        {
            db = new PMSAEntities();
            //supplierCode = "S00001";
            //supplierAccount = "SE00001";
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
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
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
        //pieChart
        public ActionResult GetPartTotalPricePercentage(string dateStart, string dateEnd)
        {
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            DateTime dateStartD = Convert.ToDateTime(dateStart);
            DateTime dateEndD = Convert.ToDateTime(dateEnd);
            if (dateStart == null || dateEnd == null)
            {
                dateStartD = DateTime.Now.AddMonths(-3);
                dateEndD = DateTime.Now;
            }
            //計算計算選取區間已出貨的商品金額，如無選取預設為今日以前三個月
            var qpo = from po in db.PurchaseOrder
                      join pod in db.PurchaseOrderDtl
                      on po.PurchaseOrderID equals pod.PurchaseOrderID
                      where po.CreateDate > dateStartD && po.CreateDate < dateEndD
                                        //&& po.PurchaseOrderStatus == "E" || po.PurchaseOrderStatus == "S"
                                        && po.SupplierCode == supplierCode
                      select new
                      {
                          pod.PartName,
                          pod.PartNumber,
                          pod.QtyPerUnit,
                          pod.Qty,
                          pod.PurchaseUnitPrice,
                          pod.Discount,
                          pod.SourceListID
                      };
            //計算百分比//搞錯了highChart會自動幫你計算百分比
            List<partTotalPriceViewModel> list = new List<partTotalPriceViewModel>();
            foreach (var data in qpo)
            {
                partTotalPriceViewModel temp = new partTotalPriceViewModel();
                temp.ToalPrice = data.Qty * data.PurchaseUnitPrice * data.QtyPerUnit * (1 - data.Discount);
                temp.PartNumber = data.PartNumber;
                temp.PartName = data.PartName;
                temp.SourceListID = data.SourceListID;
                list.Add(temp);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //pieChart使用的ViewModel
        public class partTotalPriceViewModel
        {
            public string PartName { get; set; }
            public string PartNumber { get; set; }
            public string SourceListID { get; set; }
            public decimal ToalPrice { get; set; }
            public double Percentage { get; set; }
        }

        //BUBBLECHART
        [HttpGet]
        public ActionResult GetPartQtyByShipStatus()
        {
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            //未出貨料件的出貨數量
            var qpodUnship = from pod in db.PurchaseOrderDtl.AsEnumerable()
                             join po in db.PurchaseOrder
                             on pod.PurchaseOrderID equals po.PurchaseOrderID
                             where po.SupplierCode == supplierCode && pod.ShipDate == null
                             select new OrderPartQty
                             {
                                 name = pod.PartName,
                                 value = pod.Qty,
                             };
            //已出貨料件的出貨數量
            var qpodShipped = from po in db.PurchaseOrder.AsEnumerable()
                              join pod in db.PurchaseOrderDtl
                              on po.PurchaseOrderID equals pod.PurchaseOrderID
                              where po.SupplierCode == supplierCode && pod.ShipDate != null
                              select new OrderPartQty
                              {
                                  name = pod.PartName,
                                  value = pod.Qty,
                              };
            //未答交料件的出貨數量
            var qpodNoApplied = from po in db.PurchaseOrder.AsEnumerable()
                                join pod in db.PurchaseOrderDtl
                                on po.PurchaseOrderID equals pod.PurchaseOrderID
                                where po.SupplierCode == supplierCode && po.PurchaseOrderStatus == "P"
                                select new OrderPartQty
                                {
                                    name = pod.PartName,
                                    value = pod.Qty,
                                };
            IList<OrderPart> orderParts = new List<OrderPart>();
            orderParts.Add(new OrderPart { name = "未出貨", data = qpodUnship.ToList() });
            orderParts.Add(new OrderPart { name = "已出貨", data = qpodShipped.ToList() });
            orderParts.Add(new OrderPart { name = "未答交", data = qpodNoApplied.ToList() });
            var json = orderParts;
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        //需要兩個ViewModel來幫助生成BubbleChart
        public class OrderPart
        {
            //ShipStatus是用來讓VIEW判斷是否已出貨
            public string name { get; set; }
            public List<OrderPartQty> data { get; set; }
        }
        public class OrderPartQty
        {
            public string name { get; set; }
            public int value { get; set; }
        }
        public ActionResult ShipQtyByStatus()
        {
            //取得供應商帳號資料
            SupplierAccount supplier = User.Identity.GetSupplierAccount();
            supplierAccount = supplier.SupplierAccountID;
            supplierCode = supplier.SupplierCode;
            ////////////////////////////////////////////////////
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(@"SELECT  pod.PartName, pod.PartNumber, pod.Qty, sl.UnitsOnOrder, snd.ShipQty FROM  PurchaseOrderDtl AS pod LEFT OUTER JOIN PurchaseOrder AS po ON pod.PurchaseOrderID = po.PurchaseOrderID LEFT OUTER JOIN SourceList AS sl ON pod.SourceListID = sl.SourceListID LEFT OUTER JOIN ShipNoticeDtl AS snd ON pod.PurchaseOrderDtlCode = snd.PurchaseOrderDtlCode  WHERE po.SupplierCode = @supplier", connection))
                {
                    SqlParameter supParameter = new SqlParameter("@supplier", SqlDbType.VarChar);
                    supParameter.Value = supplierCode;
                    cmd.Parameters.Add(supParameter);
                    SqlDataReader Reader = cmd.ExecuteReader();
                    var qpod = Reader.Cast<IDataRecord>().Select(x => new
                    {
                        PartName = x["PartName"],
                        PartNumber = x["PartNumber"],
                        Qty = x["Qty"],
                        UnitsOnOrder = x["UnitsOnOrder"],
                        ShipQty = x["ShipQty"]
                    }).ToList();
                    return Json(qpod, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}