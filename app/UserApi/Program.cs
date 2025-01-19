
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Models;
using Repository;
using Services;
using System.Text;

namespace UserApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped< IUserService, UserService>();
        builder.Services.AddScoped<IRepository, PostgreRepository>();
        builder.Services.AddScoped<ISecuriteService, SecuriteService>();
        
        builder.Services
                .AddOptions<Settings>()
                .Bind(builder.Configuration)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var settings = builder.Configuration.Get<Settings>() ?? throw new ArgumentNullException("Not load Settings");


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = settings.AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = settings.AuthOptions.AUDIENCE,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.AuthOptions.Key)),
                ValidateIssuerSigningKey = true,
            };
        });

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}