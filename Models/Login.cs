using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Login
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
    }
}
