# CoreHackerNews

## Assumptions
Given the sentence _The API should return an array of the first 20 "best stories" as returned by the Hacker News API, sorted by their score in a descending order._
I assumed that I didn't need to filter out the items that have `Dead` and `Deleted` as `true`.

I also assumed that the endpoint https://hacker-news.firebaseio.com/v0/beststories.json only returns items with `"type": "story"`.

## Improvements
The main improvement I would do would be to use a proper distributed cache like Redis, instead of the built in IMemoryCache.  
Some other small improvements that could be made would be to improve the swagger generated documentation and to use Serilog instead of the default Microsoft logging.  
If this service increased in scope and size other improvements could be done, like add authentication, add logging to file, extract businness rules to a new class library project (maybe even make it a NuGet package if that business logic would need to be reused somewhere else), etc.

## How to build and run
### Build
From the command line navigate to `CoreHackerNews\CoreHackerNews` and run the command `dotnet build`.
### Run
From the command line navigate to `CoreHackerNews\CoreHackerNews` (same directory as for the build) and run the command `dotnet run`.

## How to run the unit tests
From the command line navigate to `CoreHackerNews\CoreHackerNewsTests` and run the command `dotnet test`.

## Url for the SwaggerUI
For checking the auto-generated documentation open the following url on the browser: https://localhost:5001/api/documentation.
