using PMSAWebMVC.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class SourceListMetadata
    {
        [Display(Name = "SourceListOID",ResourceType =typeof(AppResource))]
        public int SourceListOID { get; set; }
        [Display(Name = "SourceListID",ResourceType =typeof(AppResource))]
        public string SourceListID { get; set; }
        [Display(Name = "PartNumber", ResourceType = typeof(AppResource))]
        public string PartNumber { get; set; }
        [Display(Name = "QtyPerUnit", ResourceType = typeof(AppResource))]
        public int QtyPerUnit { get; set; }
        public Nullable<int> MOQ { get; set; }
        [Display(Name = "UnitPrice", ResourceType = typeof(AppResource))]
        public int UnitPrice { get; set; }
        [Display(Name = "SupplierCode", ResourceType = typeof(AppResource))]
        public string SupplierCode { get; set; }
        [Display(Name = "UnitsInStock", ResourceType = typeof(AppResource))]
        public int UnitsInStock { get; set; }
        [Display(Name = "UnitsOnOrder", ResourceType = typeof(AppResource))]
        public int UnitsOnOrder { get; set; }
        [Display(Name = "SafetyQty", ResourceType = typeof(AppResource))]
        public Nullable<int> SafetyQty { get; set; }
        [Display(Name = "EXP", ResourceType = typeof(AppResource))]
        public Nullable<int> EXP { get; set; }


    }
}