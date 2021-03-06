/******************************************************************************
 *                          � 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 18Sep2017 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.DAL
{
    using GenericFramework;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TSOProductivityOutcomePlanned")]
    public partial class TSOProductivityOutcomePlanned : IBaseEntity
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int TSOProductivityOutcomeId { get; set; }

        [Required]
        public int WeekNumber { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public double? OutcomeValue { get; set; }

        [Required]
        public int ChainID { get; set; }

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


        public virtual TSOProductivityOutcome TSOProductivityOutcome { get; set; }

        public virtual TSOServiceDeliveryChain TSOServiceDeliveryChainTask { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}
