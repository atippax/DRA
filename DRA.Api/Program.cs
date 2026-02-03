using Newtonsoft.Json;
using NRedisStack;
using StackExchange.Redis;
var builder = WebApplication.CreateBuilder(args);
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var muxer = ConnectionMultiplexer.Connect("localhost:6379");
var db = muxer.GetDatabase();
var test = new List<ResourceMapper>
{
    new ResourceMapper("1","2",new Dictionary<string, int>()),
    new ResourceMapper("3","4",new Dictionary<string, int>()),
};
db.KeyDelete("test");
foreach (var t in test)
{
    db.StringSet(t.areaId + t.truckId, JsonConvert.SerializeObject(t));
    db.KeyExpire(t.areaId + t.truckId, new TimeSpan(0, 0, 10));

}
Console.WriteLine(JsonConvert.DeserializeObject(db.ListRange("test")[0]));
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
