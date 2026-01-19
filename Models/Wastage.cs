using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Wastage
    {
        public Wastage()
        {
            WastageDetails = new HashSet<WastageDetail>();
        }

        public int WastageId { get; set; }
        public DateTime? Date { get; set; }

        public virtual ICollection<WastageDetail> WastageDetails { get; set; }
    }
}
