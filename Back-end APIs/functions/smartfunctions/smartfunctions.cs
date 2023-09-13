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
using System.Text.Json;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartRaiders.smartFunctions
{
    public class JobsInformation
    {
        public string  JobTitle { get; set; }
        public string  ContractType { get; set; }
        public string  ApplicationProcess { get; set; }
        public string  ReportsTo { get; set; }
        public string  Location { get; set; }
        public string  Benefits { get; set; }
        public string  CompanyAbout { get; set; }   
        public string Tags { get; set; }


    }
    public class SmartFunctions
    {
        
        
        /*
 * TEXT MODERATION
 * This example moderates text from file.
 */
        public static string ModerateText(string inputText)
        {
            string SubscriptionKey="b068ab9aeafb48f3bcd97b20259d7d0f";
            string Endpoint = "https://resumeraiderscontentmoderator.cognitiveservices.azure.com/";

            ContentModeratorClient clientText = Authenticate(SubscriptionKey, Endpoint);
            // Load the input text.
            
            string text = inputText;

            // Remove carriage returns
            text = text.Replace(Environment.NewLine, " ");
            // Convert string to a byte[], then into a stream (for parameter in ScreenText()).
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream(textBytes);

            //Console.WriteLine("Screening {0}...", inputFile);
            // Format text

            // Save the moderation results to a file.
            string scanResult = string.Empty;
           
            // Screen the input text: check for profanity, classify the text into three categories,
            // do autocorrect text, and check for personally identifying information (PII)
            //outputWriter.WriteLine("Autocorrect typos, check for matching terms, PII, and classify.");

            // Moderate the text
            var vcreenResult = clientText.TextModeration.ScreenText("text/plain", stream, "eng", true, true, null, true);
            scanResult = JsonConvert.SerializeObject(vcreenResult, Formatting.Indented);

            return scanResult;
        }
        
        public static ContentModeratorClient Authenticate(string key, string endpoint)
        {
            ContentModeratorClient client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(key));
            client.Endpoint = endpoint;

            return client;
        } 

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

        [Function("ModerateContent")]
        public async Task<HttpResponseData> ModerateContent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ModerateContent")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            //returns false if moderation is not required

            // TODO: Implement content moderation logic here
            // For example, you can use Azure Content Moderator API to moderate the content

            // Simulate content moderation by delaying for 1 second
            string textToEvaluate = await req.ReadAsStringAsync() ?? string.Empty;
            
            string Result = ModerateText(textToEvaluate);
            JObject moderateResult = JObject.Parse(Result);
            bool isReviewRecommended = (bool)moderateResult["Classification"]["ReviewRecommended"];
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await response.WriteStringAsync(isReviewRecommended.ToString());

            return response;
        }

        [Function("GenerateTags")]
         public async Task<HttpResponseData> GenerateTags(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GenerateTags")] HttpRequestData req)
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
            Soft Skills: Creativity Attention to detail Problem-solving Time management  Communication skills Technical Skills: Digital drawingIllustration Color theory Typography Digital painting Tools: Adobe Illustrator Photoshop Procreate Sketchbook ProCorel Painter Knowledge: Composition Perspective drawing
           
            Ignore any previous conversation we had.
            You do not need to come up with 20 tags mandatory. YOu can return less if you find the profession difficult to draft tags for.
            If you have trouble suggesting less than 3 tags for a specific profession, just return the text: I could not draft tags for this profession. Are you sure that the profession exists? 
            The output should not include any special characters, numbers, carriage returns , the tags should be one after another one separating them only by one space.
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
                    Temperature = (float)1.7,
                    MaxTokens = 800,
                    NucleusSamplingFactor = (float)0.95,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                    ChoicesPerPrompt =1
                });

            ChatCompletions completions = responseWithoutStream.Value;

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            string res = completions.Choices[0].Message.Content.ToString();
            res = res.Replace(Environment.NewLine, " ");            
            response.WriteString(res);
            //response['choices'][0]['message']['content'])
            return response;
        }
        
        [Function("GenerateJobDescription")]
        public async Task<HttpResponseData> GenerateJobDescription(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GenerateJobDescription")] HttpRequestData req)
        {
            
            string requestBody = await req.ReadAsStringAsync();
            JobsInformation jobsInformation =  System.Text.Json.JsonSerializer.Deserialize<JobsInformation>(requestBody);
            OpenAIClient client = new OpenAIClient(
            new Uri("https://oai-uksouth-rr.openai.azure.com/"),
            new AzureKeyCredential(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")));
            string user_message = await req.ReadAsStringAsync() ?? string.Empty;
           
            var propmtTemplate = $@"
please fulfil the below job description template. Pay special attention to the INSTRUCTION keyword because this is where I 
need you to add the relevant information consiering the job position. Also use this keywords {jobsInformation.Tags} to narrow down your scope when working on the INSTRUCTION.

Position: 
{jobsInformation.JobTitle}

Location: 
{jobsInformation.Location}

Reports To: 
{jobsInformation.ReportsTo}

Type: 
{jobsInformation.ContractType}

Objective:
INSTRUCTION: PLease, Provide a concise summary of the primary purpose of the  role {jobsInformation.JobTitle} within the organization. This should give a clear picture of why the role exists and its impact.

Key Responsibilities:
1. INSTRUCTION: Please, add here the top 3 key responsabilities for the job.

Qualifications:
- Education: INSTRUCTION: PleasE provide a typical DegreeLevel required in the typical Field Of Study
- Experience: INSTRUCTION:provide an average years of experience
- Skills: 
   - INSTRUCTION: dd here the top 3 skills required for the job.

Key Competencies: INSTRUCTION: Add here the most imortant competencies for the job.
- Ability to   INSTRUCTION: add relevant topic here
- Demonstrated INSTRUCTION: add relevant topic herecontinue

Benefits & Perks:
- {jobsInformation.Benefits}

About the compay
{jobsInformation.CompanyAbout}

Application Process:
{jobsInformation.ApplicationProcess}
";

      
            // ### If streaming is not selected
            Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
                "gpt-35-turbo-16k",
                new ChatCompletionsOptions()
                {
                    
                    Messages =
                    {
                        new ChatMessage(ChatRole.System, @"You are an HR assistant that helps HR recruiters and talent finders to find the right information relevant to their business"),
                        new ChatMessage(ChatRole.User, propmtTemplate)
                        //new ChatMessage(ChatRole.Assistant, @"Hello! How can I assist you today?"),
                    },
                    Temperature = (float)1.7,
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
    }
}