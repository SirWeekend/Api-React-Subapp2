using Eksamen2024.DAL;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Legg til autentisering med cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Bruker/Login"; // Sett stien for påloggingssiden
        options.LogoutPath = "/Bruker/Logout"; // Sett stien for utloggingssiden
    });

// Legg til session og TempData-støtte
builder.Services.AddSession();
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve); // JSON-serialisering

// Legg til CORS-støtte
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Tillat frontend-URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Tillat cookies/autentisering
    });
});

// Legger til databaseforbindelse
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrer repositorier for Dependency Injection
builder.Services.AddScoped<IPinpointRepository, PinpointRepository>();

// Legger til Swagger og XML-kommentarer for bedre dokumentasjon av API-et
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Logger konfigurasjon
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Konfigurer pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend"); // Aktiverer CORS-policyen
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();          // For TempData
app.UseAuthentication();    // For autentisering
app.UseAuthorization();     // For autorisasjon

// Seed initial database data
DBInit.Seed(app);

app.MapControllers(); // Map API-kontrollere
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
