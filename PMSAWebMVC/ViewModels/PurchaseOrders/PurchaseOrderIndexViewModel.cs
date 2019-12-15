using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.ViewModels.PurchaseOrders
{
    public class PurchaseOrderIndexViewModel
    {
        [Key]
        [Display(Name = "採購單號")]
        public string PurchaseOrderID { get; set; }
        [Display(Name = "採購日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "供應商名稱")]
        public string SupplierName { get; set; }
        [Display(Name = "來自請購單號")]
        public string PurchaseRequisitionID { get; set; }
        [Display(Name = "狀態")]
        public string PurchaseOrderStatus { get; set; }
    }

    public partial class Repository
    {
        /// <summary>
        /// 取得主畫面採購單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PurchaseOrderIndexViewModel> GetPurchaseOrderListViewModel()
        {
            //請購單可能會無關聯
            var povm = from po in db.PurchaseOrder.AsEnumerable()
                       join si in db.SupplierInfo
                       on new { po.SupplierCode, po.EmployeeID } equals new { si.SupplierCode, emp.EmployeeID }
                       join rel in db.PRPORelation
                       on po.PurchaseOrderID equals rel.PurchaseOrderID into rels
                       from rel in rels.DefaultIfEmpty()
                       group new { po, si, rel } by new
                       {
                           po.PurchaseOrderID,
                           po.CreateDate,
                           si.SupplierName,
                           rel.PurchaseRequisitionID,
                           po.PurchaseOrderStatus
                       } into gp
                       orderby gp.Key.PurchaseOrderID descending
                       select new PurchaseOrderIndexViewModel
                       {
                           PurchaseOrderID = gp.Key.PurchaseOrderID,
                           CreateDate = gp.Key.CreateDate,
                           SupplierName = gp.Key.SupplierName,
                           PurchaseRequisitionID = gp.Key.PurchaseRequisitionID,
                           PurchaseOrderStatus = GetStatus(gp.Key.PurchaseOrderStatus)
                       };
            return povm;
        }


        private string GetStatus(string purchaseOrderStatus)
        {
            //N = 新增,P = 送出,C = 異動中,E = 答交,D = 整筆訂單取消,S = 出貨,R = 點交,O = 逾期,Z = 結案
            switch (purchaseOrderStatus)
            {
                case "N":
                    return "新增";
                case "P":
                    return "送出";
                case "C":
                    return "異動中";
                case "E":
                    return "答交";
                case "W":
                    return "雙方答交";
                case "D":
                    return "取消";
                case "S":
                    return "出貨";
                case "R":
                    return "點交";
                case "O":
                    return "逾期";
                case "Z":
                    return "結案";
                default:
                    return "";
            }
        }

    }
}