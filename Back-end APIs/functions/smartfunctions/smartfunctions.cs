using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Memory;
using Azure;
using Azure.AI.OpenAI;

namespace SmartRaiders.smartFunctions
{
    public class SmartFunctions
    {
        private readonly ILogger _logger;
        private readonly IKernel _kernel;
        private readonly IChatCompletion _chat;
        private readonly ChatHistory _chatHistory;

        public SmartFunctions(ILoggerFactory loggerFactory, IKernel kernel, ChatHistory chatHistory, IChatCompletion chat)
        {
            _logger = loggerFactory.CreateLogger<SmartFunctions>();
            _kernel = kernel;
            _chat = chat;
            _chatHistory = chatHistory;
        }

        [Function("GenerateTags")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext context)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //_chatHistory!.AddMessage(ChatHistory.AuthorRoles.User, await req.ReadAsStringAsync() ?? string.Empty);
            //string message = await SearchMemoriesAsync(_kernel, await req.ReadAsStringAsync() ?? string.Empty);
            //_chatHistory!.AddMessage(ChatHistory.AuthorRoles.User, message);

            //string reply = await _chat.GenerateMessageAsync(_chatHistory, new ChatRequestSettings());

            //_chatHistory.AddMessage(ChatHistory.AuthorRoles.Assistant, reply);

            OpenAIClient client = new OpenAIClient(
            new Uri("https://oai-uksouth-rr.openai.azure.com/"),
            new AzureKeyCredential(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")));
            string user_message = await req.ReadAsStringAsync() ?? string.Empty;
            string propmttemplate = @"Can you please give me 20 tags based on the follow job position? 
            5 of these tags should mention popular tools used to excel in that position
            I want you to categorize the tags based on classification. For example this is a very good response: 
            Soft Skills:
            1. Creativity
            2. Attention to detail
            3. Problem-solving
            4. Time management
            5. Communication skills

            Technical Skills:
            6. Digital drawing
            7. Illustration
            8. Color theory
            9. Typography
            10. Digital painting

            Tools:
            11. Adobe Illustrator
            12. Photoshop
            13. Procreate
            14. Sketchbook Pro
            15. Corel Painter

            Knowledge:
            16. Composition
            17. Perspective drawing
            18. Anatomy
            19. Storytelling
            20. Digital art trends 
            Ignore any previous conversation we had.
            You do not need to come up with 20 tags mandatory. YOu can return less if you find the profession difficult to draft tags for.
            If you have trouble suggesting less than 3 tags for a specific profession, just return the text: I could not draft tags for this profession. Are you sure that the profession exists? 
            This is the profession" + user_message;
       
            // ### If streaming is not selected
            Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
                "gpt-35-turbo-16k",
                new ChatCompletionsOptions()
                {
                    
                    Messages =
                    {
                        new ChatMessage(ChatRole.System, @"You are an HR assistant that helps HR recruiters and talent finders to find the right information relevant to their business"),
                        new ChatMessage(ChatRole.User, propmttemplate)
                        //new ChatMessage(ChatRole.Assistant, @"Hello! How can I assist you today?"),
                    },
                    Temperature = (float)0.7,
                    MaxTokens = 800,
                    NucleusSamplingFactor = (float)0.95,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                    ChoicesPerPrompt =1
                });

            ChatCompletions completions = responseWithoutStream.Value;

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
           
            response.WriteString(completions.Choices[0].Message.Content.ToString());
            //response['choices'][0]['message']['content'])
            return response;
        }

        private async Task<string> SearchMemoriesAsync(IKernel kernel, string query)
        {
            StringBuilder result = new StringBuilder();
            result.Append("The below is relevant information.\n[START INFO]");

            const string memoryCollectionName = "ms10k";

            IAsyncEnumerable<MemoryQueryResult> queryResults =
                kernel.Memory.SearchAsync(memoryCollectionName, query, limit: 3, minRelevanceScore: 0.77);

            // For each memory found, get previous and next memories.
            await foreach (MemoryQueryResult r in queryResults)
            {
                int id = int.Parse(r.Metadata.Id);
                MemoryQueryResult? rb2 = await kernel.Memory.GetAsync(memoryCollectionName, (id - 2).ToString());
                MemoryQueryResult? rb = await kernel.Memory.GetAsync(memoryCollectionName, (id - 1).ToString());
                MemoryQueryResult? ra = await kernel.Memory.GetAsync(memoryCollectionName, (id + 1).ToString());
                MemoryQueryResult? ra2 = await kernel.Memory.GetAsync(memoryCollectionName, (id + 2).ToString());

                if (rb2 != null) result.Append("\n " + rb2.Metadata.Id + ": " + rb2.Metadata.Description + "\n");
                if (rb != null) result.Append("\n " + rb.Metadata.Description + "\n");
                if (r != null) result.Append("\n " + r.Metadata.Description + "\n");
                if (ra != null) result.Append("\n " + ra.Metadata.Description + "\n");
                if (ra2 != null) result.Append("\n " + ra2.Metadata.Id + ": " + ra2.Metadata.Description + "\n");
            }

            result.Append("\n[END INFO]");
            result.Append($"\n{query}");

            Console.WriteLine(result);
            return result.ToString();
        }
    }
}