namespace SQS.nTier.TTM.DataImport
{
    internal class TableColumns
    {

        /// <summary>
        /// GetExcelColumns
        /// </summary>
        /// <param name="tableName">string</param>
        /// <returns>string</returns>
        public static string GetExcelColumns(string tableName)
        {
            string strTableColumns = string.Empty;

            if (tableName.ToLower() == "tsr")
            {
                strTableColumns = TSR.ExcelColumns;
            }
            else if (tableName.ToLower() == "tso")
            {
                strTableColumns = TSO.ExcelColumns;
            }
            else if (tableName.ToLower() == "tasks")
            {
                strTableColumns = Tasks.ExcelColumns;
            }

            return strTableColumns;
        }

        /// <summary>
        /// GetTableColumns
        /// </summary>
        /// <param name="tableName">string</param>
        /// <returns>string</returns>
        public static string GetTableColumns(string tableName)
        {
            string strTableColumns = string.Empty;

            if (tableName.ToLower() == "tsr")
            {
                strTableColumns = TSR.TableColumns;
            }
            else if (tableName.ToLower() == "tso")
            {
                strTableColumns = TSO.TableColumns;
            }
            else if (tableName.ToLower() == "tasks")
            {
                strTableColumns = Tasks.TableColumns;
            }

            return strTableColumns;
        }

        /// <summary>
        /// ProjectType
        /// </summary>
        public static class TSR
        {
            public static string ExcelColumns = @"[ID],[Title],[Client],[TSR Status],[Engagement Model],[Pricing Model],[Project Model],[Client Region],[Account],[ERP order number],[Solution centre],[Core Service (s)],[Estimated effort],[Planned effort],[Actual effort],[Operational risk],[Start Date],[Actual start date],[Target Completion Date],[Actual completion date],[Planned processing time],[Actual processing time],[Account Manager],[Delivery manager],[Test Manager],[Vertical],[Practice],[Market Offering],[Relevant repositories],[Modified],[Modified By],[Description],[Created By],[Created]";
            public static string TableColumns = @"[SP_Id],[Title],[ClientId],[TSRStatusID],[EngagementId],[PricingModelId],[ProjectModelID],[ClientRegionId],[Account],[ERPordernumber],[SolutionCentreId],[Estimatedeffort],[Plannedeffort],[OperationalRiskId],[StartDate],[ActualStartDate],[TargetCompletionDate],[ActualCompletionDate],[AccountManagerId],[DeliveryManagerId],[TestManagerId],[VerticalId],[PracticeId],[MarketOfferingId],[CreatedBy],[CreatedOn],[UpdatedBy],[UpdatedOn],[Version],[Description],[ERPOrderDescription]";
        }

        /// <summary>
        /// ProjView
        /// </summary>
        public static class TSO
        {
            public static string ExcelColumns = @"[ID],[Title],[TSO Status],[Related TSR],[Relevant repositories],[Service Delivery Chain],[Core Service],[Operational risk],[Start Date],[Actual start date],[Target Completion Date],[Actual completion date],[Estimated effort],[Planned effort],[Team Lead],[Modified],[Modified By],[Description],[Created By],[Created]";
            public static string TableColumns = @"";
        }

        /// <summary>
        /// ResView
        /// </summary>
        public static class Tasks
        {
            public static string ExcelColumns = @"[ID],[Task Status],[Related TSO],[Actual Effort],[Start Date],[Target Completion Date],[Planned effort],[Actual Start Date],[Actual Completion Date],[Notes],[Planned Processing time],[Actual Processing Time],[Planned Input],[Actual Input],[Planned Outcome],[Actual Outcome],[Planned Outcome (Test Steps)], [Actual Outcome (Test Steps)],[Method],[Defects Raised],[Defects Rejected],[Headcount],[Idle Time (Duration)],[Idle Time (Effort)],[Title], [Planned Review Rounds],[Actual Review Rounds],[Created By],[Created],[Modified],[Modified By],[Week Number]"; //29
            public static string TableColumns = @"";
        }
    }
}
