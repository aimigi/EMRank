using CustomerRankAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// change CachedRankService here.
builder.Services.AddSingleton<IRankService, CustomerRankOffsetAlg>();
builder.Services.AddMvcCore();

var app = builder.Build();

app.MapControllers();

app.Run();
