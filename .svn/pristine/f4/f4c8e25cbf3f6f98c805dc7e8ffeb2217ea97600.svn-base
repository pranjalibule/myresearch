/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * Pranjali Mankar 22August2018 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.DTO
{
    using GenericFramework;
    using Newtonsoft.Json;
    using System;

    public class TSRTMOUserDTO : IBaseDTO
    {
        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("TSRId")]
        public int TSRId { get; set; }

        [JsonProperty("UserId")]
        public int UserId { get; set; }

        public UserDTO User { get; set; }

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
