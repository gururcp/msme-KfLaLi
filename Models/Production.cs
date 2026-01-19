using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Production
    {
        public Production()
        {
            ProductionDetails = new HashSet<ProductionDetail>();
        }

        public int ProductionId { get; set; }
        public string? ProductionName { get; set; }
        public int? RawMaterialId { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public DateTime? Date { get; set; }

        public virtual RawMaterial? RawMaterial { get; set; }
        public virtual ICollection<ProductionDetail> ProductionDetails { get; set; }
    }
}
