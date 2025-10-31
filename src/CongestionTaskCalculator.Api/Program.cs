using Serilog;

var builder = WebApplication.CreateBuilder(args);


//builder.Host.UseSerilog();        

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region minimal Apis

app.MapPost("/tax/calculate", (CalculateTaxRequest req) =>
{
    return Results.Ok(new { total = 0, windows = Array.Empty<object>(), capped = false });
})
.WithName("CalculateTax")
.Produces(StatusCodes.Status200OK);

#endregion

app.Run();


public record CalculateTaxRequest(string City, string VehicleType, DateTime[] DateTimes);
