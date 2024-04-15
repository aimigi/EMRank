using CustomerRankAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// change CachedRankService here.
// Solution1  SkipList 
// Solution2  RBTree     RBTreeSimple
builder.Services.AddSingleton<IRankService, SkipListRankService>();
builder.Services.AddMvcCore();

var app = builder.Build();

app.MapControllers();

app.Run();
