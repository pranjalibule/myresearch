using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS.nTier.TTM.DTO
{
    using GenericFramework;
    using Newtonsoft.Json;
    using System;

    public partial class TSRFileUploadDTO : IBaseDTO
    {
        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("TSRId")]
        public int TSRId { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("GUID")]
        public string GUID { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

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

        [JsonProperty("TSR")]
        public virtual TSRDTO TSR { get; set; }
    }
}
