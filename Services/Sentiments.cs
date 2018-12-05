using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;

namespace pusher_chat_server.Services
{
    public class Sentiments : ISentiments
    {
        /// <summary>
        /// Container for subscription credentials. Make sure to enter your valid key.
        static string subscriptionKey = Startup.Configuration["AzureCognitive:SentimentsKey"]; //Insert your Text Anaytics subscription key
        /// </summary>
        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }
        public double getSentimentsScore(string Text)
        {

            // Create a client.
            ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = "https://westeurope.api.cognitive.microsoft.com"
            }; //Replace 'westus' with the correct region for your Text Analytics subscription

            SentimentBatchResult result = client.SentimentAsync(
                new MultiLanguageBatchInput(
                    new List<MultiLanguageInput>(){
                        new MultiLanguageInput("en", "0", Text)
                    }
                )
            ).Result;
            var SentimentResultQuery = result.Documents.FirstOrDefault();

            return (double)SentimentResultQuery.Score;
        }
    }

}