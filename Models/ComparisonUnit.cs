using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class ComparisonUnit
    {
        public int EquivalentId { get; set; }
        public string FromUnit { get; set; } = null!;
        public string ToUnit { get; set; } = null!;
        public string EquivalentValue { get; set; } = null!;
    }
}
