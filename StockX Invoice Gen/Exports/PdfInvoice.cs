using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Invoicer.Models;
using Invoicer.Services;
using Serilog;
using StockX_Invoice_Gen.Sale;
using StockX_Invoice_Gen.util;

namespace StockX_Invoice_Gen.Exports
{
    internal class PdfInvoice : Export
    {
        private PdfConfig config;

        public PdfInvoice(Settings config)
        {
            settings = config;
            this.config = settings.pdfInvoiceConfig;
            
            //Registers encoding Provider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            // checks if folder Exports exist, if not create it
            if (!Directory.Exists("exported"))
            {
                Log.Debug("Directory exported does not exist, creating it");
                Directory.CreateDirectory("exported");
                Log.Debug("Directory exported created");
            }
        }

        private Settings settings { get; }

        public override string Name => "PDF";


        public override string createInvoice(UnifiedSale sale)
        {
            Log.Information("Creating invoice for {sale}", sale.orderNumber);
            
            //check if folder for this sale exists, if not create it
            if (!Directory.Exists($"exported/{sale.orderNumber}"))
            {
                Log.Debug("Directory for sale {sale} does not exist, creating it", sale.orderNumber);
                Directory.CreateDirectory($"exported/{sale.orderNumber}");
                Log.Debug("Directory for sale {sale} created", sale.orderNumber);
            }
            
            new InvoicerApi(SizeOption.A4, OrientationOption.Portrait, "€")
                .Reference($"{config.prefix}{config.currentNumber}{config.suffix}")
                .Client(convertAdress(settings.customer, "customer"))
                .Company(convertAdress(settings.CompanyAddress, "Billed From"))
                .Title(config.title)
                .DueDate(sale.payoutDate)
                .BillingDate(sale.invoiceDate)
                .CompanyOrientation(PositionOption.Right)
                .Items(sale.LineItems
                    .Select(x => ItemRow.Make(x.Name, x.Description, x.Quantity, x.Tax, x.Price, x.Total)).ToList())
                .Totals(new List<TotalRow>
                {
                    //TODO handle multiple tax rates
                    TotalRow.Make("Total", sale.lineTotal.GrossTotalPrice)
                })
                .Details(new List<DetailRow>()
                {
                    DetailRow.Make("Order Information: ", sale.orderNumber),
                    DetailRow.Make("Tax Information: ", $"VAT Reverse Charge to customer VAT ID: {settings.customer.VatID}"),
                    DetailRow.Make("Note", sale.note)
                })
                .Save($"exported/{sale.orderNumber}/invoice.pdf");
            Log.Information("Wrote invoice to {File}", $"exported/{sale.orderNumber}/invoice.pdf");

            Log.Debug("Incrementing invoice number");
            settings.pdfInvoiceConfig.currentNumber++;
            settings.Save();
            Log.Debug("Incremented invoice Number");

            return "Success";
        }

        private static Address convertAdress(Adress c, string title)
        {
            return Address.Make(
                title,
                new[]
                {
                    c.Name,
                    c.AdressLine1, c.AddressLine2,
                    $"{c.zip} {c.city}"
                },
                null,
                c.VatID
            );
        }
    }

    public class PdfConfig
    {
        public string title { get; init; }
        public string prefix { get; init; }
        public string suffix { get; init; }
        public int currentNumber { get; set;  }
    }
}