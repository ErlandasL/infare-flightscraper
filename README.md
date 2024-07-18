### After running application path to file:
/FlightScraper/bin/Debug/net8.0/flights...csv

### Select your flight here, from, to and dates:
```
static string BuildUrl()
        {
            string baseUrl = "http://homeworktask.infare.lt/search.php";
            string from = "JFK";
            string to = "AUH";
            string depart = "2024-08-25";
            string returnDate = "2024-08-30";
            return $"{baseUrl}?from={from}&to={to}&depart={depart}&return={returnDate}";
        }
```
