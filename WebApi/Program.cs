// /////////////////////////////////////////////////////////////////////////////
// PLEASE DO NOT RENAME OR REMOVE ANY OF THE CODE BELOW. 
// YOU CAN ADD YOUR CODE TO THIS FILE TO EXTEND THE FEATURES TO USE THEM IN YOUR WORK.
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using WebApi.Entities;
using WebApi.Extension;
using WebApi.Helpers;
using WebApi.Interface;
using WebApi.Services;

_.__();

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    builder.WebHost.UseUrls("http://localhost:3000");
    builder.WebHost.ConfigureLogging((context, logging) =>
    {
        var config = context.Configuration.GetSection("Logging");
        logging.AddConfiguration(config);
        logging.AddConsole();
        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
        logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
    });

    var services = builder.Services;
    services.AddControllers();
    services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<DataContext>()
        .AddDefaultTokenProviders();
    services.AddSqlite<DataContext>("DataSource=webApi.db");

    services.AddDataProtection().UseCryptographicAlgorithms(
        new AuthenticatedEncryptorConfiguration
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        });
}
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddScoped<IValidServices, ValidServices>();
builder.Services.AddScoped<IPlayersServices, PlayersServices>();
builder.Services.AddScoped<ITeamServices, TeamServices>();
builder.Services.AddTransient<IJWTAuthenticator, JwtAuthenticator>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "please insert a token",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "UserMgt API", Version = "v1" });
    options.EnableAnnotations();
    options.UseInlineDefinitionsForEnums();
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddLogging(builder =>
{
    builder.AddConsole(); 
    builder.AddDebug();
});



var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlayerApp API v1");
    });
}


// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.EnsureCreated();
}

// configure HTTP request pipeline
{
    app.MapControllers();
}

app.Run();

public partial class Program { }
