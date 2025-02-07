using Serilog;
using Teste.Application;

var builder = WebApplication.CreateSlimBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseSerilog((context, options) =>
    options.ReadFrom.Configuration(context.Configuration));

services.AddControllers();

services.AddDataProtection();

await services.AddApplicationInjection(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    configuration.AddUserSecrets<Program>();
}

app.MapControllers();

app.UseHttpsRedirection();

await app.RunAsync();