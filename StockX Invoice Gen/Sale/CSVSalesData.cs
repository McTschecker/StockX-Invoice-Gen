using CsvHelper.Configuration.Attributes;
using System;
using System.Globalization;
using Serilog;

namespace StockX_Invoice_Gen.Sale
{
    public class CSVSalesData
    {
        [Name("Seller Name")]
        public string sellerName {  get; set; }

        [Name("Seller Address (Billing)")]
        public string sellerAdress {  get; set; }

        [Name("Seller VAT Number")]
        public string SellerVatId { get; set; }

        [Name("Sold To Name")]
        public string buyerName { get; set; }

        [Name("Sold To address")]
        public string billingAdress {  get; set; }

        [Name("Sold to VAT Number")]
        public string buyerVATID {  get; set; }

        [Name("Fiscal Representative Name")]
        public string shippingName { get; set; }

        [Name("Fiscal Representative Address")]
        public string shippingFiscalAdress {  get; set; }

        [Name("StockX Destination Address")]
        public string shippingAdress { get; set; }

        [Name("Sale Date")]
        public string saleDate {  get; set; }

        [Name("Payout date")]
        public string payoutDate { get; set; }
        
        [Name("Invoice Date")]
        public string invoiceDate { get; set; }

        [Name("Invoice Number")]
        public string invoiceNumber { get; set; }

        [Name("Order Number")]
        public string orderNumber { get; set; }

        /// <summary>
        /// Aprox item description
        /// </summary>
        [Name("Item")]
        public string item { get; set; }


        //// <summary> Exact item description </summary>
        [Name("Sku Name")]
        public string skuName { get; set; }

        [Name("Sku Size")]
        public string size { get; set; }

        /// <summary>
        /// SKU id of the shoe
        /// </summary>
        [Name("Style")]
        public string style { get; set; }

         /// <summary>
        /// Quantity in sale shoe
        /// </summary>
        [Name("Quantity")]
        public string quantity { get; set; }

        /// <summary>
        /// Ship From Address
        /// </summary>
        [Name("Ship From Address")]
        public string shippedFrom { get; set; }

        /// <summary>
        /// Ship To Address
        /// </summary>
        [Name("Ship To Address")]
        public string shippedTo { get; set; }

        /// <summary>
        /// Listing Price
        /// </summary>
        [Name("Listing Price")]
        public string listPrice { get; set; }

        /// <summary>
        /// Listing Price Currency
        /// </summary>
        [Name("Listing Price Currency")]
        public string listCurrency { get; set; }

        /// <summary>
        /// Seller Fee
        /// </summary>
        [Name("Seller Fee")]
        public string saleFee { get; set; }

        /// <summary>
        /// Seller Fee Currency
        /// </summary>
        [Name("Seller Fee Currency")]
        public string saleFeeCurrency { get; set; }

        /// <summary>
        /// Payment Processing Fee
        /// </summary>
        [Name("Payment Processing")]
        public string paymentFee { get; set; }

        /// <summary>
        /// Payment Processing Currency
        /// </summary>
        [Name("Payment Processing Currency")]
        public string paymentFeeCurrency { get; set; }

        /// <summary>
        /// Shipping Fee
        /// </summary>
        [Name("Shipping Fee")]
        public string shippingFee { get; set; }

        /// <summary>
        /// Shipping Fee Currency
        /// </summary>
        [Name("Shipping Fee Currency")]
        public string shippingFeeCurrency { get; set; }

        /// <summary>
        /// VAT Rate
        /// </summary>
        [Name("VAT Rate")]
        public string vatRate { get; set; }

        /// <summary>
        /// Total Net Amount (Payout Excluding VAT)
        /// </summary>
        [Name("Total Net Amount (Payout Excluding VAT)")]
        public string netPayout { get; set; }

        /// <summary>
        /// Total Net Amount (Payout Excluding VAT) Currency
        /// </summary>
        [Name("Total Net Amount (Payout Excluding VAT) Currency")]
        public string netPayoutCurrency { get; set; }

        /// <summary>
        /// Total VAT Amount
        /// </summary>
        [Name("Total VAT Amount")]
        public string totalVAT { get; set; }

        /// <summary>
        /// Total VAT Amount Currency
        /// </summary>
        [Name("Total VAT Amount Currency")]
        public string totalVATCurrency { get; set; }

        /// <summary>
        /// Total Gross Amount (Total Payout)
        /// </summary>
        [Name("Total Gross Amount (Total Payout)")]
        public string totalPayout { get; set; }

        /// <summary>
        /// Total Gross Amount (Total Payout) Currency
        /// </summary>
        [Name("Total Gross Amount (Total Payout) Currency")]
        public string totalPayoutCurrency { get; set; }

        /// <summary>
        /// Special references
        /// </summary>
        [Name("Special references")]
        public string specialReferences { get; set; }

        


        public DateTime getSaleDate()
        {
            if (string.IsNullOrEmpty(shippingName)) throw new InvalidOperationException("Sale date is not defined");
            return DateTime.SpecifyKind(DateTime.Parse(this.saleDate), DateTimeKind.Local);
        }

        public DateTime getPayoutDate()
        {
            if (string.IsNullOrEmpty(shippingName)) throw new InvalidOperationException("Sale date is not defined");
            return DateTime.SpecifyKind(DateTime.Parse(this.payoutDate), DateTimeKind.Local);
        }

        public DateTime getInvoiceDate()
        {
            if (string.IsNullOrEmpty(shippingName)) throw new InvalidOperationException("Sale date is not defined");

            return DateTime.SpecifyKind(DateTime.Parse(this.invoiceDate), DateTimeKind.Local);
        }

                    
        override
        public string ToString()
        {
            return this.orderNumber + ": " + item + " " + size + " sold at " + saleDate;

        }

        public decimal getListPrice()
        {
            try
            {
                return Decimal.Parse(listPrice, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Log.Error("Error parsing list price", e);
                return (decimal)0.0;
            }
        }
        
        public decimal getSaleFee()
        {
            try
            {
                return Decimal.Parse(saleFee, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Log.Error("Error parsing sale fee", e);
                return (decimal)0.0;
            }
        }
        
        public decimal getPaymentFee()
        {
            try
            {
                return Decimal.Parse(paymentFee, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Log.Error("Error parsing payment fee", e);
                return (decimal)0.0;
            }
        }
        
        public decimal getShippingFee()
        {
            try
            {
                return Decimal.Parse(shippingFee, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Log.Error("Error parsing shipping fee", e);
                return (decimal)0.0;
            }
        }
        
        public decimal getNetPayout()
        {
            try
            {
                return Decimal.Parse(netPayout, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Log.Error("Error parsing net Payout", e);
                return (decimal)0.0;
            }
        }

        public static string convertToString(CSVSalesData salesData)
        {
            return salesData.ToString();
        }


        public UnifiedSale convertToUnifiedSale(Adress sellerAdress, Adress stockxAdress)
        {
            var sale = new UnifiedSale(sellerAdress, stockxAdress, new LineItem[]
            {
                new LineItem()
                {
                    Name = this.skuName,
                    Description = this.size,
                    Quantity = 1.0m,
                    Price = this.getListPrice(),
                    Tax = 0.0m,
                    currency = listCurrency
                },
                new LineItem()
                {
                    Name = "Sale Fee",
                    Description ="",
                    Quantity = 1.0m,
                    Price = -this.getSaleFee(),
                    Tax = 0.0m,
                    currency = saleFeeCurrency
                },
                new LineItem()
                {
                    Name = "Payment Fee",
                    Description ="",
                    Quantity = 1.0m,
                    Price = -this.getPaymentFee(),
                    Tax = 0.0m,
                    currency = paymentFeeCurrency
                },
                new LineItem()
                {
                    Name = "Shipping Fee",
                    Description ="",
                    Quantity = 1.0m,
                    Price = -this.getShippingFee(),
                    Tax = 0.0m,
                    currency = shippingFeeCurrency
                },
            }, specialReferences, orderNumber);
            //TODO check adjustment works as intended
            sale.ensureSumWorks(getNetPayout(), "Payout Adjustment", 0.0m, getSaleDate(), getPayoutDate());
            return sale;
        }
        
        

    }
}
