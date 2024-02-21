using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ObjectMapperApp
{
    public class Message
    {

        public int Id { get; set; }

        public long Date { get; set; }
        public long? EditedAt { get; set; }
        public long? DeletedAt { get; set; }
        public string Sender { get; set; }

        public string? TextContent { get; set; }

        public string MessageThreadId { get; set; }

        public long CountdownStart { get; set; }
        public long CountdownEnd { get; set; }
    }
}