using System;
using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class SignFlowMetadata
    {
        [Display(Name = "簽核總表識別碼")]
        public int SignFlowOID { get; set; }
        [Display(Name = "簽核發起人")]
        public string OriginatorID { get; set; }
        [Display(Name = "簽核發起時間")]
        public Nullable<System.DateTime> SignBeginDate { get; set; }
        public string SignEvent { get; set; }
        public string SignStatusCode { get; set; }
    }
}