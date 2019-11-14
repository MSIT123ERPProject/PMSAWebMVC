
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    public class WarehouseInfoMetadata
    {
        [Display(Name = "編號")]
        public int WarehouseInfoOID { get; set; }
        
        [Display(Name = "倉庫代碼")]
        [Required(ErrorMessage = "{0} 不可為空值!!!")]
        public string WarehouseCode { get; set; }
        
        [Display(Name = "倉庫名稱")]
        [Required(ErrorMessage = "{0} 不可為空值!!!")]
        public string WarehouseName { get; set; }
        
        [Display(Name = "倉庫地址")]
        [Required(ErrorMessage = "{0} 不可為空值!!!")]
        public string Address { get; set; }

        [Display(Name = "聯絡人")]
        public string EmployeeID { get; set; }

        [Display(Name = "電話")]
        public string Tel { get; set; }

        [Display(Name = "備註")]
        public string Remark { get; set; }
    }
}