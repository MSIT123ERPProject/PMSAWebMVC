using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    public class PurchaseOrderReceiveDtlMetadata
    {
        [Display(Name = "編號")]
        public int PurchaseOrderReceiveDtlOID { get; set; }

        [Display(Name = "進貨明細編號")]
        public string PurchaseOrderReceiveDtlCode { get; set; }

        [Display(Name = "進貨單號")]
        public string PurchaseOrderReceiveID { get; set; }

        [Display(Name = "採購單明細編號")]
        public string PurchaseOrderDtlCode { get; set; }

        [Display(Name = "本次進貨數")]
        public int PurchaseQty { get; set; }

        [Display(Name = "本次進貨金額")]
        public int PurchaseAmount { get; set; }

        [Display(Name = "驗退數量")]
        public int RejectQty { get; set; }

        [Display(Name = "可入庫數量")]
        public int AcceptQty { get; set; }

        [Display(Name = "驗退原因")]
        public string RejectReason { get; set; }

        [Display(Name = "備註")]
        public string Remark { get; set; }
    }
}