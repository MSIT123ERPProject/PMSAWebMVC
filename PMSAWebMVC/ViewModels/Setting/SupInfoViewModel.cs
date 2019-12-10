using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.ViewModels.Setting
{
    public class SupInfoViewModel
    {
        [Display(Name = "供應商名稱")]
        public string SupplierName { get; set; }

        [Display(Name = "統一編號")]
        public string TaxID { get; set; }

        [Display(Name = "公司信箱")]
        public string Email { get; set; }

        [Display(Name = "公司市話")]
        public string Tel { get; set; }

        [Display(Name = "公司地址")]
        public string Address { get; set; }
    }
}