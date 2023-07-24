using MagicVillaWeb;
using MagicVillaWeb.Services;
using MagicVillaWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

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

/***** Register httpClient on the AuthService ****/
builder.Services.AddHttpClient<IAuthService, AuthService>();
/***** Register AuthService to Dependency Injection ****/
builder.Services.AddScoped<IAuthService, AuthService>();

/***** Register HttpContext Accessor we use in _Layout ****/
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

/*** We need to add this to pass the token in authentication
 * to save it for all subsequent requests - so the api will know
 * the user is authenticated - for this we need to save the token
 * into a session */
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
		.AddCookie(options =>
		{
			options.Cookie.HttpOnly = true;
			options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
			options.LoginPath = "/Auth/Login"; // // here we code the right path
            options.AccessDeniedPath = "/Auth/AccessDenied"; // here we code the right path
			options.SlidingExpiration = true;
		});
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(100);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});
/***************************************/

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
app.UseAuthentication();
app.UseAuthorization();
// We add session
app.UseSession();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
