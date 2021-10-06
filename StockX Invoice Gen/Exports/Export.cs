using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockX_Invoice_Gen.Exports
{
    abstract class Export
    {
        public abstract string Name { get; }
        public abstract string createInvoice(Sale.CSVSalesData sale);
    }
}
