﻿/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 02Nov2017 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.DAL
{
    using GenericFramework;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TSOServiceDeliveryChainTask")]
    public class TSOServiceDeliveryChainTask : IBaseEntity
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int TSOServiceDeliveryChainId { get; set; }

        public int? ServiceDeliveryChainMethodId { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime PlannedStartDate { get; set; }

        public DateTime PlannedCompletionDate { get; set; }

        public double PlannedEffort { get; set; }

        //public DateTime ActualStartDate { get; set; }

        //public DateTime? ActualCompletionDate { get; set; }

        public double? ActualEffort { get; set; }

        public int WeekNumber { get; set; }

        public double PlannedProductivity { get; set; }

        public double ActualProductivity { get; set; }

        public int PlannedOutcome { get; set; }

        public int PlannedOutcomeTestSteps { get; set; }

        public int ActualOutcome { get; set; }

        public int ActualOutcomeTestSteps { get; set; }

        public int PlannedReviewRounds { get; set; }

        public int ActualReviewRounds { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int Version { get; set; }      

        public virtual TSOServiceDeliveryChain TSOServiceDeliveryChain { get; set; }

        public virtual ServiceDeliveryChainMethod ServiceDeliveryChainMethod { get; set; }

        public virtual TaskStatus TaskStatus { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }

        public int DefectRaised { get; set; }

        public int DefectRejected { get; set; }

        public double PlannedProcessingTime { get; set; }

        public double ActualProcessingTime { get; set; }

        public int PlannedInput { get; set; }

        public int ActualInput { get; set; }

        public int TaskStatusId { get; set; }
        public double IdleTimeEffort { get; set; }
        public double IdleTimeDuration { get; set; }
        public double Headcount { get; set; }

    }
}
