using FutureNHS.Api.Configuration;
using GPConnect.Provider.AcceptanceTests.Helpers;
using GPMigratorApp.Configuration;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Database.Providers;
using GPMigratorApp.Data.Database.Providers.Interfaces;
using GPMigratorApp.Data.Database.Providers.RetryPolicy;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.GPConnect;
using GPMigratorApp.Services;
using GPMigratorApp.Services.Interfaces;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration;
// Add services to the container.

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.Configure<AppSettings>(settings.GetSection("AppSettings"));
builder.Services.Configure<PlatformConfiguration>(settings.GetSection("Platform"));
builder.Services.AddScoped<IDbRetryPolicy, DbRetryPolicy>();

builder.Services.AddScoped<IJwtHelper>(
    sp =>
    {

        var config = sp.GetRequiredService<IOptionsSnapshot<AppSettings>>().Value;
        return new JwtHelper(config);
    });

builder.Services.AddScoped<IAzureSqlDbConnectionFactory>(
    sp => {
        var config = sp.GetRequiredService<IOptionsSnapshot<PlatformConfiguration>>().Value.Sql;

        if (config is null) throw new ApplicationException("Unable to load the sql configuration");
        if (string.IsNullOrWhiteSpace(config.ReadWriteConnectionString)) throw new ApplicationException("The read write connection string is missing from the files configuration section");
        if (string.IsNullOrWhiteSpace(config.ReadOnlyConnectionString)) throw new ApplicationException("The read only connection string is missing from the files configuration section");

        var logger = sp.GetRequiredService<ILogger<AzureSqlDbConnectionFactory>>();

        return new AzureSqlDbConnectionFactory(config.ReadWriteConnectionString, config.ReadOnlyConnectionString, sp.GetRequiredService<IDbRetryPolicy>(), logger);
    });

//builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddScoped<IStoreRecordService, StoreRecordService>();
builder.Services.AddScoped<IGPConnectService, GPConnectService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IPracticionerService, PracticionerService>();
builder.Services.AddScoped<IPatientService, PatientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
