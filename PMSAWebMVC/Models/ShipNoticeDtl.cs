//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMSAWebMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ShipNoticeDtl
    {
        public int ShipNoticeDtlOID { get; set; }
        public string ShipNoticeID { get; set; }
        public string PurchaseOrderDtlCode { get; set; }
        public int ShipQty { get; set; }
        public int ShipAmount { get; set; }
        public Nullable<System.DateTime> EXP { get; set; }
    
        public virtual PurchaseOrderDtl PurchaseOrderDtl { get; set; }
        public virtual ShipNotice ShipNotice { get; set; }
    }
}
