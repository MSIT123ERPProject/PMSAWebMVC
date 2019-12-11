using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.ViewModels.UsersAdmin
{
    public class UsersAdminViewModel
    {
        [DisplayName("員工編號")]
        public string EmployeeID { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("角色")]
        public string Role { get; set; }

        [DisplayName("信箱")]
        public string Email { get; set; }

        [DisplayName("手機")]
        public string Mobile { get; set; }

        [DisplayName("市話")]
        public string Tel { get; set; }

        [DisplayName("帳號狀態")]
        public string AccountStatus { get; set; }

        [DisplayName("經理ID")]
        public string ManagerID { get; set; }

        [DisplayName("修改日期")]
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [DisplayName("新增日期")]
        public System.DateTime CreateDate { get; set; }

        [DisplayName("寄信日期")]
        public Nullable<System.DateTime> SendLetterDate { get; set; }

        [DisplayName("使用者更改密碼日期")]
        public Nullable<System.DateTime> LastPasswordChangedDate { get; set; }

        [DisplayName("驗證狀態")]
        public bool EmailConfirm { get; set; }
    }
}