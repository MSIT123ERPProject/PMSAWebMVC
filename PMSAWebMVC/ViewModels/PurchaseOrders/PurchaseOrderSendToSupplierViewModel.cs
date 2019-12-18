using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.YaChen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using static PMSAWebMVC.ViewModels.PurchaseOrders.POSendToSupplierViewModel;

namespace PMSAWebMVC.ViewModels.PurchaseOrders
{
    /// <summary>
    /// 檢視ViewModel
    /// </summary>
    public class POSendToSupplierViewModel
    {
        /// <summary>
        /// 交易記錄
        /// </summary>
        public class POChangedRecordViewModel
        {
            public int POChangedOID { get; set; }
            public string PurchaseOrderID { get; set; }
            /// <summary>
            /// 異動分類代碼
            /// </summary>
            public string POChangedCategoryCode { get; set; }
            public string POChangedCategoryCodeToShow
            {
                get
                {
                    return RepositoryUtils.GetStatus(POChangedCategoryCode);
                }
                private set { }
            }
            /// <summary>
            /// 異動提出日期
            /// </summary>
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}")]
            public System.DateTime RequestDate { get; set; }
            /// <summary>
            /// 提出角色
            /// </summary>
            public string RequesterRole { get; set; }
            public string RequesterRoleToShow
            {
                get
                {
                    switch (this.RequesterRole)
                    {
                        case "P":
                            return "採購方";
                        case "S":
                            return "供應方";
                        case "A":
                            return "系統";
                        default:
                            break;
                    }
                    return "";
                }
                private set { }
            }
            /// <summary>
            /// 角色帳號
            /// </summary>
            public string RequesterID { get; set; }
            public string RequesterName { get; set; }
            /// <summary>
            /// 採購單明細編號
            /// </summary>
            public string PurchaseOrderDtlCode { get; set; }
            public Nullable<int> Qty { get; set; }
            public Nullable<System.DateTime> DateRequired { get; set; }
            /// <summary>
            /// 交易記錄字串，需人工組合
            /// </summary>
            public string Record { get; set; }
        }

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
            /// <summary>
            /// 交易記錄：檢視使用          
            /// </summary>
            public IList<POChangedRecordViewModel> POChangedItems { get; set; }
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
            public Nullable<int> SignFlowDtlOID { get; set; }
            [Display(Name = "簽核人姓名")]
            public string ApprovingOfficerName { get; set; }
            [Display(Name = "簽核意見")]
            public string SignOpinion { get; set; }
            [Display(Name = "簽核身份驗證密碼")]
            public string SignPassword { get; set; }
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
            var pocs = db.POChanged.Where(item => item.PurchaseOrderID == purchaseOrderID)
                .OrderByDescending(item => item.POChangedOID).ToList();
            var POChangedItems = new List<POChangedRecordViewModel>();

            foreach (var item in pocs)
            {
                var poc = new POChangedRecordViewModel
                {
                    POChangedCategoryCode = item.POChangedCategoryCode,
                    RequestDate = item.RequestDate,
                    RequesterRole = item.RequesterRole,
                    RequesterID = item.RequesterID,
                    RequesterName = item.RequesterID == emp.EmployeeID ? emp.Name :
                    RepositoryUtils.GetAccountName(item.RequesterID, item.RequesterRole, db),
                    PurchaseOrderDtlCode = item.PurchaseOrderDtlCode,
                    Qty = item.Qty,
                    DateRequired = item.DateRequired
                };
                POChangedItems.Add(poc);
            }
            StringBuilder sb = new StringBuilder();
            for (int i = POChangedItems.Count - 1; i >= 0; i--)
            {
                sb.Clear();
                sb.Append(POChangedItems[i].POChangedCategoryCodeToShow);
                if (POChangedItems[i].PurchaseOrderDtlCode != null)
                {
                    sb.Append("<br/>").Append("明細編號：").Append(POChangedItems[i].PurchaseOrderDtlCode).Append("<br/>")
                        .Append("數量：").Append(POChangedItems[i].Qty).Append("<br/>")
                         .Append("要求到貨日期：").Append(POChangedItems[i].DateRequired.Value
                         .ToString("yyyy/MM/dd")).Append("<br/>");
                }
                POChangedItems[i].Record = sb.ToString();
            }

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
                PODItemsAggregate = poditems.Sum(item => item.Total),
                POChangedItems = POChangedItems
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
                PurchaseOrderStatus = RepositoryUtils.GetStatus(po.PurchaseOrderStatus),
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

            //寫入簽核內容
            SignFlowDtl sfd = null;
            if (po.SignFlowOID.HasValue)
            {
                sfd = db.SignFlowDtl
                    .Where(item => item.SignFlowOID == po.SignFlowOID)
                    .OrderByDescending(item => item.SignFlowDtlOID)
                    .FirstOrDefault();
            }
            if (sfd != null)
            {
                povm.ApprovingOfficerName = db.Employee.Find(sfd.ApprovingOfficerID).Name;
                povm.SignFlowDtlOID = sfd.SignFlowDtlOID;
            }

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