using AestusDemoAPI.BackgroundServices;
using AestusDemoAPI.Extensions;
using AestusDemoAPI.Infrastructure;
using AestusDemoAPI.Services;
using AestusDemoAPI.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FinTechAestusContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<TransactionSettings>(builder.Configuration.GetSection("TransactionSettings"));
builder.Services.Configure<ValidationSettings>(builder.Configuration.GetSection("ValidationSettings"));

builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddSingleton<ITransactionQueueService, TransactionQueueService>();
builder.Services.AddSingleton<IAnomalyDetectionService, AnomalyTransactionService>();

builder.Services.AddHostedService<DailySuspiciousTransactionService>();
builder.Services.AddHostedService<TransactionBatchService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(c =>
    c.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseHttpsRedirection();

app.RegisterEndpoints();
app.Run();

