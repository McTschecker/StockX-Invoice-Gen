using System.Collections.Generic;
using StockX_Invoice_Gen.Sale;

namespace StockX_Invoice_Gen.Reducer
{
    public interface IReducer
    {
        public abstract List<CSVSalesData> Reduce(List<CSVSalesData> input);
    }
}