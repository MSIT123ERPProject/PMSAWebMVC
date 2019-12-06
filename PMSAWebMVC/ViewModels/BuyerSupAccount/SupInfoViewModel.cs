using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.ViewModels.BuyerSupAccount
{
    public class SupInfoViewModel
    {
        [DisplayName("供應商名稱")]
        [Required(ErrorMessage = "{0}為必填欄位")]
        public string SupplierName { get; set; }

        [DisplayName("供應商代碼")]
        public string SupplierCode { get; set; }

        [DisplayName("統一編號")]
        [Required(ErrorMessage = "{0}為必填欄位")]
        public string TaxID { get; set; }

        [DisplayName("信箱")]
        [Required(ErrorMessage = "{0}為必填欄位")]
        public string Email { get; set; }

        [DisplayName("市話")]
        [Required(ErrorMessage = "{0}為必填欄位")]
        public string Tel { get; set; }

        [DisplayName("地址")]
        [Required(ErrorMessage = "{0}為必填欄位")]
        public string Address { get; set; }

        [DisplayName("供應商等級")]
        [Required(ErrorMessage = "{0}為必填欄位")]
        public List<SelectListItem> SupplierRatingOID { get; set; }
    }
}