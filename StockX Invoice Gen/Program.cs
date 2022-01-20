
using InquirerCS;
using StockX_Invoice_Gen.Sale;
using System;
using System.Collections.Generic;
using System.Globalization;
using Serilog;
using StockX_Invoice_Gen.Exports;
using StockX_Invoice_Gen.Reducer;
using StockX_Invoice_Gen.Reducer.Save;
using Microsoft.Extensions.Configuration;
using StockX_Invoice_Gen.util;

namespace StockX_Invoice_Gen
{
    class Program
    {
        static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Starting....");

            Log.Information("Reading Config");

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("settings.json").Build();

            Log.Debug("Read config, parsing it");
            
            
            var section = config.GetSection(nameof(Settings));
            var settings = section.Get<Settings>();
            if (settings == null)
            {
                Log.Fatal("general settings are not added, please reference https://github.com/McTschecker/StockX-Invoice-Gen to fix");
                while (true)
                {
                    string str = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(str))
                    {
                        return;
                    }
                    Log.Information("Press enter to exit");
                }
            }

            Log.Debug("Parsed Settings");

            if (!(settings.Validate()))
            {
                Log.Fatal("Settings validation failed, press enter to exit");
                while (true)
                {
                    string str = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(str))
                    {
                        return;
                    }
                    else
                    {
                        Log.Information("Press enter to exit");
                    }
                }
            }
            
            
            var path = "stockx_sales.csv";

            if(args.Length == 1)
            {
                Log.Information("Overriden path to: {@Path}", path);
                path = args[0];
            }
            Log.Debug("getting sales data from: {@Path}", path);

            Run(path, new SaveReducer("./exported.csv"), settings, new Lexoffice(settings.LexofficeApiKey, settings.Finalize, config));
            while (true)
            {
                string str = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(str))
                {
                    return;
                }
                else
                {
                    Log.Information("Press enter to exit");
                }
            }

        }

        static void Run(string path, IReducer reducer, Settings settings, Export export)
        {
            List<CSVSalesData> sales = CSVLoader<CSVSalesData>.readCSV(path);
            Log.Debug("Read Sales Data \nLatest sale is: {OrderNumber} from {date}", sales[0].orderNumber, sales[0].saleDate);



            List<CSVSalesData> salesToExport = reducer.Reduce(sales);

            Log.Information("Beginning to export {number} sales", salesToExport.Count);

            foreach (CSVSalesData salesData in salesToExport)
            {
                Log.Information("Exporting sale {sale}", salesData);
                if (!settings.DryRun)
                {
                    export.createInvoice(salesData);
                }
                else
                {
                    Log.Information("Running dry (not performing any export action)");
                }
                
                List<CsvExported> exported = new List<CsvExported>();
                CsvExported exportedSale = new CsvExported();
                exportedSale.AssignValues(salesData.orderNumber, DateTime.Now.ToString("u", CultureInfo.CurrentCulture));
                if (!settings.DryRun)
                {
                    exported.Add(exportedSale);    
                }
                else
                {
                    Log.Information("Running Dry - not saving exported sales");
                }
                CSVLoader<CsvExported>.writeCsv("./exported.csv", exported, true);
            }

        }
    }
}
