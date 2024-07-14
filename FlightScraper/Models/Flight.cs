namespace FlightScraper.Models;

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

public class FlightCsv
{
    public double Price { get; set; }
    public double Taxes { get; set; }
    public string OutboundAirportDeparture { get; set; }
    public string OutboundAirportArrival { get; set; }
    public string OutboundTimeDeparture { get; set; }
    public string OutboundTimeArrival { get; set; }
    public string OutboundFlightNumber { get; set; }
    public string InboundAirportDeparture { get; set; }
    public string InboundAirportArrival { get; set; }
    public string InboundTimeDeparture { get; set; }
    public string InboundTimeArrival { get; set; }
    public string InboundFlightNumber { get; set; }
}