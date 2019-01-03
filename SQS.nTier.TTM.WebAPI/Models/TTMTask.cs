﻿using System;

namespace SQS.nTier.TTM.WebAPI.Models
{

    public class TTMTask
    {
        public int WeekNumber { get; set; }
        public int Task_ID { get; set; }
        public string Task_Title { get; set; }
        public string Task_Description { get; set; }
        public string Task_Notes { get; set; }
        public int Related_TSR { get; set; }
        public string TSR_Name { get; set; }
        public int Related_TSO { get; set; }
        public string TSO_Title { get; set; }
        public string TaskStatus { get; set; }
        public string Client_Name { get; set; }
        public string Engagement_Name { get; set; }
        public string PricingModel_Name { get; set; }
        public string ProjectModel_Name { get; set; }
        public string ClientRegion_Name { get; set; }
        public string Account_Name { get; set; }
        public string ERPordernumber { get; set; }
        public string SolutionCenter_Name { get; set; }
        public string CoreServiceName { get; set; }
        public string Method { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedCompletionDate { get; set; }
        public float ActualEffort { get; set; }
        public float PlannedEffort { get; set; }
        public int ActualInput { get; set; }
        public int PlannedInput { get; set; }
        public int ActualOutcome { get; set; }
        public int PlannedOutcome { get; set; }
        public int ActualOutcomeTestSteps { get; set; }
        public int PlannedOutcomeTestSteps { get; set; }
        public float ActualProcessingTime { get; set; }
        public float PlannedProcessingTime { get; set; }
        public float ActualProductivity { get; set; }
        public float PlannedProductivity { get; set; }
        public float ActualOutcomeRatio { get; set; }
        public float PlannedOutcomeRatio { get; set; }
        public float ActualThroughput { get; set; }
        public float PlannedThroughput { get; set; }
        public int ActualReviewRounds { get; set; }
        public int PlannedReviewRounds { get; set; }
        public int DefectRaised { get; set; }
        public int DefectRejected { get; set; }
        public float DefectDensity { get; set; }
        public float DefectRejectionRatio { get; set; }
        public float Headcount { get; set; }
        public float IdleTimeEffort { get; set; }
        public float IdleTimeDuration { get; set; }
        public string AccountManager_Name { get; set; }
        public string DeliveryManager_Name { get; set; }
        public string TestManager_Name { get; set; }
        public string TeamLead_Name { get; set; }
        public string Vertical_Name { get; set; }
        public string Practice_Name { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string TSR_Status { get; set; }
        public string TSO_Status { get; set; }
        public int TSO_OperationalRisk { get; set; }
        public int TSR_OperationalRisk { get; set; }
    }
}