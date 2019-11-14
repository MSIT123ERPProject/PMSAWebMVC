using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    public class InventoryDtlMetadata
    {
        [Display(Name = "識別碼")]
        public int InventoryDtlOID { get; set; }

        [Display(Name = "庫存編號")]
        public string InventoryCode { get; set; }

        [Display(Name = "倉庫代碼")]
        public string WarehouseCode { get; set; }

        [Display(Name = "庫存分類代碼")]
        public string InventoryCategoryCode { get; set; }

        [Display(Name = "貨源清單代碼")]
        public string SourceListID { get; set; }

        [Display(Name = "料件編號")]
        public string PartNumber { get; set; }

        [Display(Name = "庫存數量")]
        public int UnitsInStock { get; set; }

        [Display(Name = "出庫申請數量")]
        public int UnitsOnStockOutOrder { get; set; }

        [Display(Name = "入庫申請數量")]
        public int UnitsOnStockInOrder { get; set; }

        [Display(Name = "安全庫存量")]
        public Nullable<int> SafetyQty { get; set; }

        [Display(Name = "建檔時間")]
        public System.DateTime CreateDate { get; set; }

        [Display(Name = "建檔人員")]
        public string CreateEmployeeID { get; set; }

        [Display(Name = "最後修改時間")]
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [Display(Name = "最後修改人員")]
        public string LastModifiedEmployeeID { get; set; }
    }
}