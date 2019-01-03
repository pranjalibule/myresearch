using Newtonsoft.Json;
using SQS.nTier.TTM.GenericFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS.nTier.TTM.DTO
{
    public partial class RiskDTO: IBaseDTO
    {
         [JsonProperty("ID")]
         public int ID { get; set; }

        [JsonProperty("WeekNumber")]
        public int WeekNumber { get; set; }    

        [JsonProperty("ResponsiblePersonId")]
        public int? ResponsiblePersonId { get; set; }

        [JsonProperty("DueDate")]
        public DateTime? DueDate { get; set; }
        [JsonProperty("ResponsiblePerson")]
        public virtual UserDTO ResponsiblePerson { get; set; }

        [JsonProperty("ActualOperationalRiskId")]
        public int ActualOperationalRiskId { get; set; }

        [JsonProperty("ActualOperationalRisk")]
        public virtual OperationalRiskDTO ActualOperationalRisk { get; set; }

        [JsonProperty("ActualOperationalRiskIndicatorId")]
        public int ActualOperationalRiskIndicatorId { get; set; }

        [JsonProperty("TSOServiceDeliveryChainId")]
        public int TSOServiceDeliveryChainId { get; set; }

        [JsonProperty("TSOServiceDeliveryChain")]
        public virtual TSOServiceDeliveryChainDTO TSOServiceDeliveryChain { get; set; }

        [JsonProperty("ActualOperationalRiskIndicator")]
        public virtual OperationalRiskIndicatorDTO ActualOperationalRiskIndicator { get; set; }

        [JsonProperty("ActualOperationalRiskDescription")]
        public string ActualOperationalRiskDescription { get; set; }

        [JsonProperty("ActualOperationalRiskMitigation")]
        public string ActualOperationalRiskMitigation { get; set; }

        //[JsonProperty("PlannedRiskId")]
        //public int PlannedRiskId { get; set; }

        //[JsonProperty("PlannedOperationalRiskId")]
        //public int PlannedOperationalRiskId { get; set; }

        //[JsonProperty("PlannedOperationalRisk")]
        //public virtual OperationalRiskDTO PlannedOperationalRisk { get; set; }
        //[JsonProperty("PlannedOperationalRiskIndicatorId")]
        //public int PlannedOperationalRiskIndicatorId { get; set; }
        //[JsonProperty("PlannedOperationalRiskIndicator")]
        //public virtual OperationalRiskIndicatorDTO PlannedOperationalRiskIndicator { get; set; }

        //[JsonProperty("PlannedOperationalRiskDescription")]
        //public string PlannedOperationalRiskDescription { get; set; }

        //[JsonProperty("PlannedOperationalRiskMitigation")]
        //public string PlannedOperationalRiskMitigation { get; set; }


        [JsonProperty("Impact")]
        public string Impact { get; set; }
        [JsonProperty("Resolution")]
        public string Resolution { get; set; }
        [JsonProperty("RaisedById")]
        public int RaisedById { get; set; }
        [JsonProperty("RaisedBy")]
        public virtual UserDTO RaisedBy { get; set; }
        [JsonProperty("Status")]
        public virtual RiskStatusDTO Status { get; set; }
        [JsonProperty("StatusId")]
        public int StatusId { get; set; }


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
