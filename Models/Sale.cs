using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class Sale
    {
        public Sale()
        {
            SaleDetails = new HashSet<SaleDetail>();
        }

        public int SaleId { get; set; }
        public string? InvoiceNo { get; set; }
        public int? LedgerId { get; set; }
        public int? BranchId { get; set; }
        public int? UserId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalGst { get; set; }
        public decimal? TotalDiscount { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMode { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Received { get; set; }

        public virtual Branch? Branch { get; set; }
        public virtual Ledger? Ledger { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<SaleDetail> SaleDetails { get; set; }
    }
}
