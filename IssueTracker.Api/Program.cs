using System.Text.Json.Serialization;
using IssueTracker.Api;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "IssueTracker API",
        Version = "v1",
        Description = "API REST pentru gestionarea proiectelor, issue-urilor si comentariilor."
    });
});

builder.Services.AddSingleton<INotificationService, ConsoleNotificationService>();
builder.Services.AddScoped<IssueTrackerService>();
builder.Services.AddScoped<IssueReportService>();

builder.Services.AddDbContext<IssueTrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IssueTrackerDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
