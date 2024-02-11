using System.Reflection;
using Microsoft.OpenApi.Models;
using SpotifyAuthApi.Services;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
var isDev = builder.Environment.IsDevelopment();
if (isDev)
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000", 
                    "https://grooveguru.xyz") // note the port is included 
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        }
        
        );
});

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCookiePolicy(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddSession(options =>
{
    // Set appropriate options, if needed
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.Name = ".GrooveGuru.Auth.Cookie";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = 0;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

// Add the custom service registration here
builder.Services.AddSingleton<ISpotifyClientService, SpotifyClientService>();

var app = builder.Build();

if (isDev)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    });
}

app.UseCors("MyAllowedOrigins");

// Use sessions
app.UseSession();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();