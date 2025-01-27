using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore; // DODAC
using shop.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using shop.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
 {
    //  options.LoginPath = "/Account/Login";
    //  options.LogoutPath = "/Home/Index";
     options.SlidingExpiration = true;
     options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
 });

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ShoppingCartService>();
builder.Services.AddScoped<ProdCatService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    var prodCatService = scope.ServiceProvider.GetRequiredService<ProdCatService>();
    await GenerateData.CreateRoles(context);
    await GenerateData.CreateAdmin(context, authService);
    await GenerateData.CreateCategories(context, prodCatService);
    await GenerateData.CreateProducts(context, prodCatService);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// bezspsrednio serwuje statyczne pliki bez potrzeby obslugi kotrolera
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
