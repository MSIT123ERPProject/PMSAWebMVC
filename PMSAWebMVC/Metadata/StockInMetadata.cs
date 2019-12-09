using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    public class StockInMetadata
    {
        [Display(Name = "編號")]
        public int StockInOID { get; set; }

        [Display(Name = "入庫單號")]
        public string StockInID { get; set; }

        [Display(Name = "進貨單號")]
        public string PurchaseOrderReceiveID { get; set; }

        [Display(Name = "單據備註")]
        public string Remark { get; set; }

        [Display(Name = "加庫存日期")]
        public Nullable<System.DateTime> AddStockDate { get; set; }

        [Display(Name = "建檔人員")]
        public string CreateEmployeeID { get; set; }

        [Display(Name = "建檔日期")]
        public System.DateTime CreateDate { get; set; }

        [Display(Name = "簽核狀態代碼")]
        public string SignStatus { get; set; }

        [Display(Name = "簽核流程總表識別碼")]
        public Nullable<int> SignFlowOID { get; set; }
    }
}