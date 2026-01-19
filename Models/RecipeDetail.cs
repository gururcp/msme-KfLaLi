using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class RecipeDetail
    {
        public int RecipeDetailsId { get; set; }
        public int? RecipeId { get; set; }
        public int? RawMaterialId { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Description { get; set; }

        public virtual RawMaterial? RawMaterial { get; set; }
        public virtual Recipe? Recipe { get; set; }
    }
}
