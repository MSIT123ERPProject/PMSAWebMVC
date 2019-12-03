using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    [MetadataType(typeof(CompanyInfoMetadata))]
    public partial class CompanyInfo
    {
    }

    public partial class CompanyInfoMetadata
    {
        public int CompanyInfoOID { get; set; }

        [DisplayName("公司代碼")]
        public string CompanyCode { get; set; }

        [DisplayName("公司名稱")]
        public string CompanyName { get; set; }

        [DisplayName("統編")]
        public string TaxID { get; set; }

        [DisplayName("電子信箱")]
        public string Email { get; set; }

        [DisplayName("市話")]
        public string Tel { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }
    }
}