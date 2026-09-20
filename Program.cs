using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ConstructionApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    var materialsCsvPath = Path.Combine(AppContext.BaseDirectory, "Samples", "test_materials.csv");
    DbInitializer.SeedPriceLists(db, materialsCsvPath);
    DbInitializer.SeedPhases(db, Path.Combine(AppContext.BaseDirectory, "Samples", "test_phases.csv"));
    DbInitializer.SeedAssemblies(db, Path.Combine(AppContext.BaseDirectory, "Samples", "test_assemblies.csv"), materialsCsvPath);
}

app.Run();
