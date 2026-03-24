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
