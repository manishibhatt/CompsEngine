using System.Text;
using CompsEngine.Data;                     // your Data namespace after scaffold
using CompsEngine.Data.Models;              // entities namespace from scaffold
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// EF Core MySQL
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CompsEngineDbContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs)));

// Add services to DI
builder.Services.AddControllers();

//Register JWT service
builder.Services.AddSingleton<JwtTokenService>();

//Add Swagger/OpenAPI
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT : Add Authentication/Authorization
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// app
var app = builder.Build();

//Configure middleware
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
