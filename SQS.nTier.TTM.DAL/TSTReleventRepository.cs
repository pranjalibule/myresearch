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
    using SQS.nTier.TTM.GenericFramework;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TSRRelevantRepositories")]
    public partial class TSRRelevantRepository : IBaseEntity
    {
        public int ID { get; set; }

        public int TSRId { get; set; }

        public int RelevantRepositoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int Version { get; set; }

        public virtual RelevantRepository RelevantRepository { get; set; }

        public virtual TSR TSR { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}
