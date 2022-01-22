using System.Collections.Generic;
using System.Linq;
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
        }

        private Settings settings { get; }

        public override string Name => "PDF";


        public override string createInvoice(UnifiedSale sale)
        {
            Log.Information("Creating invoice for {sale}", sale.orderNumber);
            new InvoicerApi(SizeOption.A4, OrientationOption.Landscape, "€")
                .Reference(sale.orderNumber)
                .Client(convertAdress(settings.customer))
                .Company(convertAdress(settings.CompanyAddress))
                .Title(config.title)
                .DueDate(sale.payoutDate)
                .BillingDate(sale.invoiceDate)
                .CompanyOrientation(PositionOption.Right)
                .Items(sale.LineItems
                    .Select(x => ItemRow.Make(x.Name, x.Description, x.Quantity, x.Tax, x.Price, x.Total)).ToList())
                .Totals(new List<TotalRow>
                {
                    TotalRow.Make("Total - VAT Reverse Charge", sale.lineTotal.GrossTotalPrice)
                })
                .Save($"export/{sale.orderNumber}/invoice.pdf");
            Log.Information("Wrote invoice to {File}", $"export/{sale.orderNumber}/invoice.pdf");

            Log.Debug("Incrementing invoice number");
            settings.pdfInvoiceConfig.currentNumber++;
            settings.Save();
            Log.Debug("Incremented invoice Number");

            return "Success";
        }

        private static Address convertAdress(Adress c)
        {
            return Address.Make(
                "Company",
                new[]
                {
                    c.AdressLine1, c.AddressLine2,
                    $"{c.zip} {c.city}"
                },
                c.Name,
                c.VatID
            );
        }
    }

    internal class PdfConfig
    {
        public string title { get; init; }
        public string prefix { get; init; }
        public string suffix { get; init; }
        public int currentNumber { get; set;  }
    }
}