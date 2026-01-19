using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Ledger
    {
        public Ledger()
        {
            Purchases = new HashSet<Purchase>();
            Sales = new HashSet<Sale>();
            TransactionData = new HashSet<TransactionDatum>();
        }

        public int LedgerId { get; set; }
        public string? LedgerName { get; set; }
        public int? LedgerTypeId { get; set; }
        public string? Gsttype { get; set; }
        public string? MobileNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? BillingAddress { get; set; }
        public string? ShipingAddress { get; set; }
        public string? EmailId { get; set; }
        public string? State { get; set; }
        public decimal? OpeningBalance { get; set; }
        public DateTime? OpeningBalanceDate { get; set; }
        public bool? CreditLimitStatus { get; set; }
        public decimal? CreditLimit { get; set; }
        public string? Gstnumber { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
        public virtual ICollection<TransactionDatum> TransactionData { get; set; }
    }
}
