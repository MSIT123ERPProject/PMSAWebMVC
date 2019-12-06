using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    [MetadataType(typeof(SupplierAccountMetadata))]
    public partial class SupplierAccount
    {
    }

    public partial class SupplierAccountMetadata
    {
        public int SupplierAccountOID { get; set; }
        public string SupplierAccountID { get; set; }
        public string ContactName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string SupplierCode { get; set; }
        public string AccountStatus { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreatorEmployeeID { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string SendLetterStatus { get; set; }
        public Nullable<System.DateTime> SendLetterDate { get; set; }
        public string AuthenticateToken { get; set; }
        public Nullable<System.DateTime> TokenCreateTime { get; set; }
        public Nullable<bool> EnableTwoFactorAuth { get; set; }

        [JsonIgnore]
        public virtual Employee Employee { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseOrderReceive> PurchaseOrderReceive { get; set; }

        [JsonIgnore]
        public virtual ICollection<ShipNotice> ShipNotice { get; set; }

        [JsonIgnore]
        public virtual SupplierInfo SupplierInfo { get; set; }
    }
}