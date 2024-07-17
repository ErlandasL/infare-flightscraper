namespace FlightScraper.Models
{
    public class FlightModel
    {
        public Header Header { get; set; }
        public Body Body { get; set; }
    }

    public class Header
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public bool Error { get; set; }
    }

    public class Body
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public List<Journey> Journeys { get; set; }
        public List<TotalAvailability> TotalAvailabilities { get; set; }
    }

    public class Journey
    {
        public int RecommendationId { get; set; }
        public List<Flight> Flights { get; set; }
        public double ImportTaxAdl { get; set; }
    }

    public class Flight
    {
        public string Number { get; set; }
        public Airport AirportDeparture { get; set; }
        public Airport AirportArrival { get; set; }
        public string DateDeparture { get; set; }
        public string DateArrival { get; set; }
        public string CompanyCode { get; set; }
    }

    public class Airport
    {
        public string Code { get; set; }
    }

    public class TotalAvailability
    {
        public int RecommendationId { get; set; }
        public double Total { get; set; }
    }

    public class FlightCsvTwoWays
    {
        public double Price { get; set; }
        public double Taxes { get; set; }
        public string Outbound1AirportDeparture { get; set; }
        public string Outbound1AirportArrival { get; set; }
        public string Outbound1TimeDeparture { get; set; }
        public string Outbound1TimeArrival { get; set; }
        public string Outbound1FlightNumber { get; set; }
        public string Inbound1AirportDeparture { get; set; }
        public string Inbound1AirportArrival { get; set; }
        public string Inbound1TimeDeparture { get; set; }
        public string Inbound1TimeArrival { get; set; }
        public string Inbound1FlightNumber { get; set; }
    }

    public class FlightCsv
    {
        public double Price { get; set; }
        public double Taxes { get; set; }
        public string Outbound1AirportDeparture { get; set; }
        public string Outbound1AirportArrival { get; set; }
        public string Outbound1TimeDeparture { get; set; }
        public string Outbound1TimeArrival { get; set; }
        public string Outbound1FlightNumber { get; set; }
        public string Outbound2AirportDeparture { get; set; }
        public string Outbound2AirportArrival { get; set; }
        public string Outbound2TimeDeparture { get; set; }
        public string Outbound2TimeArrival { get; set; }
        public string Outbound2FlightNumber { get; set; }
        public string Inbound1AirportDeparture { get; set; }
        public string Inbound1AirportArrival { get; set; }
        public string Inbound1TimeDeparture { get; set; }
        public string Inbound1TimeArrival { get; set; }
        public string Inbound1FlightNumber { get; set; }
        public string Inbound2AirportDeparture { get; set; }
        public string Inbound2AirportArrival { get; set; }
        public string Inbound2TimeDeparture { get; set; }
        public string Inbound2TimeArrival { get; set; }
        public string Inbound2FlightNumber { get; set; }
    }
}