using System;
using Newtonsoft.Json;

namespace Ordina.Unite.Api.Models
{
    public class ApiCourse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("availableSeats")]
        public int AvailableSeats { get; set; }
    }
}
