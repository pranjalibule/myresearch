﻿/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 04Oct2017 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.DTO
{
    using Newtonsoft.Json;
    using SQS.nTier.TTM.GenericFramework;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class UserDTO : IBaseDTO
    {
        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("UserTypeID")]
        public int? UserTypeID { get; set; }

        [JsonProperty("EmailID")]
        public string EmailID { get; set; }

        [JsonProperty("UserId")]
        public string UserId { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ActivationKey")]
        public string ActivationKey { get; set; }

        [JsonProperty("ProfilePicLocation")]
        public string ProfilePicLocation { get; set; }

        [JsonProperty("Activated")]
        public bool Activated { get; set; }

        [JsonProperty("Locked")]
        public bool Locked { get; set; }

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

        [JsonProperty("RoleId")]
        public int? RoleId { get; set; }

        [JsonProperty("Role")]
        public virtual RolesDTO Role { get; set; }

        //[JsonProperty("Roles")]
        //public virtual ICollection<RolesDTO> Roles { get; set; }

        [JsonIgnore]
        public ObjectSate ObjectSate { get; set; }
    }
}
