using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    public class SupplierInfoMetadata
    {
        [Display(Name = "供應商名稱")]
        public string SupplierName { get; set; }

        public string SupplierCode { get; set; }
        public string TaxID { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public Nullable<int> SupplierRatingOID { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseOrder> PurchaseOrder { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseOrderReceive> PurchaseOrderReceive { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseOrderTemp> PurchaseOrderTemp { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseRequisitionDtl> PurchaseRequisitionDtl { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseRequisitionDtlTemp> PurchaseRequisitionDtlTemp { get; set; }

        [JsonIgnore]
        public virtual ICollection<SourceList> SourceList { get; set; }

        [JsonIgnore]
        public virtual ICollection<SupplierAccount> SupplierAccount { get; set; }

        [JsonIgnore]
        public virtual SupplierRating SupplierRating { get; set; }
    }
}