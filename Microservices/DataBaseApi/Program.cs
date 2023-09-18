using DataBaseApi.Data;
using DataBaseApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
//builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<FolderService>();
//builder.Services.AddScoped<ConnectionService>();
builder.Services.AddGrpc().AddJsonTranscoding();


var app = builder.Build();

app.UseRouting();
app.UseGrpcWeb();
app.MapGrpcService<UserService>().EnableGrpcWeb();
app.MapGrpcService<FolderService>().EnableGrpcWeb();
app.MapGrpcService<ConnectionService>().EnableGrpcWeb();
app.MapGet("/", () => "gRPC app is running");

app.UseHttpsRedirection();
app.Run();