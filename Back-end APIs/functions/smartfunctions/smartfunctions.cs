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

         [Function("GenerateJobDescription")]
        public async Task<HttpResponseData> GenerateJobDescription(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GenerateJobDescription")] HttpRequestData req)
        {
             OpenAIClient client = new OpenAIClient(
            new Uri("https://oai-uksouth-rr.openai.azure.com/"),
            new AzureKeyCredential(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")));
            string user_message = await req.ReadAsStringAsync() ?? string.Empty;
            string propmttemplate = @"

---

## Position: 
**{Job Title}**

## Department: 
**{Department/Team}**

## Location: 
**{Office Location/Remote}**

## Reports To: 
**{Supervising Position}**

## Type: 
**{Full-Time/Part-Time/Contract}**

---

### Objective:

Provide a concise summary of the primary purpose of this role within the organization. This should give a clear picture of why the role exists and its impact.

`{Brief overview of the role's main objective and its contribution to the company's goals.}`

---

### Key Responsibilities:

1. **{Primary Responsibility}**: 
   - `{Specific task or duty related to the primary responsibility}`
   - `{Another task or duty}`

2. **{Secondary Responsibility}**: 
   - `{Specific task or duty related to the secondary responsibility}`
   - `{Another task or duty}`

(Continue listing responsibilities as necessary.)

---

### Qualifications:

- **Education:** `{Degree Level (e.g., Bachelor's, Master's)} in {Field of Study}`
- **Experience:** `{Number of years} years of experience in {specific field or role}`
- **Skills:** 
   - `{Specific skill or competency}`
   - `{Another skill or competency}`
   
(Continue listing qualifications as necessary.)

---

### Key Competencies:

- **{Competency 1}**: Ability to `{specific behavior or trait related to competency}`
- **{Competency 2}**: Demonstrated `{specific behavior or trait}`
   
(Continue listing competencies as necessary.)

---

### Benefits & Perks:

- `{Benefit such as health insurance, dental coverage, etc.}`
- `{Perk such as flexible working hours, remote work options, etc.}`

(Continue listing benefits and perks as necessary.)

---

### About {Company Name}:

Provide a brief description of the company, its mission, and its culture. This helps potential candidates understand the company's values and what it stands for.

`{Brief overview of the company, its history, mission, vision, and values.}`

---

### Equal Opportunity Statement:

`{Company Name}` is an equal opportunity employer. We celebrate diversity and are committed to creating an inclusive environment for all employees.

---

### Application Process:

Interested candidates are invited to submit their `{resume/CV, cover letter, portfolio, etc.}` to `{HR email or application portal link}`. Only those selected for an interview will be contacted.
" + user_message;

       
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
           
            response.WriteString(completions.Choices[0].Message.Content.ToString());
            //response['choices'][0]['message']['content'])
            return response;
        }
    }
}