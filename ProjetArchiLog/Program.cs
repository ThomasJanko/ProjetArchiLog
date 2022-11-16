
using LibraryArchiLog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyModel;
using ProjetArchiLog.data;
using static LibraryArchiLog.Services.IUriService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();


//builder.Services.AddScoped<IUriService, UriService>();
var myKey = builder.Configuration["MyKey"];
builder.Services.AddSingleton<IUriService>(sp => new UriService(myKey));
//builder.Services.AddTransient<IUriService, UriService>();
//builder.Services.AddSingleton<IUriService, UriService>();

//Versioning
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver"));
});

builder.Services.AddDbContext<ArchiLogDbContext>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


