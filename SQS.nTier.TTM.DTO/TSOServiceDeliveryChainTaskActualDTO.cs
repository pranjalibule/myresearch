﻿using Newtonsoft.Json;
using SQS.nTier.TTM.GenericFramework;
using System;

namespace SQS.nTier.TTM.DTO
{
    public partial class TSOServiceDeliveryChainTaskActualDTO : IBaseDTO
    {
        #region Properties

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("ActualEffort")]
        public double? ActualEffort { get; set; }

        [JsonProperty("WeekNumber")]
        public int WeekNumber { get; set; }

        [JsonProperty("Year")]
        public int Year { get; set; }

        [JsonProperty("ActualProductivity")]
        public double ActualProductivity { get; set; }

        [JsonProperty("ActualOutcome")]
        public int ActualOutcome { get; set; }

        [JsonProperty("ActualOutcomeTestSteps")]
        public int ActualOutcomeTestSteps { get; set; }

        [JsonProperty("ActualReviewRounds")]
        public int ActualReviewRounds { get; set; }

        [JsonProperty("DefectRaised")]
        public int DefectRaised { get; set; }

        [JsonProperty("DefectRejected")]
        public int DefectRejected { get; set; }

        [JsonProperty("ActualProcessingTime")]
        public double ActualProcessingTime { get; set; }

        [JsonProperty("ActualInput")]
        public int ActualInput { get; set; }

        [JsonProperty("IdleTimeEffort")]
        public float IdleTimeEffort { get; set; }

        [JsonProperty("IdleTimeDuration")]
        public float IdleTimeDuration { get; set; }

        [JsonProperty("Headcount")]
        public double Headcount { get; set; }

        //Actual Productivity (Input based)
        [JsonProperty("ActualOutcomeRatio")]
        public double ActualOutcomeRatio { get; set; }

        //Actual Productivity(Time Based):
        [JsonProperty("ActualThroughput")]
        public double ActualThroughput { get; set; }

        //Defect Density
        [JsonProperty("DefectDensity")]
        public double DefectDensity { get; set; }

        //Defect Rejection Ration
        [JsonProperty("DefectRejectionRatio")]
        public double DefectRejectionRatio { get; set; }

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

        [JsonProperty("TSOServiceDeliveryChainTaskId")]
        public int TSOServiceDeliveryChainTaskId { get; set; }

        [JsonProperty("TSOServiceDeliveryChainTask")]
        public virtual TSOServiceDeliveryChainDTO TSOServiceDeliveryChainTask { get; set; }

        //[JsonProperty("ActualOperationalRiskId")]
        //public int? ActualOperationalRiskId { get; set; }

        //[JsonProperty("ActualOperationalRisk")]
        //public virtual OperationalRiskDTO ActualOperationalRisk { get; set; }

        //[JsonProperty("ActualOperationalRiskIndicatorId")]
        //public int? ActualOperationalRiskIndicatorId { get; set; }
        //[JsonProperty("ActualOperationalRiskIndicator")]
        //public virtual OperationalRiskIndicatorDTO ActualOperationalRiskIndicator { get; set; }

        //[JsonProperty("ActualOperationalRiskDescription")]
        //public string ActualOperationalRiskDescription { get; set; }

        //[JsonProperty("ActualOperationalRiskMitigation")]
        //public string ActualOperationalRiskMitigation { get; set; }

        [JsonIgnore]
        public ObjectSate ObjectSate { get; set; }

        #endregion
      

        public void CalculateFields()
        {           
            ActualOutcomeRatio = ActualInput != 0 ? ActualOutcome / ActualInput : 0;           
            ActualThroughput = ActualProcessingTime != 0 ? ActualOutcome / ActualProcessingTime : 0;           
            ActualProductivity = ActualEffort != 0 ? ActualOutcome / (double)ActualEffort : 0;
            DefectDensity = ActualOutcome != 0 ? DefectRaised / ActualOutcome : 0;
            DefectRejectionRatio = DefectRaised != 0 ? DefectRejected / DefectRaised * 100 : 0;
        }
    }
}
