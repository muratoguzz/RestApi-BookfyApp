using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation.ActionFilters;
using Repositories.EFCore;
using Services.Contracts;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.CacheProfiles.Add("5mins", new CacheProfile() { Duration = 300 });
})
.AddNewtonsoftJson(opt =>
    opt.SerializerSettings.ReferenceLoopHandling =
    Newtonsoft.Json.ReferenceLoopHandling.Ignore)
.AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
.AddXmlDataContractSerializerFormatters()
.AddCustomCsvFormatter();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program)); //Bu yöntem, AutoMapper'ýn Program sýnýfýnýn bulunduðu assembly'deki
builder.Services.ConfigureActionFilters();      //tüm profilleri otomatik olarak bulup kaydetmesini saðlar.
builder.Services.ConfigureCors();
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();


var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "bsStoreApp v1");
        s.SwaggerEndpoint("/swagger/v2/swagger.json", "bsStoreApp v2");
    });
}

if (app.Environment.IsProduction()) 
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting();
app.UseCors("CorsPolicy");//Microsoft cors tan sonra cachingin çaðýrýlmasýný öneriyor, headerin clientler tarafýndan okunabilmesi için verilen izin
app.UseResponseCaching();
app.UseHttpCacheHeaders();


app.UseAuthentication();//doðrulama
app.UseAuthorization();//yetkilendirme

app.MapControllers();

app.Run();
