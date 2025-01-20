using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore; // DODAC
using shop.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using shop.Authentication;
using shop.ProductCategory;

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
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProdCatService>();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await GenerateData.CreateRoles(context);
    // dodatkowo dodac jeszce uzytkownia admina jezeli nie ma !!!!!!!!!!
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
