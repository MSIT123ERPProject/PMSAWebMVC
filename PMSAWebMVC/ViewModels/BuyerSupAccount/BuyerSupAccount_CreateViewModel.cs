using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.ViewModels.BuyerSupAccount
{
    public class BuyerSupAccount_CreateViewModel
    {
        [DisplayName("供應商名稱")]
        public string SupplierName { get; set; }

        [DisplayName("供應商代碼")]
        public string SupplierCode { get; set; }

        [DisplayName("聯絡人")]
        public string ContactName { get; set; }

        [DisplayName("信箱")]
        public string Email { get; set; }

        [DisplayName("手機")]
        public string Mobile { get; set; }

        [DisplayName("市話")]
        public string Tel { get; set; }

        [DisplayName("帳號狀態")]
        public bool AccountStatus { get; set; }
    }
}