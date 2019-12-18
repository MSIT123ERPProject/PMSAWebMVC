using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.YaChen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.ViewModels.PurchaseOrders
{
    /// <summary>
    /// 供應商資訊
    /// </summary>
    public class SUPInfoViewModel
    {
        [Display(Name = "供應商名稱")]
        public string SupplierName { get; set; }
        [Display(Name = "聯絡人")]
        public string ContactName { get; set; }
        [Display(Name = "電子信箱")]
        public string Email { get; set; }
        [Display(Name = "聯絡電話")]
        public string Tel { get; set; }
    }

    /// <summary>
    /// 採購單新增編輯時的貨源清單
    /// </summary>
    public class POCSourceListViewModel
    {
        /// <summary>
        /// 前端使用是否需不顯示或禁用
        /// </summary>
        public bool Disable { get; set; }
        public string SourceListID { get; set; }
        [Display(Name = "供應商")]
        public string SupplierName { get; set; }
        [Display(Name = "評等")]
        public string RatingName { get; set; }
        [Display(Name = "批量")]
        public int QtyPerUnit { get; set; }
        [Display(Name = "最小訂貨量")]
        public int? MOQ { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        [Display(Name = "單價")]
        public int UnitPrice { get; set; }
        [Display(Name = "供應商庫存數量")]
        public int UnitsInStock { get; set; }
        [Display(Name = "購買折扣")]
        public IEnumerable<POCSourceListDtlItem> SourceListDtlItem { get; set; }
    }
    /// <summary>
    /// 採購單新增編輯時的貨源清單明細
    /// </summary>
    public class POCSourceListDtlItem
    {
        [Display(Name = "需求量")]
        public int QtyDemanded { get; set; }
        [DisplayFormat(DataFormatString = "{0:P0}")]
        [Display(Name = "折扣")]
        public decimal Discount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "折扣開始時間")]
        public DateTime DiscountBeginDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "折扣結束時間")]
        public DateTime DiscountEndDate { get; set; }
    }
    /// <summary>
    /// 採購單建立時主畫面的請購人員資訊
    /// </summary>
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
    /// <summary>
    /// 採購單建立時主畫面的請購明細表格
    /// </summary>
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
        //請購單新增使用
        [Display(Name = "料件批量")]
        public int QtyPerUnit { get; set; }
        [Display(Name = "請購料件總數")]
        //請購數量 * 料件批量
        public int TotalPartQty { get; set; }
        [Display(Name = "料件編號")]
        public string PartNumber { get; set; }
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
        [Display(Name = "料件品名")]
        public string PartName { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        [Display(Name = "單價")]
        public int OriginalUnitPrice { get; set; }
        public string OriginalUnitPriceToShow
        {
            get
            {
                return $"{OriginalUnitPrice:C0}";
            }
        }
        [Display(Name = "料件總數")]
        public int TotalSourceListQty { get; set; }
        [Required(ErrorMessage = "{0}為必填")]
        [Display(Name = "數量")]
        public int Qty { get; set; }
        [DisplayFormat(DataFormatString = "{0:P0}")]
        [Column(TypeName = "decimal(3, 2)")]
        [Display(Name = "折扣")]
        public decimal Discount { get; set; }
        public string DiscountToShow
        {
            get
            {
                return $"{Discount:P0}";
            }
        }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        [Display(Name = "小計")]
        public int Total { get; set; }
        public string TotalToShow
        {
            get
            {
                return $"{Total:C0}";
            }
        }
        [Required(ErrorMessage = "{0}為必填")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "要求到貨日期")]
        public DateTime DateRequired { get; set; }
        public string DateRequiredToShow
        {
            get
            {
                if (DateRequired == null)
                {
                    return "";
                }
                else
                {
                    return DateRequired.ToString("yyyy/MM/dd");
                }
            }
        }

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
        [Display(Name = "採購明細編號")]
        public string PurchaseOrderDtlCode { get; set; }
        public string PurchaseOrderID { get; set; }
        public string LastModifiedAccountID { get; set; }
    }

    public partial class Repository
    {
        private Employee emp;
        private PMSAEntities db;
        private PurchaseOrderCreateSession session;

        private Repository() { }
        public Repository(Employee emp)
        {
            this.emp = emp;
            this.session = PurchaseOrderCreateSession.Current;
        }
        public Repository(Employee emp, PMSAEntities ent)
        {
            this.emp = emp;
            this.db = ent;
            this.session = PurchaseOrderCreateSession.Current;
        }

        /// <summary>
        /// 查詢料件的貨源清單
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        public IEnumerable<POCSourceListViewModel> GetPOCSourceListViewModel(string partNumber)
        {
            //存在供應商表採購單已新增一筆明細
            string supplierName = "";
            if (session.Supplier != null)
            {
                supplierName = session.Supplier.SupplierInfo.SupplierName;
            }

            //排除時間
            DateTime now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day);

            using (PMSAEntities db = new PMSAEntities())
            {
                var slq = from sl in db.SourceList
                          where sl.PartNumber == partNumber &&
                          sl.SourceListDtl.Where(d => d.DiscountBeginDate <= now && d.DiscountEndDate >= now).Any()
                          select new POCSourceListViewModel
                          {
                              Disable = supplierName == "" ? false : sl.SupplierInfo.SupplierName != supplierName,
                              SourceListID = sl.SourceListID,
                              SupplierName = sl.SupplierInfo.SupplierName,
                              RatingName = sl.SupplierInfo.SupplierRating.RatingName,
                              QtyPerUnit = sl.QtyPerUnit,
                              MOQ = sl.MOQ,
                              UnitPrice = sl.UnitPrice,
                              UnitsInStock = sl.UnitsInStock,
                              SourceListDtlItem = (from sld in sl.SourceListDtl
                                                   select new POCSourceListDtlItem
                                                   {
                                                       QtyDemanded = sld.QtyDemanded,
                                                       Discount = sld.Discount,
                                                       DiscountBeginDate = sld.DiscountBeginDate.Value,
                                                       DiscountEndDate = sld.DiscountEndDate.Value
                                                   })
                          };

                return slq.ToList();
            }
        }

        /// <summary>
        /// 查詢請購明細資料
        /// </summary>
        /// <param name="purchaseRequisitionDtlCode"></param>
        /// <returns></returns>
        public PRDtlTableViewModel GetPRDtlInfoViewModel(string purchaseRequisitionDtlCode)
        {
            var prdq = from prd in db.PurchaseRequisitionDtl
                       where prd.PurchaseRequisitionDtlCode == purchaseRequisitionDtlCode
                       select new PRDtlTableViewModel
                       {
                           PurchaseRequisitionDtlCode = prd.PurchaseRequisitionDtlCode,
                           PartName = prd.Part.PartName,
                           Qty = prd.Qty,
                           DateRequired = prd.DateRequired,
                           SupplierName = prd.SupplierInfo.SupplierName,
                           QtyPerUnit = prd.Part.QtyPerUnit,
                           TotalPartQty = prd.Part.QtyPerUnit * prd.Qty,
                           PartNumber = prd.Part.PartNumber
                       };
            return prdq.FirstOrDefault();
        }

        /// <summary>
        /// 取得請購單主表
        /// </summary>
        /// <param name="purchaseRequisitionID"></param>
        /// <returns></returns>
        public PurchaseRequisition GetPurchaseRequisition(string purchaseRequisitionID)
        {
            var pr = db.PurchaseRequisition.Find(purchaseRequisitionID);
            return pr;
        }

        /// <summary>
        /// 查詢尚未被採購的請購明細
        /// </summary>
        /// <param name="purchaseRequisitionID"></param>
        /// <returns></returns>
        public IList<PRDtlTableViewModel> GetPRDtlTableViewModel(string purchaseRequisitionID)
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
                           SupplierName = prd.SupplierInfo.SupplierName,
                           QtyPerUnit = prd.Part.QtyPerUnit,
                           TotalPartQty = prd.Part.QtyPerUnit * prd.Qty,
                           PartNumber = prd.Part.PartNumber
                       };
            return prdq.ToList();
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

        /// <summary>
        /// 取得請購單選單
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetPurchaseRequisitionList()
        {
            using (PMSAEntities db = new PMSAEntities())
            {
                //有可能是不同來源
                var prq = from pr in db.PurchaseRequisition
                          where pr.ProcessStatus == "N"
                          //where pr.ProcessStatus == "N" && pr.EmployeeID == emp.EmployeeID
                          select new SelectListItem
                          {
                              Display = pr.PurchaseRequisitionID,
                              Value = pr.PurchaseRequisitionID
                          };
                return prq.ToList();
            }
        }

        /// <summary>
        /// 取得倉庫資訊選單
        /// </summary>
        /// <returns></returns>
        public IList<System.Web.Mvc.SelectListItem> GetWarehouseInfoList()
        {
            using (PMSAEntities db = new PMSAEntities())
            {
                //有可能是不同來源
                var wiq = from wi in db.WarehouseInfo
                          orderby wi.WarehouseInfoOID ascending
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = wi.WarehouseName,
                              Value = wi.WarehouseCode
                          };
                return wiq.ToList();
            }
        }

        /// <summary>
        /// 取得供應商選單
        /// </summary>
        /// <param name="purchaseRequisitionID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 加入新增的採購明細資料到表格
        /// </summary>
        public IList<PurchaseOrderDtlItem> AddPODtlToTableViewModel()
        {
            if (session.PODItemEditting != null)
            {
                session.PODItems.Add(session.PODItemEditting);
                session.PODItemEditting = null;
            }
            return session.PODItems;
        }

        /// <summary>
        /// 取得單一採購明細編輯後資料 - 新版使用方法
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="dateRequired"></param>
        /// <param name="sourceListID"></param>
        /// <param name="purchaseRequisitionDtlCode"></param>
        /// <returns></returns>
        public PurchaseOrderDtlItem GetPODtlUpdateItemViewModel(int qty, DateTime dateRequired, string sourceListID, string purchaseRequisitionDtlCode, string mode)
        {
            PurchaseOrderDtlItem podedit = null;
            bool updateMode = session.PODItems.Where(item => item.PurchaseRequisitionDtlCode == purchaseRequisitionDtlCode).Any();
            if (updateMode)
            {
                //編輯
                if (mode == "add")
                {
                    //參考
                    podedit = session.PODItems.Where(item => item.PurchaseRequisitionDtlCode == purchaseRequisitionDtlCode).FirstOrDefault();
                }
                else
                {
                    //複製
                    podedit = session.PODItems.Where(item => item.PurchaseRequisitionDtlCode == purchaseRequisitionDtlCode).ToList().FirstOrDefault();
                }
            }
            else
            {
                //新增
                podedit = session.PODItemEditting;
            }

            //採購明細編輯中
            DateTime now = DateTime.Now;
            //排除時間
            now = new DateTime(now.Year, now.Month, now.Day);
            //取得顯示資料
            PurchaseRequisitionDtl prd = db.PurchaseRequisitionDtl.Find(purchaseRequisitionDtlCode);

            SourceList sl = db.SourceList.Find(sourceListID);
            Part p = prd.Part;

            //更新資料
            podedit.QtyPerUnit = sl.QtyPerUnit;
            podedit.OriginalUnitPrice = sl.UnitPrice;
            podedit.Qty = qty;
            podedit.TotalSourceListQty = podedit.Qty * sl.QtyPerUnit;
            podedit.Discount = 0M;
            podedit.DateRequired = dateRequired;
            podedit.SourceListID = sl.SourceListID;

            //計算折扣，需從已加入的相同貨源請購明細計算總數來得到折扣
            int totalQty = session.PODItems.Where(item => item.SourceListID == sl.SourceListID &&
            item.PurchaseRequisitionDtlCode != purchaseRequisitionDtlCode).Sum(item => item.Qty) + qty;
            List<SourceListDtl> slds = db.SourceListDtl.Where(s =>
                    s.SourceListID == sl.SourceListID &&
                    s.DiscountBeginDate <= now &&
                    s.DiscountEndDate >= now).OrderBy(o => o.QtyDemanded).ToList();
            decimal discount = 0M;
            foreach (SourceListDtl sld in slds)
            {
                if (totalQty >= sld.QtyDemanded)
                {
                    discount = sld.Discount;
                }
            }

            //設定折扣

            //新增的
            podedit.Discount = discount;
            podedit.PurchaseUnitPrice = (int)Math.Ceiling(podedit.OriginalUnitPrice * (1 - discount));
            podedit.Total = podedit.PurchaseUnitPrice * podedit.Qty;

            //已存在的(需正式加入採購單才做改變)
            if (mode == "add" || updateMode)
            {
                foreach (var item in session.PODItems.Where(item => item.SourceListID == sl.SourceListID))
                {
                    item.Discount = discount;
                    item.PurchaseUnitPrice = (int)Math.Ceiling(item.OriginalUnitPrice * (1 - discount));
                    item.Total = item.PurchaseUnitPrice * item.Qty;
                }
                //假如貨源清單沒值需加入
                if (session.Supplier == null)
                {
                    var sa = db.SupplierAccount.Where(item => item.SupplierCode == sl.SupplierInfo.SupplierCode).ToList().FirstOrDefault();
                    session.Supplier = sa;
                }
            }

            return podedit;
        }

        /// <summary>
        /// 刪除採購明細
        /// </summary>
        /// <param name="purchaseRequisitionDtlCode">請購單明細編號</param>
        public void DeletePODtlItem(string purchaseRequisitionDtlCode)
        {
            //假如只有一筆那就重設Session就好
            if (session.PODItems.Count() <= 1)
            {
                session.ResetAllItems();
                return;
            }
            //刪除明細
            var pod = session.PODItems.Where(item => item.PurchaseRequisitionDtlCode == purchaseRequisitionDtlCode).FirstOrDefault();
            string sourceListId = pod.SourceListID;
            session.PODItems.Remove(pod);

            //重新計算相同貨源的個數以計算折扣
            //計算折扣，需從已加入的相同貨源請購明細計算總數來得到折扣
            var sameSourcePODItems = session.PODItems.Where(item => item.SourceListID == sourceListId);
            int totalQty = sameSourcePODItems.Sum(item => item.Qty);
            if (totalQty == 0)
            {
                //不存在相同貨源
                return;
            }

            DateTime now = DateTime.Now;
            //排除時間
            now = new DateTime(now.Year, now.Month, now.Day);
            List<SourceListDtl> slds = db.SourceListDtl.Where(s =>
                    s.SourceListID == sourceListId &&
                    s.DiscountBeginDate <= now &&
                    s.DiscountEndDate >= now).OrderBy(o => o.QtyDemanded).ToList();
            decimal discount = 0M;
            foreach (SourceListDtl sld in slds)
            {
                if (totalQty >= sld.QtyDemanded)
                {
                    discount = sld.Discount;
                }
            }

            //設定折扣
            foreach (var item in sameSourcePODItems)
            {
                item.Discount = discount;
                item.PurchaseUnitPrice = (int)Math.Ceiling(item.OriginalUnitPrice * (1 - discount));
                item.Total = item.PurchaseUnitPrice * item.Qty;
            }
        }

        /// <summary>
        /// 取得單一採購明細 - 新版使用方法
        /// </summary>
        /// <param name="purchaseRequisitionID"></param>
        /// <returns></returns>
        public PurchaseOrderDtlItem GetPODtlItemInitViewModel(string purchaseRequisitionDtlCode)
        {
            //採購明細已加入
            PurchaseOrderDtlItem pod = session.PODItems.Where(item => item.PurchaseRequisitionDtlCode == purchaseRequisitionDtlCode).ToList().FirstOrDefault();
            if (pod != null)
            {
                return pod;
            }

            PurchaseOrderDtlItem podedit = null;
            //採購明細未加入
            DateTime now = DateTime.Now;
            //排除時間
            now = new DateTime(now.Year, now.Month, now.Day);
            //取得顯示資料
            PurchaseRequisitionDtl prd = db.PurchaseRequisitionDtl.Find(purchaseRequisitionDtlCode);

            SourceList sl = null;
            Part p = prd.Part;

            //採購單已設定供應商
            if (session.Supplier != null)
            {
                sl = db.SourceList.Find($"{prd.PartNumber}-{session.Supplier.SupplierCode}");
            }
            else if (prd.SuggestSupplierCode != null)
            {
                //有建議供應商，預先給定值
                sl = db.SourceList.Find($"{prd.PartNumber}-{prd.SuggestSupplierCode}");
            }
            else
            {
                //找出等級較高的供應商
                //1	未設定
                //2   不佳
                //3   優良
                var slsq = db.SourceList.Where(item => item.PartNumber == prd.PartNumber)
                    .Select(item => new { item, item.SupplierInfo }).ToList();
                //調整SupplierRatingOID大小
                foreach (var item in slsq)
                {
                    if (!item.SupplierInfo.SupplierRatingOID.HasValue)
                    {
                        item.SupplierInfo.SupplierRatingOID = 1;
                    }
                    else if (item.SupplierInfo.SupplierRatingOID.Value == 2)
                    {
                        item.SupplierInfo.SupplierRatingOID = -1;
                    }
                }
                //重新排序
                var sup = slsq.OrderByDescending(item => item.SupplierInfo.SupplierRatingOID);
                sl = sup.First().item;
            }

            //設定基本資料

            //請購料件總數
            int totalPartQty = p.QtyPerUnit * prd.Qty;
            //請購批量數量(預設不得超買)
            int qty = totalPartQty / sl.QtyPerUnit;
            //採購料件總數
            int totalSourceListQty = qty * sl.QtyPerUnit;

            podedit = new PurchaseOrderDtlItem
            {
                PartNumber = prd.PartNumber,
                PartName = p.PartName,
                PartSpec = p.PartSpec,
                QtyPerUnit = sl.QtyPerUnit,
                OriginalUnitPrice = sl.UnitPrice,
                //暫不實作最小訂貨量MOQ
                //Qty = sl.MOQ.HasValue && prd.Qty < sl.MOQ.Value ? sl.MOQ.Value : prd.Qty,
                Qty = qty,
                TotalPartQty = totalPartQty,
                TotalSourceListQty = totalSourceListQty,
                Discount = 0M,
                DateRequired = prd.DateRequired.AddDays(-7),
                SourceListID = sl.SourceListID,
                PurchaseRequisitionDtlCode = prd.PurchaseRequisitionDtlCode
            };

            //計算折扣，需從已加入的相同貨源請購明細計算總數來得到折扣
            int totalQty = session.PODItems.Where(item => item.SourceListID == sl.SourceListID).Sum(item => item.Qty) + qty;
            List<SourceListDtl> slds = db.SourceListDtl.Where(s =>
                    s.SourceListID == sl.SourceListID &&
                    s.DiscountBeginDate <= now &&
                    s.DiscountEndDate >= now).OrderBy(o => o.QtyDemanded).ToList();
            decimal discount = 0M;
            foreach (SourceListDtl sld in slds)
            {
                if (totalQty >= sld.QtyDemanded)
                {
                    discount = sld.Discount;
                }
            }

            //設定折扣

            //新增的
            podedit.Discount = discount;
            podedit.PurchaseUnitPrice = (int)Math.Ceiling(podedit.OriginalUnitPrice * (1 - discount));
            podedit.Total = podedit.PurchaseUnitPrice * podedit.Qty;

            session.PODItemEditting = podedit;

            return session.PODItemEditting;
        }

        /// <summary>
        /// 取得採購明細 - 舊版使用方法
        /// </summary>
        /// <param name="purchaseRequisitionID">請購單編號</param>
        /// <param name="supplierCode">供應商代碼</param>
        /// <returns></returns>
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

        /// <summary>
        /// 更新採購單狀態
        /// </summary>
        /// <param name=purchaseOrderID"></param>
        /// <param name="status"></param>
        public void UpdatePOStatus(string purchaseOrderID, string status)
        {
            PurchaseOrder po = db.PurchaseOrder.Find(purchaseOrderID);
            po.PurchaseOrderStatus = status;
            db.Entry(po).Property(pop => pop.PurchaseOrderStatus).IsModified = true;
            db.SaveChanges();

            //採購單雙方答交W，異動表要改為E(單方答交)
            if (status == "W")
            {
                status = "E";
            }

            //採購單異動總表 
            DateTime now = DateTime.Now;
            POChanged poc = new POChanged
            {
                PurchaseOrderID = purchaseOrderID,
                POChangedCategoryCode = status,
                RequestDate = now,
                RequesterRole = "P",
                RequesterID = emp.EmployeeID,
                PurchaseOrderDtlCode = null,
                Qty = null,
                DateRequired = null
            };
            db.POChanged.Add(poc);
            db.SaveChanges();
        }
    }

    public class PurchaseRequisitionItem
    {
        public string PurchaseRequisitionIdDisplay { get; set; }
        public string PurchaseRequisitionIdValue { get; set; }
    }

    public class SelectListItem
    {
        public string Display { get; set; }
        public string Value { get; set; }
    }

    public class PurchaseOrderCreateViewModel
    {
        [Required(ErrorMessage = "請選擇一個請購單號")]
        [Display(Name = "請購單號")]
        public string SelectedPurchaseRequisitionID { get; set; }
        [Display(Name = "倉庫資訊")]
        public string SelectedWarehouseCode { get; set; }

        public SelectList PurchaseRequisitionList { get; set; }
        public SelectList WarehouseInfoList { get; set; }

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
        /// <summary>
        /// 交貨資訊
        /// </summary>
        public PurchaseOrder POInfoItem { get; set; }
        public IList<PurchaseOrderDtlItem> PODItems { get; set; }
    }
}