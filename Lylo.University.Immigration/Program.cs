using Lylo.University.Immigration.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Lylo.University.Immigration.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Exchange API", Version = "v1" });
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<ExchangeRateService>();


builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});


builder.Services.AddDbContext<LyloUniversityImmigrationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LyloUniversityImmigrationContext") ?? throw new InvalidOperationException("Connection string 'LyloUniversityImmigrationContext' not found.")));


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("LyloUniversityImmigrationContext") ?? throw new InvalidOperationException("Connection string 'LyloUniversityImmigrationContext' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//Application Insights
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});

app.MapGet("/exchange-rate", async (HttpContext context, ExchangeRateService exchangeRateService) =>
{
    string exchangeRate = await exchangeRateService.GetExchangeRate("UAH", "USD");
    await context.Response.WriteAsync($"Exchange rate EUR/USD: {exchangeRate}");
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
