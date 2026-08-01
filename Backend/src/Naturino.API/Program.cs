using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Naturino.API.Extensions;
using Naturino.API.Middleware;
using Naturino.Application;
using Naturino.Infrastructure;
using Naturino.Infrastructure.Logging;
using Naturino.Infrastructure.Persistence;
using Naturino.Infrastructure.Persistence.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ConfigureSerilog(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddSwaggerDocs();

var app = builder.Build();

// Nginx terminates TLS and proxies to this container over plain HTTP on the internal
// Docker network, so without this the app sees every request as HTTP and
// UseHttpsRedirection() below would bounce it back out as an unnecessary redirect.
// Backend is never published directly to the internet, so trusting any proxy here
// (clearing the known-networks allowlist) only means trusting the nginx container.
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

// Serilog's request logger must wrap OUTSIDE the exception handler so it logs the
// status code the client actually receives (e.g. 404 for NotFoundException) instead
// of the raw exception it observes bubbling through — otherwise every handled
// NotFoundException/ValidationException etc. gets misreported as "responded 500".
app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Naturino API v1");
    });
}

// Applying pending migrations on startup must run in every environment, not just
// Development — otherwise production keeps the schema from whenever it was last
// restored from a dump, silently drifting from what the deployed code expects.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db, app.Logger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("DefaultCorsPolicy");

var uploadsRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads");
Directory.CreateDirectory(uploadsRoot);
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsRoot),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
