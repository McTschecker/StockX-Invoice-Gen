using StockX_Invoice_Gen.Sale;
using System;
using Serilog;
using RestSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using CsvHelper;
using Microsoft.Extensions.Configuration;

namespace StockX_Invoice_Gen.Exports
{
    class Lexoffice : Export
    {
        public override string Name => "Lexoffice";

        private string apiKey { get; }
        private RestClient restClient { get; }

        private LexAddress adress { get; }
        public Lexoffice(string apiKey, bool finalize, IConfigurationRoot config)
        {
            this.apiKey = apiKey;
            this.adress = config.GetSection("lexOfficeAdress").Get<LexAddress>();
            if (this.adress == null)
            {
                Log.Fatal("Lexoffice adress settings are not added, please reference https://github.com/McTschecker/StockX-Invoice-Gen to fix");
                while (true)
                {
                    string str = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(str))
                    {
                        return;
                    }
                    Log.Information("Press enter to exit");
                }
            }
            Log.Debug("https://api.lexoffice.io/v1/invoices?finalize=" + finalize);
            this.restClient = new RestClient("https://api.lexoffice.io/v1/invoices?finalize="+finalize);
            Log.Information("Initialized Lexoffice");
        }

        public override string createInvoice(UnifiedSale sale)
        {
            Log.Information("Creating invoice for {sale}", sale.orderNumber);
            Log.Debug("Formatting information for invoice");
            InvoiceCreateRequest body = getBody(sale);
            Log.Debug("Creating Request");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + this.apiKey);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);

            IRestResponse response = this.restClient.Execute(request);
            Log.Debug("Got response with status{status}, body: {body}", response.StatusCode, response.Content);
            return response.Content;
        }

        private InvoiceCreateRequest getBody(UnifiedSale sale)
        {
            var lineItems = new List<LineItem>();

            foreach (var lines in sale.LineItems)
            {
                lineItems.Add(
                    new LineItem
                    {
                        name = lines.Name + "\n" + lines.Description,
                        quantity = lines.Quantity,
                        unitName = "Stück",
                        discountPercentage = 0,
                        type = "custom",
                        unitPrice = new UnitPrice()
                        {
                            currency = lines.currency,
                            netAmount = lines.Price.ToString(CultureInfo.InvariantCulture),
                            taxRatePercentage = (int)lines.Tax
                        }
                        
                    }
                    
                    );
            }
            

            return new InvoiceCreateRequest
            {
                archived = false,
                version = 0,
                language = "en",
                voucherStatus = "open",
                lexAddress = this.adress,
                voucherDate = FormatDate(sale.invoiceDate),
                dueDate = FormatDate(sale.payoutDate),
                lineItems = lineItems,
                totalPrice = new TotalPrice { currency = "EUR", totalNetAmount = sale.lineTotal.Price.ToString(CultureInfo.InvariantCulture), totalGrossAmount = sale.lineTotal.GrossTotalPrice.ToString(CultureInfo.InvariantCulture), totalTaxAmount = 0 },
                taxAmounts = new TaxAmount[] { new TaxAmount { taxRatePercentage = 0, taxAmount = 0, amount = 0 } },
                taxConditions = new TaxConditions { taxType = "intraCommunitySupply" },
                paymentConditions = new PaymentConditions { paymentTermDuration = 1, paymentTermLabel = "instant" },
                shippingConditions = new ShippingConditions { shippingDate = FormatDate(sale.invoiceDate), shippingType = "delivery" },
                title = "Invoice",
                introduction = "Invoiced to StockX LLC, address 1046 Woodward Avenue, Detroit, MI, 48226 US, VAT ID: NL826418247B01.",
                remark = sale.orderNumber + "\n" + sale.note
            };
        }


        private static string FormatDate(DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified) throw new Exception("Timezone must be specified");
            return date.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        }

    }

    class LexAddress
    {
        public string name { get; set; }
        public string supplement { get; set; }
        public string street { get; set; }
        public string zip { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
        public string contactId { get; set; }

        public bool Validate()
        {
            return !(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(supplement) || string.IsNullOrEmpty(street) ||
                     string.IsNullOrEmpty(zip) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(countryCode) ||
                     string.IsNullOrEmpty(contactId));
        }
    }

    public class UnitPrice
    {
        public string currency { get; set; }
        public string netAmount { get; set; }
        public int taxRatePercentage { get; set; }
    }

    public class LineItem
    {
        public string type { get; set; }
        public string name { get; set; }
        public object quantity { get; set; }
        public string unitName { get; set; }
        public UnitPrice unitPrice { get; set; }
        public int discountPercentage { get; set; }
    }

    public class TotalPrice
    {
        public string currency { get; set; }
        public string totalNetAmount { get; set; }
        public string totalGrossAmount { get; set; }
        public object taxRatePercentage { get; set; }
        public int totalTaxAmount { get; set; }
        public object totalDiscountAbsolute { get; set; }
        public object totalDiscountPercentage { get; set; }
    }

    public class TaxAmount
    {
        public int taxRatePercentage { get; set; }
        public double taxAmount { get; set; }
        public double amount { get; set; }
    }

    public class TaxConditions
    {
        public string taxType { get; set; }
    }

    public class PaymentConditions
    {
        public string paymentTermLabel { get; set; }
        public int paymentTermDuration { get; set; }
    }

    public class ShippingConditions
    {
        public string shippingDate { get; set; }
        public object shippingEndDate { get; set; }
        public string shippingType { get; set; }
    }

    class InvoiceCreateRequest
    {
        public bool archived { get; set; }
        public int version { get; set; }
        public string language { get; set; }
        public string voucherStatus { get; set; }
        public object voucherNumber { get; set; }
        public string voucherDate { get; set; }
        public string dueDate { get; set; }
        public LexAddress lexAddress { get; set; }
        public IList<LineItem> lineItems { get; set; }
        public TotalPrice totalPrice { get; set; }
        public IList<TaxAmount> taxAmounts { get; set; }
        public TaxConditions taxConditions { get; set; }
        public PaymentConditions paymentConditions { get; set; }
        public ShippingConditions shippingConditions { get; set; }
        public string title { get; set; }
        public string introduction { get; set; }
        public string remark { get; set; }
    }

}
