using EduFlix.Web.Components;
using EduFlix.Web.Components.Account;
using EduFlix.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// grotere berichten toelaten op het circuit, nodig om videobestanden te kunnen uploaden
builder.Services.Configure<HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 512 * 1024 * 1024;
});

builder.Services.AddMudServices();

// Blob storage voor de videobestanden (Azurite in dev).
builder.AddAzureBlobServiceClient("blobs");
builder.Services.AddInfrastructureServices();

// Identity: cascading auth state + de account-services.
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Database-context via Aspire (Npgsql). "eduflixdb" komt overeen met de resource in de AppHost.
builder.AddNpgsqlDbContext<ApplicationDbContext>("eduflixdb");

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        // geen e-mailbevestiging nodig voor dit project
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoints voor de Identity /Account Razor-componenten.
app.MapAdditionalIdentityEndpoints();

app.MapDefaultEndpoints();

// Migreren + seeden gebeurt in EduFlix.MigrationService, web wacht daarop (zie AppHost).

app.Run();
