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

    [Table("TSOProductivityInput")]
    public partial class TSOProductivityInput : IBaseEntity
    {
        public TSOProductivityInput()
        {
            TSOProductivityInputActuals = new HashSet<TSOProductivityInputActual>();
            TSOProductivityInputPlanneds = new HashSet<TSOProductivityInputPlanned>();
        }

        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int TSOId { get; set; }

        [Required]
        public int ProductivityInputId { get; set; }

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

        public virtual ProductivityInput ProductivityInput { get; set; }

        public virtual ICollection<TSOProductivityInputActual> TSOProductivityInputActuals { get; set; }

        public virtual ICollection<TSOProductivityInputPlanned> TSOProductivityInputPlanneds { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}
