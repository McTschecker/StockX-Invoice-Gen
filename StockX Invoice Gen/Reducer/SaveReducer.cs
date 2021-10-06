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
            List<CsvExported> exported = CSVLoader<CsvExported>.readCSV(path);

            this._exportedOrderIds = new HashSet<string>();
            foreach (var csvExported in exported)
            {
                this._exportedOrderIds.Add(csvExported.SaleID);
            }
        }

        public List<CSVSalesData> Reduce(List<CSVSalesData> input)
        {
            return input.FindAll(sale => !this._exportedOrderIds.Contains(sale.orderNumber));
        }
    }
}