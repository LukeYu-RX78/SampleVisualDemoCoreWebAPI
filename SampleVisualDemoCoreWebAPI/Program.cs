using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using SampleVisualDemoCoreWebAPI.Models.Entities;
using SampleVisualDemoCoreWebAPI.Interfaces;
using SampleVisualDemoCoreWebAPI.Services;
using SampleVisualDemoCoreWebAPI.Events;
using SampleVisualDemoCoreWebAPI.Infrastructure;
using SampleVisualDemoCoreWebAPI.EventHandlers;

var builder = WebApplication.CreateBuilder(args);

// Optional: custom HTTP port
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5247);
    // options.ListenAnyIP(7075, listenOptions => listenOptions.UseHttps("Certs/cert.pem", "Certs/key.pem"));
});

// DI setup
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DorsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SampleVisualDemoDBConn")));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IDDRRejectLogService, DDRRejectLogService>();
builder.Services.AddScoped<IEmailService, MailKitEmailService>();
builder.Services.AddScoped<IEventBus, InMemoryEventBus>();
builder.Services.AddScoped<IEventHandler<DDRRejectLogCreatedEvent>, DDRRejectLogEmailHandler>();

// Build app
var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
