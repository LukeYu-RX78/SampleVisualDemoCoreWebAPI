using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core; // Add this for Kestrel configuration
//using System.IO;
using Microsoft.EntityFrameworkCore;
using SampleVisualDemoCoreWebAPI.Models.Entities;
using SampleVisualDemoCoreWebAPI.Interfaces;
using SampleVisualDemoCoreWebAPI.Services;
using SampleVisualDemoCoreWebAPI.Events;
using SampleVisualDemoCoreWebAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5247); // HTTP port
    /*
    options.ListenAnyIP(7075, listenOptions =>
    {
        // Assuming cert.pem and key.pem are in the "Certs" directory
        listenOptions.UseHttps("Certs/cert.pem", "Certs/key.pem");
    });
    */
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register EF Core DbContext
builder.Services.AddDbContext<DorsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SampleVisualDemoDBConn")));

// Register business service layer
builder.Services.AddScoped<IDDRRejectLogService, DDRRejectLogService>();

// Register event-driven components
builder.Services.AddScoped<IEventBus, InMemoryEventBus>();
builder.Services.AddScoped<IEmailService, MailKitEmailService>();
builder.Services.AddScoped<IEventHandler<DDRRejectLogCreatedEvent>, DDRRejectLogEmailHandler>();


// Json Serializer
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson(
    options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

var app = builder.Build();

//Enable CORS
app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
