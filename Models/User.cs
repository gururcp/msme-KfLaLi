using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class User
    {
        public User()
        {
            Purchases = new HashSet<Purchase>();
            Sales = new HashSet<Sale>();
        }

        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public int? BranchId { get; set; }
        public bool? IsActive { get; set; }

        public virtual Branch? Branch { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
