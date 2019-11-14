using System;
using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class PurchaseRequisitionMetadata
    {
        [Display(Name ="請購單識別碼")]
        public int PurchaseRequisitionOID { get; set; }
        [Display(Name = "請購單編號")]
        public string PurchaseRequisitionID { get; set; }
        [Display(Name = "產品編號")]
        public string ProductNumber { get; set; }
        [Display(Name = "員工編號")]
        public string EmployeeID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "產生日期")]
        public System.DateTime PRBeginDate { get; set; }
        [Display(Name = "處理狀態代碼")]
        public string ProcessStatus { get; set; }
        [Display(Name = "簽核狀態代碼")]
        public string SignStatus { get; set; }
        [Display(Name = "簽核流程總表識別碼")]
        public Nullable<int> SignFlowOID { get; set; }
    }
}