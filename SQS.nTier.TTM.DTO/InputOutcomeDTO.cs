
namespace SQS.nTier.TTM.DTO
{
    using Newtonsoft.Json;
    public partial class InputOutcomeDTO
    {
        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Value")]
        public float Value { get; set; }

        [JsonProperty("Week")]
        public int Week { get; set; }

        [JsonProperty("Year")]
        public int Year { get; set; }

        [JsonProperty("TSOPID")]
        public virtual int TSOPID { get; set; }
    }
}
