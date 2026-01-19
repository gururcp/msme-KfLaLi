using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class RawMaterialCategory
    {
        public RawMaterialCategory()
        {
            RawMaterials = new HashSet<RawMaterial>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public virtual ICollection<RawMaterial> RawMaterials { get; set; }
    }
}
