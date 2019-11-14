using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.ViewModels.PurchaseOrders
{
    public class PurchaseOrderIndexViewModel
    {
        [Key]
        [Display(Name = "採購單號")]
        public string PurchaseOrderID { get; set; }
        [Display(Name = "採購日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "供應商名稱")]
        public string SupplierName { get; set; }
        [Display(Name = "來自請購號")]
        public string PurchaseRequisitionID { get; set; }
        [Display(Name = "狀態")]
        public string PurchaseOrderStatus { get; set; }
    }
}