using System.Diagnostics;
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
            string url = BuildUrl();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                
                if (response == "<H1> Departure Date Not Available </H1>")
                {
                    Console.WriteLine("Departure date not available. Stopping execution.");
                    return;
                }
            }

            try
            {
                var flightsData = await FetchFlightData();
                if (flightsData != null)
                {
                    var flightCsvTwoWays = MapToCsvTwoWays(flightsData);
                    var flightCsv = MapToCsv(flightsData);
                    ExportToCsv(flightCsvTwoWays, flightCsv);
                }
                else
                {
                    Console.WriteLine("Failed to fetch flight data.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
            }
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
            string from = "JFK";
            string to = "AUH";
            string depart = "2024-08-25";
            string returnDate = "2024-08-30";
            return $"{baseUrl}?from={from}&to={to}&depart={depart}&return={returnDate}";
        }

        static string BuildFilename(string url, string suffix)
        {
            Uri uri = new Uri(url);
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);

            string from = queryParams["from"];
            string to = queryParams["to"];
            string depart = queryParams["depart"];
            string returnDate = queryParams["return"];

            return $"flights-{from}-{to}-{depart}-{returnDate}-{suffix}.csv";
        }

        static List<FlightCsvTwoWays> MapToCsvTwoWays(FlightModel flightData)
        {
            var flightCsvTwoWays = new List<FlightCsvTwoWays>();
            var journeys = flightData.Body.Data.Journeys;
            var totalAvailabilities = flightData.Body.Data.TotalAvailabilities.ToDictionary(ta => ta.RecommendationId, ta => ta.Total);

            var groupedJourneys = journeys
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
                if (group.Flights.Count == 2)
                {
                    var firstFlight = group.Flights[0];
                    var secondFlight = group.Flights[1];

                    flightCsvTwoWays.Add(new FlightCsvTwoWays
                    {
                        Price = Math.Round(group.TotalPrice, 2),
                        Taxes = Math.Round(group.TotalTaxes, 2),
                        Outbound1AirportDeparture = firstFlight.AirportDeparture.Code,
                        Outbound1AirportArrival = firstFlight.AirportArrival.Code,
                        Outbound1TimeDeparture = firstFlight.DateDeparture,
                        Outbound1TimeArrival = firstFlight.DateArrival,
                        Outbound1FlightNumber = firstFlight.CompanyCode + firstFlight.Number,
                        Inbound1AirportDeparture = secondFlight.AirportDeparture.Code,
                        Inbound1AirportArrival = secondFlight.AirportArrival.Code,
                        Inbound1TimeDeparture = secondFlight.DateDeparture,
                        Inbound1TimeArrival = secondFlight.DateArrival,
                        Inbound1FlightNumber = secondFlight.CompanyCode + secondFlight.Number,
                    });
                }
            }

            return flightCsvTwoWays;
        }

        static List<FlightCsv> MapToCsv(FlightModel flightData)
        {
            var flightCsv = new List<FlightCsv>();
            var journeys = flightData.Body.Data.Journeys;
            var totalAvailabilities = flightData.Body.Data.TotalAvailabilities.ToDictionary(ta => ta.RecommendationId, ta => ta.Total);

            var groupedJourneys = journeys
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
                if (group.Flights.Count == 4)
                {
                    var firstOutboundFlight = group.Flights[0];
                    var secondOutboundFlight = group.Flights[1];
                    var firstInboundFlight = group.Flights[2];
                    var secondInboundFlight = group.Flights[3];

                    flightCsv.Add(new FlightCsv
                    {
                        Price = Math.Round(group.TotalPrice, 2),
                        Taxes = Math.Round(group.TotalTaxes, 2),
                        Outbound1AirportDeparture = firstOutboundFlight.AirportDeparture.Code,
                        Outbound1AirportArrival = firstOutboundFlight.AirportArrival.Code,
                        Outbound1TimeDeparture = firstOutboundFlight.DateDeparture,
                        Outbound1TimeArrival = firstOutboundFlight.DateArrival,
                        Outbound1FlightNumber = firstOutboundFlight.CompanyCode + firstOutboundFlight.Number,
                        Outbound2AirportDeparture = secondOutboundFlight.AirportDeparture.Code,
                        Outbound2AirportArrival = secondOutboundFlight.AirportArrival.Code,
                        Outbound2TimeDeparture = secondOutboundFlight.DateDeparture,
                        Outbound2TimeArrival = secondOutboundFlight.DateArrival,
                        Outbound2FlightNumber = secondOutboundFlight.CompanyCode + secondOutboundFlight.Number,
                        Inbound1AirportDeparture = firstInboundFlight.AirportDeparture.Code,
                        Inbound1AirportArrival = firstInboundFlight.AirportArrival.Code,
                        Inbound1TimeDeparture = firstInboundFlight.DateDeparture,
                        Inbound1TimeArrival = firstInboundFlight.DateArrival,
                        Inbound1FlightNumber = firstInboundFlight.CompanyCode + firstInboundFlight.Number,
                        Inbound2AirportDeparture = secondInboundFlight.AirportDeparture.Code,
                        Inbound2AirportArrival = secondInboundFlight.AirportArrival.Code,
                        Inbound2TimeDeparture = secondInboundFlight.DateDeparture,
                        Inbound2TimeArrival = secondInboundFlight.DateArrival,
                        Inbound2FlightNumber = secondInboundFlight.CompanyCode + secondInboundFlight.Number
                    });
                }
            }

            return flightCsv;
        }

        static void ExportToCsv(List<FlightCsvTwoWays> flightCsvTwoWays, List<FlightCsv> flightCsv)
        {
            string url = BuildUrl();

            if (flightCsvTwoWays.Any())
            {
                using (var writer = new StreamWriter(BuildFilename(url, "two-ways")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(flightCsvTwoWays);
                }
                Console.WriteLine("Two-ways CSV file saved successfully.");
            }
            else
            {
                Console.WriteLine("No two-ways flight data to save.");
            }

            if (flightCsv.Any())
            {
                using (var writer = new StreamWriter(BuildFilename(url, "multi-legs")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(flightCsv);
                }
                Console.WriteLine("Multi-legs CSV file saved successfully.");
            }
            else
            {
                Console.WriteLine("No multi-legs flight data to save.");
            }
        }
    }
}