using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    public class PurchaseOrderReceiveMetadata
    {
        [Display(Name = "編號")]
        public int PurchaseOrderReceiveOID { get; set; }

        [Display(Name = "進貨單號")]
        public string PurchaseOrderReceiveID { get; set; }

        [Display(Name = "採購單編號")]
        public string PurchaseOrderID { get; set; }

        [Display(Name = "進貨時間")]
        public System.DateTime PurchaseDate { get; set; }

        [Display(Name = "進貨人員")]
        public string PurchaseEmployeeID { get; set; }

        [Display(Name = "進貨廠商代碼")]
        public string SupplierCode { get; set; }

        [Display(Name = "廠商聯絡人帳號")]
        public string SupplierAccountID { get; set; }

        [Display(Name = "簽核狀態代碼")]
        public string SignStatus { get; set; }

        [Display(Name = "簽核流程總表識別碼")]
        public Nullable<int> SignFlowOID { get; set; }
    }
}