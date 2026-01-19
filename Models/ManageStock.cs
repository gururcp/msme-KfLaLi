using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class ManageStock
    {
        public int ClosingStockId { get; set; }
        public int? RawMaterialId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? ClosingQuantity { get; set; }
        public decimal? SubQuantity { get; set; }
        public string? Unit { get; set; }
        public string? Comments { get; set; }

        public virtual RawMaterial? RawMaterial { get; set; }
    }
}
