using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Repository;
using Service.Validations;
using Core.Mappings;
using AutoMapper;

// Autofac DI kullanımı
var builder = WebApplication.CreateBuilder(args);

// Environment belirleme
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"🚀 Starting application in {environment} environment");

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new Api.Autofac.DependencyResolverModule());
});

// AutoMapper yapılandırması
builder.Services.AddAutoMapper(typeof(MapProfile));

// ***************************************************************
// 🔥 SUPABASE DATABASE CONFIGURATION
// ***************************************************************
string finalConnectionString;

// Hem Development hem Production için Supabase kullanıyoruz
// Environment variable'dan DATABASE_URL alıyoruz
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Environment variable varsa onu kullan (Production + Development)
    Console.WriteLine($"📦 Using DATABASE_URL from environment ({environment})");
    
    // Supabase PostgreSQL URI formatını parse et veya direkt kullan
    if (databaseUrl.StartsWith("postgresql://") || databaseUrl.StartsWith("postgres://"))
    {
        try
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Database = uri.AbsolutePath.TrimStart('/'),
                Username = userInfo[0],
                Password = userInfo.Length > 1 ? userInfo[1] : "",
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                Timeout = 30,
                CommandTimeout = 30,
                Pooling = true,
                MinPoolSize = 1,
                MaxPoolSize = builder.Environment.IsProduction() ? 20 : 5
            };
            
            finalConnectionString = connectionStringBuilder.ToString();
            Console.WriteLine($"✅ Successfully connected to Supabase: {uri.Host}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to parse DATABASE_URL. Error: {ex.Message}", ex);
        }
    }
    else
    {
        // Standart format ise direkt kullan
        finalConnectionString = databaseUrl;
        Console.WriteLine("✅ Using DATABASE_URL as-is (Supabase standard format)");
    }
}
else
{
    // Fallback: appsettings.json kullan (local test için)
    Console.WriteLine("💻 DATABASE_URL not found, using appsettings.json");
    
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException(
            "❌ DATABASE_URL environment variable or DefaultConnection in appsettings.json is required!\n" +
            "For Supabase setup, check: https://supabase.com/dashboard/project/_/settings/database");
    }
    
    finalConnectionString = connectionString;
    Console.WriteLine("⚠️ Using appsettings.json connection (not recommended for production)");
}

// Connection info logging
Console.WriteLine("=== SUPABASE CONNECTION INFO ===");
Console.WriteLine($"Environment: {environment}");
Console.WriteLine($"Connection preview: {finalConnectionString.Substring(0, Math.Min(40, finalConnectionString.Length))}...");
Console.WriteLine("================================");

// DbContext (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(finalConnectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });
    
    // Development'ta query logging aç
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ***************************************************************
// END DATABASE CONFIGURATION
// ***************************************************************

// CORS yapılandırması
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication yapılandırması
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TurlaGitsinAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TurlaGitsinApp";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Port yapılandırması
if (builder.Environment.IsProduction())
{
    // Render'ın PORT environment variable'ını kullan
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    Console.WriteLine($"🌐 Production: Listening on port {port}");
}
else
{
    // Local development için varsayılan portları kullan
    Console.WriteLine("🌐 Development: Using default ports");
}

// Controller, Swagger ve diğer servisler
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bitur API",
        Version = "v1",
        Description = $"Tour Management API - {environment}",
        Contact = new OpenApiContact
        {
            Name = "Bitur Team"
        }
    });
});

var app = builder.Build();

// ***************************************************************
// 🔥 DATABASE MIGRATION
// ***************************************************************
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        logger.LogInformation("🔄 Starting database migration...");
        
        // Bağlantıyı test et
        var canConnect = await context.Database.CanConnectAsync();
        if (!canConnect)
        {
            logger.LogError("❌ Cannot connect to database!");
            throw new Exception("Database connection failed!");
        }
        
        logger.LogInformation("✅ Database connection successful");
        
        // Migration'ları uygula
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation($"📦 Found {pendingMigrations.Count()} pending migrations: {string.Join(", ", pendingMigrations)}");
            
            // Production'da ek güvenlik kontrolü
            if (builder.Environment.IsProduction())
            {
                var autoMigrate = Environment.GetEnvironmentVariable("AUTO_MIGRATE") ?? "true";
                if (autoMigrate.ToLower() == "false")
                {
                    logger.LogWarning("⚠️ AUTO_MIGRATE is disabled. Skipping migrations.");
                    logger.LogWarning("⚠️ Run migrations manually or set AUTO_MIGRATE=true");
                }
                else
                {
                    logger.LogInformation("🔄 Applying migrations in production...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("✅ Database migration completed successfully");
                }
            }
            else
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("✅ Database migration completed successfully");
            }
        }
        else
        {
            logger.LogInformation("✅ Database is up to date, no migrations needed");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred while migrating the database");
        
        // Production'da migration hatası uygulamayı durdursun
        if (builder.Environment.IsProduction())
        {
            logger.LogCritical("🚨 Migration failed in production! Application cannot start.");
            throw;
        }
    }
}

// ***************************************************************
// MIDDLEWARE CONFIGURATION
// ***************************************************************

// Swagger yapılandırması
if (app.Environment.IsDevelopment())
{
    // Development'ta Swagger UI root'ta
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bitur API v1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "Bitur API - Development";
    });
}
else
{
    // Production'da Swagger /swagger altında
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bitur API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Bitur API - Production";
    });
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// HTTPS redirect sadece production'da ve gerekiyorsa
if (app.Environment.IsProduction())
{
    // Render otomatik HTTPS yönlendirme yapıyor
    Console.WriteLine("ℹ️ HTTPS redirect disabled (Render handles this)");
}

app.MapControllers();

// ***************************************************************
// HEALTH CHECK & ENDPOINTS
// ***************************************************************

// Health check endpoint (Render için kritik)
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    environment = environment,
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}))
.WithName("HealthCheck")
.WithOpenApi()
.AllowAnonymous();

// Weather forecast endpoint (test için)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm",
    "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

Console.WriteLine($"✅ Bitur API started successfully in {environment} mode");
Console.WriteLine($"📍 Swagger UI: {(app.Environment.IsDevelopment() ? "/" : "/swagger")}");
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}