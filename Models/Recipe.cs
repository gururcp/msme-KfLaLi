using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Recipe
    {
        public Recipe()
        {
            RecipeDetails = new HashSet<RecipeDetail>();
        }

        public int RecipeId { get; set; }
        public int? ItemId { get; set; }

        public virtual Item? Item { get; set; }
        public virtual ICollection<RecipeDetail> RecipeDetails { get; set; }
    }
}
