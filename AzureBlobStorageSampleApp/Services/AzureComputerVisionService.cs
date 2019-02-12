using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;

namespace AzureBlobStorageSampleApp.Services
{
    public class VisionService
    {

        private const string subscriptionKey = "";
        //static readonly AzureRegions ComputerVisionRegion = AzureRegions.Eastus;

        //readonly IComputerVisionAPI visionAPI;

        public ComputerVisionClient computerVisionClient; // = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey)

        public VisionService()
        {
            //var creds = new ApiKeyServiceClientCredentials(ComputerVisionKey);
            //visionAPI = new ComputerVisionAPI(creds) { AzureRegion = CouputerVisionRegion };
            //trainingApi = new TrainingApi() { ApiKey = TrainingKey };


            computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey));


            //computerVision.Endpoint = "https://eastus.api.cognitive.microsoft.com/";  //as listed in portal
            computerVisionClient.Endpoint = "https://eastus.api.cognitive.microsoft.com";  //as listed in sample


        }
    }
}
