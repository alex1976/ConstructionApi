using ConstructionApi.McpServer.Data;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=../construction.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<SearchTools>()
    .WithTools<PriceListTools>()
    .WithTools<AssemblyTools>()
    .WithTools<PhaseTools>();

var app = builder.Build();

app.MapMcp("/mcp");

app.Run();
