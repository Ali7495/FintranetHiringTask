Congestion Tax Calculator (.NET 8)



Run Locally
dotnet restore
dotnet ef database update --project ./src/CongestionTaskCalculator.Infrastructure --startup-project ./src/CongestionTaskCalculator.Api
dotnet run --project ./src/CongestionTaskCalculator.Api


Swagger : http://localhost:5262/swagger

Example:

POST /tax/calculate

{
  "city": "GOTH",
  "vehicleType": "Car",
  "dateTimes": [
    "2013-02-08T06:23:27",
    "2013-02-08T15:27:00",
    "2013-02-08T16:01:00"
  ]
}


Response:

{
  "total": 31,
  "capped": false,
  "windows": [
    { "from": "2013-02-08T06:23:27", "to": "2013-02-08T06:23:27", "appliedFee": 8 },
    { "from": "2013-02-08T15:27:00", "to": "2013-02-08T16:01:00", "appliedFee": 18 }
  ]
}

Tech Overview:

.NET 8 + Minimal API

EF Core + SQLite (Code-First, seeded rules)

FluentValidation for inputs

IMemoryCache for rule lookups

Middleware , ProblemDetails + Correlation ID

xUnit unit tests 

Clean Architecture:
Layer	Responsibility
Domain	Tax logic (GothenburgTaxCalculator)
Application	Interfaces, contracts
Infrastructure	EF Core, caching, providers
API	Endpoints, validation, middleware
Tests	xUnit


Test:
dotnet test
