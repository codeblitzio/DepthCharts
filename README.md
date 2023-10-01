## Architecture
The solution is build using .NET 6 (LTS) and is hosted as a Web API. It implements a CQRS pattern using Mediatr. The API uses minimal endpoints that delegate requests to a Mediatr handler to perform the heavy lifting. A repository layer has also been included to provide data access; for this demo, a mocked in-memory repository has been used. Dependency injection is used throughout.

## Use cases
The use cases specified in the Instructions.md have been implemented as API endpoints. The names and signatures have been tweaked.
- AddPlayer
- RemovePlayer
- GetDepthChart
- GetTrailingPlayers

## DataModel
The DepthChart exposes a list of positions, a position belongs to sport, and each position holds a ranked list of players. For this demo, a mocked in-memory repository has been used.  

## Testing
The application can be tested locally using Swagger or Postman. There's also a suite of unit tests; at the root of the unit test project, the ExampleTests test class covers the examples detailed in the Instructions.md file.

 ## Scalability
 Scalability is achieved by seeding the datastore with positions for a new sport. A "Sport" configuration property has been added to appsettings.json i.e. "NBL, "NFL"; set this config property to enable the desired sport. No further code changes are required to add a new sport.

 ## Error handling
 The API uses the Hellang exception handling middleware to convert exceptions into problem details responses.

 ## Logging
 Serilog is used to provide structured logging.

 ## Validation
 The FluentValidations library is used to validate requests. A validator has been provided for each Mediatr command or query.
