using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace pusher_chat_server.Models
{
   
    public class MessageModel
    {
        public string Id { get; set; }
        public string Text { get; set; }

    }

}