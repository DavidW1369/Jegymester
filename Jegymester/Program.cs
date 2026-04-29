using Jegymester.Context;
using Jegymester.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SZOLGÁLTATÁSOK REGISZTRÁLÁSA ---

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Adatbázis kontextus (SQLite használatával)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Jegymester.db"));

// CORS szabály: Itt engedélyezzük, hogy a React (localhost:5174) elérje az API-t
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:5173") // A frontend pontos címe
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Szolgáltatások regisztrálása Dependency Injection-höz
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CashierService>();
builder.Services.AddScoped<AdminService>();
var app = builder.Build();

// --- 2. ADATBÁZIS INICIALIZÁLÁSA ---

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Létrehozza az adatbázist és a táblákat, ha még nem léteznek
    db.Database.EnsureCreated();
}

// --- 3. MIDDLEWARE KONFIGURÁCIÓ (A SORREND FONTOS!) ---

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 1. A CORS-nak az elsõk között kell lennie, mindenképp a MapControllers elõtt!
app.UseCors();

// 2. Fejlesztés alatt ezt érdemes kikapcsolni, ha gond van az SSL/HTTPS tanúsítványokkal
// app.UseHttpsRedirection(); 

app.UseAuthorization();

// Végpontok regisztrálása
app.MapControllers();

app.Run();