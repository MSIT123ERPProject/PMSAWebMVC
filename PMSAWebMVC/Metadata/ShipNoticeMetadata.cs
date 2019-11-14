using PMSAWebMVC.Resources;
using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class ShipNoticeMetadata
    {
        
        public int ShipNoticeOID { get; set; }

        public string ShipNoticeID { get; set; }
        [Display(Name = "PurchaseOrderID",ResourceType =typeof(AppResource))]
        public string PurchaseOrderID { get; set; }
        [Display(Name = "ShipDate", ResourceType = typeof(AppResource))]
        public System.DateTime ShipDate { get; set; }
        [Display(Name = "EmployeeID", ResourceType = typeof(AppResource))]
        public string EmployeeID { get; set; }
        [Display(Name = "CompanyCode", ResourceType = typeof(AppResource))]
        public string CompanyCode { get; set; }
        [Display(Name = "SupplierAccountID", ResourceType = typeof(AppResource))]
        public string SupplierAccountID { get; set; }

    }
}