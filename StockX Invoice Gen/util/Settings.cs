using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Serilog;

namespace StockX_Invoice_Gen.util
{
    public class Settings        
    {
        public string LexofficeApiKey { get; set; }
        public bool Finalize { get; set; }
        public bool DryRun { get; set; }

        
        //returns true if Settings are valid
        //returns false if Settings are invalid
        public bool Validate()
        {
            if (LexofficeApiKey == "")
            {
                Log.Fatal("Please set the Lexoffice API Key");
                return false;
            }

            return true;
        }
    }
}
