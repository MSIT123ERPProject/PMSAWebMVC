using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.ViewModels.BuyerSupAccount
{
    public class SupCompanyInfoViewModel
    {
        [DisplayName("供應商代碼")]
        public string SupplierCode { get; set; }

        [DisplayName("供應商名稱")]
        public string SupplierName { get; set; }

        [DisplayName("統一編號")]
        public string TaxID { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }

        [DisplayName("信箱")]
        public string Email { get; set; }

        [DisplayName("市話")]
        public string Tel { get; set; }

        [DisplayName("供應商等級")]
        public string SupplierRatingOID { get; set; }
    }
}