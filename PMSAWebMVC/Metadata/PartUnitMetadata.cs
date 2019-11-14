using System.ComponentModel.DataAnnotations;

namespace PMSAWebMVC.Models
{
    public class PartUnitMetadata
    {
        [Display(Name = "料件單位")]
        public string PartUnitName { get; set; }
    }
}