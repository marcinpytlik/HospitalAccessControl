using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Infrastructure.DependencyInjection;
using HospitalAccessControl.Web.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization();

builder.Services.Configure<DevelopmentUserOptions>(
    builder.Configuration.GetSection("DevelopmentUser"));

var securityMode = builder.Configuration["SecurityMode"];

if (string.Equals(securityMode, "Development", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();
}
else if (string.Equals(securityMode, "Windows", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ICurrentUserService, WindowsCurrentUserService>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported SecurityMode: '{securityMode}'.");
}

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

public partial class Program;
