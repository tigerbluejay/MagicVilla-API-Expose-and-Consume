var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// We add the static method AddNewtonssoftJson to support PATCH operations
//builder.Services.AddControllers().AddNewtonsoftJson();
// Here we specify that the application should never return anything other than JSON
//builder.Services.AddControllers( option => { option.ReturnHttpNotAcceptable= true;}).
//	AddNewtonsoftJson();
// Here we add support to return data in XML
builder.Services.AddControllers(option =>
{
	option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
