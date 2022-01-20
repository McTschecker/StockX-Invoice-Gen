using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Serilog;
using StockX_Invoice_Gen.Exports;

namespace StockX_Invoice_Gen.util
{
    public class Settings        
    {
        public string LexofficeApiKey { get; set; }
        public bool Finalize { get; set; }
        public bool DryRun { get; set; }
        public CreatorType InvoiceCreator { get; set; }
        

        
        //returns true if Settings are valid
        //returns false if Settings are invalid
        public bool Validate()
        {
            if (LexofficeApiKey == "")
            {
                Log.Fatal("Please set the Lexoffice API Key");
                return false;
            }
            
            if (InvoiceCreator == CreatorType.None)
            {
                Log.Fatal("Please set the Invoice Creator");
                return false;
            }

            return true;
        }
        
        Export getExport( IConfigurationRoot configFile)
        {
            switch (InvoiceCreator)
            {
                case CreatorType.Lexoffice:
                    return new Lexoffice(LexofficeApiKey, Finalize, configFile );
                case CreatorType.pdf:
                    return new PdfInvoice(configFile);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }

    public enum CreatorType
    {
        [EnumMember(Value = "")]
        None,
        [EnumMember(Value = "Lexoffice")]
        Lexoffice,
        [EnumMember(Value = "PDF")]
        pdf
    }
    
}
