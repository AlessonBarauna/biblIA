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
// Em dev, usa SQLite local.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Railway PostgreSQL: persiste entre deploys, sem necessidade de volume
        options.UseNpgsql(databaseUrl);
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

    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // PostgreSQL (Railway): EnsureCreated cria o schema direto do modelo.
        // Não usa migrations — mais simples para produção sem volume.
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
