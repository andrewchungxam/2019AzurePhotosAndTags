using System;
using Microsoft.Cognitive.CustomVision.Prediction;

namespace AzureBlobStorageSampleApp.Services
{
    public class CustomVisionService
    {
        //#error You need to set up your API keys.
        // Start by registering for an account at https://customvision.ai
        // Then create a new project.
        // From the settings tab, find:

        // and update the values below
        //public static string PredictionKey = "<Your Prediction Key>";
        //public static Guid ProjectId = Guid.Parse("<Your Project GUID>");

        public static string PredictionKey = "";
        public static Guid ProjectId = Guid.Parse("");

        public const double ProbabilityThreshold = 0.15; ////0.0085; //0.1; //0.5

        public Microsoft.Cognitive.CustomVision.Prediction.PredictionEndpoint _endpoint = new PredictionEndpoint
        {
            ApiKey = PredictionKey,
        };

        public CustomVisionService()
        {



        }
    }
}
