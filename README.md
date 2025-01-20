# Microsoft.SemanticKernel.Connectors.YandexAI

The `Microsoft.SemanticKernel.Connectors.YandexAI` project is a robust connector between Microsoft's SemanticKernel and
Yandex's AI service. This project offers an easy integration with Yandex's AI capabilities, enabling developers with
virtually endless possibilities in the field of artificial intelligence.

## Key Features

1. **Easy Installation:** It can be easily added to any .NET project through the NuGet package manager.

2. **Simplicity of Usage:** After defining the necessary configurations, one can start using Yandex AI's chat
   completion.

3. **Flexible Integration:** The project allows functions from prompty files to be created and invoked, enabling a wide
   range of AI tasks to be performed with ease.

4. **Versatility:** The project can be used in varied scenarios - from generating basic prompts to suggesting space
   tags, all with just a few lines of code.

This connector is designed to enhance the AI capabilities of your applications by leveraging the power of Yandex AI,
catapulting your applications into the realm of intelligent, next-generation applications.

## Microsoft's SemanticKernel

Microsoft's SemanticKernel is a powerful, flexible, and scalable platform built to handle a wide range of AI tasks. Here
are some of its key features:

- **Scale:**
  SemanticKernel is build with scale in mind. Whether your application needs to handle hundreds or millions of AI tasks,
  SemanticKernel can adapt to meet your needs.

- **Interoperability:**
  SemanticKernel is designed to work smoothly with a variety of AI services. This includes the Yandex AI service thanks
  to the `Microsoft.SemanticKernel.Connectors.YandexAI` project.

- **Performance:**
  Performance is paramount in the world of artificial intelligence. SemanticKernel is built with features to ensure
  quick response times and efficient processing.

- **Customization:**
  SemanticKernel understands that not all AI tasks are the same. That is why it provides you with the tools you need to
  customize your AI services to best fit your application's needs.

- **Ease of Use:**
  You do not need to be an expert in AI to use SemanticKernel. With its user-friendly design, you can easily integrate
  and manage AI services in your applications.

Please refer to the [official Microsoft SemanticKernel documentation](https://docs.microsoft.com/semantic-kernel) for
more detailed information and usage guidelines.

## Microsoft.SemanticKernel.Prompty

Prompty is a powerful library in Microsoft's SemanticKernel platform with a number of advantages:

- **Simplicity:** Prompty offers straightforward syntax and usage methods which makes handling AI tasks easier, even for
  novice developers.

- **Integration:** Prompty supports seamless integration with variety of AI services, including the Yandex AI service,
  adding to the flexibility and capability of your applications.

- **Efficiency:** Prompty is built to optimize AI task performance, ensuring that your applications run effectively and
  efficiently.

- **Flexibility:** With Prompty, you can customize the ways in which you interact with AI services to best suit your
  application needs.

For more detailed information and usage guidelines related to Prompty, refer to
the [official Microsoft Prompty documentation](https://prompty.ai/).

## Installation

```bash
dotnet add package Microsoft.SemanticKernel.Connectors.YandexAI
```

## Configuration to Yandex AI

Let's start configuration with Yandex GPT 4 [official docs](https://yandex.cloud/ru/docs/foundation-models/quickstart/yandexgpt)

## Usage

```csharp
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
    { "question", "What can you tell me about your tents?" },
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
```
