using PMSAWebMVC.Controllers;
using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMSAWebMVC.ViewModels.PurchaseRequisitions.PurchaseRequisitionSendToSupplierViewModel;

namespace PMSAWebMVC.ViewModels.PurchaseRequisitions
{
    public class PurchaseRequisitionSendToSupplierViewModel 
    {
        /// <summary>
        /// 交易記錄
        /// </summary>
        public class PRChangedRecordViewModel
        {
            PurchaseRequisitionsController purchaseRequisitionsController = new PurchaseRequisitionsController();
            public int PRChangedOID { get; set; }
            public string PurchaseRequisitionID { get; set; }
            /// <summary>
            /// 異動分類代碼
            /// </summary>
            public string PRChangedCategoryCode { get; set; }
            public string PRChangedCategoryCodeToShow
            {
                get
                { 
                    return purchaseRequisitionsController.GetSignStatus(PRChangedCategoryCode);
                }
                private set { }
            }
            /// <summary>
            /// 異動提出日期
            /// </summary>
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}")]
            public System.DateTime RequestDate { get; set; }
            ///// <summary>
            ///// 提出角色
            ///// </summary>
            //public string RequesterRole { get; set; }
            //public string RequesterRoleToShow
            //{
            //    get
            //    {
            //        switch (this.RequesterRole)
            //        {
            //            case "P":
            //                return "採購方";
            //            case "S":
            //                return "供應方";
            //            case "A":
            //                return "系統";
            //            default:
            //                break;
            //        }
            //        return "";
            //    }
            //    private set { }
            //}
            /// <summary>
            /// 角色帳號
            /// </summary>
            public string RequesterID { get; set; }
            public string RequesterName { get; set; }
            /// <summary>
            /// 請購單明細編號
            /// </summary>
            public string PurchaseRequisitionDtlCode { get; set; }
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
            public PRInfoViewModel PRItem { get; set; }
            ///// <summary>
            ///// 供應商資訊
            ///// </summary>
            //public SUPInfoViewModel SUPItem { get; set; }
            /// <summary>
            /// 採購單明細
            /// </summary>
            public IEnumerable<PurchaseRequisitionDtlItem> PRDItems { get; set; }
            //[DisplayFormat(DataFormatString = "{0:C0}")]
            //[DataType(DataType.Currency)]
            //[Display(Name = "合計金額")]
            //public int PODItemsAggregate { get; set; }
            /// <summary>
            /// 交易記錄：檢視使用          
            /// </summary>
            public IList<PRChangedRecordViewModel> PRChangedItems { get; set; }
        }

        public class PRInfoViewModel
        {
            [Key]
            [Display(Name = "請購單編號")]
            public string PurchaseRequisitionID { get; set; }//請購單識別碼
            [Display(Name = "產品編號")]
            public string ProductNumber { get; set; }//產品料件編號
            [Display(Name = "產品名稱")]
            public string ProductName { get; set; }
            [Display(Name = "員工編號")]
            public string EmployeeID { get; set; }
            [Display(Name = "員工姓名")]
            public string EmployeeName { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
            [DataType(DataType.Date)]
            [Display(Name = "產生日期")]
            public DateTime PRBeginDate { get; set; }
            [Display(Name = "處理狀態")]
            public string ProcessStatus { get; set; }
            [Display(Name = "簽核狀態")]
            public string SignStatus { get; set; }
            [Display(Name = "簽核流程表識別碼")]
            public int? SignFlowOID { get; set; }
            public string Buyer { get; set; }

            [Display(Name = "請購單明細識別碼")]
            public int PurchaseRequisitionDtlOID { get; set; }//請購單明細識別碼
            [Display(Name = "料件編號")]
            public string PartNumber { get; set; }
            [Display(Name = "料件名稱")]
            public string PartName { get; set; }
            [Display(Name = "請購數量")]
            public int Qty { get; set; }
            [Display(Name = "建議供應商名稱")]
            public string SupplierName { get; set; }
            [Display(Name = "建議供應商編號")]
            public string SuggestSupplierCode { get; set; }
            [Display(Name = "需求日期")]
            public DateTime DateRequired { get; set; }//需求日期
            [DisplayFormat(DataFormatString = "{0:C0}")]
            [DataType(DataType.Currency)]
            [Display(Name = "單價")]
            public int OriginalUnitPrice { get; set; }
            [Display(Name = "主管簽核")]
            public string SignStatusToShow
            {
                get
                {
                    switch (SignStatus)
                    {
                        case "Y":
                            return "同意";
                        case "N":
                            return "拒絕";
                        case "S":
                            return "等待簽核中";
                        default:
                            return "";
                    }
                }
                private set { }
            }
            //public Nullable<int> SignFlowOID { get; set; }
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
        /// <param name="purchaseRequisitionID">請購單編號</param>
        /// <returns></returns>
        //public SendToSupplierViewModel GetPRSendViewModel(string purchaseRequisitionID)
        //{
        //    var pr= GetPRInfoViewModel(purchaseRequisitionID);
        //    var prt = GetPODItemsViewModel(purchaseRequisitionID);




        //}

        /// <summary>
        /// 取得請購單資訊
        /// </summary>
        /// <param name="purchaseOrderID">請購單編號</param>
        /// <returns></returns>
        public PRInfoViewModel GetPRInfoViewModel(string purchaseRequisitionID)
        {
            
        PurchaseRequisition pr = db.PurchaseRequisition.Find(purchaseRequisitionID);
            PRPORelation rel = pr.PRPORelation.Where(item => item.PurchaseRequisitionID == purchaseRequisitionID).FirstOrDefault();
            Employee emp = pr.Employee;
            PurchaseRequisitionsController purchaseRequisitionsController = new PurchaseRequisitionsController();
            var povm = new PRInfoViewModel
            {
                PurchaseRequisitionID = pr.PurchaseRequisitionID,
                PRBeginDate = pr.PRBeginDate,
               
                ProcessStatus = purchaseRequisitionsController.GetProcessStatus(pr.ProcessStatus),
                Buyer = pr.Employee.Name,
                EmployeeID = pr.EmployeeID,
                ProductNumber = pr.ProductNumber,
                SignStatus = pr.SignStatus,
                SignFlowOID = pr.SignFlowOID,
                Tel = emp.Tel,
                Email = emp.Email
            };

            //寫入簽核內容
            SignFlowDtl sfd = null;
            if (pr.SignFlowOID.HasValue)
            {
                sfd = db.SignFlowDtl
                    .Where(item => item.SignFlowOID == pr.SignFlowOID)
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
        /// 請購明細
        /// </summary>
        /// <param name="purchaseOrderID"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseRequisitionDtlItem> GetPODItemsViewModel(string purchaseRequisitionID)
        {
            var pods = db.PurchaseRequisitionDtl.
                Where(item => item.PurchaseRequisitionID == purchaseRequisitionID).
                Select(item => new PurchaseRequisitionDtlItem
                {
                    PurchaseRequisitionID = item.PurchaseRequisitionID,
                    PurchaseRequisitionDtlOID = item.PurchaseRequisitionDtlOID,
                    PartNumber = item.PartNumber,
                    PartName = item.Part.PartName,
                    Qty=item.Qty,
                    SuggestSupplierCode=item.SuggestSupplierCode,
                    DateRequired=item.DateRequired,
                });
            return pods;
        }
    }
}