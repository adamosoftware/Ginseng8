﻿using Ginseng.Mvc.Enums.Freshdesk;
using System;

namespace Ginseng.Mvc.Models.Freshdesk
{
    public class Webhook
    {
        public DateTimeOffset Received { get; set; }
        public WebhookEvent Event { get; set; }
        public TicketInfo Ticket { get; set; }
    }
}