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
            /// <summary>
            /// 採購單主表
            /// </summary>
            public POInfoViewModel POItem { get; set; }
            /// <summary>
            /// 供應商資訊
            /// </summary>
            public SUPInfoViewModel SUPItem { get; set; }
            /// <summary>
            /// 採購單明細
            /// </summary>
            public IEnumerable<PurchaseOrderDtlItem> PODItems { get; set; }
            [DisplayFormat(DataFormatString = "{0:C0}")]
            [DataType(DataType.Currency)]
            [Display(Name = "合計金額")]
            public int PODItemsAggregate { get; set; }
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
            [Display(Name = "電子信箱")]
            public string Email { get; set; }
            [Display(Name = "聯絡電話")]
            public string Tel { get; set; }
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
            var si = db.SupplierInfo.Find(poitem.SupplierCode);
            var supacc = si.SupplierAccount.FirstOrDefault();

            var supitem = new SUPInfoViewModel
            {
                SupplierName = si.SupplierName,
                ContactName = supacc.ContactName,
                Email = si.Email,
                Tel = si.Tel
            };

            var poditems = GetPODItemsViewModel(purchaseOrderID);

            SendToSupplierViewModel po = new SendToSupplierViewModel
            {
                POItem = poitem,
                SUPItem = supitem,
                PODItems = poditems,
                PODItemsAggregate = poditems.Sum(item => item.Total)
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
            Employee emp = po.Employee;
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
                SignFlowOID = po.SignFlowOID,
                Tel = emp.Tel,
                Email = emp.Email
            };
            return povm;
        }

        /// <summary>
        /// 採購明細
        /// </summary>
        /// <param name="purchaseOrderID"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseOrderDtlItem> GetPODItemsViewModel(string purchaseOrderID)
        {
            var pods = db.PurchaseOrderDtl.
                Where(item => item.PurchaseOrderID == purchaseOrderID).
                Select(item => new PurchaseOrderDtlItem
                {
                    PartNumber = item.PartNumber,
                    PartName = item.PartName,
                    OriginalUnitPrice = item.OriginalUnitPrice,
                    TotalSourceListQty = item.TotalSourceListQty,
                    Qty = item.Qty,
                    Discount = item.Discount,
                    Total = item.Total.Value,
                    DateRequired = item.DateRequired.Value,
                    SourceListID = item.SourceListID,
                    //PurchaseOrderOID = item.PurchaseOrderOID,
                    PartSpec = item.PartSpec,
                    QtyPerUnit = item.QtyPerUnit,
                    TotalPartQty = item.TotalPartQty,
                    PurchaseUnitPrice = item.PurchaseUnitPrice,
                    PurchasedQty = item.PurchasedQty,
                    GoodsInTransitQty = item.GoodsInTransitQty,
                    CommittedArrivalDate = item.CommittedArrivalDate,
                    ShipDate = item.ShipDate,
                    ArrivedDate = item.ArrivedDate,
                    POChangedOID = item.POChangedOID,
                    //PurchaseRequisitionDtlCode = item.PurchaseRequisitionDtlCode,
                    PurchaseOrderDtlOID = item.PurchaseOrderDtlOID,
                    PurchaseOrderDtlCode = item.PurchaseOrderDtlCode,
                    PurchaseOrderID = purchaseOrderID,
                    LastModifiedAccountID = item.LastModifiedAccountID
                });
            return pods;
        }

    }


}