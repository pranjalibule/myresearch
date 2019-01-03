

namespace SQS.nTier.TTM.DTO
{
    using GenericFramework;
    using Newtonsoft.Json;
    using System;

    public partial class ServiceDeliveryChainMethodDTO : IBaseDTO
    {
        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("ServiceDeliveryChainId")]
        public int ServiceDeliveryChainId { get; set; }

        [JsonProperty("ServiceDeliveryChain")]
        public virtual ServiceDeliveryChainDTO ServiceDeliveryChain { get; set; }

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        [JsonProperty("Version")]
        public int Version { get; set; }

        [JsonIgnore]
        public ObjectSate ObjectSate { get; set; }
    }
}
