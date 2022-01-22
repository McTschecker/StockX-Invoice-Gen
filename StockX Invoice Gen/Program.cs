using System;
using System.Collections.Generic;
using System.Globalization;
using JsonSettings.Library;
using Serilog;
using StockX_Invoice_Gen.Exports;
using StockX_Invoice_Gen.Reducer;
using StockX_Invoice_Gen.Reducer.Save;
using StockX_Invoice_Gen.Sale;
using StockX_Invoice_Gen.util;

namespace StockX_Invoice_Gen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Starting....");

            Log.Information("Reading Config");

            var config = SettingsBase.Load<Settings>();


            Log.Debug("Read config, parsing it");


            Log.Debug("Parsed Settings");

            if (!config.Validate())
            {
                config.Save();
                Log.Fatal("Settings validation failed, press enter to exit. Wrote an updated Settings file, please fill it out and restart");
                while (true)
                {
                    var str = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(str))
                        return;
                    Log.Information("Press enter to exit");
                }
            }


            var path = "stockx_sales.csv";

            if (args.Length == 1)
            {
                Log.Information("Overriden path to: {@Path}", path);
                path = args[0];
            }

            Log.Debug("getting sales data from: {@Path}", path);

            Run(path, new SaveReducer("./exported.csv"), config,
                config.getExport());
            while (true)
            {
                var str = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(str))
                    return;
                Log.Information("Press enter to exit");
            }
        }

        private static void Run(string path, IReducer reducer, Settings settings, Export export)
        {
            var sales = CSVLoader<CSVSalesData>.readCSV(path);
            
            if (sales.Count < 1)
            {
                Log.Fatal("No (new) sales found, exiting");
                return;
            }

            Log.Debug("Read Sales Data \nLatest sale is: {OrderNumber} from {date}", sales[0].orderNumber,
                sales[0].saleDate);


            var salesToExport = reducer.Reduce(sales);

            Log.Information("Beginning to export {number} sales", salesToExport.Count);

            foreach (var salesData in salesToExport)
            {
                Log.Information("Exporting sale {sale}", salesData);
                if (!settings.DryRun)
                    export.createInvoice(salesData.convertToUnifiedSale(settings.CompanyAddress, settings.customer));
                else
                    Log.Information("Running dry (not performing any export action)");

                var exported = new List<CsvExported>();
                var exportedSale = new CsvExported();
                exportedSale.AssignValues(salesData.orderNumber,
                    DateTime.Now.ToString("u", CultureInfo.CurrentCulture));
                if (!settings.DryRun)
                    exported.Add(exportedSale);
                else
                    Log.Information("Running Dry - not saving exported sales");
                CSVLoader<CsvExported>.writeCsv("./exported.csv", exported, true);
            }
        }
    }
}