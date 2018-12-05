using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pusher_chat_server.Models;
using pusher_chat_server.Services;
using PusherServer;

namespace pusher_chat_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private IConfiguration _config;
        private ISentiments _sentiments;

        public MessagesController(IConfiguration config, ISentiments sentiment)
        {
            _config = config;
            _sentiments = sentiment;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MessageModel Message)
        {
            var options = new PusherOptions
            {
                Cluster = Startup.Configuration["pusher:cluster"],
                Encrypted = true
            };

            // create new instance of pusher with parameters 
            var Pusher = new Pusher(
                Startup.Configuration["pusher:appId"],
                Startup.Configuration["pusher:key"],
                Startup.Configuration["pusher:secret"],
                options
            );

            double sentimentsScore = _sentiments.getSentimentsScore(Message.Text);
            string Tone = "";
            if (sentimentsScore >= 0 && sentimentsScore <= 0.49)
            {
                Tone = "negative";
            }
            else if (sentimentsScore >= 0.51 && sentimentsScore <= 1)
            {
                Tone = "positive";
            }
            else if (sentimentsScore == 0.5)
            {
                Tone = "neutral";
            }

            // create response object
            var Response = new
            {
                text = Message.Text,
                id = Message.Id,
                timeStamp = DateTime.Now,
                sentiment = new
                {
                    tone = Tone,
                    score = sentimentsScore

                }
            };

            var result = await Pusher.TriggerAsync(
                "chat",
                "message",
                Response
            );

            return Ok(Response);
        }
    }
}