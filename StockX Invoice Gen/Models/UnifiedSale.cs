using System;

namespace StockX_Invoice_Gen.Sale
{
    public class UnifiedSale
    {
        public UnifiedSale(Adress sellerAddress, Adress buyerAddress, LineItem[] lineItems, string note,
            string orderNumber, DateTime invoiceDate, DateTime payoutDate)

        {
            sellerLexAddress = sellerAddress ?? throw new ArgumentNullException(nameof(sellerAddress));
            BuyerAddress = buyerAddress ?? throw new ArgumentNullException(nameof(buyerAddress));
            LineItems = lineItems ?? throw new ArgumentNullException(nameof(lineItems));
            this.note = note ?? throw new ArgumentNullException(nameof(note));
            this.orderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
            this.invoiceDate = invoiceDate;
            this.payoutDate = payoutDate;
        }

        public Adress sellerLexAddress { get; set; }
        public Adress BuyerAddress { get; set; }
        public LineItem[] LineItems { get; set; }

        public string note { get; set; }
        public string orderNumber { get; set; }

        public DateTime invoiceDate { get; set; }
        public DateTime payoutDate { get; set; }

        public LineItemTotal lineTotal {get {return LineItem.computeLineItemTotal(LineItems);}}

        public void ensureSumWorks(decimal shouldBeTotal, string adjustmentText, decimal adjustmentTaxRate)
        {
            LineItems =
                LineItem.computeIfAdjustmentIsNeeded(LineItems, shouldBeTotal, adjustmentText, adjustmentTaxRate);
        }
    }
}