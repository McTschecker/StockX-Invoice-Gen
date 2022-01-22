using System.Collections.Generic;
using StockX_Invoice_Gen.Reducer.Save;
using StockX_Invoice_Gen.Sale;

namespace StockX_Invoice_Gen.Reducer
{
    public class SaveReducer : IReducer
    {
        private readonly HashSet<string> _exportedOrderIds;

        public SaveReducer(string path)
        {
            var exported = CSVLoader<CsvExported>.readCSV(path);

            _exportedOrderIds = new HashSet<string>();
            foreach (var csvExported in exported) _exportedOrderIds.Add(csvExported.SaleID);
        }

        public List<CSVSalesData> Reduce(List<CSVSalesData> input)
        {
            return input.FindAll(sale => !_exportedOrderIds.Contains(sale.orderNumber));
        }
    }
}