namespace SQS.nTier.TTM.DAL
{
    using GenericFramework;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("Roles_User")]
    public partial class Roles_User : IBaseEntity
    {
        public Roles_User()
        {
          //  Users = new HashSet<User>();
        }

        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int Version { get; set; }       

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}

