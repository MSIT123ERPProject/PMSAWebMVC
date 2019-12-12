using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.ViewModels.Setting
{
    public class SupSettingViewModel
    {
        //TODO 待補 remote
        [Display(Name = "供應商帳號")]
        //[Remote("AccountCheck", "Members", ErrorMessage = "此帳號已被註冊過")]
        public string SupplierAccountID { get; set; }

        [Display(Name = "聯絡人姓名")]
        [Required(ErrorMessage = "請輸入姓名")]
        public string ContactName { get; set; }

        [Display(Name = "聯絡人信箱")]
        [Required(ErrorMessage = "請輸入電子信箱")]
        public string Email { get; set; }

        [Display(Name = "聯絡人手機")]
        public string Mobile { get; set; }

        [Display(Name = "聯絡人市話")]
        public string Tel { get; set; }

        [Display(Name = "是否啟用雙因素驗證")]
        public bool EnableTwoFactorAuth { get; set; }
    }
}