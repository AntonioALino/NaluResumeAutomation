using NaluResumeAutomation.Application.Abstractions;
using NaluResumeAutomation.Application.useCases;
using NaluResumeAutomation.Infra.ExternalServices.APIs;
using NaluResumeAutomation.Infra.ExternalServices.Telegram;
using Telegram.Bot;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var botToken = builder.Configuration["Telegram:BotToken"];
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken!)); ;

builder.Services.AddHttpClient<IPdfProcessor, PythonPdfProcessor>(client =>
{
    var pythonUrl = builder.Configuration["PythonWorker:BaseUrl"];
    client.BaseAddress = new Uri(pythonUrl!);
});

builder.Services.AddTransient<ITelegramNotifier, TelegramNotifier>();
builder.Services.AddTransient<ProcessDocumentUseCase>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
