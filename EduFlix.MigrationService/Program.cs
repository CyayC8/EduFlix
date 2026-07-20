using EduFlix.Infrastructure;
using EduFlix.MigrationService;
using Microsoft.AspNetCore.Identity;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Zelfde db-context als de rest van de app, via Aspire (Npgsql).
builder.AddNpgsqlDbContext<ApplicationDbContext>("eduflixdb");

// Enkel wat nodig is om rollen/users te seeden (geen cookies/sign-in hier, dit is geen webapp).
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();
