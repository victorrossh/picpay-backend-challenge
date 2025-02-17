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
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.Preload = true;
    options.IncludeSubDomains = true;
});
services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddHttpContextAccessor();
services.AddDataProtection();
services.AddProblemDetails();
await services.AddApplicationInjection(configuration);

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(configuration.GetConfiguration<string[]>("Cors:Origins"))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
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
        options.RequireHttpsMetadata = true;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{configuration.GetConfiguration<string>("Swagger:Title")} v1");
        c.OAuthClientId("swagger-ui-client-id");
        c.OAuthAppName("Swagger UI");
    });
    configuration.AddUserSecrets<Program>();
}

app.UseSerilogRequestLogging();
app.UseRouting();

// Ensure CORS middleware comes before authentication
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.UseHealthChecks("/health");
app.UseHsts();
app.UseAntiforgery();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
//app.UseMiddleware<RetryPolicyMiddleware>();
//app.UseMiddleware<TimeoutPolicyMiddleware>();
//app.UseMiddleware<CircuitBreakerPolicyMiddleware>();
app.UseXContentTypeOptions();
app.MapControllers();
app.MapGet("/", () => "Hello World!");
await app.RunAsync();