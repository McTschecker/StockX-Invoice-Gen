using System;
using System.Collections.Generic;
using InquirerCS;
using Serilog;
using StockX_Invoice_Gen.Sale;

namespace StockX_Invoice_Gen.Reducer
{
    public class SelectReducer : IReducer
    {
        public SelectReducer()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public List<CSVSalesData> Reduce(List<CSVSalesData> sales)
        {
            Console.Clear();
            var lastSaleToInclude = Question.List("Choose last sale which IS INCLUDED", sales)
                .WithDefaultValue(sales[0]).Prompt();

            Console.Clear();
            Log.Information("Choose: {lastSaleToInclude} as last Sale to include", lastSaleToInclude);

            return sales.GetRange(0, sales.IndexOf(lastSaleToInclude) + 1);
        }
    }
}