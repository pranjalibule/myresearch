﻿/******************************************************************************
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TSR")]
    public partial class TSR : IBaseEntity
    {
        public TSR()
        {
            TSOes = new HashSet<TSO>();
            TSRCoreServices = new HashSet<TSRCoreService>();          
            TSRReleventRepositories = new HashSet<TSRRelevantRepository>();
            TSRTMOUsers = new HashSet<TSRTMOUser>();
            TSRFiles = new HashSet<TSRFileUpload>();
        }

        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        public int DeliveryManagerId { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [Required]
        public int SP_Id { get; set; }


        [StringLength(4000)]
        public string ERPOrderDescription { get; set; }

        public int TestManagerId { get; set; }

        public int VerticalId { get; set; }

        public int PracticeId { get; set; }

        public int SolutionCentreId { get; set; }

        public int ClientRegionId { get; set; }

        public int ClientId { get; set; }

        [Required]
        [StringLength(500)]
        public string Account { get; set; }

        [Required]
        public int EngagementId { get; set; }
        [Required]
        public int PricingModelId { get; set; }

        public int AccountManagerId { get; set; }

        [Required]
        [StringLength(50)]
        public string ERPordernumber { get; set; }

        public int? MarketOfferingId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime TargetCompletionDate { get; set; }

        //public DateTime ActualStartDate { get; set; }
        //public DateTime ActualCompletionDate { get; set; }

        public double Estimatedeffort { get; set; }

        public double Plannedeffort { get; set; }

        public int OperationalRiskId { get; set; }

        public int TSRStatusID { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int Version { get; set; }

        public virtual PricingModel PricingModel { get; set; }

        public virtual Vertical Vertical { get; set; }

        public virtual ClientRegion ClientRegion { get; set; }

        public virtual MarketOffering MarketOffering { get; set; }

        public virtual OperationalRisk OperationalRisk { get; set; }

        public virtual TSRStatus TSRStatus { get; set; }

        public virtual Practice Practice { get; set; }

        public virtual SolutionCentre SolutionCentre { get; set; }

        public virtual ICollection<TSO> TSOes { get; set; }

        public virtual User DeliveryManager { get; set; }

        public virtual User TestManager { get; set; }

        public virtual User AccountManager { get; set; }

        public virtual Client Client { get; set; }

        public virtual Engagement Engagement { get; set; }

        public virtual ICollection<TSRCoreService> TSRCoreServices { get; set; }

        public virtual ICollection<TSRRelevantRepository> TSRReleventRepositories { get; set; }

        public virtual ICollection<TSRTMOUser> TSRTMOUsers { get; set; }


        public virtual ICollection<TSRFileUpload> TSRFiles { get; set; }

        [NotMapped]
        public string TSRCoreServicesArr { get; set; }

        [NotMapped]
        public string TSTRelevantRepositoriesArr { get; set; }

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }

        public int ProjectModelID { get; set; }

        public virtual ProjectModel ProjectModel { get; set; }

    }
}
