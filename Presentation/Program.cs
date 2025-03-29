using Business.Interfaces;
using Business.Services;
using Data.Context;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var connectionString = Environment.GetEnvironmentVariable("mssqlConnectionString");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<MemberEntity, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/SignIn";
    options.AccessDeniedPath = ""; // add here 
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
});
builder.Services.AddAuthentication()
    .AddGoogleOpenIdConnect(options =>
    {
        options.ClientId = builder.Configuration["GoogleAuth:ClientId"];
        options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
        options.CallbackPath = "/signin-google";
    });

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberProfileRepository, MemberProfileRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMemberProfileService, MemberProfileService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Applying database migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
