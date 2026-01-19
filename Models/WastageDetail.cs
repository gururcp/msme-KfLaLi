using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class WastageDetail
    {
        public int WastageDetailsId { get; set; }
        public int? WastageId { get; set; }
        public int? RawMaterialId { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal? AvgPurchasePrice { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; }

        public virtual RawMaterial? RawMaterial { get; set; }
        public virtual Wastage? Wastage { get; set; }
    }
}
