using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class RawMaterial
    {
        public RawMaterial()
        {
            ManageStocks = new HashSet<ManageStock>();
            ProductionDetails = new HashSet<ProductionDetail>();
            Productions = new HashSet<Production>();
            PurchaseDetails = new HashSet<PurchaseDetail>();
            RecipeDetails = new HashSet<RecipeDetail>();
            WastageDetails = new HashSet<WastageDetail>();
        }

        public int RawMaterialId { get; set; }
        public string? Name { get; set; }
        public string? PurchaseUnit { get; set; }
        public string? ConsumptionUnit { get; set; }
        public string? EquivalentUnit { get; set; }
        public int? CategoryId { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? TransferPrice { get; set; }
        public decimal? ReconciliationPrice { get; set; }
        public decimal? OpeningStock { get; set; }
        public decimal? OpeningStockPrice { get; set; }
        public DateTime? OpeningStockDate { get; set; }
        public decimal? MinStockToMaintain { get; set; }
        public string? YeildPercenatge { get; set; }

        public virtual RawMaterialCategory? Category { get; set; }
        public virtual ICollection<ManageStock> ManageStocks { get; set; }
        public virtual ICollection<ProductionDetail> ProductionDetails { get; set; }
        public virtual ICollection<Production> Productions { get; set; }
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual ICollection<RecipeDetail> RecipeDetails { get; set; }
        public virtual ICollection<WastageDetail> WastageDetails { get; set; }
    }
}
