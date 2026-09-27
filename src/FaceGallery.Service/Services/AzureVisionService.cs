using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using c__service_2026.Interfaces;

namespace c__service_2026.Services
{
    // מחלקה זו מממשת את הממשק של Azure
    public class AzureVisionService : IAzureVisionService
    {
        private readonly ImageAnalysisClient _client;

        // בנאי (Constructor) שמקבל את המפתחות ומייצר את החיבור למיקרוסופט
        public AzureVisionService(string endpoint, string apiKey)
        {
            if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(apiKey))
            {
                _client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            }
        }

        // הפונקציה המרכזית שמזהה אנשים בתמונה
        public async Task<List<FaceDetectionResult>> DetectFacesAsync(string imageUrl)
        {
            var results = new List<FaceDetectionResult>();

            // אם אין חיבור ל-Azure (חסר מפתח למשל), מחזיר רשימה ריקה כדי שהשרת לא יקרוס
            if (_client == null) return results;

            try
            {
                // קריאה לאלגוריתם של מיקרוסופט - מבקשים לזהות אנשים (People)
                var result = await _client.AnalyzeAsync(new Uri(imageUrl), VisualFeatures.People);

                // בודקים שאכן חזרו תוצאות ושהרשימה (Values) קיימת
                if (result.Value.People != null && result.Value.People.Values != null)
                {
                    foreach (var person in result.Value.People.Values)
                    {
                        results.Add(new FaceDetectionResult
                        {
                            Confidence = person.Confidence,
                            // לוקחים את הקואורדינטות של האדם שזוהה
                            FaceCoordinates = $"{person.BoundingBox.X},{person.BoundingBox.Y},{person.BoundingBox.Width},{person.BoundingBox.Height}"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // מדפיס את השגיאה רק לקונסול כדי שנוכל לדבג אם משהו משתבש
                Console.WriteLine($"שגיאה בזיהוי מול Azure: {ex.Message}");
            }

            return results;
        }
    }
}