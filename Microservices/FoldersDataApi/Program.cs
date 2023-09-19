using FoldersDataApi.ProtoServices;
using FoldersDataApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddAuthentication(
//        CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(option => option.LoginPath = "/User/GoogleLogin")
//    .AddGoogle(googleOptions =>
//    {
//        googleOptions.ClientId = "your id";
//        googleOptions.ClientSecret = "your id secret";
//    });
//builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserdataService, UserDataService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddSingleton<IUserProtoService, UserProtoService>();
builder.Services.AddSingleton<IFolderProtoService, FolderProtoService>();
builder.Services.AddSingleton<IConnectionProtoService, ConnectionProtoService>();
builder.Services.AddCors();


var app = builder.Build();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Enter}");

//pp.UseCors(MyAllowSpecificOrigins);
app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());
//app.UseAuthentication();
//app.UseAuthorization();
app.UseHttpsRedirection();
app.Run();
