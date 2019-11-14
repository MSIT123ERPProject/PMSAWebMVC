using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class PurchaseRequisitionDtlMetadata
    {
        [Display(Name ="請購單明細識別碼")]
        public int PurchaseRequisitionDtlOID { get; set; }
        [Display(Name = "請購單明細代碼")]
        public string PurchaseRequisitionDtlCode { get; set; }
        [Display(Name = "請購單代碼")]
        public string PurchaseRequisitionID { get; set; }
        [Display(Name = "料件編號")]
        public string PartNumber { get; set; }
        [Display(Name = "請購數量")]
        public int Qty { get; set; }
        [Display(Name = "建議供應商")]
        public string SuggestSupplierCode { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "需求日期")]
        public System.DateTime DateRequired { get; set; }
    }
}