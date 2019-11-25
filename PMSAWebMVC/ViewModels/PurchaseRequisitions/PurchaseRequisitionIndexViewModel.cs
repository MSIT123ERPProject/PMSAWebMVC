using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.ViewModels.PurchaseRequisitions
{
    public class PurchaseRequisitionIndexViewModel
    {
        [Key]
        [Display(Name = "請購單號")]
        public string PurchaseRequisitionID { get; set; }
        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }
        [Display(Name = "產生日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime PRBeginDate { get; set; }
        [Display(Name = "處理狀態")]
        public string ProcessStatus { get; set; }
        [Display(Name = "簽核狀態")]
        public string SignStatus { get; set; }

    }
}