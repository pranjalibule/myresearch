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

namespace SQS.nTier.TTM.BAL
{
    using DAL;
    using GenericFramework;
    using System;
    public class BusinessLayer : IBusinessLayer, IDisposable
    {
        #region private variables

        private readonly IUnitOfWork uow;
        private readonly TTMContext ttmContext;

        private IGenericDataRepository<ClientRegion> clientRegionRepository = null;
        private IGenericDataRepository<CoreService> coreServiceRepository = null;
        private IGenericDataRepository<MarketOffering> marketOfferingRepository = null;
        private IGenericDataRepository<TSRFileUpload> tsrFileUploadRepository = null;
        private IGenericDataRepository<OperationalRisk> operationalRiskRepository = null;//
        private IGenericDataRepository<OperationalRiskIndicator> operationalRiskIndicatorRepository = null;//
        private IGenericDataRepository<Practice> practiceRepository = null;
        private IGenericDataRepository<QGPassed> qgPassedRepository = null;
        private IGenericDataRepository<RelevantRepository> relevantRepositoryRepository = null;
        private IGenericDataRepository<ServiceDeliveryChain> serviceDeliveryChainRepository = null;
        private IGenericDataRepository<SolutionCentre> solutionCentreRepository = null;
        private IGenericDataRepository<TSOStatus> tsoStatusRepository = null;
        private IGenericDataRepository<TSRStatus> tsrStatusRepository = null;
        private IGenericDataRepository<Vertical> verticalReposiory = null;
        private IGenericDataRepository<Roles> roleRepository = null;
        private IGenericDataRepository<Engagement> engagementRepository = null;
        private IGenericDataRepository<ServiceDeliveryChainMethod> methodRepository = null;
        private IGenericDataRepository<PricingModel> pricingModelRepository = null;
        private IGenericDataRepository<UserType> userTypeRepository = null;
        private UserRepository userRepository = null;

        private IGenericDataRepository<TSRCoreService> tsrCoreServicesRepository = null;
        private IGenericDataRepository<TSRTMOUser> tsrTMUSerRepository = null;
        private IGenericDataRepository<TSRRelevantRepository> tsrReleventRepositoriesRepository = null;

        private IGenericDataRepository<TSR> tsrRepository = null;
        private IGenericDataRepository<TSO> tsoRepository = null;
        private IGenericDataRepository<TSOServiceDeliveryChain> tsoServiceDeliveryChainRepository = null;
        private IGenericDataRepository<TSOProductivityInput> tsoProductivityInputRepository = null;
        private IGenericDataRepository<TSOProductivityInputActual> tsoProductivityInputActualRepository = null;
        private IGenericDataRepository<TSOProductivityInputPlanned> tsoProductivityInputPlannedRepository = null;
        private IGenericDataRepository<TSOProductivityOutcome> tsoProductivityOutcomeRepository = null;
        private IGenericDataRepository<TSOProductivityOutcomeActual> tsoProductivityOutcomeActualRepository = null;
        private IGenericDataRepository<TSOProductivityOutcomePlanned> tsoProductivityOutcomePlannedRepository = null;
        //private IGenericDataRepository<TSOServiceDeliveryChainTask> tsoServiceDeliveryChainTaskRepository = null;

        private IGenericDataRepository<TSOServiceDeliveryChainTaskActual> tsoServiceDeliveryChainTaskActualRepository = null;
        private IGenericDataRepository<TSOServiceDeliveryChainTaskPlanned> tsoServiceDeliveryChainTaskPlannedRepository = null;

        private IGenericDataRepository<TSOServiceDeliveryChainActualRisk> tsoServiceDeliveryChainActualRiskRepository = null;
        //private IGenericDataRepository<TSOServiceDeliveryChainPlannedRisk> tsoServiceDeliveryChainPlannedRiskRepository = null;

        private IGenericDataRepository<ProjectModel> projectModelRepository = null;
        private IGenericDataRepository<TaskStatus> taskstatusRepository = null;
        private IGenericDataRepository<Client> clientRepository = null;

        private IGenericDataRepository<ProductivityInput> productivityinputRepository = null;
        private IGenericDataRepository<ProductivityOutcome> productivityoutcomeRepository = null;

        private IGenericDataRepository<RiskStatus> riskStatusRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BusinessLayer(LoginSession loginSession)
        {
            ttmContext = new TTMContext();

            ttmContext.Configuration.ProxyCreationEnabled = false;

            uow = new UnitOfWork(ttmContext, loginSession);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// ClientRegion Repository
        /// </summary>
        public IGenericDataRepository<ClientRegion> ClientRegionRepository
        {
            get
            {
                if (null == clientRegionRepository)
                    clientRegionRepository = uow.GetRepository<ClientRegion>();
                return clientRegionRepository;
            }
        }

        public IGenericDataRepository<ServiceDeliveryChainMethod> MethodRepository
        {
            get
            {
                if (null == clientRegionRepository)
                    methodRepository = uow.GetRepository<ServiceDeliveryChainMethod>();
                return methodRepository;
            }
        }

        /// <summary>
        /// CoreService Repository
        /// </summary>
        public IGenericDataRepository<CoreService> CoreServiceRepository
        {
            get
            {
                if (null == coreServiceRepository)
                    coreServiceRepository = uow.GetRepository<CoreService>();
                return coreServiceRepository;
            }
        }

        /// <summary>
        /// MarketOffering Repository
        /// </summary>
        public IGenericDataRepository<MarketOffering> MarketOfferingRepository
        {
            get
            {
                if (null == marketOfferingRepository)
                    marketOfferingRepository = uow.GetRepository<MarketOffering>();
                return marketOfferingRepository;
            }
        }


        /// <summary>
        /// MarketOffering Repository
        /// </summary>
        public IGenericDataRepository<TaskStatus> TaskStatusRepository
        {
            get
            {
                if (null == taskstatusRepository)
                    taskstatusRepository = uow.GetRepository<TaskStatus>();
                return taskstatusRepository;
            }
        }

        /// <summary>
        /// TSR File Upload Repository
        /// </summary>
        public IGenericDataRepository<TSRFileUpload> TSRFileUploadRepository
        {
            get
            {
                if (null == tsrFileUploadRepository)
                    tsrFileUploadRepository = uow.GetRepository<TSRFileUpload>();
                return tsrFileUploadRepository;
            }
        }

        /// <summary>
        /// OperationalRisk Repository
        /// </summary>
        public IGenericDataRepository<OperationalRisk> OperationalRiskRepository
        {
            get
            {
                if (null == operationalRiskRepository)
                    operationalRiskRepository = uow.GetRepository<OperationalRisk>();
                return operationalRiskRepository;
            }
        }

        /// <summary>
        /// OperationalRisk Repository
        /// </summary>
        public IGenericDataRepository<OperationalRiskIndicator> OperationalRiskIndicatorRepository
        {
            get
            {
                if (null == operationalRiskIndicatorRepository)
                    operationalRiskIndicatorRepository = uow.GetRepository<OperationalRiskIndicator>();
                return operationalRiskIndicatorRepository;
            }
        }

        /// <summary>
        /// Practice Repository
        /// </summary>
        public IGenericDataRepository<Practice> PracticeRepository
        {
            get
            {
                if (null == practiceRepository)
                    practiceRepository = uow.GetRepository<Practice>();
                return practiceRepository;
            }
        }

        /// <summary>
        /// QGPassed Repository
        /// </summary>
        public IGenericDataRepository<QGPassed> QGPassedRepository
        {
            get
            {
                if (null == qgPassedRepository)
                    qgPassedRepository = uow.GetRepository<QGPassed>();
                return qgPassedRepository;
            }
        }

        /// <summary>
        /// RelevantRepository Repository
        /// </summary>
        public IGenericDataRepository<RelevantRepository> RelevantRepositoryRepository
        {
            get
            {
                if (null == relevantRepositoryRepository)
                    relevantRepositoryRepository = uow.GetRepository<RelevantRepository>();
                return relevantRepositoryRepository;
            }
        }

        /// <summary>
        /// ServiceDeliveryChain Repository
        /// </summary>
        public IGenericDataRepository<ServiceDeliveryChain> ServiceDeliveryChainRepository
        {
            get
            {
                if (null == serviceDeliveryChainRepository)
                    serviceDeliveryChainRepository = uow.GetRepository<ServiceDeliveryChain>();
                return serviceDeliveryChainRepository;
            }
        }

        /// <summary>
        /// SolutionCentre Repository
        /// </summary>
        public IGenericDataRepository<SolutionCentre> SolutionCentreRepository
        {
            get
            {
                if (null == solutionCentreRepository)
                    solutionCentreRepository = uow.GetRepository<SolutionCentre>();
                return solutionCentreRepository;
            }
        }

        /// <summary>
        /// TSOStatus Repository
        /// </summary>
        public IGenericDataRepository<TSOStatus> TSOStatusRepository
        {
            get
            {
                if (null == tsoStatusRepository)
                    tsoStatusRepository = uow.GetRepository<TSOStatus>();
                return tsoStatusRepository;
            }
        }

        /// <summary>
        /// TSRStatus Repository
        /// </summary>
        public IGenericDataRepository<TSRStatus> TSRStatusRepository
        {
            get
            {
                if (null == tsrStatusRepository)
                    tsrStatusRepository = uow.GetRepository<TSRStatus>();
                return tsrStatusRepository;
            }
        }

        /// <summary>
        /// Project Model Repository
        /// </summary>
        public IGenericDataRepository<ProjectModel> ProjectModelRepository
        {
            get
            {
                if (null == projectModelRepository)
                    projectModelRepository = uow.GetRepository<ProjectModel>();
                return projectModelRepository;
            }
        }

        /// <summary>
        /// Project Model Repository
        /// </summary>
        public IGenericDataRepository<Client> ClientRepository
        {
            get
            {
                if (null == clientRepository)
                    clientRepository = uow.GetRepository<Client>();
                return clientRepository;
            }
        }

        /// <summary>
        /// ProductivityOutcome Repository
        /// </summary>
        public IGenericDataRepository<ProductivityOutcome> ProductivityOutcomeRepository
        {
            get
            {
                if (null == productivityoutcomeRepository)
                    productivityoutcomeRepository = uow.GetRepository<ProductivityOutcome>();
                return productivityoutcomeRepository;
            }
        }


        /// <summary>
        /// ProductivityInput Repository
        /// </summary>
        public IGenericDataRepository<ProductivityInput> ProductivityInputRepository
        {
            get
            {
                if (null == productivityinputRepository)
                    productivityinputRepository = uow.GetRepository<ProductivityInput>();
                return productivityinputRepository;
            }
        }

        /// <summary>
        /// Vertical Repository
        /// </summary>
        public IGenericDataRepository<Vertical> VerticalRepository
        {
            get
            {
                if (null == verticalReposiory)
                    verticalReposiory = uow.GetRepository<Vertical>();
                return verticalReposiory;
            }
        }

        /// <summary>
        /// UserType Repository
        /// </summary>
        public IGenericDataRepository<UserType> UserTypeRepository
        {
            get
            {
                if (null == userTypeRepository)
                    userTypeRepository = uow.GetRepository<UserType>();
                return userTypeRepository;
            }
        }

        /// <summary>
        /// User Repository
        /// </summary>
        public UserRepository UserRepository
        {
            get
            {
                if (null == userRepository)
                    userRepository = new UserRepository(ttmContext, uow);
                return userRepository;
            }
        }

        /// <summary>
        /// TSRRepository
        /// </summary>
        public IGenericDataRepository<TSR> TSRRepository
        {
            get
            {
                if (null == tsrRepository)
                    tsrRepository = uow.GetRepository<TSR>();

                return tsrRepository;
            }
        }

        /// <summary>
        /// TSRCoreServicesRepository
        /// </summary>
        public IGenericDataRepository<TSRCoreService> TSRCoreServicesRepository
        {
            get
            {
                if (null == tsrCoreServicesRepository)
                    tsrCoreServicesRepository = uow.GetRepository<TSRCoreService>();
                return tsrCoreServicesRepository;
            }
        }

        /// <summary>
        /// TSRCoreServicesRepository
        /// </summary>
        public IGenericDataRepository<TSRTMOUser> TSRTMOUserRepository
        {
            get
            {
                if (null == tsrTMUSerRepository)
                    tsrTMUSerRepository = uow.GetRepository<TSRTMOUser>();
                return tsrTMUSerRepository;
            }
        }

        /// <summary>
        /// TSRReleventRepositoriesRepository
        /// </summary>
        public IGenericDataRepository<TSRRelevantRepository> TSRRelevantRepositoriesRepository
        {
            get
            {
                if (null == tsrReleventRepositoriesRepository)
                    tsrReleventRepositoriesRepository = uow.GetRepository<TSRRelevantRepository>();
                return tsrReleventRepositoriesRepository;
            }
        }

        /// <summary>
        /// TSORepository
        /// </summary>
        public IGenericDataRepository<TSO> TSORepository
        {
            get
            {
                if (null == tsoRepository)
                    tsoRepository = uow.GetRepository<TSO>();

                return tsoRepository;
            }
        }

        /// <summary>
        /// TSOServiceDeliveryChainRepository
        /// </summary>
        public IGenericDataRepository<TSOServiceDeliveryChain> TSOServiceDeliveryChainRepository
        {
            get
            {
                if (null == tsoServiceDeliveryChainRepository)
                    tsoServiceDeliveryChainRepository = uow.GetRepository<TSOServiceDeliveryChain>();

                return tsoServiceDeliveryChainRepository;
            }
        }

        /// <summary>
        /// TSOProductivityInput
        /// </summary>
        public IGenericDataRepository<TSOProductivityInput> TSOProductivityInputRepository
        {
            get
            {
                if (null == tsoProductivityInputRepository)
                    tsoProductivityInputRepository = uow.GetRepository<TSOProductivityInput>();

                return tsoProductivityInputRepository;
            }
        }

        /// <summary>
        /// TSOProductivityInput Actual
        /// </summary>
        public IGenericDataRepository<TSOProductivityInputActual> TSOProductivityInputActualRepository
        {
            get
            {
                if (null == tsoProductivityInputActualRepository)
                    tsoProductivityInputActualRepository = uow.GetRepository<TSOProductivityInputActual>();

                return tsoProductivityInputActualRepository;
            }
        }

        /// <summary>
        /// TSOProductivityInput Planned
        /// </summary>
        public IGenericDataRepository<TSOProductivityInputPlanned> TSOProductivityInputPlannedRepository
        {
            get
            {
                if (null == tsoProductivityInputPlannedRepository)
                    tsoProductivityInputPlannedRepository = uow.GetRepository<TSOProductivityInputPlanned>();

                return tsoProductivityInputPlannedRepository;
            }
        }

        /// <summary>
        /// TSOProductivityInput
        /// </summary>
        public IGenericDataRepository<TSOProductivityOutcome> TSOProductivityOutcomeRepository
        {
            get
            {
                if (null == tsoProductivityOutcomeRepository)
                    tsoProductivityOutcomeRepository = uow.GetRepository<TSOProductivityOutcome>();

                return tsoProductivityOutcomeRepository;
            }
        }

        /// <summary>
        /// TSOProductivityInput Actual
        /// </summary>
        public IGenericDataRepository<TSOProductivityOutcomeActual> TSOProductivityOutcomeActualRepository
        {
            get
            {
                if (null == tsoProductivityOutcomeActualRepository)
                    tsoProductivityOutcomeActualRepository = uow.GetRepository<TSOProductivityOutcomeActual>();

                return tsoProductivityOutcomeActualRepository;
            }
        }

        /// <summary>
        /// TSOProductivityInput Planned
        /// </summary>
        public IGenericDataRepository<TSOProductivityOutcomePlanned> TSOProductivityOutcomePlannedRepository
        {
            get
            {
                if (null == tsoProductivityOutcomePlannedRepository)
                    tsoProductivityOutcomePlannedRepository = uow.GetRepository<TSOProductivityOutcomePlanned>();

                return tsoProductivityOutcomePlannedRepository;
            }
        }

        /// <summary>
        /// TSOServiceDeliveryChainTaskActualRepository
        /// </summary>
        public IGenericDataRepository<TSOServiceDeliveryChainTaskActual> TSOServiceDeliveryChainTaskActualRepository
        {
            get
            {
                if (null == tsoServiceDeliveryChainTaskActualRepository)
                    tsoServiceDeliveryChainTaskActualRepository = uow.GetRepository<TSOServiceDeliveryChainTaskActual>();

                return tsoServiceDeliveryChainTaskActualRepository;
            }
        }

        /// <summary>
        /// TSOServiceDeliveryChainTaskPlannedRepository
        /// </summary>
        public IGenericDataRepository<TSOServiceDeliveryChainTaskPlanned> TSOServiceDeliveryChainTaskPlannedRepository
        {
            get
            {
                if (null == tsoServiceDeliveryChainTaskPlannedRepository)
                    tsoServiceDeliveryChainTaskPlannedRepository = uow.GetRepository<TSOServiceDeliveryChainTaskPlanned>();

                return tsoServiceDeliveryChainTaskPlannedRepository;
            }
        }


        public IGenericDataRepository<TSOServiceDeliveryChainActualRisk> TSOServiceDeliveryChainActualRiskRepository
        {
            get
            {
                if (null == tsoServiceDeliveryChainActualRiskRepository)
                    tsoServiceDeliveryChainActualRiskRepository = uow.GetRepository<TSOServiceDeliveryChainActualRisk>();

                return tsoServiceDeliveryChainActualRiskRepository;
            }
        }

        //public IGenericDataRepository<TSOServiceDeliveryChainPlannedRisk> TSOServiceDeliveryChainPlannedRiskRepository
        //{
        //    get
        //    {
        //        if (null == tsoServiceDeliveryChainPlannedRiskRepository)
        //            tsoServiceDeliveryChainPlannedRiskRepository = uow.GetRepository<TSOServiceDeliveryChainPlannedRisk>();

        //        return tsoServiceDeliveryChainPlannedRiskRepository;
        //    }
        //}
        /// <summary>
        /// Roles Repository
        /// </summary>
        public IGenericDataRepository<Roles> RoleRepository
        {
            get
            {
                if (null == roleRepository)
                    roleRepository = uow.GetRepository<Roles>();
                return roleRepository;
            }
        }

        /// <summary>
        /// EngagementRepository
        /// </summary>
        public IGenericDataRepository<Engagement> EngagementRepository
        {
            get
            {
                if (null == engagementRepository)
                    engagementRepository = uow.GetRepository<Engagement>();
                return engagementRepository;
            }
        }

        /// <summary>
        /// PricingModelRepository
        /// </summary>
        public IGenericDataRepository<PricingModel> PricingModelRepository
        {
            get
            {
                if (null == pricingModelRepository)
                    pricingModelRepository = uow.GetRepository<PricingModel>();
                return pricingModelRepository;
            }
        }

        /// <summary>
        /// RiskStatus Repository
        /// </summary>
        public IGenericDataRepository<RiskStatus> RiskStatusRepository
        {
            get
            {
                if (null == riskStatusRepository)
                    riskStatusRepository = uow.GetRepository<RiskStatus>();
                return riskStatusRepository;
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ttmContext.Dispose();
                    uow.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
