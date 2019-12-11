using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMSAWebMVC.Models
{
    public class EmployeeMetadata
    {
        public int EmployeeOID { get; set; }

        [Display(Name = "員工編號(帳號)")]
        [Remote("AccountCheck", "Members", ErrorMessage = "此帳號已被註冊過")]
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

        [Display(Name = "公司代碼")]
        [Required(ErrorMessage = "請輸入公司代碼")]
        public string CompanyCode { get; set; }

        [Display(Name = "主管員工編號")]
        public string ManagerID { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        [Display(Name = "帳號狀態")]
        public string AccountStatus { get; set; }

        [Display(Name = "新增時間")]
        public System.DateTime CreateDate { get; set; }

        [Display(Name = "更新時間")]
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [Display(Name = "發信狀態")]
        public string SendLetterStatus { get; set; }

        [Display(Name = "發信時間")]
        public Nullable<System.DateTime> SendLetterDate { get; set; }

        [Display(Name = "權限角色代碼")]
        public string AuthRoleID { get; set; }

        [Display(Name = "驗證碼")]
        public string AuthenticateToken { get; set; }

        [Display(Name = "驗證碼建立時間")]
        public Nullable<System.DateTime> TokenCreateTime { get; set; }

        [Display(Name = "是否啟用雙因素驗證")]
        public Nullable<bool> EnableTwoFactorAuth { get; set; }

        [Display(Name = "職稱")]
        public string Title { get; set; }
    }
}