using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Teste.API.Middlewares;
using Teste.Application;
using Teste.Shared.Utilities;

var builder = WebApplication.CreateSlimBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseSerilog((context, options) =>
    options.ReadFrom.Configuration(context.Configuration));

services.AddHealthChecks();

services.AddControllers();
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

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});
services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
var key = Encoding.ASCII.GetBytes(configuration.GetConfiguration<string>("Jwt:Secret"));

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidIssuer = configuration.GetConfiguration<string>("Jwt:Issuer"),
    ValidateAudience = true,
    ValidAudience = configuration.GetConfiguration<string>("Jwt:Audience"),
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

services.AddSingleton(tokenValidationParameters);

services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = tokenValidationParameters;
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

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors();
app.UseHsts();
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthChecks("/health");
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
await app.RunAsync();