/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 18Sep2017 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.DTO
{
    using GenericFramework;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;


    public partial class TSOProductivityInputDTO : IBaseDTO
    {
        public TSOProductivityInputDTO()
        {
            TSOProductivityInputActuals = new HashSet<TSOProductivityInputActualDTO>();
            TSOProductivityInputPlanneds = new HashSet<TSOProductivityInputPlannedDTO>();
        }

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("TSOId")]
        public int TSOId { get; set; }

        [JsonProperty("ProductivityInputId")]
        public int ProductivityInputId { get; set; }

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

        [JsonProperty("TSO")]
        public virtual TSODTO TSO { get; set; }

        [JsonProperty("ProductivityInput")]
        public virtual ProductivityInputDTO ProductivityInput { get; set; }

        [JsonProperty("TSOProductivityInputActuals")]
        public virtual ICollection<TSOProductivityInputActualDTO> TSOProductivityInputActuals { get; set; }

        [JsonProperty("TSOProductivityInputPlanneds")]
        public virtual ICollection<TSOProductivityInputPlannedDTO> TSOProductivityInputPlanneds { get; set; }

        [JsonIgnore]
        public ObjectSate ObjectSate { get; set; }
    }
}
