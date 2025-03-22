using CrickerScorer.Data;
using CrickerScorer.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<CricketService>();

// Configuring Npgsql to enable dynamic JSON serialization
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();  // Enabling dynamic JSON serialization

// Add DbContext using Npgsql with dynamic JSON support
string _CMSDBConection = builder.Configuration.GetValue<string>("ConnectionStrings:CricketConnection");
builder.Services.AddDbContext<CricketDbContext>(opt => opt.UseNpgsql(_CMSDBConection));

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
    pattern: "{controller=cricket}/{action=Index}/{id?}");

app.Run();
