using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class ItemCategory
    {
        public ItemCategory()
        {
            Items = new HashSet<Item>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
