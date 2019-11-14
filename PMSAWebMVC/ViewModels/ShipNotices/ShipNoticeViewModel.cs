using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.ViewModels.ShipNotices
{
    public class PurchaseOrderViewModel
    {
        public int PurchaseOrderOID { get; set; }
        public string PurchaseOrderID { get; set; }
        public string SupplierCode { get; set; }
        public string EmployeeID { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverTel { get; set; }
        public string ReceiverMobile { get; set; }
        public string ReceiptAddress { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string PurchaseOrderStatus { get; set; }
        public string SignStatus { get; set; }
        public Nullable<int> SignFlowOID { get; set; }
        public string failMessage { get; set; }
    }
}