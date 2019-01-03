/******************************************************************************
 *                          © 2018 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * Pranjali Mankar 12Sept2018 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.DAL
{
    using GenericFramework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OperationalRiskIndicator")]
    public partial class OperationalRiskIndicator : IBaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OperationalRiskIndicator()
        {
            //TSOServiceDeliveryChainTaskActuals = new HashSet<TSOServiceDeliveryChainTaskActual>();
            //TSOServiceDeliveryChainTaskPlanneds = new HashSet<TSOServiceDeliveryChainTaskPlanned>();
            TSOServiceDeliveryChainActualRisks = new HashSet<TSOServiceDeliveryChainActualRisk>();
           // TSOServiceDeliveryChainPlannedRisks = new HashSet<TSOServiceDeliveryChainPlannedRisk>();
        }

        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int Version { get; set; }

        //public virtual ICollection<TSOServiceDeliveryChainTaskActual> TSOServiceDeliveryChainTaskActuals { get; set; }

        //public virtual ICollection<TSOServiceDeliveryChainTaskPlanned> TSOServiceDeliveryChainTaskPlanneds { get; set; }

        public virtual ICollection<TSOServiceDeliveryChainActualRisk> TSOServiceDeliveryChainActualRisks { get; set; }
        //public virtual ICollection<TSOServiceDeliveryChainPlannedRisk> TSOServiceDeliveryChainPlannedRisks { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}
