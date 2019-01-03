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
namespace SQS.nTier.TTM.DAL
{
    using GenericFramework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TSOServiceDeliveryChainTaskActual")]
    public class TSOServiceDeliveryChainTaskActual : IBaseEntity
    {

        public TSOServiceDeliveryChainTaskActual()
        {
            
        }
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int WeekNumber { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public double ActualEffort { get; set; }

        [Required]
        public double ActualProductivity { get; set; }

        [Required]
        public int ActualOutcome { get; set; }

        [Required]
        public int ActualReviewRounds { get; set; }

        [Required]
        public int DefectRaised { get; set; }

        [Required]
        public int DefectRejected { get; set; }

        [Required]
        public int ActualInput { get; set; }

        [Required]
        public double ActualProcessingTime { get; set; }

        [Required]
        public double IdleTimeEffort { get; set; }

        [Required]
        public double IdleTimeDuration { get; set; }

        [Required]
        public double Headcount { get; set; }

        [Required]
        public int ActualOutcomeTestSteps { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        [Required]
        public DateTime UpdatedOn { get; set; }

        [Required]
        public int Version { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }


        //public int? ActualOperationalRiskId { get; set; }

        //public virtual OperationalRisk ActualOperationalRisk { get; set; }

        //public int? ActualOperationalRiskIndicatorId { get; set; }

        //public virtual OperationalRiskIndicator ActualOperationalRiskIndicator { get; set; }

        //[StringLength(500)]
        //public string ActualOperationalRiskDescription { get; set; }

        //[StringLength(500)]
        //public string ActualOperationalRiskMitigation { get; set; }


       

        [Required]
        public int TSOServiceDeliveryChainTaskId { get; set; }

        public virtual TSOServiceDeliveryChain TSOServiceDeliveryChainTask { get; set; }
    }
}
