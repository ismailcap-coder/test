using BuergerPortal.Business.Services;
using BuergerPortal.Business.Validators;
using BuergerPortal.Data;
using BuergerPortal.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
builder.Services.AddDbContext<BuergerPortalContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BuergerPortalContext")));

// --- Repositories ---
builder.Services.AddScoped<CitizenRepository>();
builder.Services.AddScoped<ServiceApplicationRepository>();
builder.Services.AddScoped<ServiceTypeRepository>();
builder.Services.AddScoped<PublicOfficeRepository>();

// --- Validators ---
builder.Services.AddTransient<CitizenValidator>();
builder.Services.AddTransient<ApplicationValidator>();

// --- Services ---
builder.Services.AddScoped<IFeeCalculationService, FeeCalculationService>();
builder.Services.AddScoped<ICitizenService, CitizenService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

// --- MVC ---
builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- Middleware Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- Database Initialization ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BuergerPortalContext>();
    context.Database.EnsureCreated();
    await BuergerPortalSeeder.SeedAsync(context);
}

app.Run();
