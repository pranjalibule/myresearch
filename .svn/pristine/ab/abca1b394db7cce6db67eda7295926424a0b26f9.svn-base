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

    public partial class TSOProductivityOutcomeDTO : IBaseDTO
    {
        public TSOProductivityOutcomeDTO()
        {
            TSOProductivityOutcomeActuals = new HashSet<TSOProductivityOutcomeActualDTO>();
            TSOProductivityOutcomePlanneds = new HashSet<TSOProductivityOutcomePlannedDTO>();
        }

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("TSOId")]
        public int TSOId { get; set; }

        [JsonProperty("ProductivityOutcomeId")]
        public int ProductivityOutcomeId { get; set; }

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

        [JsonProperty("ProductivityOutcome")]
        public virtual ProductivityOutcomeDTO ProductivityOutcome { get; set; }

        [JsonProperty("TSOProductivityOutcomeActuals")]
        public virtual ICollection<TSOProductivityOutcomeActualDTO> TSOProductivityOutcomeActuals { get; set; }

        [JsonProperty("TSOProductivityOutcomePlanneds")]
        public virtual ICollection<TSOProductivityOutcomePlannedDTO> TSOProductivityOutcomePlanneds { get; set; }

        [JsonIgnore]
        public ObjectSate ObjectSate { get; set; }
    }
}
