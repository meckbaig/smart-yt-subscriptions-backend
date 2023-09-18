using Microsoft.AspNetCore.Authentication.Cookies;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables();

//https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-6.0
//builder.Services.AddAuthentication(
//        CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(option => option.LoginPath = "/User/GoogleLogin")
//    .AddGoogle(googleOptions =>
//    {
//        googleOptions.ClientId = "ClientId";
//        googleOptions.ClientSecret = "ClientSecret";
//    });
//builder.Services.AddAuthorization();
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());
//app.UseAuthentication();
//app.UseAuthorization();
await app.UseOcelot();


app.Run();