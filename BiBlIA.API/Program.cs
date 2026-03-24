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

// SQLite: em produção usa /data/biblia.db (volume persistente do Railway)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
if (builder.Environment.IsProduction())
{
    var dataDir = Environment.GetEnvironmentVariable("DATA_DIR") ?? "/data";
    Directory.CreateDirectory(dataDir);
    connectionString = $"Data Source={Path.Combine(dataDir, "biblia.db")}";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

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

// CORS: origens locais + domínios de produção configurados via env var CORS_ORIGINS
// Ex: CORS_ORIGINS=https://meuapp.vercel.app,https://outro.vercel.app
var corsOrigins = new List<string>
{
    "http://localhost:4200",
    "http://localhost:60919",
    "http://127.0.0.1:4200",
    "http://127.0.0.1:60919"
};

var extraOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS");
if (!string.IsNullOrWhiteSpace(extraOrigins))
    corsOrigins.AddRange(extraOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins(corsOrigins.ToArray())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

// Aplica migrations pendentes e popula com dados iniciais se estiver vazio
// Migrate() é idempotente: só roda migrations que ainda não foram aplicadas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
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
