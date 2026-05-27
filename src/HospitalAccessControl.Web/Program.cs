using HospitalAccessControl.Infrastructure.DependencyInjection;
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.Configure<DevelopmentUserOptions>(
    builder.Configuration.GetSection("DevelopmentUser"));

var securityMode = builder.Configuration["SecurityMode"];

if (string.Equals(securityMode, "Development", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported SecurityMode: '{securityMode}'.");
}
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
