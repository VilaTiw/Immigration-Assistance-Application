using Lylo.University.Immigration.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Lylo.University.Immigration.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.AI.FormRecognizer;
using Azure;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Lylo.University.Immigration;
using System.Net.Http;
using Microsoft.Rest;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;



builder.Services.AddScoped<IComputerVisionClient>(provider =>
{
    // Код для створення та налаштування IComputerVisionClient

    var endpoint = configuration["ComputerVisionApi:Endpoint"];
    var apiKey = configuration["ComputerVisionApi:ApiKey"];
    var credentials = new ApiKeyServiceClientCredentials(apiKey);
    var computerVisionClient = new ComputerVisionClient(credentials)
    {
        Endpoint = endpoint
    };
    return computerVisionClient;
});



builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
});


builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Exchange API", Version = "v1" });
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<ExchangeRateService>();
builder.Services.AddScoped<ImageProcessingServiceFactory>();


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



if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});

app.MapGet("/exchange-rate", async (HttpContext context, ExchangeRateService exchangeRateService) =>
{
    string exchangeRate = await exchangeRateService.GetExchangeRate("USD", "USD");
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



app.MapPost("/process-image", async (HttpContext context, ImageProcessingServiceFactory serviceFactory) =>
{
    if (context.Request.HasFormContentType)
    {
        var form = await context.Request.ReadFormAsync();
        var imageFile = form.Files.GetFile("imageFile");

        Console.WriteLine($"Received image: {imageFile.FileName}");
        Console.WriteLine($"Image size: {imageFile.Length} bytes");

        if (imageFile != null && imageFile.Length > 0)
        {
            using var imageStream = imageFile.OpenReadStream();

            if (imageStream != null && imageStream.Length > 0)
            {
                try
                {
                    //екземпляр ImageProcessingService з фабрики
                    var imageProcessingService = serviceFactory.CreateImageProcessingService();

                    using var memoryStream = new MemoryStream();
                    await imageStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0; // Скидання позиції потоку на початок
                    imageStream.Position = 0;

                    //метод для обробки зображення та отримання тексту
                    if (imageStream != null) Console.WriteLine("Image stream isn`t null!");
                    var extractedText = await imageProcessingService.ExtractTextFromImageAsync(memoryStream);

                    Console.WriteLine($"Extracted text: {extractedText}");

                    //відповідь з отриманим текстом
                    await context.Response.WriteAsync(extractedText);

                    imageProcessingService.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during image processing_try: {ex}");
                    throw; 
                }

            }
            else
            {
                Console.WriteLine("Image stream is null or empty.");
            }
        }
    }
    else
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Bad Request");
    }
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();


