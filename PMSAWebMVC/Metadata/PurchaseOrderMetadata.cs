using PMSAWebMVC.Resources;
using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class PurchaseOrderMetadata
    {
        public int PurchaseOrderOID { get; set; }
        [Display(Name = "PurchaseOrderID", ResourceType = typeof(AppResource))]
        public string PurchaseOrderID { get; set; }
        public string SupplierCode { get; set; }
        public string EmployeeID { get; set; }
        [Display(Name = "ReceiverName", ResourceType = typeof(AppResource))]
        [Required(ErrorMessage = "{0}為必填")]
        public string ReceiverName { get; set; }
        [StringLength(30, ErrorMessage = "{0}最長{1}個字")]
        [Display(Name = "ReceiverTel", ResourceType = typeof(AppResource))]
        public string ReceiverTel { get; set; }
        [StringLength(30, ErrorMessage = "{0}最長{1}個字")]
        [Display(Name = "ReceiverMobile", ResourceType = typeof(AppResource))]
        public string ReceiverMobile { get; set; }
        [StringLength(256, ErrorMessage ="{0}最長{1}個字")]
        [Required(ErrorMessage ="{0}為必填")]
        [Display(Name = "ReceiptAddress", ResourceType = typeof(AppResource))]
        public string ReceiptAddress { get; set; }
        [Display(Name = "CreateDate", ResourceType = typeof(AppResource))]
        public System.DateTime CreateDate { get; set; }
        [Display(Name = "PurchaseOrderStatus", ResourceType = typeof(AppResource))]
        public string PurchaseOrderStatus { get; set; }
        [Display(Name = "SignStatus", ResourceType = typeof(AppResource))]
        public string SignStatus { get; set; }
        public System.Nullable<int> SignFlowOID { get; set; }
    }
}