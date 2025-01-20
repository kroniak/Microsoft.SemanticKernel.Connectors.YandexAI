using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0040

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var folderId = config.GetSection("YandexAI")["FolderId"] ?? throw new InvalidOperationException();
var apiKey = config.GetSection("YandexAI")["ApiKey"] ?? throw new InvalidOperationException();

const string deployment = "yandexgpt/latest";

Console.WriteLine("Start app");

var kernel = Kernel.CreateBuilder()
    .AddYandexAIChatCompletion(deployment, apiKey, folderId)
    .Build();

// update the input below to match your prompty
KernelArguments kernelArguments = new()
{
    { "question", "What can you tell me about your tents?" }
};

Console.WriteLine("1. Start prompt promties/basic.prompty");

var prompty = kernel.CreateFunctionFromPromptyFile("promties/basic.prompty");
var result = await prompty.InvokeAsync<string>(kernel, kernelArguments);

Console.WriteLine(result);

Console.WriteLine("2. Start prompt promties/town.prompty");

prompty = kernel.CreateFunctionFromPromptyFile("promties/town.prompty");
result = await prompty.InvokeAsync<string>(kernel);

Console.WriteLine(result);

Console.WriteLine("3. Start prompt promties/space-tags-suggestion.prompty");

prompty = kernel.CreateFunctionFromPromptyFile("promties/space-tags-suggestion.prompty");
result = await prompty.InvokeAsync<string>(kernel);

Console.WriteLine(result);

Console.WriteLine("END");