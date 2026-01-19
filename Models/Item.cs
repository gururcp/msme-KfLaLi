using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Item
    {
        public Item()
        {
            Recipes = new HashSet<Recipe>();
            SaleDetails = new HashSet<SaleDetail>();
        }

        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemNameOnline { get; set; }
        public int? CategoryId { get; set; }
        public string? ItemCode { get; set; }
        public decimal? ContainerCharges { get; set; }
        public decimal? Weight { get; set; }
        public string? Unit { get; set; }
        public string? Dietary { get; set; }
        public decimal? Mrpprice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public int? GstId { get; set; }
        public decimal? OpeningStock { get; set; }
        public decimal? OpeningStockPrice { get; set; }
        public DateTime? OpeningStockDate { get; set; }
        public decimal? MinStockToMaintain { get; set; }

        public virtual ItemCategory? Category { get; set; }
        public virtual Gst? Gst { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<SaleDetail> SaleDetails { get; set; }
    }
}
