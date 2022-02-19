using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using renderer.Models;

namespace renderer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Camera()
    {
        var table = GetLocations();
        return View(table);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public Table GetLocations()
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";" };

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

            var col1 = records.Where(r => r.GetNumber() % 3 == 0).ToList();
            var col2 = records.Where(r => r.GetNumber() % 5 == 0).ToList();
            var col3 = records.Where(r => r.GetNumber() % 3 == 0 && r.GetNumber() % 5 == 0).ToList();
            var col4 = records.Where(r => r.GetNumber() % 3 != 0 && r.GetNumber() % 5 != 0).ToList();

            return new Table
            {
                First = col1,
                Second = col2,
                Third = col3,
                Fourth = col4,
            };
     
        }
    }

    public class Table
    {
        public List<Location> First { get; set; } 
        public List<Location> Second { get; set; }
        public List<Location> Third { get; set; }
        public List<Location> Fourth { get; set; }
    }

    public class Location
    {
        public string Camera { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int GetNumber()
        {
            var pattern = @"[A-Z]{3}-[A-Z]{2}-[0-9]{3}";
            var rg = new Regex(pattern);
            var match = rg.Match(Camera);
            var number = match.Value.Split("-").Last();
            return int.Parse(number);
        }

        public override string ToString()
        {
            var model = Camera.Split(' ').First();
            var number = model.Split('-').Last();
            var cameraName = $"{number} | {Camera}";
            return $"{cameraName} | {Latitude} | {Longitude}";
        }
    }
}
