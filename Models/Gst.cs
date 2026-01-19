using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Gst
    {
        public Gst()
        {
            Items = new HashSet<Item>();
        }

        public int GstId { get; set; }
        public decimal? Gstpercentage { get; set; }
        public decimal? Cgst { get; set; }
        public decimal? Sgst { get; set; }
        public decimal? Igst { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
