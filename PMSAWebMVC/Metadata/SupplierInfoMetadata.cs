using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class SupplierInfoMetadata
    {
        [Display(Name ="供應商名稱")]
        public string SupplierName { get; set; }
    }
}