using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiTallyController : ControllerBase
    {
        private const string GEMINI_API_KEY = "AIzaSyBswdibdWpb_3sbz5d3HC1hfiBAIel18o0"; // Replace with your API Key
        private const string GEMINI_ENDPOINT = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=AIzaSyBswdibdWpb_3sbz5d3HC1hfiBAIel18o0";

        [HttpPost("tally")]
        public async Task<IActionResult> TallyCv([FromBody] CvRequest cvRequest)
        {
            // Prepare the content based on education and experience
            string educationCriteria = "Bachelor of Science in Information Technology";
            string experienceCriteria = "Software Engineer with 3+ years of experience";

            // Assuming you're checking this based on some logic
            int educationScore = CheckEducation(cvRequest.CvText, educationCriteria);
            int experienceScore = CheckExperience(cvRequest.CvText, experienceCriteria);

            // Average the scores for the final tally
            int totalScore = (educationScore + experienceScore) / 2;

            // Send the CV to Gemini API for validation (if needed)
            string geminiResponse = await SendToGeminiApi(cvRequest.CvText);

            // Return the tally and Gemini response
            return Ok(new
            {
                EducationScore = educationScore,
                ExperienceScore = experienceScore,
                TotalScore = totalScore,
                GeminiResponse = geminiResponse
            });
        }

        // Function to check education match score
        private int CheckEducation(string cvText, string educationCriteria)
        {
            // This logic is just an example. You can modify it based on your actual needs.
            return cvText.Contains(educationCriteria) ? 100 : 0; // Example: 100 if matched, 0 if not
        }

        // Function to check experience match score
        private int CheckExperience(string cvText, string experienceCriteria)
        {
            // Again, this is an example. Modify it as per your business logic.
            return cvText.Contains(experienceCriteria) ? 100 : 0; // Example: 100 if matched, 0 if not
        }

        // Send CV to Gemini API
        private async Task<string> SendToGeminiApi(string cvText)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Construct the Gemini API request body
                var jsonPayload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = cvText }
                            }
                        }
                    }
                };

                // Prepare the request
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer AIzaSyBswdibdWpb_3sbz5d3HC1hfiBAIel18o0");

                // Send the request
                HttpResponseMessage response = await httpClient.PostAsync(GEMINI_ENDPOINT + GEMINI_API_KEY, content);

                if (response.IsSuccessStatusCode)
                {
                    // If successful, return the response as a string (you can further process it as needed)
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Return an error message if the call fails
                    return "Failed to contact Gemini API. Error: " + response.ReasonPhrase;
                }
            }
        }
    }

    // Model for the incoming CV Request
    public class CvRequest
    {
        public string CvText { get; set; }
    }
}
