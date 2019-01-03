namespace SQS.nTier.TTM.DAL
{
    using GenericFramework;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("User")]
    public partial class User : IBaseEntity
    {
        public User()
        {
            TSRDeliveryManager = new HashSet<TSR>();
            TSRTestManager = new HashSet<TSR>();
            TSRAccountManager = new HashSet<TSR>();
            TSOTeamLead = new HashSet<TSO>();
            // Roles = new HashSet<Roles>();
            TSRTMOUser = new HashSet<TSRTMOUser>();
        }

        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? UserTypeID { get; set; }

        [Required]
        [StringLength(200)]
        public string EmailID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Password { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string ActivationKey { get; set; }

        [StringLength(255)]
        public string ProfilePicLocation { get; set; }

        public bool Activated { get; set; }

        public bool Locked { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int Version { get; set; }

        public virtual UserType UserType { get; set; }

        public virtual ICollection<TSR> TSRDeliveryManager { get; set; }

        public virtual ICollection<TSR> TSRTestManager { get; set; }

        public virtual ICollection<TSR> TSRAccountManager { get; set; }

        public virtual ICollection<TSO> TSOTeamLead { get; set; }

        public virtual ICollection<TSRTMOUser> TSRTMOUser { get; set; }

        // public virtual ICollection<Roles> Roles { get; set; }

        public int? RoleId { get; set; }

        public virtual Roles Role { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }
    }
}
