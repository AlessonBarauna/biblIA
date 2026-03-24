using BíblIA.Api.Data;
using BíblIA.Api.Seed;
using BíblIA.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Em produção (Railway), DATABASE_URL é setado automaticamente pelo addon PostgreSQL.
// Railway fornece no formato URI: postgresql://user:pass@host:port/db
// Npgsql espera key-value: Host=...;Port=...;Database=...;Username=...;Password=...
// A função abaixo converte o formato URI para o formato aceito pelo Npgsql.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

static string? ConvertPostgresUrlToNpgsql(string? url)
{
    if (string.IsNullOrEmpty(url)) return null;
    if (!url.StartsWith("postgres://") && !url.StartsWith("postgresql://")) return url;

    var uri = new Uri(url);
    var userInfo = uri.UserInfo.Split(':', 2);
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;

    return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}

var npgsqlConnectionString = ConvertPostgresUrlToNpgsql(databaseUrl);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrEmpty(npgsqlConnectionString))
    {
        options.UseNpgsql(npgsqlConnectionString);
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        options.UseSqlite(connectionString);
    }
});

// HttpClient para chamadas à API Gemini
builder.Services.AddHttpClient();

// Services de negócio
builder.Services.AddScoped<GroqService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();

// JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS: localhost em dev + qualquer subdomínio *.vercel.app em produção.
// SetIsOriginAllowed é necessário com AllowCredentials() — não aceita wildcard.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.SetIsOriginAllowed(origin =>
                  origin.StartsWith("http://localhost") ||
                  origin.StartsWith("http://127.0.0.1") ||
                  origin.EndsWith(".vercel.app"))
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

// Inicializa o banco e semeia dados iniciais
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!string.IsNullOrEmpty(npgsqlConnectionString))
    {
        // PostgreSQL (Railway): EnsureCreated cria o schema direto do modelo.
        db.Database.EnsureCreated();
    }
    else
    {
        // SQLite (dev): aplica migrations incrementais normalmente
        db.Database.Migrate();
    }

    await DataSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS deve vir ANTES do UseHttpsRedirection — o redirect 301/302 não carrega
// headers CORS e o browser bloqueia o preflight OPTIONS com "Failed to fetch"
app.UseCors("AllowAngular");

// HTTPS redirect só faz sentido em produção — em dev a API roda em HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
