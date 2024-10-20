// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace com.functions.appconfigurationwrapper
{
    public class ConfigurationUpdateTrigger
    {
        private readonly ILogger<ConfigurationUpdateTrigger> _logger;

        public ConfigurationUpdateTrigger(ILogger<ConfigurationUpdateTrigger> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ConfigurationUpdateTrigger))]
        public async Task<IActionResult> RunAsync([EventGridTrigger] EventGridEvent[] events)
        {
            foreach (EventGridEvent @event in events)
            {
                _logger.LogInformation("Event type: {type}, Event subject: {subject}", @event.EventType, @event.Subject);
                if (@event.EventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
                {
                    return new OkObjectResult(new
                    {
                        ValidationResponse = @event.Data.ToObjectFromJson<SubscriptionValidationEventData>().ValidationCode
                    });
                }
                else if (@event.EventType == "Microsoft.AppConfiguration.KeyValueUpdated")
                {
                    var http = new HttpClient();
                    var apiUrl = Environment.GetEnvironmentVariable("CentralizedWrapperAPIURL");
                    var response = await http.GetAsync(apiUrl);
                }
            }
            return new OkObjectResult("");
        }
    }
}
