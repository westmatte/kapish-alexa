using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using System;

namespace lunchroulette
{
    public static class LunchRoulette
    {
        private static string[] restaurants = new string[]{"Gustav Adolfs Café och Restaurang", "Shiso Burgers", "Icha Mochi", "Vespa", "Tugg", "Sumo Sushi", "Bishops Arms", "Mrs. Saigon", "Delikatessen", "Bullens", "Nam Do", "Indiskt - Sizzlar", "Green Chili"};

        [FunctionName("LunchRoulette")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);
            return ProcessRequest(skillRequest);
        }
        private static IActionResult ProcessRequest(SkillRequest skillRequest)
        {
            var requestType = skillRequest.GetRequestType();
            SkillResponse response = null;
            if (requestType == typeof(LaunchRequest))
            {
                var output = new SsmlOutputSpeech();
                output.Ssml = "<speak>Welcome to Lunch Roulette! How can I help you today?</speak>";

                response = ResponseBuilder.Tell(output);
                response.Response.ShouldEndSession = false;
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;
                if (intentRequest.Intent.Name == "Choose")
                {
                    var random = new Random().Next(restaurants.Length);
                    var randomizedRestaurant = restaurants[random];

                    response = ResponseBuilder.Tell($"Today I suggest you go eat at {randomizedRestaurant}");
                    response.Response.ShouldEndSession = false;
                }
                if (intentRequest.Intent.Name == "List")
                {
                    var availableRestaurants = string.Join(',', restaurants);

                    response = ResponseBuilder.Tell($"The available restaurants to choose from are the following: {availableRestaurants}");
                    response.Response.ShouldEndSession = false;
                }
            }
            else if (requestType == typeof(SessionEndedRequest))
            {
                response = ResponseBuilder.Tell("I hope you got the answer you wanted. Goodbye.");
                response.Response.ShouldEndSession = true;
            }
            return new OkObjectResult(response);
        }                                                                                                                                             
    }
}
