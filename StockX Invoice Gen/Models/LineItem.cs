using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace StockX_Invoice_Gen.Sale
{
    public class LineItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        /// <summary>
        ///     Tax Rate of the Item in percent (e.g. 0.0825 for 8.25%)
        /// </summary>
        public decimal Tax { get; set; }

        public decimal Total => Quantity * Price * (1 + Tax);

        public string currency { get; set; }

        internal static LineItemTotal computeLineItemTotal(LineItem[] lineItem)
        {


            var groupedTaxRates = lineItem.GroupBy(x => x.Tax);
            
            decimal quantity = 0;
            decimal price = 0;
            var grossTotalPrice = 0.0m;
            var taxSummary = new List<TaxSummary>();

            foreach (var group in groupedTaxRates)
            {
                Log.Debug("Tax: {tax}", group.Key);
                decimal groupTax = 0.0m;
                foreach (var item in group)
                {
                    quantity += item.Quantity;
                    price += item.Price;
                    grossTotalPrice = item.Total;
                    groupTax += item.Price * item.Tax;
                }
                taxSummary.Add(new TaxSummary(group.Key, groupTax));
            }

            return new LineItemTotal(quantity, price, taxSummary, grossTotalPrice);
        }

        internal static LineItem[] computeIfAdjustmentIsNeeded(LineItem[] lineItem, decimal shouldBeTotalAmount,
            string adjustmentText, decimal adjustmentTaxRate)
        {
            var sum = lineItem.Select(x => x.Total).Sum();
            if (shouldBeTotalAmount == sum) return lineItem;

            var lineList = lineItem.ToList();
            lineList.Add(new LineItem
            {
                Name = adjustmentText,
                Description = "",
                Quantity = (decimal)1.0,
                Tax = adjustmentTaxRate
            });

            return lineList.ToArray();
        }
    }

    public class TaxSummary
    {
        protected internal TaxSummary(decimal rate, decimal amount)
        {
            taxRate = rate;
            taxAmount = amount;
        }

        public decimal taxRate { get; set; }
        public decimal taxAmount { get; set; }
    }

    public class LineItemTotal
    {
        internal LineItemTotal(decimal quantity, decimal price, List<TaxSummary> taxSummary,
            decimal grossTotalPrice)

        {
            Quantity = quantity;
            Price = price;
            taxSummaries = taxSummary.ToArray();
            GrossTotalPrice = grossTotalPrice;
        }

        public decimal GrossTotalPrice { get; }
        public decimal Quantity { get; }
        public decimal Price { get; }
        public TaxSummary[] taxSummaries { get; }
    }
}