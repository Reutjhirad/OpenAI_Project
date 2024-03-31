using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prog3_WebApi_Javascript.DTOs;
using System.Data;
using System.Text.Json.Nodes;

namespace Prog3_WebApi_Javascript.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class GPTController : ControllerBase
    {
        private readonly HttpClient _client;

        public GPTController(IConfiguration config)
        {
            _client = new HttpClient();
            string api_key = config.GetValue<string>("OpenAI:Key");
            string auth = "Bearer " + api_key;
            _client.DefaultRequestHeaders.Add("Authorization", auth);
        }

        [HttpPost("GPTChat")]
        public async Task<IActionResult> GPTChat(Prompt promptFromUser)
        {
            Console.WriteLine("check 1" );
            // API endpoint for OpenAI GPT
            string endpoint = "https://api.openai.com/v1/chat/completions";
            // Specifies the model to use for chat completions (GPT-3.5 Turbo)
            string model = "gpt-3.5-turbo-0125";
            // Maximum number of tokens in the generated response
            int max_tokens = 300;
            // Construct the prompt to send to the model
            string promptToSend = $"{promptFromUser.Subject}, {promptFromUser.Language}";

            double temperature = 0.8;

            // Create a GPTRequest object to send to the API
            GPTRequest request = new GPTRequest()
            {
                max_tokens = max_tokens,
                model = model,
                temperature = temperature,
                response_format = new { type = "json_object" },
                messages = new List<Message>() {
                    new Message
                    {
                        role="system",
                        content = "You are an Excel expert. You contain questions and their answers, on Excel in Hebrew, English and Arabic for the following topics: data visualization, sorting and filtering data, formulas and functions. Given a topic, as well as a language, you return 5 single-choice questions as many as possible, each answer is shown as a radio button (4 answers in total with 3 wrong answers and 1 correct answer, show the correct answer). Whenever you receive a request, you must reply in the following JSON format: {'question': string, 'answers': ['text' : string , 'isCorrect':boolean]}}."
                    }, 
                    new Message
                    {
                        role = "user",
                        content = "Data visualization, Hebrew"
                    },
                    new Message
                    {
                        role = "assistant",
                        content = "{'question': 'איזה סוג גרף באקסל מתאים כדי להשוות בין שני עמודות של נתונים ולזהות את הטרנדים העיקריים בהם?', 'answers':[{'text':'גרף קו', 'isCorrect':true}, {'text':'גרף עמודות', 'isCorrect':false}, {'text':'גרף פאי', 'isCorrect':false}]}"
                    },
                    new Message
                    {
                        role = "user",
                        content = "Sorting and filtering data, English"
                    },
                    new Message
                    {
                        role = "assistant",
                        content ="{'question': 'When dealing with data analysis in Excel, why is it important to use data sorting and filtering tools?', 'answers':[{'text':'Sorting helps arrange data in a meaningful order, aiding analysis', 'isCorrect':true}, {'text':'Filtering allows focusing on specific data subsets for deeper insights', 'isCorrect':true}, {'text':'Using these tools enhances efficiency and accuracy in data analysis', 'isCorrect':false}]}"
                    },
                    new Message
                    {
                        role = "user",
                        content = "Formulas and functions, Hebrew"
                    },
                    new Message
                    {
                        role = "assistant",
                        content = "{'question': 'מה ההבדל בין פונקציית סכום לפונקציית ספירה', 'answers':[{'text':'אותה פונקציה' , 'isCorrect':false}, {'text':'אחת סוכמת והשנייה סופרת', 'isCorrect':true}, {'text':'אפשר לעשות יותר פעולות עם פונקציית סכום מאשר ספירה', 'isCorrect':false}]}"
                    },
                 new Message
            {
                role = "user",
                content = promptToSend
            }
        }
            };


            Console.WriteLine("check 2");
            // Send the GPTRequest object to the OpenAI API
            var res = await _client.PostAsJsonAsync(endpoint, request);
            Console.WriteLine(res.ToString());

            // Check if the API response indicates an error
            if (!res.IsSuccessStatusCode)
                return BadRequest("problem: " + res.Content.ReadAsStringAsync());

            Console.WriteLine("check 3");
            // Read the JSON response from the API
            JsonObject? jsonFromGPT = res.Content.ReadFromJsonAsync<JsonObject>().Result;
            if (jsonFromGPT == null)
            {
                Console.WriteLine("check 4");
                return BadRequest("empty");
            }
            // Extract the generated content from the JSON response
            string content = jsonFromGPT["choices"][0]["message"]["content"].ToString();

            Console.WriteLine("content is : " + content);
            // Return the generated content
            return Ok(content);

        }

    }

}

