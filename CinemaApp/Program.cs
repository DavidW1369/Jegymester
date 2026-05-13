using CinemaApp.Components;
using CinemaApp.Data;
using CinemaApp.Data.Models;
using CinemaApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=cinema.db"));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageTickets", p => p.RequireRole("Cashier", "Admin"));
    options.AddPolicy("CanManageContent", p => p.RequireRole("Admin"));
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<FilmService>();
builder.Services.AddScoped<ScreeningService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// All three auth actions need a real HTTP response to set/clear the cookie.
// DisableAntiforgery() is safe here — no state change without valid credentials.
// On failure, non-sensitive field values are passed back as query params so the
// form can repopulate them; the password is never echoed back.

app.MapPost("/auth/login", async (HttpContext ctx, SignInManager<AppUser> signInManager) =>
{
    var form     = await ctx.Request.ReadFormAsync();
    var email    = form["email"].ToString();
    var password = form["password"].ToString();
    var result   = await signInManager.PasswordSignInAsync(email, password, false, false);
    return result.Succeeded
        ? Results.Redirect("/")
        : Results.Redirect($"/login?email={Uri.EscapeDataString(email)}&error=1");
}).DisableAntiforgery();

app.MapPost("/auth/register", async (
    HttpContext ctx,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager) =>
{
    var form  = await ctx.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var phone = form["phone"].ToString();
    var pass  = form["password"].ToString();

    var user = new AppUser
    {
        UserName    = email,
        Email       = email,
        PhoneNumber = phone,
        Role        = Role.User
    };

    var result = await userManager.CreateAsync(user, pass);
    if (!result.Succeeded)
    {
        var first = result.Errors.First();
        var field = first.Code.Contains("Password") ? "password" : "email";
        var msg   = first.Code is "DuplicateUserName" or "DuplicateEmail"
                        ? "An account with this email already exists."
                        : first.Description;
        return Results.Redirect(
            $"/register?email={Uri.EscapeDataString(email)}" +
            $"&phone={Uri.EscapeDataString(phone)}" +
            $"&field={field}" +
            $"&msg={Uri.EscapeDataString(msg)}");
    }

    await userManager.AddToRoleAsync(user, "User");
    await signInManager.SignInAsync(user, isPersistent: false);
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapPost("/auth/logout", async (SignInManager<AppUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();

using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
