using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class ProductionDetail
    {
        public int ProductionDetailsId { get; set; }
        public int? ProductionId { get; set; }
        public int? RawMaterialId { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }

        public virtual Production? Production { get; set; }
        public virtual RawMaterial? RawMaterial { get; set; }
    }
}
