using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace search 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var searchPhrase = args[0];
            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";"};

            using (var reader = new StreamReader(@"C:\repos\everybody-codes\data\cameras-defb.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var lineNumber = 0;
                var records = new List<Location>();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    lineNumber++;
                    try
                    {
                        var record = new Location
                        {
                            Camera = csv.GetField<string>("Camera"),
                            Latitude = csv.GetField<double>("Latitude"),
                            Longitude = csv.GetField<double>("Longitude"),
                        };
                        records.Add(record);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bad record at line {lineNumber}.");
                    }
                }

                Console.WriteLine($"Records count: {records.Count}");

                var result = records.Where(r => r.Camera.Contains(searchPhrase)).ToList();
                result.ForEach(item => Console.WriteLine($"{item} \r\n"));
            }
        }

        public class Location
        {
            public string Camera { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public override string ToString()
            {
                var model = Camera.Split(' ').First();
                var number = model.Split('-').Last();
                var cameraName = $"{number} | {Camera}";
                return $"{cameraName} | {Latitude} | {Longitude}";
            }
        }
    }
}