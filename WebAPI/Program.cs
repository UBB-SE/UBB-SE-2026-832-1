using ClassLibrary.Extensions;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);
var apiBaseUrl = builder.Configuration["Api:BaseUrl"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddClassLibraryDataAccess();
builder.Services.AddWebApiServices();

var app = builder.Build();
app.Services.SeedClassLibraryData();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

