namespace SQS.nTier.TTM.DTO
{
    using Newtonsoft.Json;
    public class TSOServiceDeliveryChainTaskDumpDTO
    {
        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Client")]
        public string Client { get; set; }

        [JsonProperty("TaskStatus")]
        public string TaskStatus { get; set; }

        [JsonProperty("RelatedTSR")]
        public string RelatedTSR { get; set; }

        [JsonProperty("RelatedTSO")]
        public string RelatedTSO { get; set; }

        [JsonProperty("EnagementModel")]
        public string EnagementModel { get; set; }

        [JsonProperty("PricingModel")]
        public string PricingModel { get; set; }

        [JsonProperty("ProjectModel")]
        public string ProjectModel { get; set; }

        [JsonProperty("ClientRegion")]
        public string ClientRegion { get; set; }

        [JsonProperty("Account")]
        public string Account { get; set; }

        [JsonProperty("ERPOrderNumber")]
        public string ERPOrderNumber { get; set; }

        [JsonProperty("SolutionCenter")]
        public string SolutionCenter { get; set; }

        [JsonProperty("CoreService")]
        public string CoreService { get; set; }

        [JsonProperty("Method")]
        public string Method { get; set; }

        [JsonProperty("PlannedProcessingTime")]
        public string PlannedProcessingTime { get; set; }

        [JsonProperty("ActualProcessingTime")]
        public string ActualProcessingTime { get; set; }

        [JsonProperty("PlannedEffort")]
        public string PlannedEffort { get; set; }

        [JsonProperty("ActualEffort")]
        public string ActualEffort { get; set; }

        [JsonProperty("PlannedInput")]
        public string PlannedInput { get; set; }

        [JsonProperty("ActualInput")]
        public string ActualInput { get; set; }

        [JsonProperty("PlannedOutcome")]
        public string PlannedOutcome { get; set; }

        [JsonProperty("ActualOutcome")]
        public string ActualOutcome { get; set; }

        [JsonProperty("PlannedOutcomeTestSteps")]
        public string PlannedOutcomeTestSteps { get; set; }

        [JsonProperty("ActualOutcomeTestSteps")]
        public string ActualOutcomeTestSteps { get; set; }

        [JsonProperty("PlannedOutcomeRatio")]
        public string PlannedOutcomeRatio { get; set; }

        [JsonProperty("ActualOutcomeRatio")]
        public string ActualOutcomeRatio { get; set; }

        [JsonProperty("PlannedThroughput")]
        public string PlannedThroughput { get; set; }

        [JsonProperty("ActualThroughput")]
        public string ActualThroughput { get; set; }

        [JsonProperty("PlannedProductivity")]
        public string PlannedProductivity { get; set; }

        [JsonProperty("ActualProductivity")]
        public string ActualProductivity { get; set; }

        [JsonProperty("PlannedReviewRounds")]
        public string PlannedReviewRounds { get; set; }

        [JsonProperty("ActualReviewRounds")]
        public string ActualReviewRounds { get; set; }

        [JsonProperty("DefectRaised")]
        public string DefectRaised { get; set; }

        [JsonProperty("DefectRejected")]
        public string DefectRejected { get; set; }

        [JsonProperty("DefectDensity")]
        public string DefectDensity { get; set; }

        [JsonProperty("DefectRejectionRatio")]
        public string DefectRejectionRatio { get; set; }

        [JsonProperty("Headcount")]
        public string Headcount { get; set; }

        [JsonProperty("IdleTimeDuration")]
        public string IdleTimeDuration { get; set; }

        [JsonProperty("IdleTimeEffort")]
        public string IdleTimeEffort { get; set; }

        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        [JsonProperty("TargetCompletionDate")]
        public string TargetCompletionDate { get; set; }

        [JsonProperty("AccountManager")]
        public string AccountManager { get; set; }

        [JsonProperty("DeliveryManager")]
        public string DeliveryManager { get; set; }

        [JsonProperty("TestManager")]
        public string TestManager { get; set; }

        [JsonProperty("TeamLead")]
        public string TeamLead { get; set; }

        [JsonProperty("Vertical")]
        public string Vertical { get; set; }

        [JsonProperty("Practice")]
        public string Practice { get; set; }

        [JsonProperty("Modified")]
        public string Modified { get; set; }

        [JsonProperty("ModifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonProperty("Notes")]
        public string Notes { get; set; }

        [JsonProperty("WeekNumber")]
        public string WeekNumber { get; set; }


    }
}
