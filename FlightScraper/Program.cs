using CsvHelper;
using Newtonsoft.Json;
using System.Globalization;
using FlightScraper.Models;

namespace FlightScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var flightsData = await FetchFlightData();
            var flightCsv = MapToCsv(flightsData);
            ExportToCsv(flightCsv);
        }

        static async Task<FlightModel> FetchFlightData()
        {
            string url = BuildUrl();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject<FlightModel>(response);
            }
        }

        static string BuildUrl()
        {
            string baseUrl = "http://homeworktask.infare.lt/search.php";
            string from = "MAD";
            string to = "AUH";
            string depart = "2024-08-23";
            string returnDate = "2024-08-29";
            return $"{baseUrl}?from={from}&to={to}&depart={depart}&return={returnDate}";
        }
        
        static string BuildFilename(string url)
        {
            Uri uri = new Uri(url);
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);

            string from = queryParams["from"];
            string to = queryParams["to"];
            string depart = queryParams["depart"];
            string returnDate = queryParams["return"];

            return $"flights-{from}-{to}-{depart}-{returnDate}.csv";
        }

        static List<FlightCsv> MapToCsv(FlightModel flightData)
        {
            var flightCsv = new List<FlightCsv>();
            var journeys = flightData.Body.Data.Journeys;
            var totalAvailabilities = flightData.Body.Data.TotalAvailabilities.ToDictionary(ta => ta.RecommendationId, ta => ta.Total);

            var groupedJourneys = journeys
                .Where(j => j.Flights.Count == 1)
                .GroupBy(j => j.RecommendationId)
                .Select(g => new
                {
                    RecommendationId = g.Key,
                    Flights = g.SelectMany(j => j.Flights).ToList(),
                    TotalPrice = totalAvailabilities[g.Key],
                    TotalTaxes = g.Sum(j => j.ImportTaxAdl)
                });

            foreach (var group in groupedJourneys)
            {
                var firstFlight = group.Flights.First();
                var secondFlight = group.Flights.Last();

                flightCsv.Add(new FlightCsv
                {
                    Price = group.TotalPrice,
                    Taxes = group.TotalTaxes,
                    OutboundAirportDeparture = firstFlight.AirportDeparture.Code,
                    OutboundAirportArrival = firstFlight.AirportArrival.Code,
                    OutboundTimeDeparture = firstFlight.DateDeparture,
                    OutboundTimeArrival = firstFlight.DateArrival,
                    OutboundFlightNumber = firstFlight.CompanyCode + firstFlight.Number,
                    InboundAirportDeparture = secondFlight.AirportDeparture.Code,
                    InboundAirportArrival = secondFlight.AirportArrival.Code,
                    InboundTimeDeparture = secondFlight.DateDeparture,
                    InboundTimeArrival = secondFlight.DateArrival,
                    InboundFlightNumber = secondFlight.CompanyCode + secondFlight.Number
                });
            }

            return flightCsv;
        }

        static void ExportToCsv(List<FlightCsv> flightCsv)
        {
            using (var writer = new StreamWriter(BuildFilename(BuildUrl())))  // path to file /FlightScraper/bin/Debug/net8.0/flights-MAD-AUH-2024-08-23-2024-08-29.csv
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(flightCsv);
            }

            Console.WriteLine("CSV file saved to: flights.csv");
        }
    }
}