using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Branch
    {
        public Branch()
        {
            Purchases = new HashSet<Purchase>();
            Sales = new HashSet<Sale>();
            Users = new HashSet<User>();
        }

        public int BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
