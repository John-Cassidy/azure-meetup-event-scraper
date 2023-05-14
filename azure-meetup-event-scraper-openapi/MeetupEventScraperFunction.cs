using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace azure_meetup_event_scraper_openapi
{
    public class MeetupEventScraperFunction
    {
        private readonly ILogger<MeetupEventScraperFunction> _logger;

        public MeetupEventScraperFunction(ILogger<MeetupEventScraperFunction> log)
        {
            _logger = log;
        }

        [FunctionName("MeetupEventScraperFunction")]
        [OpenApiOperation(operationId: "Run")]
        //[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            Regex _regex = new Regex("[^0-9]");

            string name = req.Query["name"];

            List<GroupEvent> groupEvents = new List<GroupEvent>();

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc = web.Load("https://www.meetup.com/bostonazure/events");

            // extract each event id, link, and title
            foreach (HtmlNode eventsNodes in doc.DocumentNode.SelectNodes("//div[@class='card card--hasHoverShadow eventCard border--none border--none buttonPersonality']")) {

                HtmlNode? eventNode = eventsNodes.SelectSingleNode("//a[@class='eventCard--link']");
                if (eventNode != null) {
                    GroupEvent groupEvent = new GroupEvent();

                    groupEvent.Title = eventNode.InnerText;

                    HtmlAttribute eventLink = eventNode.Attributes["href"];
                    if (eventLink.Value.Contains("a")) {
                        groupEvent.Link = eventLink.Value;
                    }

                    if (int.TryParse(_regex.Replace(eventLink.Value, ""), out var result)) {
                        groupEvent.Id = result;
                    }

                    groupEvent.EventTypeId = groupEvent.Title.Contains("virtual", StringComparison.OrdinalIgnoreCase) ? EventType.VBA : EventType.NBA;
                    groupEvent.EventTypeName = groupEvent.Title.Contains("virtual", StringComparison.OrdinalIgnoreCase) ? EventType.VBA.ToString() : EventType.NBA.ToString();

                    groupEvents.Add(groupEvent);
                }
            }

            return new OkObjectResult(groupEvents);
        }
    }
}

