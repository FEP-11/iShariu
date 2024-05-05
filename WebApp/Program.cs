using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp.Models;
using WebApp.Services;
using DotNetEnv;
using System;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Get MongoDB connection string from environment variables
string connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MongoDB service with connection string from environment variables
MongoClient client = new MongoClient(connectionString);
IMongoDatabase database = client.GetDatabase("iShariu"); // replace "YourDatabaseName" with the name of your database
builder.Services.AddSingleton(database);

builder.Services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/signin";
        options.LogoutPath = "/account/logout";
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireRole(UserRole.Admin));
    options.AddPolicy("Creator", p => p.RequireRole(UserRole.Creator));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
});

// Register EntityService with a factory method
builder.Services.AddScoped(typeof(IEntityService<User>), serviceProvider => 
    new EntityService<User>(database, "User")); 
builder.Services.AddScoped(typeof(IEntityService<Course>), serviceProvider => 
    new EntityService<Course>(database, "Course")); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();