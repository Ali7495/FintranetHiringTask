
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


//builder.Host.UseSerilog();        

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<CalculateTaxRequestValidator>();
builder.Services.Configure<TaxOptions>(builder.Configuration.GetSection("TaxOption"));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<ITaxCalculator, GothenburgTaxCalculator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.MigrationAndSeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();

#region minimal Apis

app.MapPost("/tax/calculate", async (
    CalculateTaxRequest request,
    IRulesProvider rulesProvider,
    ITaxCalculator taxCalculator,
    IOptionsSnapshot<TaxOptions> taxOptions,
    CancellationToken cancellationToken) =>
{
    var cityCode = string.IsNullOrWhiteSpace(request.City) ? taxOptions.Value.DefaultCityCode : request.City;
    var rules = await rulesProvider.GetRulesAsync(cityCode, cancellationToken);
    if (rules is null) return Results.NotFound($"City '{cityCode}' not found.");

    if (!Enum.TryParse<VehicleType>(request.VehicleType, true, out var vehicleType))
        return Results.BadRequest("Unknown vehicle type.");

    TaxResult result = taxCalculator.CalculateDailyTax(vehicleType, request.DateTimes, rules.City, rules.TollBands, rules.Holidays);

    return Results.Ok(result);
})
.WithName("CalculateTax")
.Produces(StatusCodes.Status200OK);

app.MapGet("/rules/{city}", async (string city, IRulesProvider rulesProvider) =>
{
    var rules = await rulesProvider.GetRulesAsync(city);
    return rules is null ? Results.NotFound() : Results.Ok(rules);
})
.WithName("GetRules");

app.MapGet("/", () => Results.Ok("API is OK. Use /swagger"));

#endregion

app.Run();


public record CalculateTaxRequest(string City, string VehicleType, DateTime[] DateTimes);
