using System.Text.Json.Serialization;
using ParkeoApp.Application;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Middlewares;
using ParkeoApp.Application.Services.AuthService;
using ParkeoApp.Application.Services.TenantService;
using ParkeoApp.Domain.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ParkeoApp.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostBuilderContext, loggerConfig) => loggerConfig.ReadFrom.Configuration(hostBuilderContext.Configuration));

// Add services to the container.
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddAuthentication(option =>
{
	option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearerConfiguration(builder);

builder.Services.AddSwaggerGen(option =>
{
	option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = JwtBearerDefaults.AuthenticationScheme,
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "JWT Authorization using the Bearer scheme. Insert a JWT"
	});
	option.AddSecurityRequirement
	(
		new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme } },
				Array.Empty<string>()
			}
		}
	);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseGlobalErrorHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
