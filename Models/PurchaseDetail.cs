using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class PurchaseDetail
    {
        public int PurchaseDetailId { get; set; }
        public int? PurchaseId { get; set; }
        public int? RawMaterialId { get; set; }
        public string? Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Gstpercentage { get; set; }
        public decimal? Gstamount { get; set; }
        public decimal? Total { get; set; }

        public virtual Purchase? Purchase { get; set; }
        public virtual RawMaterial? RawMaterial { get; set; }
    }
}
