using PMSAWebMVC.Models;
using PMSAWebMVC.Utilities.YaChen;
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
        [Display(Name = "簽核狀態")]
        public string SignStatus { get; set; }
        [Display(Name = "簽核")]
        public string SignStatusToShow
        {
            get
            {
                return RepositoryUtils.GetSignStatus(SignStatus);
            }
            private set { }
        }
    }

    public partial class Repository
    {
        /// <summary>
        /// 取得主畫面採購單資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PurchaseOrderIndexViewModel> GetPurchaseOrderListViewModel()
        {
            //採購主管可以查屬下的採購單
            List<string> empid = null;
            if (emp.Title == "採購主管")
            {
                empid = db.Employee.Where(e => e.ManagerID == emp.EmployeeID).Select(e => e.EmployeeID).ToList();
            }
            else
            {
                empid = new List<string> { emp.EmployeeID };
            }
            //請購單可能會無關聯
            var povm = from po in (from poin in db.PurchaseOrder
                                   where empid.Any(id => id == poin.EmployeeID)
                                   select poin
                                    ).AsEnumerable()
                       join si in db.SupplierInfo
                       on new { po.SupplierCode } equals new { si.SupplierCode }
                       join rel in db.PRPORelation
                       on po.PurchaseOrderID equals rel.PurchaseOrderID into rels
                       from rel in rels.DefaultIfEmpty()
                       group new { po, si, rel } by new
                       {
                           po.PurchaseOrderID,
                           po.CreateDate,
                           si.SupplierName,
                           rel.PurchaseRequisitionID,
                           po.PurchaseOrderStatus,
                           po.SignStatus
                       } into gp
                       orderby gp.Key.PurchaseOrderID descending
                       select new PurchaseOrderIndexViewModel
                       {
                           PurchaseOrderID = gp.Key.PurchaseOrderID,
                           CreateDate = gp.Key.CreateDate,
                           SupplierName = gp.Key.SupplierName,
                           PurchaseRequisitionID = gp.Key.PurchaseRequisitionID,
                           PurchaseOrderStatus = RepositoryUtils.GetStatus(gp.Key.PurchaseOrderStatus),
                           SignStatus = gp.Key.SignStatus
                       };
            return povm;
        }

        

    }
}