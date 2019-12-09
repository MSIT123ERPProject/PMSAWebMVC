using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.ViewModels.PurchaseOrders
{
    public class PurchaseRequisitionItem
    {
        public string PurchaseRequisitionIdDisplay { get; set; }
        public string PurchaseRequisitionIdValue { get; set; }
    }

    public class PRInfoViewModel
    {
        [Display(Name = "請購人員")]
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "請購日期")]
        public DateTime PRBeginDate { get; set; }
        [Display(Name = "電子信箱")]
        public string Email { get; set; }
        [Display(Name = "聯絡電話")]
        public string Tel { get; set; }
    }

    public class PRDtlTableViewModel
    {
        [Display(Name = "請購單明細編號")]
        public string PurchaseRequisitionDtlCode { get; set; }
        [Display(Name = "料件品名")]
        public string PartName { get; set; }
        [Display(Name = "請購數量")]
        public int Qty { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "需求日期")]
        public DateTime DateRequired { get; set; }
        [Display(Name = "建議供應商")]
        public string SupplierName { get; set; }
    }

    public class SupplierItem
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string PurchaseRequisitionID { get; set; }
    }

    public class PurchaseOrderDtlItemChecked
    {
        public string PurchaseRequisitionDtlCode { get; set; }
        public int PurchaseOrderDtlOID { get; set; }
        [Display(Name = "選取")]
        public bool Checked { get; set; }
    }

    public class PurchaseOrderDtlItem
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
        public DateTime DateRequired { get; set; }
        public string SourceListID { get; set; }
        public int PurchaseOrderOID { get; set; }
        public string PartSpec { get; set; }
        public int QtyPerUnit { get; set; }
        public int TotalPartQty { get; set; }
        public int PurchaseUnitPrice { get; set; }
        public int PurchasedQty { get; set; }
        public int GoodsInTransitQty { get; set; }
        public Nullable<System.DateTime> CommittedArrivalDate { get; set; }
        public Nullable<System.DateTime> ShipDate { get; set; }
        public Nullable<System.DateTime> ArrivedDate { get; set; }
        public Nullable<int> POChangedOID { get; set; }
        public string PurchaseRequisitionDtlCode { get; set; }
        public int PurchaseOrderDtlOID { get; set; }
    }

    public class Repository
    {
        private Repository() { }
        Employee emp;
        public Repository(Employee emp)
        {
            this.emp = emp;
        }
        private PMSAEntities db;
        public Repository(Employee emp, PMSAEntities ent)
        {
            this.emp = emp;
            this.db = ent;
        }

        /// <summary>
        /// 查詢尚未被採購的請購明細
        /// </summary>
        /// <param name="purchaseRequisitionID"></param>
        /// <returns></returns>
        public IEnumerable<PRDtlTableViewModel> GetPRDtlTableViewModel(string purchaseRequisitionID)
        {
            var prdq = from prd in db.PurchaseRequisitionDtl
                       where !(from prdRel in db.PRPORelation
                               select prdRel.PurchaseRequisitionDtlCode).Contains(prd.PurchaseRequisitionDtlCode)
                       select new PRDtlTableViewModel
                       {
                           PurchaseRequisitionDtlCode = prd.PurchaseRequisitionDtlCode,
                           PartName = prd.Part.PartName,
                           Qty = prd.Qty,
                           DateRequired = prd.DateRequired,
                           SupplierName = prd.SupplierInfo.SupplierName
                       };
            return prdq;
        }

        /// <summary>
        /// 查詢請購單基本資料
        /// </summary>
        /// <param name="purchaseRequisitionID"></param>
        /// <returns></returns>
        public PRInfoViewModel GetPRInfoViewModel(string purchaseRequisitionID)
        {
            using (PMSAEntities db = new PMSAEntities())
            {
                var infoq = from pr in db.PurchaseRequisition
                            where pr.PurchaseRequisitionID == purchaseRequisitionID
                            select new PRInfoViewModel
                            {
                                Name = pr.Employee.Name,
                                PRBeginDate = pr.PRBeginDate,
                                Email = pr.Employee.Email,
                                Tel = pr.Employee.Tel
                            };
                return infoq.FirstOrDefault();
            }
        }

        public IList<PurchaseRequisitionItem> GetPurchaseRequisitionList()
        {
            using (PMSAEntities db = new PMSAEntities())
            {
                //有可能是不同來源
                var prq = from pr in db.PurchaseRequisition
                          where pr.ProcessStatus == "N"
                          //where pr.ProcessStatus == "N" && pr.EmployeeID == emp.EmployeeID
                          select new PurchaseRequisitionItem
                          {
                              PurchaseRequisitionIdDisplay = pr.PurchaseRequisitionID,
                              PurchaseRequisitionIdValue = pr.PurchaseRequisitionID
                          };
                return prq.ToList();
            }
        }

        public IList<SupplierItem> GetSupplierList(string purchaseRequisitionID)
        {
            DateTime now = DateTime.Now;
            //排除時間
            now = new DateTime(now.Year, now.Month, now.Day);
            using (PMSAEntities db = new PMSAEntities())
            {
                var supq = from pr in db.PurchaseRequisition
                           join prd in db.PurchaseRequisitionDtl
                            on new { pr.PurchaseRequisitionID, ID = pr.PurchaseRequisitionID } equals
                            new { prd.PurchaseRequisitionID, ID = purchaseRequisitionID }
                           join sl in db.SourceList
                            on prd.PartNumber equals sl.PartNumber
                           where !(from prdRel in db.PRPORelation
                                   select prdRel.PurchaseRequisitionDtlCode).Contains(prd.PurchaseRequisitionDtlCode) &&
                                sl.SourceListDtl.Where(d => d.DiscountBeginDate <= now && d.DiscountEndDate >= now).Any()
                           group sl by new { sl.SupplierCode, sl.SupplierInfo.SupplierName } into g
                           orderby g.Key.SupplierCode
                           select new SupplierItem
                           {
                               SupplierCode = g.Key.SupplierCode,
                               SupplierName = "(" + g.Key.SupplierCode + ") " + g.Key.SupplierName,
                               PurchaseRequisitionID = purchaseRequisitionID
                           };
                IList<SupplierItem> sups = supq.ToList();
                return sups;
            }
        }

        public IEnumerable<PurchaseOrderDtlItem> GetPurchaseOrderDtlList(string purchaseRequisitionID, string supplierCode)
        {
            IEnumerable<PurchaseOrderDtlItem> pods = null;
            DateTime now = DateTime.Now;
            //排除時間
            now = new DateTime(now.Year, now.Month, now.Day);
            //取得顯示資料
            using (PMSAEntities db = new PMSAEntities())
            {
                var podq = from pr in db.PurchaseRequisition
                           join prd in db.PurchaseRequisitionDtl
                            on new { pr.PurchaseRequisitionID, ID = pr.PurchaseRequisitionID } equals
                            new { prd.PurchaseRequisitionID, ID = purchaseRequisitionID }
                           join sl in db.SourceList
                            on new { prd.PartNumber, SupplierCode = supplierCode } equals
                            new { sl.PartNumber, sl.SupplierCode }
                           join p in db.Part
                           on sl.PartNumber equals p.PartNumber
                           where !(from prdRel in db.PRPORelation
                                   select prdRel.PurchaseRequisitionDtlCode).Contains(prd.PurchaseRequisitionDtlCode) &&
                                sl.SourceListDtl.Where(d => d.DiscountBeginDate <= now && d.DiscountEndDate >= now).Any()
                           orderby prd.PurchaseRequisitionDtlCode
                           select new PurchaseOrderDtlItem
                           {
                               PartNumber = prd.PartNumber,
                               PartName = p.PartName,
                               PartSpec = p.PartSpec,
                               QtyPerUnit = sl.QtyPerUnit,
                               OriginalUnitPrice = sl.UnitPrice,
                               Qty = sl.MOQ.HasValue && prd.Qty < sl.MOQ.Value ? sl.MOQ.Value : prd.Qty,
                               Discount = 0M,
                               DateRequired = prd.DateRequired,
                               SourceListID = sl.SourceListID,
                               PurchaseRequisitionDtlCode = prd.PurchaseRequisitionDtlCode
                           };
                pods = podq.ToList();
            }
            //設定折扣
            foreach (PurchaseOrderDtlItem item in pods)
            {
                using (PMSAEntities db = new PMSAEntities())
                {
                    IEnumerable<SourceListDtl> sldq = db.SourceListDtl.Where(s =>
                    s.SourceListID == item.SourceListID &&
                    s.DiscountBeginDate <= now &&
                    s.DiscountEndDate >= now).OrderBy(o => o.QtyDemanded);
                    foreach (SourceListDtl sld in sldq)
                    {
                        if (item.Qty >= sld.QtyDemanded)
                        {
                            item.Discount = sld.Discount;
                        }
                    }
                    item.TotalPartQty = item.QtyPerUnit * item.Qty;
                    item.PurchaseUnitPrice = (int)Math.Ceiling(item.OriginalUnitPrice * (1 - item.Discount));
                    item.Total = item.PurchaseUnitPrice * item.Qty;
                    item.DateRequired = item.DateRequired.AddDays(-7);
                }
            }

            //寫入暫存資料表
            using (PMSAEntities db = new PMSAEntities())
            {
                //TODO: 多人新增相同請購單來源會有刪除同一筆資料的問題，請購單需要設定[新增中]的狀態
                //移除現有資料
                var rortq = db.PRPORelationTemp.Where(p => p.PurchaseRequisitionID == purchaseRequisitionID);
                int? poOid = rortq.FirstOrDefault()?.PurchaseOrderOID;
                if (poOid.HasValue)
                {
                    db.PRPORelationTemp.RemoveRange(rortq);
                    var podtq = db.PurchaseOrderDtlTemp.Where(p => p.PurchaseOrderOID == poOid);
                    db.PurchaseOrderDtlTemp.RemoveRange(podtq);
                    var potq = db.PurchaseOrderTemp.Find(poOid);
                    db.PurchaseOrderTemp.Remove(potq);
                    db.SaveChanges();
                }
                //新增暫存資料
                PurchaseOrderTemp pot = new PurchaseOrderTemp
                {
                    SupplierCode = supplierCode,
                    EmployeeID = emp.EmployeeID,
                    CreateDate = DateTime.Now
                };
                db.PurchaseOrderTemp.Add(pot);
                db.SaveChanges();

                //更新暫存OID
                foreach (var item in pods)
                {
                    item.PurchaseOrderOID = pot.PurchaseOrderOID;
                }

                foreach (var item in pods)
                {
                    PurchaseOrderDtlTemp podt = new PurchaseOrderDtlTemp
                    {
                        PurchaseOrderOID = pot.PurchaseOrderOID,
                        PartNumber = item.PartNumber,
                        PartName = item.PartName,
                        PartSpec = item.PartSpec,
                        QtyPerUnit = item.QtyPerUnit,
                        TotalPartQty = item.TotalPartQty,
                        OriginalUnitPrice = item.OriginalUnitPrice,
                        Discount = item.Discount,
                        PurchaseUnitPrice = item.PurchaseUnitPrice,
                        Qty = item.Qty,
                        PurchasedQty = 0,
                        GoodsInTransitQty = 0,
                        Total = item.Total,
                        SourceListID = item.SourceListID
                    };
                    db.PurchaseOrderDtlTemp.Add(podt);
                    db.SaveChanges();

                    item.PurchaseOrderDtlOID = podt.PurchaseOrderDtlOID;

                    PRPORelationTemp rort = new PRPORelationTemp
                    {
                        PurchaseRequisitionID = purchaseRequisitionID,
                        PurchaseRequisitionDtlCode = item.PurchaseRequisitionDtlCode,
                        PurchaseOrderOID = pot.PurchaseOrderOID,
                        PurchaseOrderDtlOID = podt.PurchaseOrderDtlOID
                    };
                    db.PRPORelationTemp.Add(rort);
                    db.SaveChanges();
                }
            }

            return pods;
        }
    }

    public class PurchaseOrderCreateViewModel
    {
        [Required(ErrorMessage = "請選擇一個請購單號")]
        [Display(Name = "請購單號")]
        public string SelectedPurchaseRequisitionID { get; set; }
        [Required(ErrorMessage = "請選擇一個供應商")]
        [Display(Name = "供應商名稱")]
        public string SelectedSupplierName { get; set; }

        public SelectList PurchaseRequisitionList { get; set; }
        public SelectList SupplierList { get; set; }

        //TODO: 應是多筆的狀況，之後需作修正
        public string PurchaseRequisitionID { get; set; }
        public int PurchaseOrderOID { get; set; }
        /// <summary>
        /// 顯示內容
        /// </summary>
        public IEnumerable<PurchaseOrderDtlItem> PurchaseOrderDtlSetVM { get; set; }
        /// <summary>
        /// 表單內容
        /// </summary>
        public IList<PurchaseOrderDtlItemChecked> CheckedResultSetVM { get; set; }
    }
}