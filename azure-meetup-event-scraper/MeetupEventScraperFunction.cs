using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace azure_meetup_event_scraper
{
    public static class MeetupEventScraperFunction
    {
        [FunctionName("MeetupEventScraperFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //return new OkObjectResult(responseMessage);

            Regex _regex = new Regex("[^0-9]");

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

                    groupEvent.Type = groupEvent.Title.Contains("virtual", StringComparison.OrdinalIgnoreCase) ? EventType.VBA : EventType.NBA;

                    groupEvents.Add(groupEvent);
                }
            }

            return new OkObjectResult(groupEvents);
        }
    }
}
