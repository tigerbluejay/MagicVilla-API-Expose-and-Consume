using MagicVillaAPI;
using MagicVillaAPI.Customlog;
using MagicVillaAPI.Data;
using MagicVillaAPI.Repository;
using MagicVillaAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/******* Add Database *******/
builder.Services.AddDbContext<ApplicationDbContext>(option => { 
	option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

/******* Add Repository *******/
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();

/******* Add AutoMapper *******/
/** Add support for converting DTOS to Models **/
builder.Services.AddAutoMapper(typeof(MappingConfig));
/******* Add AutoMapper *******/


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
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
