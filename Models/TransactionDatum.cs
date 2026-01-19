using System;
using System.Collections.Generic;

namespace HotelwebLisMVC.Models
{
    public partial class TransactionDatum
    {
        public int TransactionId { get; set; }
        public DateTime? Date { get; set; }
        public string? PayMode { get; set; }
        public int? LedgerId { get; set; }
        public decimal? Total { get; set; }
        public decimal? Received { get; set; }
        public decimal? Balance { get; set; }
        public string? TransactionMode { get; set; }
        public string? Status { get; set; }
        public string? Narration { get; set; }
        public string? VoucherNo { get; set; }

        public virtual Ledger? Ledger { get; set; }
    }
}
