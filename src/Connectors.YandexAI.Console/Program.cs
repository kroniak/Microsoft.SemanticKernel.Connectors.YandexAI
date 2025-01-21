using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

// =========== CONFIG ===========
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var folderId = config.GetSection("YandexAI")["FolderId"] ?? throw new InvalidOperationException();
var apiKey = config.GetSection("YandexAI")["ApiKey"] ?? throw new InvalidOperationException();
const string deployment = "yandexgpt/latest";

// =========== APP ===========
using var factory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
var logger = factory.CreateLogger("YandexAI Console");

logger.LogInformation("=========== BUILD KERNEL ===========");

var kernel = Kernel.CreateBuilder()
    .AddYandexAIChatCompletion(deployment, apiKey, folderId)
    .Build();

await WithStopWatch(async () =>
{
    logger.LogInformation("1. Start prompt promties/basic.prompty");

    KernelArguments kernelArguments = new()
    {
        { "question", "What can you tell me about your tents?" }
    };

    var prompty = kernel.CreateFunctionFromPromptyFile("promties/basic.prompty");
    var result = await prompty.InvokeAsync<string>(kernel, kernelArguments);
    logger.LogInformation(result);
});

await WithStopWatch(async () =>
{
    logger.LogInformation("2. Start prompt promties/town.prompty");
    var prompty = kernel.CreateFunctionFromPromptyFile("promties/town.prompty");
    var result = await prompty.InvokeAsync<string>(kernel);
    logger.LogInformation(result);
});

await WithStopWatch(async () =>
{
    logger.LogInformation("3. Start prompt promties/space-tags-suggestion.prompty");
    var prompty = kernel.CreateFunctionFromPromptyFile("promties/space-tags-suggestion.prompty");
    var result = await prompty.InvokeAsync<string>(kernel);
    logger.LogInformation(result);
});

logger.LogInformation("END");
return;

async Task WithStopWatch(Func<Task> action)
{
    var stopWatch = new Stopwatch();
    stopWatch.Start();

    await action();

    stopWatch.Stop();
    var ts = stopWatch.Elapsed;
    var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

    logger.LogInformation("RunTime {ElapsedTime}", elapsedTime);
}