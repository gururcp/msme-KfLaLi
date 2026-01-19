using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class SaleDetail
    {
        public int SaleDetailId { get; set; }
        public int? SaleId { get; set; }
        public int? ItemId { get; set; }
        public string? Hsncode { get; set; }
        public int? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Gstpercentage { get; set; }
        public decimal? Gstamount { get; set; }
        public decimal? Total { get; set; }

        public virtual Item? Item { get; set; }
        public virtual Sale? Sale { get; set; }
    }
}
