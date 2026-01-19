using Microsoft.VisualBasic;

namespace HotelwebLisMVC.Dto
{
    public class StockSummary
    {
        public int  RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }

        public string PurchaseUnit { get; set; }

        public string ConsumptionUnit { get; set; }

        public string EquivalentUnit { get; set; }

        public string YeildPercentage { get; set; }
        public decimal Opening { get; set; }
        public decimal Purchase { get; set; }
        public decimal Excess { get; set; }
        public decimal Consumed { get; set; } 
        public decimal Wastage { get; set; } 
        public decimal YeildPercenatge { get; set; } = 0;
        public decimal Transfer { get; set; } = 0;
        public decimal Shortage { get; set; } = 0;
        public decimal Production { get; set; } = 0;



        public string ConsumedFormatted => FormatAsDecimal(Consumed);
        public string WastageFormatted => FormatAsDecimal(Wastage); 

        public string ConversionFormatted => FormatAsDecimal(Production);

        private string FormatAsDecimal(decimal value)
        {
            if (decimal.TryParse(EquivalentUnit, out var equivalent) && equivalent > 0)
            {
                // Convert consumptionUnit → purchaseUnit
                var converted = value / equivalent;

                // Always show 3 decimal places
                return $"{converted:0.00}";
            }

            // Fallback
            return $"{value} {PurchaseUnit}";
        }



    }
}
