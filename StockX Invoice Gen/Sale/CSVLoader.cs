using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;
using Serilog;

namespace StockX_Invoice_Gen.Sale
{
    class CSVLoader<T>
    {
        public CSVLoader(string path)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static List<T> readCSV(string path)
        {
            if (!File.Exists(path))
            {
                Log.Warning("File does not exist at {path}", path);
                Log.Information("Creating empty CSV file");
                writeCsv(path, new List<T>(), false);
                Log.Information("Wrote empty CSV file, only with header. Expect to close with error");
                throw new Exception("Written file does not exist");
            }

            try
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<T>();
                    return records.ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
           
        }

        public static void writeCsv(string path, List<T> elements, bool append)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.HasHeaderRecord = !append;

            using (var writer = new StreamWriter(path, append))
            {
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(elements);
                }
            }
        }
    }
}