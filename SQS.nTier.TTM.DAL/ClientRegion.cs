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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ClientRegion")]
    public partial class ClientRegion : IBaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClientRegion()
        {
            TSRs = new HashSet<TSR>();
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

        public virtual ICollection<TSR> TSRs { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}
