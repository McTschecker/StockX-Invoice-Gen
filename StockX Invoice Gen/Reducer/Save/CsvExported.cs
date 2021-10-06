using System;
using System.Globalization;
using CsvHelper.Configuration.Attributes;

namespace StockX_Invoice_Gen.Reducer.Save
{
    public class CsvExported
    {
        [Name("SaleID")]
        public string SaleID {  get; set; }

        [Name("Date")]
        public string Date {  get; set; }
        
        public void AssignValues(string SaleID, string Date)
        {
            this.SaleID = SaleID;
            this.Date = Date;
        }
    }
}