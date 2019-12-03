using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    public class StockInDtlMetadata
    {
        [Display(Name ="編號")]
        public int StockInDtlOID { get; set; }

        [Display(Name = "入庫單號")]
        public string StockInID { get; set; }

        [Display(Name = "庫存編號")]
        public string InventoryCode { get; set; }

        [Display(Name = "料件編號")]
        public string PartNumber { get; set; }

        [Display(Name = "入庫數量")]
        public int StockInQty { get; set; }

        [Display(Name = "保存期限")]
        public Nullable<System.DateTime> EXP { get; set; }

        [Display(Name = "備註")]
        public string Remark { get; set; }
    }
}