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

    [Table("TSOServiceDeliveryChainTaskPlanned")]
    public class TSOServiceDeliveryChainTaskPlanned : IBaseEntity
    {
        public TSOServiceDeliveryChainTaskPlanned()
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
        public double PlannedEffort { get; set; }

        [Required]
        public double PlannedProductivity { get; set; }

        [Required]
        public int PlannedOutcome { get; set; }

        [Required]
        public int PlannedReviewRounds { get; set; }

        [Required]
        public int PlannedInput { get; set; }

        [Required]
        public double PlannedProcessingTime { get; set; }

        [Required]
        public int PlannedOutcomeTestSteps { get; set; }

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

        //public int? PlannedOperationalRiskId { get; set; }

        //public virtual OperationalRisk PlannedOperationalRisk { get; set; }

        //public int? PlannedOperationalRiskIndicatorId { get; set; }

        //public virtual OperationalRiskIndicator PlannedOperationalRiskIndicator { get; set; }

        //[StringLength(500)]
        //public string PlannedOperationalRiskDescription { get; set; }

        //[StringLength(500)]
        //public string PlannedOperationalRiskMitigation { get; set; }

       

        [Required]
        public int TSOServiceDeliveryChainTaskId { get; set; }

        public virtual TSOServiceDeliveryChain TSOServiceDeliveryChainTask { get; set; }

      
    }
}
