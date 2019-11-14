using System;
using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class SourceListDtlMetadata
    {
        [Display(Name = "貨源清單明細識別碼")]
        public int SourceListDtlOID { get; set; }
        [Display(Name = "貨源清單代碼")]
        public string SourceListID { get; set; }
        [Display(Name = "需求量")]
        public int QtyDemanded { get; set; }
        [Display(Name = "折扣")]
        public decimal Discount { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "折扣開始時間")]
        public Nullable<System.DateTime> DiscountBeginDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "折扣結束時間")]
        public Nullable<System.DateTime> DiscountEndDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "建檔日期")]
        public System.DateTime CreateDate { get; set; }
    }
}