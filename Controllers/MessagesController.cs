using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pusher_chat_server.Models;
using PusherServer;

namespace pusher_chat_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private IConfiguration _config;

        public MessagesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MessageModel Message)
        {
            var options = new PusherOptions
            {
                Cluster = Startup.Configuration["pusher:cluster"],
                Encrypted = true
            };

            var pusher = new Pusher(
                Startup.Configuration["pusher:appId"],
                Startup.Configuration["pusher:key"],
                Startup.Configuration["pusher:secret"],
                options
            );

            var Response = new {
                text = Message.Text,
                id = Message.Id,
                timeStamp = DateTime.Now
            };
            var result = await pusher.TriggerAsync(
                "chat",
                "message",
                Response
            );

            return Ok(Response);
        }
    }
}