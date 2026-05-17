using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IUserSession, UserSession>();
builder.Services.AddHttpClient<ICreateWorkoutProxy, CreateWorkoutProxy>();
builder.Services.AddHttpClient<IActiveWorkoutProxy, ActiveWorkoutProxy>();
builder.Services.AddHttpClient<IWorkoutLogProxy, WorkoutLogProxy>();
builder.Services.AddHttpClient<IMealPlanProxy, MealPlanProxy>();
builder.Services.AddHttpClient<IDailyLogProxy, DailyLogProxy>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

UserSession.SetCurrentSession(2, UserSession.CLIENT_ROLE);

app.Run();
