var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddHttpClient<ClassLibrary.Proxies.CalendarIntegrationProxy>();
builder.Services.AddHttpClient<ClassLibrary.Proxies.CalendarWorkoutCatalogProxy>();
builder.Services.AddHttpClient<ClassLibrary.Proxies.CalendarExportProxy>();

builder.Services.AddSingleton<ClassLibrary.Proxies.Interfaces.IUserSession, ClassLibrary.Proxies.UserSession>();
builder.Services.AddTransient<ClassLibrary.Proxies.Interfaces.ICalendarIntegrationProxy, ClassLibrary.Proxies.CalendarIntegrationProxy>();
builder.Services.AddTransient<ClassLibrary.Proxies.Interfaces.ICalendarWorkoutCatalogProxy, ClassLibrary.Proxies.CalendarWorkoutCatalogProxy>();
builder.Services.AddTransient<ClassLibrary.Proxies.Interfaces.ICalendarExportProxy, ClassLibrary.Proxies.CalendarExportProxy>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
