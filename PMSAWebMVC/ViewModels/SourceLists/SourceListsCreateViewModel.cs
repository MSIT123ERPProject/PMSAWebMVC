using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.ViewModels.SourceLists
{
    public class PartItem//料件模型
    {
        public string PartNumber { get; set; }//料件編號
        public string PartName { get; set; }//料件名稱
    }
    public class SupplierItem//供應商模型
    {
        public string SupplierCode { get; set; }//供應商編號
        public string SupplierName { get; set; }//供應商名稱
    }
    public class SourceListsDtlItem//貨源清單明細模型
    {
        [Display(Name = "貨源清單代碼")]
        public string SourceListID { get; set; }
        [Display(Name = "料件編號")]
        public string PartNumber { get; set; }
        [Display(Name = "料件名稱")]
        public string PartName { get; set; }
        [Display(Name = "批量")]
        public int QtyPerUnit { get; set; }
        [Display(Name = "最小訂貨量")]
        public int? MOQ { get; set; }
        [Display(Name = "單價")]
        public int UnitPrice { get; set; }
        [Display(Name = "供應商代碼")]
        public string SupplierCode { get; set; }
        [Display(Name = "供應商名稱")]
        public string SupplierName { get; set; }
        [Display(Name = "庫存數量")]
        public int UnitsInStock { get; set; }
        [Display(Name = "採購單數量")]
        public int UnitsOnOrder { get; set; }
        [Display(Name = "安全庫存量")]
        public int? SafetyQty { get; set; }
        [Display(Name = "保存期限")]
        public int? EXP { get; set; }


        [Display(Name = "貨源清單明細識別碼")]
        public int SourceListDtlOID { get; set; }
        [Display(Name = "需求量")]
        public int QtyDemanded { get; set; }
        [Display(Name = "折扣")]
        public decimal Discount { get; set; }
        [Display(Name = "請購數量")]
        public int Qty { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "折扣開始時間")]
        public DateTime? DiscountBeginDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "折扣結束時間")]
        public DateTime? DiscountEndDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "建檔日期")]
        public DateTime CreateDate { get; set; }

    }

    //資料庫相關方法
    public class Repository
    {
        //取得供應商資料集
        public static IList<SupplierItem> GetSupplierList()
        {

            using (PMSAEntities db = new PMSAEntities())
            {
                var sup = from sl in db.SupplierInfo
                          select new SupplierItem
                          {
                              SupplierCode = sl.SupplierCode,
                              SupplierName = sl.SupplierName,
                          };
                return sup.ToList(); //將資料裝在陣列
            }
        }
        //取得料件資料集
        public static IList<PartItem> GetPartList()
        {

            using (PMSAEntities db = new PMSAEntities())
            {
                var p = from pa in db.Part
                          select new PartItem
                          {
                              PartName = pa.PartName,
                              PartNumber = pa.PartNumber,
                          };
                return p.ToList(); //將資料裝在陣列
            }
        }

    }

    public class SourceListsCreateViewModel
    {
        [Required(ErrorMessage = "請選擇供應商")]
        [Display(Name = "供應商名稱")]
        public string SelectedSupplierName { get; set; }
        [Required(ErrorMessage = "請選擇料件")]
        [Display(Name = "料件名稱")]
        public string SelectedPartName { get; set; }

        public SelectList SupplierList { get; set; }
        public SelectList PartList { get; set; }

        [Display(Name = "貨源清單代碼")]
        public string SourceListID { get; set; }
        [Display(Name = "料件編號")]
        public string PartNumber { get; set; }
        [Display(Name = "料件名稱")]
        public string PartName { get; set; }
        [Display(Name = "批量")]
        public int QtyPerUnit { get; set; }
        [Display(Name = "最小訂貨量")]
        public int MOQ { get; set; }
        [Display(Name = "單價")]
        public int UnitPrice { get; set; }
        [Display(Name = "供應商代碼")]
        public string SupplierCode { get; set; }
        [Display(Name = "供應商名稱")]
        public string SupplierName { get; set; }
        [Display(Name = "庫存數量")]
        public int UnitsInStock { get; set; }
        [Display(Name = "採購單數量")]
        public int UnitsOnOrder { get; set; }
        [Display(Name = "安全庫存量")]
        public int SafetyQty { get; set; }
        [Display(Name = "保存期限")]
        public int EXP { get; set; }


        [Display(Name = "貨源清單明細識別碼")]
        public int SourceListDtlOID { get; set; }
        [Display(Name = "需求量")]
        public int QtyDemanded { get; set; }
        [Display(Name = "折扣")]
        public decimal Discount { get; set; }
        [Display(Name = "請購數量")]
        public int Qty { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "折扣開始時間")]
        public DateTime DiscountBeginDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "折扣結束時間")]
        public DateTime DiscountEndDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "建檔日期")]
        public DateTime CreateDate { get; set; }

        public string DiscountBeginDatestring { get; set; }//傳值用
        public string DiscountEndDatestring { get; set; }
        /// <summary>
        /// 顯示內容
        /// </summary>
        public IEnumerable<SourceListsDtlItem> SourceListsDtlSetVM { get; set; }
        /// <summary>
        /// 表單內容
        /// </summary>
        public IList<SourceListsDtlItem> CheckedResultSetVM { get; set; }
    }
}