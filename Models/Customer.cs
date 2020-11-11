using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intercom_JsonRead.Models
{
    public class Customer
    {
        [JsonProperty("user_id")]
        public int user_id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("latitude")]
        public Double latitude { get; set; }

        [JsonProperty("longitude")]
        public Double longitude { get; set; }

        public Double distance { get; set; }
    }

    
}
