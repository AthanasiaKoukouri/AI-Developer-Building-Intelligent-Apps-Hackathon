// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using azure_semantic_kernel_sdk_hackathon.Plugins.LightsPlugin;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using OpenAI.Chat;
//using ChatCompletion = OpenAI.Chat.ChatCompletion;
using System.ClientModel;
using ChatMessage = OpenAI.Chat.ChatMessage;
using ChatCompletion = OpenAI.Chat.ChatCompletion;


// Create kernel
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion("gpt-35-turbo-16k", 
    "https://ai-hackathonhub638365411525.openai.azure.com", "6RSA0LnYbgIFNnIi3DaPdp5h75w0EZJOHv1iPVVYvJ0CvvxCcqMrJQQJ99AKACHYHv6XJ3w3AAAAACOG6rIS");



//Disable the experimental warning
#pragma warning disable SKEXP0001

builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

Kernel kernel = builder.Build();

// Retrieve the chat completion service
var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();

// Add the plugin to the kernel
kernel.Plugins.AddFromType<LightsPlugin>("Lights");


OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};


// Create chat history
var history = new ChatHistory();
history.AddSystemMessage("You are an AI assistant managing the lights product informatiion.");



//########### EXTRA
string endpoint = "https://ai-hackathonhub638365411525.openai.azure.com/";
string deploymentName = "gpt-35-turbo-16k";
string openAiApiKey = "6RSA0LnYbgIFNnIi3DaPdp5h75w0EZJOHv1iPVVYvJ0CvvxCcqMrJQQJ99AKACHYHv6XJ3w3AAAAACOG6rIS";

string searchEndpoint = "https://hackathonservicetest.search.windows.net/";
string searchIndex = "yellow-snail-19g6h2rmkw";
string searchApiKey = "J1KrSszqChK9wei0XdRUASmTAsWd37kl7E7IagnkQJAzSeCcxv6J";


AzureOpenAIClient azureClient = new(
    new Uri(endpoint),
    new ApiKeyCredential(openAiApiKey));
ChatClient chatClient = azureClient.GetChatClient(deploymentName);


#pragma warning disable AOAI001

//Add chat completion options with data source 
ChatCompletionOptions options = new ChatCompletionOptions();
options.AddDataSource(new AzureSearchChatDataSource()
{
    Endpoint = new Uri(searchEndpoint),
    IndexName = searchIndex,
    Authentication = DataSourceAuthentication.FromApiKey(searchApiKey),
});

//Add system message and user question
//List<ChatMessage> messages = new List<ChatMessage>();
//messages.Add(ChatMessage.CreateSystemMessage("You are an AI assistant that helps people find product information."));


while (true)
{
    Console.Write("User: ");
    string userInput = Console.ReadLine();

    history.AddUserMessage(userInput);


    var result = await chatCompletionService.GetChatMessageContentAsync(
history,
executionSettings: openAIPromptExecutionSettings,
kernel: kernel
);


    Console.WriteLine("Bot: " + result);

    //ChatCompletion completion = chatClient.CompleteChat(messages, options);

    //foreach (var response in completion.Content)
    //{
    //    string botResponse = response.Text;
    //    Console.WriteLine("Bot: " + botResponse);

    //    history.Add(ChatMessage.CreateAssistantMessage(botResponse));

    //    history.AddUserMessage(botResponse);

    //}
}


//########### END EXTRAS





// Get the response from the AI
//var result = await chatCompletionService.GetChatMessageContentAsync(
//history,
//executionSettings: openAIPromptExecutionSettings,
//kernel: kernel
//);



//// Print the results
//Console.WriteLine("Assistant > " + result);

//// Add the message from the agent to the chat history
//history.AddAssistantMessage(result.ToString());