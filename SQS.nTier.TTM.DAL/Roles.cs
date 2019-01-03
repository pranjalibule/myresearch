namespace SQS.nTier.TTM.DAL
{
    using GenericFramework;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("Roles")]
    public partial class Roles : IBaseEntity
    {
        public Roles()
        {
           Users = new HashSet<User>();          
        }

        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
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

        public virtual ICollection<User> Users { get; set; }      

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}

