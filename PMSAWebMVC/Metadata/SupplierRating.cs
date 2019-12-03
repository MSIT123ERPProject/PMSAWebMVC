using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Models
{
    [MetadataType(typeof(SupplierRatingMetadata))]
    public partial class SupplierRating
    {
    }

    public partial class SupplierRatingMetadata
    {
        public int SupplierRatingOID { get; set; }
        public string RatingName { get; set; }

        [JsonIgnore]
        public virtual ICollection<SupplierInfo> SupplierInfo { get; set; }
    }
}