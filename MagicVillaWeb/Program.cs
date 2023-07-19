using MagicVillaWeb;
using MagicVillaWeb.Services;
using MagicVillaWeb.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/***** Register Automapper ****/
builder.Services.AddAutoMapper(typeof(MappingConfig));

/***** Register httpClient on the VillaService ****/
builder.Services.AddHttpClient<IVillaService, VillaService>();
/***** Register VillaService to Dependency Injection ****/
builder.Services.AddScoped<IVillaService, VillaService>();

/***** Register httpClient on the VillaNumberService ****/
builder.Services.AddHttpClient<IVillaNumberService, VillaNumberService>();
/***** Register VillaNumberService to Dependency Injection ****/
builder.Services.AddScoped<IVillaNumberService, VillaNumberService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
