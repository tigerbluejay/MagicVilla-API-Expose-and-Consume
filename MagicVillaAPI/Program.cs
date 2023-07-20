using MagicVillaAPI;
using MagicVillaAPI.Customlog;
using MagicVillaAPI.Data;
using MagicVillaAPI.Repository;
using MagicVillaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/******* Add Database *******/
builder.Services.AddDbContext<ApplicationDbContext>(option => { 
	option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

/******* Add Repository *******/
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

/******* Add AutoMapper *******/
/** Add support for converting DTOS to Models **/
builder.Services.AddAutoMapper(typeof(MappingConfig));
/******* Add AutoMapper *******/


/******* Add Authentication *******/
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x =>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(x =>
	{
		x.RequireHttpsMetadata = false;
		x.SaveToken = true;
		x.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
			ValidateIssuer = false,
			ValidateAudience = false
		};
});
/******* Add Authentication *******/


/******* Serilog Logging *******/
// Configure Serilog
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
	.WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Infinite)
	.CreateLogger();
// Tell .NET to use Serilog - this comes with the Serilog.ASPNETCore package
builder.Host.UseSerilog();
/******* Serilog Logging *******/


// We add the static method AddNewtonssoftJson to support PATCH operations
//builder.Services.AddControllers().AddNewtonsoftJson();
// Here we specify that the application should never return anything other than JSON
//builder.Services.AddControllers( option => { option.ReturnHttpNotAcceptable= true;}).
//	AddNewtonsoftJson();
// Here we add support to return data in XML
//builder.Services.AddControllers(option =>
//{
//	option.ReturnHttpNotAcceptable = true;
//}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
builder.Services.AddControllers().AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


/******* Configure Swagger to allow to accept a bearer token to authenticate users *******/
// this is all unintelligible but i can ask ChatGPT what all of this means or read in the 
// swagger documentation
builder.Services.AddSwaggerGen(options => {
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description =
			"JWT Authorization header using the Bearer scheme. \r\n\r\n " +
			"Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n " +
			"Example: \"Bearer 12345abcdef\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Scheme = "Bearer"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "oauth2",
				Name = "Bearer",
				In = ParameterLocation.Header
			},
			new List<string>()
		}
	});
});
/******* Configure Swagger to allow to accept a bearer token to authenticate users *******/


/******* Custom Logging *******/
builder.Services.AddSingleton<ILogging, Logging>(); // object will be created when application starts, will be used every time an application requests an implementation
// scoped - for every request it will create a new object
// transient - every time object is accessed a new object is created
//builder.Services.AddSingleton<ILogging, Loggingv2>();
/******* Custom Logging *******/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // we add to access the endpoints for authenticated users

app.UseAuthorization();

app.MapControllers();

app.Run();
