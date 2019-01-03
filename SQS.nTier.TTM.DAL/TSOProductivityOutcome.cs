/******************************************************************************
 *                          © 2017 SQS India                            *
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TSOProductivityOutcome")]
    public partial class TSOProductivityOutcome : IBaseEntity
    {
        public TSOProductivityOutcome()
        {
            TSOProductivityOutcomeActuals = new HashSet<TSOProductivityOutcomeActual>();
            TSOProductivityOutcomePlanneds = new HashSet<TSOProductivityOutcomePlanned>();
        }

        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int TSOId { get; set; }

        [Required]
        public int ProductivityOutcomeId { get; set; }

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

        public virtual TSO TSO { get; set; }

        public virtual ProductivityOutcome ProductivityOutcome { get; set; }

        public virtual ICollection<TSOProductivityOutcomeActual> TSOProductivityOutcomeActuals { get; set; }

        public virtual ICollection<TSOProductivityOutcomePlanned> TSOProductivityOutcomePlanneds { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}
