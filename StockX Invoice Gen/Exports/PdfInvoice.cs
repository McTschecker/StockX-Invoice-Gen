using System;
using System.Collections.Generic;
using System.Linq;
using Invoicer.Models;
using Invoicer.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using StockX_Invoice_Gen.Sale;

namespace StockX_Invoice_Gen.Exports
{
    class PdfInvoice : Export
    {
        private PdfConfig config;

        public PdfInvoice(IConfigurationRoot configFile)
        {
            this.config = configFile.GetSection("PdfInvoice").Get<PdfConfig>();
        }

        public override string Name => "PDF";


        public override string createInvoice(UnifiedSale sale)
        {
            Log.Information("Creating invoice for {sale}", sale.orderNumber);
            new InvoicerApi(SizeOption.A4, OrientationOption.Landscape, "€")
                .Client(this.config.customerInvoiceAddress())
                .Company(this.config.companyAddress())
                .Title(config.title)
                .DueDate(sale.payoutDate)
                .BillingDate(sale.invoiceDate)
                .CompanyOrientation(PositionOption.Right)
                .Items(sale.LineItems.Select(x=> ItemRow.Make(x.Name, x.Description, x.Quantity, x.Tax, x.Price, x.Total)).ToList())
                .Totals(new List<TotalRow>
                {
                    TotalRow.Make("Total - VAT Reverse Charge", sale.lineTotal.GrossTotalPrice),
                })
                
                .Save(String.Format("export/{0}/invoice.pdf", sale.orderNumber));
                Log.Information("Wrote invoice to {file}", String.Format("export/{0}/invoice.pdf", sale.orderNumber));    
            
            return "Success";
        }
    }

    class PdfConfig
    {
        public string title { get; set; }
        public string companyName { get; set; }
        public string companyAddressLine1 { get; set; }
        public string companyAddressLine2 { get; set; }
        public string companyAddressLine3 { get; set; } // needs documentation
        public string VAT_ID { get; set; }

        public string prefix { get; set; }
        public string suffix { get; set; }
        public int currentNumber { get; set; }

        public string customerInvoiceName { get; set; }
        public string[] customerInvoiceAdress { get; set; }
        public string customerVatId { get; set; }

        public string customerDeliveryName { get; set; }
        public string[] customerDeliveryAdress { get; set; }

        public Invoicer.Models.Address customerInvoiceAddress()
        {
            return Invoicer.Models.Address.Make(
                "Bill To",
                customerDeliveryAdress,
                customerInvoiceName,
                customerVatId
            );
        }

        public Invoicer.Models.Address companyAddress()
        {
            return Invoicer.Models.Address.Make(
                "Company",
                new string[] { companyAddressLine1, companyAddressLine2, companyAddressLine3 },
                companyName,
                VAT_ID
            );
        }
    }
}