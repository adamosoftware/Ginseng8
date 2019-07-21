﻿using Newtonsoft.Json;

namespace Ginseng.Mvc.Models.Freshdesk.Dto
{
    public class CustomFields
    {
        [JsonProperty("cf_work_item")] // note, this is generated by Freshdesk's custom field logic based on the name you give the field
        public string GinsengWorkItem { get; set; }
    }
}