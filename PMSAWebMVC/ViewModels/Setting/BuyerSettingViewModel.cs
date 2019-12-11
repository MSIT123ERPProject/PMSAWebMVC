using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.ViewModels.Setting
{
    public class BuyerSettingViewModel
    {
        //TODO 待補 remote
        [Display(Name = "員工編號(帳號)")]
        //[Remote("AccountCheck", "Members", ErrorMessage = "此帳號已被註冊過")]
        public string EmployeeID { get; set; }

        [Display(Name = "採購員姓名")]
        [Required(ErrorMessage = "請輸入姓名")]
        public string Name { get; set; }

        [Display(Name = "電子信箱")]
        [Required(ErrorMessage = "請輸入電子信箱")]
        public string Email { get; set; }

        [Display(Name = "手機")]
        public string Mobile { get; set; }

        [Display(Name = "市話")]
        public string Tel { get; set; }

        [Display(Name = "是否啟用雙因素驗證")]
        public bool EnableTwoFactorAuth { get; set; }
    }
}