/******************************************************************************
 *                          © 2017 SQS India                                  *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 18Sep2017 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.BAL
{
    using DAL;
    using GenericFramework;
    using System;

    /// <summary>
    /// IBusinessLayer
    /// </summary>
    public interface IBusinessLayer : IDisposable
    {
        /// <summary>
        /// ClientRegionRepository
        /// </summary>
        IGenericDataRepository<ClientRegion> ClientRegionRepository { get; }
        /// <summary>
        /// MethodRepository
        /// </summary>
        IGenericDataRepository<ServiceDeliveryChainMethod> MethodRepository { get; }

        IGenericDataRepository<OperationalRiskIndicator> OperationalRiskIndicatorRepository { get; }

        /// <summary>
        /// PricingModelRepository
        /// </summary>
        IGenericDataRepository<PricingModel> PricingModelRepository { get; }

        IGenericDataRepository<TSOServiceDeliveryChainActualRisk> TSOServiceDeliveryChainActualRiskRepository { get; }

        //IGenericDataRepository<TSOServiceDeliveryChainPlannedRisk> TSOServiceDeliveryChainPlannedRiskRepository { get; }

        /// <summary>
        /// CoreServiceRepository
        /// </summary>
        IGenericDataRepository<CoreService> CoreServiceRepository { get; }

        /// <summary>
        /// MarketOfferingRepository
        /// </summary>
        IGenericDataRepository<MarketOffering> MarketOfferingRepository { get; }

        /// <summary>
        /// TaskStatusRepository
        /// </summary>
        IGenericDataRepository<TaskStatus> TaskStatusRepository { get; }

        /// <summary>
        /// TSRFileUploadRepository
        /// </summary>
        IGenericDataRepository<TSRFileUpload> TSRFileUploadRepository { get; }

        /// <summary>
        /// OperationalRiskRepository
        /// </summary>
        IGenericDataRepository<OperationalRisk> OperationalRiskRepository { get; }

        /// <summary>
        /// PracticeRepository
        /// </summary>
        IGenericDataRepository<Practice> PracticeRepository { get; }

        /// <summary>
        /// QGPassedRepository
        /// </summary>
        IGenericDataRepository<QGPassed> QGPassedRepository { get; }

        /// <summary>
        /// RelevantRepositoryRepository
        /// </summary>
        IGenericDataRepository<RelevantRepository> RelevantRepositoryRepository { get; }

        /// <summary>
        /// ServiceDeliveryChainRepository
        /// </summary>
        IGenericDataRepository<ServiceDeliveryChain> ServiceDeliveryChainRepository { get; }

        /// <summary>
        /// SolutionCentreRepository
        /// </summary>
        IGenericDataRepository<SolutionCentre> SolutionCentreRepository { get; }

        /// <summary>
        /// TSOStatusRepository
        /// </summary>
        IGenericDataRepository<TSOStatus> TSOStatusRepository { get; }

        /// <summary>
        /// TSRStatusRepository
        /// </summary>
        IGenericDataRepository<TSRStatus> TSRStatusRepository { get; }

        /// <summary>
        /// TSRStatusRepository
        /// </summary>
        IGenericDataRepository<ProjectModel> ProjectModelRepository { get; }

        /// <summary>
        /// ClientRepository
        /// </summary>
        IGenericDataRepository<Client> ClientRepository { get; }

        /// <summary>
        /// ProductivityOutcomeRepository
        /// </summary>
        IGenericDataRepository<ProductivityOutcome> ProductivityOutcomeRepository { get; }

        /// <summary>
        /// ProductivityInputRepository
        /// </summary>
        IGenericDataRepository<ProductivityInput> ProductivityInputRepository { get; }

        /// <summary>
        /// VerticalRepository
        /// </summary>
        IGenericDataRepository<Vertical> VerticalRepository { get; }

        /// <summary>
        /// UserTypeRepository
        /// </summary>
        IGenericDataRepository<UserType> UserTypeRepository { get; }

        /// <summary>
        /// UserRepository
        /// </summary>
        UserRepository UserRepository { get; }

        /// <summary>
        /// TSRCoreServicesRepository
        /// </summary>
        IGenericDataRepository<TSRCoreService> TSRCoreServicesRepository { get; }

        /// <summary>
        /// TSRTMOUserRepository
        /// </summary>
        IGenericDataRepository<TSRTMOUser> TSRTMOUserRepository { get; }

        /// <summary>
        /// TSRReleventRepositoriesRepository
        /// </summary>
        IGenericDataRepository<TSRRelevantRepository> TSRRelevantRepositoriesRepository { get; }

        /// <summary>
        /// TSRRepository
        /// </summary>
        IGenericDataRepository<TSR> TSRRepository { get; }

        /// <summary>
        /// TSORepository
        /// </summary>
        IGenericDataRepository<TSO> TSORepository { get; }

        /// <summary>
        /// TSOServiceDeliveryChainRepository
        /// </summary>
        IGenericDataRepository<TSOServiceDeliveryChain> TSOServiceDeliveryChainRepository { get; }

        /// <summary>
        /// TSOServiceDeliveryChainTaskActualRepository
        /// </summary>
        IGenericDataRepository<TSOServiceDeliveryChainTaskActual> TSOServiceDeliveryChainTaskActualRepository { get; }

        /// <summary>
        /// TSOServiceDeliveryChainTaskPlannedRepository
        /// </summary>
        IGenericDataRepository<TSOServiceDeliveryChainTaskPlanned> TSOServiceDeliveryChainTaskPlannedRepository { get; }

        IGenericDataRepository<Engagement> EngagementRepository { get; }

        /// <summary>
        /// RoleRepository
        /// </summary>
        IGenericDataRepository<Roles> RoleRepository { get; }


        /// <summary>
        /// TSOProductivityInputRepository
        /// </summary>
        IGenericDataRepository<TSOProductivityInput> TSOProductivityInputRepository { get; }

        /// <summary>
        /// TSOProductivityOutcomeRepository
        /// </summary>
        IGenericDataRepository<TSOProductivityOutcome> TSOProductivityOutcomeRepository { get; }

        /// <summary>
        /// TSOProductivityInputRepository
        /// </summary>
        IGenericDataRepository<TSOProductivityInputActual> TSOProductivityInputActualRepository { get; }

        /// <summary>
        /// TSOProductivityOutcomeRepository
        /// </summary>
        IGenericDataRepository<TSOProductivityOutcomeActual> TSOProductivityOutcomeActualRepository { get; }

        /// <summary>
        /// TSOProductivityInputRepository
        /// </summary>
        IGenericDataRepository<TSOProductivityInputPlanned> TSOProductivityInputPlannedRepository { get; }

        /// <summary>
        /// TSOProductivityOutcomeRepository
        /// </summary>
        IGenericDataRepository<TSOProductivityOutcomePlanned> TSOProductivityOutcomePlannedRepository { get; }

        /// <summary>
        /// RiskStatusRepository
        /// </summary>
        IGenericDataRepository<RiskStatus> RiskStatusRepository { get; }

    }
}
