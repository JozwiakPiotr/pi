using FastEndpoints;
using Inventory.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
	.ReadFrom.Configuration(context.Configuration)
	.ReadFrom.Services(services)
	.Enrich.FromLogContext()
	.WriteTo.Console());

builder.Services.AddFastEndpoints();
builder.Services.AddDbContext<InventoryDbContext>(options =>
{
	var connectionString = builder.Configuration.GetConnectionString("InventoryDb")
		?? throw new InvalidOperationException("Connection string 'InventoryDb' was not found.");

	options.UseNpgsql(connectionString);
});

builder.Host.UseWolverine(options =>
{
	var rabbitMqUri = builder.Configuration.GetConnectionString("RabbitMq")
		?? throw new InvalidOperationException("Connection string 'RabbitMq' was not found.");

	options.UseRabbitMq(new Uri(rabbitMqUri));
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseFastEndpoints();

app.MapGet("/", () => Results.Ok(new { Service = "Inventory" }));

app.Run();
