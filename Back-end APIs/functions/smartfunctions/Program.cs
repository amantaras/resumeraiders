using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
//using Microsoft.Extensions.Logging;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults();

hostBuilder.ConfigureAppConfiguration((context, config) =>
{
    config.AddUserSecrets<Program>();
});

hostBuilder.ConfigureServices(services =>
{
    services.AddSingleton<IKernel>(sp =>
    {
        IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
        //string openAiApiKey = configuration["OPENAI_APIKEY"];
        string openAiApiKey = "8317bab4fadd43049b15f39f5a61a0ec";
        IKernel kernel = new KernelBuilder()
            .WithLogger(sp.GetRequiredService<ILogger<IKernel>>())
            .Configure(config => config.AddOpenAIChatCompletionService(
                modelId: "gpt-35-turbo-16k",
                apiKey: openAiApiKey))
            .Build();

        return kernel;
    });

    services.AddSingleton<IChatCompletion>(sp =>
    sp.GetRequiredService<IKernel>().GetService<IChatCompletion>());

    const string instructions = "You are a professional HR recruiter.";
    services.AddSingleton<ChatHistory>(sp =>
        sp.GetRequiredService<IChatCompletion>().CreateNewChat(instructions));
});

IHost host = hostBuilder.Build();
host.Run();