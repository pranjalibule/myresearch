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
    using System.Data.Entity;
    public partial class TTMContext : DbContext
    {
        #region  Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TTMContext()
            : base("name=TTMContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        #endregion

        public virtual DbSet<ClientRegion> ClientRegions { get; set; }
        public virtual DbSet<CoreService> CoreServices { get; set; }
        public virtual DbSet<MarketOffering> MarketOfferings { get; set; }
        public virtual DbSet<TaskStatus> TaskStatuses { get; set; }
        public virtual DbSet<OperationalRisk> OperationalRisks { get; set; }
        public virtual DbSet<OperationalRiskIndicator> OperationalRiskIndicators { get; set; }

        public virtual DbSet<TSOServiceDeliveryChainActualRisk> TSOServiceDeliveryChainActualRisks { get; set; }

       // public virtual DbSet<TSOServiceDeliveryChainPlannedRisk> TSOServiceDeliveryChainPlannedRiska { get; set; }

        public virtual DbSet<Practice> Practices { get; set; }
        public virtual DbSet<QGPassed> QGPasseds { get; set; }
        public virtual DbSet<RelevantRepository> RelevantRepositories { get; set; }
        public virtual DbSet<ServiceDeliveryChain> ServiceDeliveryChains { get; set; }
        public virtual DbSet<SolutionCentre> SolutionCentres { get; set; }
        public virtual DbSet<TSO> TSOes { get; set; }
        public virtual DbSet<TSOServiceDeliveryChain> TSOServiceDeliveryChains { get; set; }

        public virtual DbSet<TSOStatus> TSOStatuses { get; set; }
        public virtual DbSet<TSR> TSRs { get; set; }
        public virtual DbSet<TSRFileUpload> TSRFileUploads { get; set; }
        public virtual DbSet<TSRCoreService> TSRCoreServices { get; set; }
        public virtual DbSet<TSRStatus> TSRStatuses { get; set; }

        public virtual DbSet<TSRTMOUser> TSRTMOUsers { get; set; }

        public virtual DbSet<TSRRelevantRepository> TSTReleventRepositories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<Vertical> Verticals { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<ServiceDeliveryChainMethod> ServiceDeliveryChainMethods { get; set; }
        public virtual DbSet<Engagement> Engagements { get; set; }

        public virtual DbSet<ProjectModel> ProjectModel { get; set; }

        public virtual DbSet<PricingModel> PricingModel { get; set; }

        public virtual DbSet<ProductivityInput> ProductivityInputs { get; set; }
        public virtual DbSet<ProductivityOutcome> ProductivityOutcomes { get; set; }

        public virtual DbSet<TSOProductivityInput> TSOProductivityInputs { get; set; }
        public virtual DbSet<TSOProductivityInputActual> TSOProductivityInputActuals { get; set; }
        public virtual DbSet<TSOProductivityInputPlanned> TSOProductivityInputPlanneds { get; set; }
        public virtual DbSet<TSOProductivityOutcome> TSOProductivityOutcomes { get; set; }
        public virtual DbSet<TSOProductivityOutcomeActual> TSOProductivityOutcomeActuals { get; set; }
        public virtual DbSet<TSOProductivityOutcomePlanned> TSOProductivityOutcomePlanneds { get; set; }

        public virtual DbSet<RiskStatus> RiskStatuses { get; set; }


        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClientRegion>()
                .HasMany(e => e.TSRs)
                .WithRequired(e => e.ClientRegion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CoreService>()
                .HasMany(e => e.TSOes)
                .WithRequired(e => e.CoreService)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CoreService>()
                .HasMany(e => e.TSRCoreServices)
                .WithRequired(e => e.CoreService)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OperationalRisk>()
                .HasMany(e => e.TSOes)
                .WithRequired(e => e.OperationalRisk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OperationalRisk>()
                .HasMany(e => e.TSRs)
                .WithRequired(e => e.OperationalRisk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Practice>()
                .HasMany(e => e.TSRs)
                .WithRequired(e => e.Practice)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RelevantRepository>()
                .HasMany(e => e.TSOes)
                .WithRequired(e => e.RelevantRepository)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RelevantRepository>()
                .HasMany(e => e.TSTReleventRepositories)
                .WithRequired(e => e.RelevantRepository)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ServiceDeliveryChain>()
                .HasMany(e => e.TSOServiceDeliveryChains)
                .WithRequired(e => e.ServiceDeliveryChain)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ServiceDeliveryChain>()
                .HasMany(e => e.ServiceDeliveryChainMethods)
                .WithRequired(e => e.ServiceDeliveryChain)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SolutionCentre>()
                .HasMany(e => e.TSRs)
                .WithRequired(e => e.SolutionCentre)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOServiceDeliveryChain>()
                .HasMany(e => e.TSOServiceDeliveryChainActualTasks)
                .WithRequired(e => e.TSOServiceDeliveryChainTask)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOServiceDeliveryChain>()
               .HasMany(e => e.TSOServiceDeliveryChainPlannedTasks)
               .WithRequired(e => e.TSOServiceDeliveryChainTask)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSO>()
                .HasMany(e => e.TSOServiceDeliveryChains)
                .WithRequired(e => e.TSO)
                .WillCascadeOnDelete(true);


            modelBuilder.Entity<TSR>()
                .HasMany(e => e.TSOes)
                .WithRequired(e => e.TSR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSR>()
                .HasMany(e => e.TSRCoreServices)
                .WithRequired(e => e.TSR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSR>()
                .HasMany(e => e.TSRFiles)
                .WithRequired(e => e.TSR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSR>()
                .HasMany(e => e.TSRReleventRepositories)
                .WithRequired(e => e.TSR)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TSRDeliveryManager)
                .WithRequired(e => e.DeliveryManager)
                .HasForeignKey(e => e.DeliveryManagerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TSRAccountManager)
                .WithRequired(e => e.AccountManager)
                .HasForeignKey(e => e.AccountManagerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
               .HasMany(e => e.TSRTestManager)
               .WithRequired(e => e.TestManager)
               .HasForeignKey(e => e.TestManagerId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
               .HasMany(e => e.TSOTeamLead)
               .WithRequired(e => e.TeamLead)
               .HasForeignKey(e => e.TeamLeadId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSR>()
                .HasMany(e => e.TSRTMOUsers)
                .WithRequired(e => e.TSR)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<User>()
                .HasMany(e => e.TSRTMOUser)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<UserType>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UserType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductivityInput>()
                .HasMany(e => e.TSOProductivityInputs)
                .WithRequired(e => e.ProductivityInput)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductivityOutcome>()
                .HasMany(e => e.TSOProductivityOutcomes)
                .WithRequired(e => e.ProductivityOutcome)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOProductivityInput>()
                .HasMany(e => e.TSOProductivityInputActuals)
                .WithRequired(e => e.TSOProductivityInput)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOProductivityInput>()
                .HasMany(e => e.TSOProductivityInputPlanneds)
                .WithRequired(e => e.TSOProductivityInput)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOProductivityOutcome>()
                .HasMany(e => e.TSOProductivityOutcomeActuals)
                .WithRequired(e => e.TSOProductivityOutcome)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOProductivityOutcome>()
                .HasMany(e => e.TSOProductivityOutcomePlanneds)
                .WithRequired(e => e.TSOProductivityOutcome)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOServiceDeliveryChain>()
                .HasMany(e => e.TSOProductivityInputActuals)
                .WithRequired(e => e.TSOServiceDeliveryChainTask)
                .HasForeignKey(e => e.ChainID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOServiceDeliveryChain>()
                .HasMany(e => e.TSOProductivityInputPlanneds)
                .WithRequired(e => e.TSOServiceDeliveryChainTask)
                .HasForeignKey(e => e.ChainID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOServiceDeliveryChain>()
                .HasMany(e => e.TSOProductivityOutcomeActuals)
                .WithRequired(e => e.TSOServiceDeliveryChainTask)
                .HasForeignKey(e => e.ChainID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TSOServiceDeliveryChain>()
                .HasMany(e => e.TSOProductivityOutcomePlanneds)
                .WithRequired(e => e.TSOServiceDeliveryChainTask)
                .HasForeignKey(e => e.ChainID)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<OperationalRisk>()
            //   .HasMany(e => e.TSOServiceDeliveryChainTaskActuals)
            //   .WithRequired(e => e.ActualOperationalRisk)
            //   .WillCascadeOnDelete(false);

            //modelBuilder.Entity<OperationalRisk>()
            // .HasMany(e => e.TSOServiceDeliveryChainTaskPlanneds)
            // .WithRequired(e => e.PlannedOperationalRisk)
            // .WillCascadeOnDelete(false);           
        }
    }
}
