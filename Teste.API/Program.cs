using Serilog;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Host.UseSerilog((context, options) =>
    options.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataProtection();


var app = builder.Build();

app.UseHttpsRedirection();

await app.RunAsync();
