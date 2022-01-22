using System;
using System.Runtime.Serialization;
using JsonSettings.Library;
using Serilog;
using StockX_Invoice_Gen.Exports;
using StockX_Invoice_Gen.Sale;

namespace StockX_Invoice_Gen.util
{
    public class Settings : SettingsBase
    {
        public override string Filename => "./settings.json";

        //TODO encrypt Lex API KEy
        public string LexofficeApiKey { get; set; } = "";
        public bool Finalize { get; set; } = false;
        public bool DryRun { get; set; } = false;
        public CreatorType InvoiceCreator { get; set; } = CreatorType.None;
        public Adress CompanyAddress { get; set; } = new Adress()
        {
            Name = "Company Name",
            AdressLine1 = "Street",
            AddressLine2 = "Address Line 2",
            zip = "Zip Code",
            city = "city",
            countryCode = "Two Letter Country Code e.g. DE",
            VatID = "DE123456789"
        };

        internal PdfConfig pdfInvoiceConfig { get; set; } = new()
        {
            currentNumber = 1,
            prefix = "stockx",
            suffix = "",
            title ="Invoice"
        };

        public LexAddress customer { get; set; } = new()
        {
            Name = "StockX LLC",
            AdressLine1 = "De Run 4312",
            AddressLine2 = "1046 Woodward Avenue, Detroit, MI, 48226",
            city = "Veldhoven",
            countryCode = "NL",
            zip ="5503",
            contactId = "",
            VatID = "NL826418247B01"
        };


        //returns true if Settings are valid
        //returns false if Settings are invalid
        public bool Validate()
        {
            if (InvoiceCreator == CreatorType.None)
            {
                Log.Fatal("Please set the Invoice Creator");
                return false;
            }

            if (LexofficeApiKey == "" && InvoiceCreator == CreatorType.Lexoffice)
            {
                Log.Fatal("Please set the Lexoffice API Key");
                return false;
            }


            return true;
        }

        internal Export getExport()
        {
            switch (InvoiceCreator)
            {
                case CreatorType.Lexoffice:
                    return new Lexoffice(LexofficeApiKey, Finalize, this.customer);
                case CreatorType.Pdf:
                    return new PdfInvoice(this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum CreatorType
    {
        [EnumMember(Value = "")] None,
        [EnumMember(Value = "Lexoffice")] Lexoffice,
        [EnumMember(Value = "PDF")] Pdf
    }
}