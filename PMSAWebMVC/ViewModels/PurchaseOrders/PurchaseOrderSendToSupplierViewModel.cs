using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMSAWebMVC.ViewModels.PurchaseOrders.POSendToSupplierViewModel;

namespace PMSAWebMVC.ViewModels.PurchaseOrders
{
    /// <summary>
    /// 檢視ViewModel
    /// </summary>
    public class POSendToSupplierViewModel
    {
        public class SendToSupplierViewModel
        {
            public POInfoViewModel POItem { get; set; }
        }

        public class POInfoViewModel
        {
            [Key]
            [Display(Name = "採購單號")]
            public string PurchaseOrderID { get; set; }
            [Display(Name = "採購日期")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
            public DateTime CreateDate { get; set; }
            [Display(Name = "來自請購單號")]
            public string PurchaseRequisitionID { get; set; }
            [Display(Name = "狀態")]
            public string PurchaseOrderStatus { get; set; }
            [Display(Name = "採購人員")]
            public string Buyer { get; set; }
            public string EmployeeID { get; set; }
            public string SupplierCode { get; set; }
            [Display(Name = "收貨人姓名")]
            public string ReceiverName { get; set; }
            [Display(Name = "市話")]
            public string ReceiverTel { get; set; }
            [Display(Name = "手機")]
            public string ReceiverMobile { get; set; }
            [Display(Name = "收貨地址")]
            public string ReceiptAddress { get; set; }
            [Display(Name = "簽核狀態")]
            public string SignStatus { get; set; }
            public Nullable<int> SignFlowOID { get; set; }
        }
    }


    public partial class Repository
    {
        /// <summary>
        /// 送出至供應商畫面
        /// </summary>
        /// <param name="purchaseOrderID">採購單編號</param>
        /// <returns></returns>
        public SendToSupplierViewModel GetPOSendToSupplierViewModel(string purchaseOrderID)
        {
            var poitem = GetPOInfoViewModel(purchaseOrderID);
            SendToSupplierViewModel po = new SendToSupplierViewModel
            {
                POItem = poitem
            };
            return po;
        }

        /// <summary>
        /// 取得採購單資訊
        /// </summary>
        /// <param name="purchaseOrderID">採購單編號</param>
        /// <returns></returns>
        public POInfoViewModel GetPOInfoViewModel(string purchaseOrderID)
        {
            PurchaseOrder po = db.PurchaseOrder.Find(purchaseOrderID);
            PRPORelation rel = po.PRPORelation.Where(item => item.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
            var povm = new POInfoViewModel
            {
                PurchaseOrderID = po.PurchaseOrderID,
                CreateDate = po.CreateDate,
                PurchaseRequisitionID = rel.PurchaseRequisitionID,
                PurchaseOrderStatus = GetStatus(po.PurchaseOrderStatus),
                Buyer = po.Employee.Name,
                EmployeeID = po.EmployeeID,
                SupplierCode = po.SupplierCode,
                ReceiverName = po.ReceiverName,
                ReceiverTel = po.ReceiverTel,
                ReceiverMobile = po.ReceiverMobile,
                ReceiptAddress = po.ReceiptAddress,
                SignStatus = po.SignStatus,
                SignFlowOID = po.SignFlowOID
            };
            return povm;
        }

    }


}