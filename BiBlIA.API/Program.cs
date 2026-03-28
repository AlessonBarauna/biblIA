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
        // Passo 1: ALTER TABLE para colunas novas em tabelas já existentes.
        // IF NOT EXISTS é idempotente — seguro para re-deploy.
        // EnsureCreated() não faz ALTER TABLE, apenas CREATE TABLE IF NOT EXISTS.
        await db.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""TheologyCourses""
                ADD COLUMN IF NOT EXISTS ""ExternalUrl"" TEXT;
            ALTER TABLE ""TheologyCourses""
                ADD COLUMN IF NOT EXISTS ""Provider""    TEXT;
        ");

        // Passo 2: CREATE TABLE para tabelas completamente novas.
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""BibleStudyNotes"" (
                ""Id""                      SERIAL PRIMARY KEY,
                ""BookId""                  INTEGER NOT NULL,
                ""Chapter""                 INTEGER NOT NULL,
                ""Title""                   TEXT NOT NULL DEFAULT '',
                ""Context""                 TEXT NOT NULL DEFAULT '',
                ""TheologicalSignificance"" TEXT NOT NULL DEFAULT '',
                ""KeyThemes""               TEXT NOT NULL DEFAULT '',
                ""CrossReferences""         TEXT NOT NULL DEFAULT '',
                ""Commentary""              TEXT NOT NULL DEFAULT '',
                ""AuthorNote""              TEXT NOT NULL DEFAULT '',
                ""CreatedAt""               TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT ""FK_BibleStudyNotes_BibleBooks_BookId""
                    FOREIGN KEY (""BookId"") REFERENCES ""BibleBooks""(""Id"") ON DELETE CASCADE,
                CONSTRAINT ""UQ_BibleStudyNotes_BookId_Chapter""
                    UNIQUE (""BookId"", ""Chapter"")
            );
        ");

        // Passo 3: BookmarkVerses — tabela antiga usava SessionId (anônimo).
        // Nova versão usa UserId (FK para Users). Sem dados para preservar (feature nunca foi exposta).
        await db.Database.ExecuteSqlRawAsync(@"
            DROP TABLE IF EXISTS ""BookmarkVerses"";
            CREATE TABLE IF NOT EXISTS ""BookmarkVerses"" (
                ""Id""          SERIAL PRIMARY KEY,
                ""UserId""      INTEGER NOT NULL,
                ""BookId""      INTEGER NOT NULL,
                ""Chapter""     INTEGER NOT NULL,
                ""Verse""       INTEGER NOT NULL,
                ""VerseText""   TEXT NOT NULL DEFAULT '',
                ""Note""        TEXT NOT NULL DEFAULT '',
                ""CreatedAt""   TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT ""FK_BookmarkVerses_Users_UserId""
                    FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE,
                CONSTRAINT ""FK_BookmarkVerses_BibleBooks_BookId""
                    FOREIGN KEY (""BookId"") REFERENCES ""BibleBooks""(""Id"") ON DELETE CASCADE
            );
            CREATE INDEX IF NOT EXISTS ""IX_BookmarkVerses_UserId"" ON ""BookmarkVerses""(""UserId"");
        ");

        // Passo 5: UserProgress — tabela antiga usava SessionId (anônimo).
        // Nova versão usa UserId (FK) + CourseId. Sem dados para preservar.
        await db.Database.ExecuteSqlRawAsync(@"
            DROP TABLE IF EXISTS ""UserProgress"";
            CREATE TABLE IF NOT EXISTS ""UserProgress"" (
                ""Id""          SERIAL PRIMARY KEY,
                ""UserId""      INTEGER NOT NULL,
                ""CourseId""    INTEGER NOT NULL,
                ""ModuleId""    INTEGER NOT NULL,
                ""Completed""   BOOLEAN NOT NULL DEFAULT FALSE,
                ""CompletedAt"" TIMESTAMP WITHOUT TIME ZONE,
                ""Score""       INTEGER NOT NULL DEFAULT 0,
                ""CreatedAt""   TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT ""FK_UserProgress_Users_UserId""
                    FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE,
                CONSTRAINT ""UQ_UserProgress_UserId_ModuleId""
                    UNIQUE (""UserId"", ""ModuleId"")
            );
            CREATE INDEX IF NOT EXISTS ""IX_UserProgress_UserId"" ON ""UserProgress""(""UserId"");
        ");

        // Passo 6: Planos de leitura bíblica
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""ReadingPlans"" (
                ""Id""          SERIAL PRIMARY KEY,
                ""Name""        TEXT NOT NULL DEFAULT '',
                ""Description"" TEXT NOT NULL DEFAULT '',
                ""TotalDays""   INTEGER NOT NULL DEFAULT 0,
                ""Strategy""    TEXT NOT NULL DEFAULT '',
                ""Icon""        TEXT NOT NULL DEFAULT ''
            );
            CREATE TABLE IF NOT EXISTS ""ReadingLogs"" (
                ""Id""          SERIAL PRIMARY KEY,
                ""UserId""      INTEGER NOT NULL,
                ""PlanId""      INTEGER NOT NULL,
                ""DayNumber""   INTEGER NOT NULL,
                ""CompletedAt"" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT ""FK_ReadingLogs_Users_UserId""
                    FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE,
                CONSTRAINT ""FK_ReadingLogs_ReadingPlans_PlanId""
                    FOREIGN KEY (""PlanId"") REFERENCES ""ReadingPlans""(""Id"") ON DELETE CASCADE,
                CONSTRAINT ""UQ_ReadingLogs_User_Plan_Day""
                    UNIQUE (""UserId"", ""PlanId"", ""DayNumber"")
            );
            CREATE INDEX IF NOT EXISTS ""IX_ReadingLogs_UserId"" ON ""ReadingLogs""(""UserId"");
        ");

        // Passo 7: Anotações pessoais em versículos
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""VerseNotes"" (
                ""Id""        SERIAL PRIMARY KEY,
                ""UserId""    INTEGER NOT NULL,
                ""BookId""    INTEGER NOT NULL,
                ""Chapter""   INTEGER NOT NULL,
                ""Verse""     INTEGER NOT NULL,
                ""Note""      TEXT NOT NULL DEFAULT '',
                ""CreatedAt"" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                ""UpdatedAt"" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                CONSTRAINT ""FK_VerseNotes_Users_UserId""
                    FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE,
                CONSTRAINT ""UQ_VerseNotes_User_Book_Chapter_Verse""
                    UNIQUE (""UserId"", ""BookId"", ""Chapter"", ""Verse"")
            );
            CREATE INDEX IF NOT EXISTS ""IX_VerseNotes_UserId"" ON ""VerseNotes""(""UserId"");
        ");

        // Passo 8: EnsureCreated cuida de tabelas novas que ainda não existem.
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
