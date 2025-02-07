using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Teste.API.Filters;
using Teste.API.MIddlewares;
using Teste.Application;
using Teste.Shared.Utilities;

var builder = WebApplication.CreateSlimBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseSerilog((context, options) =>
    options.ReadFrom.Configuration(context.Configuration));

services.AddHealthChecks();
services.AddScoped<ExceptionFilter>();

services.AddControllers(options => { options.Filters.AddService<ExceptionFilter>(); });
services.AddEndpointsApiExplorer();
services.AddHttpContextAccessor();
services.AddDataProtection();

await services.AddApplicationInjection(configuration);

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(configuration.GetConfiguration<string[]>("Cors:Origins"))
            .WithMethods(configuration.GetConfiguration<string[]>("Cors:Methods"))
            .WithHeaders(configuration.GetConfiguration<string[]>("Cors:Headers"));
    });
});

services.AddSwaggerGen(opt =>
{
    var version = configuration.GetConfiguration<string>("Swagger:Version");

    opt.SwaggerDoc(version, new OpenApiInfo
    {
        Title = configuration.GetConfiguration<string>("Swagger:Title"),
        Version = version
    });
});
services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetConfiguration<string>("Jwt:Secret"))),
            ValidIssuer = configuration.GetConfiguration<string>("Jwt:Issuer"),
            ValidAudience = configuration.GetConfiguration<string>("Jwt:Audience"),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    configuration.AddUserSecrets<Program>();
}

app.UseRouting();
app.UseCors();
app.UseHsts();
app.UseAuthentication();
app.UseHealthChecks("/health");
app.UseMiddleware<RequestIdMiddleware>();
app.MapControllers();
await app.RunAsync();