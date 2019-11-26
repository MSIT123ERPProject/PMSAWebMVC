using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace PMSAWebMVC.ViewModels.PurchaseRequisitions
{

    public class ProductItem//產品模型
    {
        public string ProductNumberDisplay { get; set; }//產品編號顯示
        public string ProductNumberValue { get; set; }//產品編號值
        public string ProductNameDisplay { get; set; }//產品名稱顯示
        public string ProductNameValue { get; set; }//產品名稱值
    }

    public class PartItem//料件模型
    {
        public string PartNumber { get; set; }//料件編號
        public string PartName { get; set; }//料件名稱
        public string ProductNumber { get; set; }//產品編號
        public string ProductNume { get; set; }//產品編號
    }

    public class PurchaseRequisitionDtlItemChecked//請購單明細確認模型
    {
        public string ProductNumber { get; set; }//產品編號 
        public int PurchaseRequisitionDtlOID { get; set; }//請購單明細識別碼
        [Display(Name = "選取")]
        public bool Checked { get; set; }
    }

    public class PurchaseRequisitionDtlItem//請購單明細模型
    {
        [Display(Name = "選取")]
        public bool Checked { get; set; }
        [Display(Name = "料件編號")]
        public string PartNumber { get; set; }
        [Display(Name = "品名")]
        public string PartName { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        [Display(Name = "單價")]
        public int OriginalUnitPrice { get; set; }
        [Display(Name = "數量")]
        public int Qty { get; set; }
        [Display(Name = "供應商名稱")]
        public string SupplierName { get; set; }
        [DisplayFormat(DataFormatString = "{0:P0}")]
        [Column(TypeName = "decimal(3, 2)")]
        [Display(Name = "折扣")]
        public decimal Discount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        [Display(Name = "金額")]
        public int Total { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "要求到貨日期")]
        public DateTime DateRequired { get; set; }//需求日期
        public string SourceListID { get; set; }//貨源清單編號
        public int PurchaseRequisitionOID { get; set; }//請購單識別碼
        public int QtyPerUnit { get; set; }//單價
        public int TotalPartQty { get; set; }//總訂貨量
        public string ProductNumber { get; set; }//產品料件編號
        public int PurchaseRequisitionDtlOID { get; set; }//請購單明細識別碼
        public int PurchaseUnitPrice { get; set; }//購買單價
    }
    //資料庫相關方法
    public class Repository
    {
        public static Employee GetEmployee()//取得員工
        {
            using (PMSAEntities db = new PMSAEntities())
            {
                return db.Employee.Find("CE00002");//預用CE00002下去找
            }
        }

        //取得產品資料集
        public static IList<ProductItem> GetProductList()
        {

            using (PMSAEntities db = new PMSAEntities())
            {
                var prq = from pr in db.Product  //從產品抓資料
                          select new ProductItem
                          {//產品編號顯示 和 值  產品名稱顯示 和 值
                              ProductNumberDisplay = pr.ProductNumber,
                              ProductNumberValue = pr.ProductNumber,
                              ProductNameDisplay =pr.ProductName,
                              ProductNameValue=pr.ProductName
                          };
                return prq.ToList(); //將資料裝在陣列
            }
        }

        //取得料件資料集
        public static IList<PartItem> GetPartList(string ProductName)
        {
            
            using (PMSAEntities db = new PMSAEntities())
            {
                Product Nuber = new Product();//產品名稱轉產品編號
                var find = from data in db.Product
                           where data.ProductName == ProductName
                           select data;
                foreach (var item in find)
                {
                     Nuber.ProductNumber = item.ProductNumber;
                }

                var supq = from pr in db.Product//產品編號
                           join prp in db.ProductPart//抓料件
                            on new { pr.ProductNumber ,ID=pr.ProductNumber } equals//產品編號一樣的話
                            new {prp.ProductNumber,ID=Nuber.ProductNumber }
                            join p in db.Part
                            on prp.PartNumber equals p.PartNumber
                           group p by new { prp.PartNumber, p.PartName } into g
                           select new PartItem //傳回料件資料集
                           {
                               PartNumber = g.Key.PartNumber, //料件編號
                               PartName = "(" + g.Key.PartNumber + ") " + g.Key.PartName,//料件名稱
                               ProductNumber = Nuber.ProductNumber,//產品編號
                           };
                IList<PartItem> sups = supq.ToList();
                return sups;
            }
        }



        //取得請購單明細資料集
        public static IEnumerable<PurchaseRequisitionDtlItem> GetPurchaseRequisitionDtlList(string productNumber, string partNumber)
        {
            IEnumerable<PurchaseRequisitionDtlItem> pods = null;//建立空的請購單明細模型
            DateTime now = DateTime.Now;
            //排除時間
            now = new DateTime(now.Year, now.Month, now.Day);//取現在時間
            //取得顯示資料
            using (PMSAEntities db = new PMSAEntities())
            { 
                var podq = from pr in db.Product//產品
                           join prp in db.ProductPart//產品料件
                            on new { pr.ProductNumber, ID = pr.ProductNumber } equals//產品編號一樣的話
                            new { prp.ProductNumber, ID = productNumber }
                           join s1 in db.SourceList//貨源清單
                           on new { prp.PartNumber ,number=prp.PartNumber  } equals
                           new { s1.PartNumber ,number=partNumber}
                           //where !s1.SourceListDtl.Where(d => d.DiscountBeginDate <= now && d.DiscountEndDate >= now).Any()//檢查是否過期
                           orderby pr.ProductNumber//用產品料件編號排序
                           select new PurchaseRequisitionDtlItem
                           {
                               PartNumber = prp.PartNumber,//料件編號=產品料件明細的
                               PartName = prp.Part.PartName,//料件名稱=料件的
                               QtyPerUnit = s1.QtyPerUnit,//批量=貨源清單的
                               OriginalUnitPrice = s1.UnitPrice,//價格=貨源清單的
                               Qty = s1.QtyPerUnit,//請購數量=貨源清單.最小訂貨量
                               //Discount = 0M,//折扣
                               //DateRequired = prd.DateRequired,//要求到貨日期=請購單明細的
                               SourceListID = s1.SourceListID,//貨源清單ID=貨源清單的
                               ProductNumber = prp.ProductNumber//料件編號=料件產品的
                           };
                pods = podq.ToList();
            }
            //設定折扣
            foreach (PurchaseRequisitionDtlItem item in pods)//採購單明細
            {
                using (PMSAEntities db = new PMSAEntities())//用資料庫
                {
                    IEnumerable<SourceListDtl> sldq = db.SourceListDtl.Where(s =>//用貨源清單明細的資料
                    s.SourceListID == item.SourceListID &&//取出貨源清單編號
                    s.DiscountBeginDate <= now &&
                    s.DiscountEndDate >= now).OrderBy(o => o.QtyDemanded);//檢查是否過期
                    foreach (SourceListDtl sld in sldq) //從篩選的資料裡面取得
                    {
                        if (item.Qty >= sld.QtyDemanded)//如果採購批量數量大於需求量的話(自動算折扣)
                        {
                            item.Discount = sld.Discount;//取折扣
                        }
                    }
                    item.TotalPartQty = item.QtyPerUnit * item.Qty;//請購料件總數=批量*請購數量
                    item.PurchaseUnitPrice = (int)Math.Ceiling(item.OriginalUnitPrice * (1 - item.Discount));//購買單價=批量原始單價*(1-折扣)
                    item.Total = item.PurchaseUnitPrice * item.Qty;//總價格=單價*數量
                    //item.DateRequired = item.DateRequired.AddDays(-7);//要求到貨日期
                }
            }

            ////寫入暫存資料表
            //using (PMSAEntities db = new PMSAEntities())
            //{
            //    //TODO: 多人新增相同請購單來源會有刪除同一筆資料的問題，請購單需要設定[新增中]的狀態
            //    //移除現有資料
            //    var rortq = db.PRPORelationTemp.Where(p => p.PurchaseRequisitionID == purchaseRequisitionID);
            //    int? poOid = rortq.FirstOrDefault()?.PurchaseOrderOID;
            //    if (poOid.HasValue)
            //    {
            //        db.PRPORelationTemp.RemoveRange(rortq);
            //        var podtq = db.PurchaseOrderDtlTemp.Where(p => p.PurchaseOrderOID == poOid);
            //        db.PurchaseOrderDtlTemp.RemoveRange(podtq);
            //        var potq = db.PurchaseOrderTemp.Find(poOid);
            //        db.PurchaseOrderTemp.Remove(potq);
            //        db.SaveChanges();
            //    }
            //    //新增暫存資料
            //    Employee emp = GetEmployee();
            //    PurchaseOrderTemp pot = new PurchaseOrderTemp
            //    {
            //        SupplierCode = supplierCode,
            //        EmployeeID = emp.EmployeeID,
            //        CreateDate = DateTime.Now
            //    };
            //    db.PurchaseOrderTemp.Add(pot);
            //    db.SaveChanges();

            //    //更新暫存OID
            //    foreach (var item in pods)
            //    {
            //        item.PurchaseOrderOID = pot.PurchaseOrderOID;
            //    }

            //    foreach (var item in pods)
            //    {
            //        PurchaseOrderDtlTemp podt = new PurchaseOrderDtlTemp
            //        {
            //            PurchaseOrderOID = pot.PurchaseOrderOID,
            //            PartNumber = item.PartNumber,
            //            PartName = item.PartName,
            //            PartSpec = item.PartSpec,
            //            QtyPerUnit = item.QtyPerUnit,
            //            TotalPartQty = item.TotalPartQty,
            //            OriginalUnitPrice = item.OriginalUnitPrice,
            //            Discount = item.Discount,
            //            PurchaseUnitPrice = item.PurchaseUnitPrice,
            //            Qty = item.Qty,
            //            PurchasedQty = 0,
            //            GoodsInTransitQty = 0,
            //            Total = item.Total,
            //            SourceListID = item.SourceListID
            //        };
            //        db.PurchaseOrderDtlTemp.Add(podt);
            //        db.SaveChanges();

            //        item.PurchaseOrderDtlOID = podt.PurchaseOrderDtlOID;

            //        PRPORelationTemp rort = new PRPORelationTemp
            //        {
            //            PurchaseRequisitionID = purchaseRequisitionID,
            //            PurchaseRequisitionDtlCode = item.PurchaseRequisitionDtlCode,
            //            PurchaseOrderOID = pot.PurchaseOrderOID,
            //            PurchaseOrderDtlOID = podt.PurchaseOrderDtlOID
            //        };
            //        db.PRPORelationTemp.Add(rort);
            //        db.SaveChanges();
            //    }
            //}

            return pods;
        }

    }

    public class PurchaseRequisitionCreateViewModel
    {
        [Required(ErrorMessage = "請選擇產品")]
        [Display(Name = "產品名稱")]
        public string SelectedProductNume { get; set; }
        [Required(ErrorMessage = "請選擇料件")]
        [Display(Name = "料件名稱")]
        public string SelectedPartName { get; set; }

        public SelectList ProductList { get; set; }
        public SelectList PartList { get; set; }

        //TODO: 應是多筆的狀況，之後需作修正
        public string ProductNumber { get; set; }
        //public string ProductName { get; set; }
        public int PurchaseRequisitionOID { get; set; }
        /// <summary>
        /// 顯示內容
        /// </summary>
        public IEnumerable<PurchaseRequisitionDtlItem> PurchaseRequisitionDtlSetVM { get; set; }
        /// <summary>
        /// 表單內容
        /// </summary>
        public IList<PurchaseRequisitionDtlItemChecked> CheckedResultSetVM { get; set; }
    }
}