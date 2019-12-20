using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    internal class ProductMetadata
    {
        [Display(Name ="產品識別碼")]
        public int ProductOID { get; set; }
        [Display(Name = "產品編號")]
        public string ProductNumber { get; set; }
        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }
        [Display(Name = "圖片位置")]
        public string PictureAdress { get; set; }
        [Display(Name = "產品說明")]
        public string PictureDescription { get; set; }
    }
}