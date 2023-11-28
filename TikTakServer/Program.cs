using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TikTakServer.ApplicationServices;
using TikTakServer.Database;
using TikTakServer.Facades;
using TikTakServer.Handlers;
using TikTakServer.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TikTakServer.Models.Business;
using TikTakServer.Middleware;
using TikTakServer.Managers;
using TikTakServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var config = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TikTakContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(client => new BlobServiceClient(config.GetConnectionString("AzureStorage")));
builder.Services.AddScoped<IJwtHandler, JwtHandler>();
builder.Services.AddScoped<IHlsHandler, HlsHandler>();
builder.Services.AddScoped<IBlobStorageRepository, BlobStorageRepository>();
builder.Services.AddScoped<IBlobStorageFacade, BlobStorageFacade>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IVideoFacade, VideoFacade>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserFacade, UserFacade>();
builder.Services.AddScoped<IRecommendationFacade, RecommendationFacade>();
builder.Services.AddScoped<IRecommendationManager, RecommendationManager>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<UserRequestAndClaims>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"])),

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(e => e.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.Run();
