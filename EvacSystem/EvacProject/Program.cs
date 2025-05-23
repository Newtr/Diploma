using EvacProject.GENERAL.Data;
using EvacProject.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:4242", "http://localhost:4343");

builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    var token = configuration["TelegramBot:Token"];
    if (string.IsNullOrEmpty(token))
    {
        logger.LogError("Telegram bot token is not configured.");
        throw new ArgumentNullException(nameof(token), "Telegram bot token is not configured.");
    }
    logger.LogInformation("Creating ITelegramBotClient with token");
    return new TelegramBotClient(token);
});

builder.Services.AddHostedService<TelegramBotService>();
builder.Services.AddSingleton<ITelegramBotService, TelegramBotService>(provider =>
{
    var service = provider.GetRequiredService<TelegramBotService>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("TelegramBotService: Singleton created");
    return service;
});
builder.Services.AddSingleton<TelegramBotService>();

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application started: Checking hosted services");
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (exception != null)
        {
            logger.LogError(exception.Error, "Unhandled exception occurred");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error");
        }
    });
});

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Incoming request: Path={context.Request.Path}, Port={context.Request.Host.Port}");
    await next();
});

app.UseWhen(context => context.Request.Host.Port == 4242, userApp =>
{
    userApp.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation($"User middleware: Path={context.Request.Path}, Port={context.Request.Host.Port}");
        if (context.Request.Path.StartsWithSegments("/Admin"))
        {
            logger.LogInformation("Blocking Admin access on port 4242");
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Admin routes are not available on port 4242");
            return;
        }
        await next();
    });

    userApp.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });
});

app.UseWhen(context => context.Request.Host.Port == 4343, adminApp =>
{
    adminApp.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation($"Admin middleware: Path={context.Request.Path}, Port={context.Request.Host.Port}");
        if (!context.Session.TryGetValue("AdminAuthenticated", out _) &&
            context.Request.Path != "/Admin/Login" &&
            context.Request.Path != "/Admin/Authenticate")
        {
            logger.LogInformation("Redirecting to /Admin/Login");
            context.Response.Redirect("/Admin/Login");
            return;
        }
        await next();
    });

    adminApp.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "admin",
            pattern: "{controller=Admin}/{action=Login}/{id?}");
    });
});

app.Run();