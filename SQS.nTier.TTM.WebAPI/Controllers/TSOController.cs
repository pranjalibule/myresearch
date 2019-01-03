﻿/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS  04Oct2017   Created the class
 * AKS  01Dec2017   Added logic to check unique TSO title
 *******************************************************************************/

namespace SQS.nTier.TTM.WebAPI.Controllers
{
    using BAL;
    using Common;
    using DAL;
    using DTO;
    using GenericFramework;
    using GenericFramework.Utility;
    using Newtonsoft.Json;
    using RoleAttribute;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;
    using System.Web.Script.Serialization;

    /// <summary>
    /// TSOController
    /// </summary>
    [RoutePrefix("api")]
    public class TSOController : ApiController
    {
        
        #region Private Functions

        /// <summary>
        /// GetTSOById
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="loadEntities">bool</param>
        /// <returns>TSO</returns>
        private TSODTO GetTSOById(int id, bool loadEntities = true)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            TSODTO objTSODTO = null;

            try
            {
                TSO objTSO = null;

                if (loadEntities)
                {
                    objTSO = objBusinessLayer.TSORepository.GetSingle(x => x.ID == id,
                    x => x.TeamLead,
                    x => x.OperationalRisk,
                    x => x.TSOServiceDeliveryChains,
                    x => x.TSOProductivityInputs,
                    x => x.TSOProductivityInputs.Select(y => y.ProductivityInput),
                    x => x.TSOServiceDeliveryChains.Select(y => y.TSOProductivityInputActuals),
                    x => x.TSOServiceDeliveryChains.Select(y => y.TSOProductivityInputPlanneds),
                    x => x.TSOProductivityOutcomes,
                    x => x.TSOProductivityOutcomes.Select(y => y.ProductivityOutcome),
                    x => x.TSOServiceDeliveryChains.Select(y => y.TSOProductivityOutcomeActuals),
                    x => x.TSOServiceDeliveryChains.Select(y => y.TSOProductivityOutcomePlanneds),
                    x => x.TSOStatus,
                   x => x.CoreService,
                    x => x.TSR);
                }
                else
                {
                    objTSO = objBusinessLayer.TSORepository.GetSingle(x => x.ID == id, x => x.TSR);
                }


                if (null != objTSO)
                {
                    objTSODTO = new TSODTO();
                    objTSODTO = Conversions.ToDTO<TSODTO, TSO>(objTSO);

                    ProjectModelController pmC = new ProjectModelController();
                    if (objTSODTO.TSR == null)
                    {
                        TSRController tsrC = new TSRController();
                        objTSODTO.TSR = tsrC.GetTSR(objTSODTO.TSRId);
                    }
                    objTSODTO.TSR.ProjectModelName = pmC.GetProjectModelNameById(objTSODTO.TSR.ProjectModelID);
                }

                //TSOStatusDTO objTSOStatusDTO = Conversions.ToDTO<TSOStatusDTO, TSOStatus>(objTSO.TSOStatus);
                //objTSODTO.TSOStatus = objTSOStatusDTO;

                //CoreServiceDTO objCoreServiceDTO = Conversions.ToDTO<CoreServiceDTO, CoreService>(objTSO.CoreService);
                //objTSODTO.CoreService = objCoreServiceDTO;

                //TSRDTO objSTRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSO.TSR);
                //objTSODTO.TSR = objSTRDTO;

                //OperationalRiskDTO objOperationalRiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objTSO.OperationalRisk);
                //objTSODTO.OperationalRisk = objOperationalRiskDTO;

                if (objTSODTO.ActualEffort == null)
                {
                    objTSODTO.ActualEffort = 0;
                }

                if (objTSODTO.ActualOutcome == null)
                {
                    objTSODTO.ActualOutcome = 0;
                }

                int taskCC = 0;
                int taskCount = 0;
                objTSODTO.CanClose = false;
                double ChildTaskPlannedEffortSum = 0;
                double? ActualOutcome = 0;
                foreach (TSOServiceDeliveryChainDTO dileverychain in objTSODTO.TSOServiceDeliveryChains)
                {
                    //TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTOList = this.GetLastTSOServiceDeliveryChainList(dileverychain.ID);
                    IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = this.GetLastTSOServiceDeliveryChainList(dileverychain.ID).OrderBy(x => x.UpdatedOn).ToList();
                    taskCount++;
                    if (objTSOServiceDeliveryChainDTOList != null)
                    {
                        foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSOServiceDeliveryChainDTOList)
                        {
                            foreach (var objTSOServiceDeliveryChainActual in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks)
                            {
                                objTSODTO.ActualEffort = objTSODTO.ActualEffort + objTSOServiceDeliveryChainActual.ActualEffort;
                            }
                            foreach (var objTSOServiceDeliveryChainPlanned in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks)
                            {
                                ChildTaskPlannedEffortSum = ChildTaskPlannedEffortSum + (objTSOServiceDeliveryChainPlanned.PlannedEffort ?? 0);
                            }

                            foreach (TSOProductivityOutcomeActualDTO objTSOProductivityOutcomeDTO in objTSOServiceDeliveryChainDTO.TSOProductivityOutcomeActuals)
                            {
                                ActualOutcome += objTSOProductivityOutcomeDTO.OutcomeValue;
                            }

                            if (objTSOServiceDeliveryChainDTO.TaskStatusId > 0)
                            {
                                if (GetTaskStatus(objTSOServiceDeliveryChainDTO.TaskStatusId).ToLower() == "cancelled"
                                    || GetTaskStatus(objTSOServiceDeliveryChainDTO.TaskStatusId).ToLower() == "closed")
                                {
                                    if (objTSODTO.CloseTaskIds == null || objTSODTO.CloseTaskIds == "")
                                    {
                                        objTSODTO.CloseTaskIds = objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId.ToString();
                                        taskCC++;
                                    }
                                    else
                                    {
                                        string[] ids = objTSODTO.CloseTaskIds.Split(',');
                                        int pos = Array.IndexOf(ids, objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId.ToString());
                                        if (pos < 0)
                                        {
                                            objTSODTO.CloseTaskIds = objTSODTO.CloseTaskIds + "," + objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId;
                                            taskCC++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //if(dileverychain.ServiceDeliveryChainId == 42)
                    //{
                    //    SetOperationRiskToTSO(objTSODTO, objTSODTO.CreatedBy);
                    //}
                }

                objTSODTO.ActualOutcome = (ActualOutcome == null) ? 0 : ActualOutcome;
                objTSODTO.OutomeCompletion = (objTSODTO.PlannedOutcome == 0) ? 0 : ((objTSODTO.ActualOutcome / objTSODTO.PlannedOutcome) * 100);
                objTSODTO.ChildTaskPlannedEffortSum = ChildTaskPlannedEffortSum;
                if (taskCC == taskCount)
                {
                    objTSODTO.CanClose = true;
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objTSODTO;
        }

        public string GetTaskStatus(int statusId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            string status = null;
            try
            {
                TaskStatus objTaskStatus = objBusinessLayer.TaskStatusRepository.GetSingle(x => x.ID == statusId);
                status = objTaskStatus.Name;
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }
            return status;
        }
        private IList<TSODTO> GetTSOByTitleOrClient(int startingRecordNumber, int pageSize, string strTitleOrClient, int status, string userId, out int totalRecords)
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            IList<TSODTO> objTSODTOList = new List<TSODTO>();
            IList<TSO> objTSOList = null; totalRecords = 0;
            TSODTO objTSODTO = new TSODTO();
            int userID = Convert.ToInt32(userId);
            objBusinessLayer = new BusinessLayer(ls);
            User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);

            try
            {
                if (strTitleOrClient != "none")
                {
                    if (status != 0)
                    {
                        if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => x.TSOStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.ID, false, out totalRecords, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.TSRCoreServices, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.TSOServiceDeliveryChains);
                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => x.TSOStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.ID, false, out totalRecords, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.TSRCoreServices, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.TSOServiceDeliveryChains);
                        }
                    }
                    else
                    {
                        if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.ID, false, out totalRecords, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.TSRCoreServices, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.TSOServiceDeliveryChains);
                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.ID, false, out totalRecords, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.TSRCoreServices, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.TSOServiceDeliveryChains);
                        }
                    }
                }
                else
                {
                    if (status != 0)
                    {
                        if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => x.TSOStatusID == status, x => x.ID, false, out totalRecords, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.TSRCoreServices, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.OperationalRisk, x => x.TSOServiceDeliveryChains);

                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => x.TSOStatusID == status && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.ID, false, out totalRecords, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.TSRCoreServices, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.OperationalRisk, x => x.TSOServiceDeliveryChains);
                        }
                    }
                    else
                    {
                        if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => x.ID != 0, x => x.ID, false, out totalRecords, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.TSRCoreServices, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.OperationalRisk, x => x.TSOServiceDeliveryChains);

                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => x.ID != 0 && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.ID, false, out totalRecords, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.TSRCoreServices, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.OperationalRisk, x => x.TSOServiceDeliveryChains);
                        }
                    }
                }

                if (objTSOList != null && objTSOList.Count > 0)
                {
                    objTSODTOList = new List<TSODTO>();

                    foreach (TSO objTSO in objTSOList)
                    {

                        double? ActualEffort = 0;
                        double? ActualOutcome = 0;
                        if (objTSO.TSOServiceDeliveryChains != null && objTSO.TSOServiceDeliveryChains.Count > 0)
                        {
                            foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSO.TSOServiceDeliveryChains)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTSOServiceDeliveryChain.ServiceDeliveryChainId);

                                if (null != objServiceDeliveryChain)
                                {
                                    objTSOServiceDeliveryChain.ServiceDeliveryChain = objServiceDeliveryChain;
                                }

                                IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = this.GetLastTSOServiceDeliveryChainList(objTSOServiceDeliveryChain.ID);
                                foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSOServiceDeliveryChainDTOList)
                                {
                                    if (objTSOServiceDeliveryChainDTO != null)
                                    {
                                        foreach (var objTSOServiceDeliveryChainActual in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks)
                                        {
                                            //ActualOutcome = ActualOutcome + objTSOServiceDeliveryChainActual.ActualOutcome;
                                            ActualEffort = ActualEffort + (objTSOServiceDeliveryChainActual.ActualEffort == null ? 0 : objTSOServiceDeliveryChainActual.ActualEffort);
                                        }
                                    }
                                }
                                //TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO = this.GetLastTSOServiceDeliveryChain(objTSOServiceDeliveryChain.ID);
                                //if (objTSOServiceDeliveryChainDTO != null)
                                //{
                                //    ActualEffort = ActualEffort + (objTSOServiceDeliveryChainDTO.ActualEffort == null ? 0 : objTSOServiceDeliveryChainDTO.ActualEffort);
                                //}
                            }
                        }

                        // TSODTO objTSODTO = new TSODTO();
                        objTSODTO = Conversions.ToDTO<TSODTO, TSO>(objTSO);
                        var TSOServiceDeliveryChainsDTO = objTSODTO.TSOServiceDeliveryChains.ToList();
                        foreach (TSOServiceDeliveryChainDTO item in TSOServiceDeliveryChainsDTO)
                        {
                            // objBusinessLayer = new BusinessLayer(ls);
                            // var task = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.TSOServiceDeliveryChainId == item.ID);
                            var task = this.GetLastTSOServiceDeliveryChain(item.ID);
                            if (task != null && task.TaskStatusId != 0)
                            {
                                item.TaskStatus = GetTaskStatus(task.TaskStatusId);
                            }
                        }
                        objTSODTO.TeamLead = Conversions.ToDTO<UserDTO, User>(objTSO.TeamLead);
                        objTSODTO.TSOStatus = Conversions.ToDTO<TSOStatusDTO, TSOStatus>(objTSO.TSOStatus);
                        objTSODTO.TSR = Conversions.ToDTO<TSRDTO, TSR>(objTSO.TSR);
                        objTSODTO.CoreService = Conversions.ToDTO<CoreServiceDTO, CoreService>(objTSO.CoreService);
                        objTSODTO.OperationalRisk = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objTSO.OperationalRisk);


                        TSOStatusDTO objTSOStatusDTO = Conversions.ToDTO<TSOStatusDTO, TSOStatus>(objTSO.TSOStatus);
                        SolutionCentreDTO objSolutionCentreDTO = Conversions.ToDTO<SolutionCentreDTO, SolutionCentre>(objTSO.TSR.SolutionCentre);
                        PracticeDTO objPracticeDTO = Conversions.ToDTO<PracticeDTO, Practice>(objTSO.TSR.Practice);

                        //objTSODTO = Conversions.ToDTO<TSODTO, TSO>(objTSO);
                        objTSODTO.TSOStatus = objTSOStatusDTO;

                        ClientDTO objClientDTO = Conversions.ToDTO<ClientDTO, Client>(objTSO.TSR.Client);

                        TSRDTO objTSRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSO.TSR);
                        objTSRDTO.SolutionCentre = objSolutionCentreDTO;
                        objTSRDTO.Client = objClientDTO;

                        // TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSO.TSOServiceDeliveryChains);
                        //objTSODTO.TSOServiceDeliveryChains = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSO.TSOServiceDeliveryChains);
                        objTSODTO.TeamLead = Conversions.ToDTO<UserDTO, User>(objTSO.TeamLead);
                        objTSODTO.TSR = objTSRDTO;
                        objTSODTO.CoreService = Conversions.ToDTO<CoreServiceDTO, CoreService>(objTSO.CoreService);
                        objTSODTO.OperationalRisk = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objTSO.OperationalRisk);
                        objTSODTO.RelevantRepository = Conversions.ToDTO<RelevantRepositoryDTO, RelevantRepository>(objTSO.RelevantRepository);

                        objTSODTO.ActualEffort = ActualEffort;
                        objTSODTO.ActualOutcome = (ActualOutcome == null) ? 0 : ActualOutcome;
                        objTSODTO.OutomeCompletion = (objTSODTO.PlannedOutcome == 0) ? 0 : ((objTSODTO.ActualOutcome / objTSODTO.PlannedOutcome) * 100);
                        objTSODTOList.Add(objTSODTO);
                    }

                    totalRecords = objTSODTOList.Count;
                }

                totalRecords = objTSOList.Count();

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            if (null == objTSOList)
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return objTSODTOList;
        }

        /// <summary>
        /// GetAllTSO
        /// </summary>
        /// <returns>IList<TSO>></returns>
        private IList<TSODTO> GetAllTSO(int tsrId, int startingRecordNumber, int pageSize, string userId, out int totalRecords)
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            IList<TSODTO> objTSODTOList = null;
            totalRecords = 0;
            try
            {
                int userID = Convert.ToInt32(userId);
                IList<TSO> objTSOList;
                List<TSO> objTSOes = new List<TSO>();
                objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);
                //Note - Currently we are showing all record in TSO dashboard as per discussion
                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSOList = objBusinessLayer.TSORepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.TSRId == tsrId, x => x.TSR, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                }
                else
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSOList = objBusinessLayer.TSORepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.TSRId == tsrId && (x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID), x => x.TSR, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                    //   objTSOList = objBusinessLayer.TSORepository.GetList(startingRecordNumber, pageSize, x => ((x.Title.ToLower().Contains(strTitleOrClient)) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);


                    objBusinessLayer = new BusinessLayer(ls);
                    List<TSRTMOUser> tmoUsers = objBusinessLayer.TSRTMOUserRepository.GetList(x => x.UserId == userID && x.TSRId == tsrId, x => x.TSR, x => x.TSR.TSOes).ToList();
                    foreach (var item in tmoUsers)
                    {
                        objTSOes.AddRange(item.TSR.TSOes);
                    }

                    foreach (TSO objTSO in objTSOes)
                    {
                        if (objTSOList.Count < pageSize)
                        {
                            // objBusinessLayer = new BusinessLayer(ls);
                            if (!objTSOList.Any(x => x.ID == objTSO.ID))
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                TSO objTso = objBusinessLayer.TSORepository.GetSingle(x => x.ID == objTSO.ID, x => x.TSR, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                objTSOList.Add(objTso);
                            }
                        }
                        else
                        {
                            break;
                        }

                    }
                }
                if (objTSOList != null && objTSOList.Count > 0)
                {
                    objTSODTOList = new List<TSODTO>();

                    foreach (TSO objTSO in objTSOList)
                    {
                        UserDTO tl = Conversions.ToDTO<UserDTO, User>(objTSO.TeamLead);
                        double? ActualEffort = 0;
                        double? ActualOutcome = 0;
                        if (objTSO.TSOServiceDeliveryChains != null && objTSO.TSOServiceDeliveryChains.Count > 0)
                        {
                            foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSO.TSOServiceDeliveryChains)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTSOServiceDeliveryChain.ServiceDeliveryChainId);

                                if (null != objServiceDeliveryChain)
                                {
                                    objTSOServiceDeliveryChain.ServiceDeliveryChain = objServiceDeliveryChain;
                                }

                                IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = this.GetLastTSOServiceDeliveryChainList(objTSOServiceDeliveryChain.ID);
                                foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSOServiceDeliveryChainDTOList)
                                {
                                    if (objTSOServiceDeliveryChainDTO != null)
                                    {
                                        foreach (var objTSOServiceDeliveryChainActual in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks)
                                        {
                                            ActualEffort = ActualEffort + (objTSOServiceDeliveryChainActual.ActualEffort == null ? 0 : objTSOServiceDeliveryChainActual.ActualEffort);
                                        }

                                        foreach (TSOProductivityOutcomeActualDTO actualOut in objTSOServiceDeliveryChainDTO.TSOProductivityOutcomeActuals)
                                        {
                                            ActualOutcome += actualOut.OutcomeValue;
                                        }
                                    }
                                }
                            }

                        }

                        //SetOperationRiskToTSO(objTSO, objTSO.UpdatedBy);
                        TSODTO objTSODTO = new TSODTO();
                        objTSODTO = Conversions.ToDTO<TSODTO, TSO>(objTSO);

                        var TSOServiceDeliveryChainsDTO = objTSODTO.TSOServiceDeliveryChains.ToList();
                        foreach (TSOServiceDeliveryChainDTO item in TSOServiceDeliveryChainsDTO)
                        {
                            var task = this.GetLastTSOServiceDeliveryChain(item.ID);
                            if (task != null && task.TaskStatusId != 0)
                            {
                                item.TaskStatus = GetTaskStatus(task.TaskStatusId);
                            }
                        }

                        TSOStatusDTO objTSOStatusDTO = Conversions.ToDTO<TSOStatusDTO, TSOStatus>(objTSO.TSOStatus);
                        objTSODTO.TSOStatus = objTSOStatusDTO;

                        CoreServiceDTO objCoreServiceDTO = Conversions.ToDTO<CoreServiceDTO, CoreService>(objTSO.CoreService);
                        objTSODTO.CoreService = objCoreServiceDTO;

                        TSRDTO objSTRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSO.TSR);
                        objTSODTO.TSR = objSTRDTO;

                        OperationalRiskDTO objOperationalRiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objTSO.OperationalRisk);
                        objTSODTO.OperationalRisk = objOperationalRiskDTO;

                        objTSODTO.ActualEffort = ActualEffort;
                        objTSODTO.ActualOutcome = (ActualOutcome == null) ? 0 : ActualOutcome;
                        objTSODTO.OutomeCompletion = (objTSODTO.PlannedOutcome == 0) ? 0 : ((objTSODTO.ActualOutcome / objTSODTO.PlannedOutcome) * 100);
                        objTSODTO.TeamLead = tl;
                        objTSODTOList.Add(objTSODTO);
                    }

                    totalRecords = objTSODTOList.Count;
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objTSODTOList;
        }

        /// <summary>
        /// GetAllTSO
        /// </summary>
        /// <returns>IList<TSO>>Created for Dumps</returns>
        private IList<TSODTO> GetAllTSO()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSODTO> objTSODTOList = null;
            try
            {
                IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetAll(x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository, x => x.TSR.PricingModel, x => x.TSR.Engagement, x => x.TSR.ClientRegion, x => x.TSR.SolutionCentre, x => x.CoreService, x => x.TSR.OperationalRisk, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.TestManager, x => x.TSR.Vertical, x => x.TSR.Practice, x => x.TSR.MarketOffering, x => x.TSR.ProjectModel);

                if (objTSOList != null && objTSOList.Count > 0)
                {
                    objTSODTOList = new List<TSODTO>();

                    foreach (TSO objTSO in objTSOList)
                    {
                        UserDTO tl = Conversions.ToDTO<UserDTO, User>(objTSO.TeamLead);
                        double? ActualEffort = 0.00;
                        if (objTSO.TSOServiceDeliveryChains != null && objTSO.TSOServiceDeliveryChains.Count > 0)
                        {
                            foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSO.TSOServiceDeliveryChains)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTSOServiceDeliveryChain.ServiceDeliveryChainId);

                                if (null != objServiceDeliveryChain)
                                {
                                    objTSOServiceDeliveryChain.ServiceDeliveryChain = objServiceDeliveryChain;
                                }

                                IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = this.GetLastTSOServiceDeliveryChainList(objTSOServiceDeliveryChain.ID);
                                foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSOServiceDeliveryChainDTOList)
                                {
                                    if (objTSOServiceDeliveryChainDTO != null)
                                    {
                                        foreach (var item in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks)
                                        {
                                            ActualEffort = ActualEffort + (item.ActualEffort == null ? 0.00 : item.ActualEffort);
                                        }

                                    }
                                }
                            }
                        }

                        TSODTO objTSODTO = new TSODTO();
                        objTSODTO = Conversions.ToDTO<TSODTO, TSO>(objTSO);
                        var TSOServiceDeliveryChainsDTO = objTSODTO.TSOServiceDeliveryChains.ToList();
                        foreach (TSOServiceDeliveryChainDTO item in TSOServiceDeliveryChainsDTO)
                        {
                            var task = this.GetLastTSOServiceDeliveryChain(item.ID);
                            if (task != null && task.TaskStatusId != 0)
                            {
                                item.TaskStatus = GetTaskStatus(task.TaskStatusId);
                            }
                        }

                        TSOStatusDTO objTSOStatusDTO = Conversions.ToDTO<TSOStatusDTO, TSOStatus>(objTSO.TSOStatus);
                        objTSODTO.TSOStatus = objTSOStatusDTO;

                        CoreServiceDTO objCoreServiceDTO = Conversions.ToDTO<CoreServiceDTO, CoreService>(objTSO.CoreService);
                        objTSODTO.CoreService = objCoreServiceDTO;

                        TSRDTO objSTRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSO.TSR);
                        objTSODTO.TSR = objSTRDTO;

                        OperationalRiskDTO objOperationalRiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objTSO.OperationalRisk);
                        objTSODTO.OperationalRisk = objOperationalRiskDTO;

                        objTSODTO.ActualEffort = (double)ActualEffort;
                        objTSODTO.TeamLead = tl;
                        objTSODTOList.Add(objTSODTO);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objTSODTOList;
        }

        /// <summary>
        /// GetAllTSO
        /// </summary>
        /// <returns>IList<TSO>></returns>
        private IList<TSODTO> GetAllUserTSO(int startingRecordNumber, int pageSize, string userId, out int totalRecords, bool isadmin = false)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSODTO> objTSODTOList = null;
            List<TSO> objTSOes = new List<TSO>();
            totalRecords = 0;
            try
            {
                int userID = Convert.ToInt32(userId);
                IList<TSO> objTSOList;
                objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);
                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSOList = objBusinessLayer.TSORepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TeamLead, x => x.TSOStatus, x => x.TSOServiceDeliveryChains, x => x.TSOProductivityInputs, x => x.TSOProductivityOutcomes, x => x.OperationalRisk, x => x.CoreService, x => x.RelevantRepository);
                }
                else
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSOList = objBusinessLayer.TSORepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TeamLead, x => x.TSOStatus, x => x.TSOServiceDeliveryChains, x => x.TSOProductivityInputs, x => x.TSOProductivityOutcomes, x => x.OperationalRisk, x => x.CoreService, x => x.RelevantRepository);

                    objBusinessLayer = new BusinessLayer(ls);
                    List<TSRTMOUser> tmoUsers = objBusinessLayer.TSRTMOUserRepository.GetList(x => x.UserId == userID, x => x.TSR, x => x.TSR.TSOes).ToList();
                    foreach (var item in tmoUsers)
                    {
                        objTSOes.AddRange(item.TSR.TSOes);
                    }

                    foreach (TSO objTSO in objTSOes)
                    {
                        if (objTSOList.Count < pageSize)
                        {
                            // objBusinessLayer = new BusinessLayer(ls);
                            if (!objTSOList.Any(x => x.ID == objTSO.ID))
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                TSO objTso = objBusinessLayer.TSORepository.GetSingle(x => x.ID == objTSO.ID, x => x.TSR, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                objTSOList.Add(objTso);
                            }
                        }
                        else
                        {
                            break;
                        }

                    }
                }

                if (objTSOList != null && objTSOList.Count > 0)
                {
                    objTSODTOList = new List<TSODTO>();

                    foreach (TSO objTSO in objTSOList)
                    {
                        if (objTSO != null)
                        {
                            double? ActualEffort = 0;
                            double? ActualOutcome = 0;
                            if (objTSO.TSOServiceDeliveryChains != null && objTSO.TSOServiceDeliveryChains.Count > 0)
                            {
                                foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSO.TSOServiceDeliveryChains)
                                {
                                    objBusinessLayer = new BusinessLayer(ls);
                                    ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTSOServiceDeliveryChain.ServiceDeliveryChainId);

                                    if (null != objServiceDeliveryChain)
                                    {
                                        objTSOServiceDeliveryChain.ServiceDeliveryChain = objServiceDeliveryChain;
                                    }

                                    IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = this.GetLastTSOServiceDeliveryChainList(objTSOServiceDeliveryChain.ID);
                                    foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSOServiceDeliveryChainDTOList)
                                    {
                                        if (objTSOServiceDeliveryChainDTO != null)
                                        {
                                            foreach (var item in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks)
                                            {
                                                ActualEffort = ActualEffort + (item.ActualEffort == null ? 0 : item.ActualEffort);
                                            }
                                        }
                                    }

                                    foreach (TSOProductivityOutcome objTSOProductivityOutcomeDTO in objTSO.TSOProductivityOutcomes)
                                    {
                                        objBusinessLayer = new BusinessLayer(ls);

                                        List<TSOProductivityOutcomeActual> objTSOProductivityOutcomeActual = objBusinessLayer.TSOProductivityOutcomeActualRepository.GetList(x => x.TSOProductivityOutcomeId == objTSOProductivityOutcomeDTO.ID && x.ChainID == objTSOServiceDeliveryChain.ID).ToList();

                                        objTSOProductivityOutcomeDTO.TSOProductivityOutcomeActuals = objTSOProductivityOutcomeActual;

                                        foreach (TSOProductivityOutcomeActual actualOut in objTSOProductivityOutcomeDTO.TSOProductivityOutcomeActuals)
                                        {
                                            ActualOutcome += actualOut.OutcomeValue;
                                        }

                                    }
                                }
                            }

                            //SetOperationRiskToTSO(objTSO, objTSO.UpdatedBy);
                            TSODTO objTSODTO = new TSODTO();
                            objTSODTO = Conversions.ToDTO<TSODTO, TSO>(objTSO);
                            var TSOServiceDeliveryChainsDTO = objTSODTO.TSOServiceDeliveryChains.ToList();
                            foreach (TSOServiceDeliveryChainDTO item in TSOServiceDeliveryChainsDTO)
                            {
                                var task = this.GetLastTSOServiceDeliveryChain(item.ID);
                                if (task != null && task.TaskStatusId != 0)
                                {
                                    item.TaskStatus = GetTaskStatus(task.TaskStatusId);
                                }
                            }
                            objTSODTO.TeamLead = Conversions.ToDTO<UserDTO, User>(objTSO.TeamLead);
                            objTSODTO.TSOStatus = Conversions.ToDTO<TSOStatusDTO, TSOStatus>(objTSO.TSOStatus);
                            objTSODTO.TSR = Conversions.ToDTO<TSRDTO, TSR>(objTSO.TSR);
                            objTSODTO.CoreService = Conversions.ToDTO<CoreServiceDTO, CoreService>(objTSO.CoreService);
                            objTSODTO.OperationalRisk = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objTSO.OperationalRisk);

                            objTSODTO.ActualEffort = ActualEffort;
                            objTSODTO.ActualOutcome = (ActualOutcome == null) ? 0 : ActualOutcome;
                            objTSODTO.OutomeCompletion = (objTSODTO.PlannedOutcome == 0) ? 0 : ((objTSODTO.ActualOutcome / objTSODTO.PlannedOutcome) * 100);
                            objTSODTOList.Add(objTSODTO);
                        }
                    }

                    totalRecords = objTSODTOList.Count;
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objTSODTOList;
        }

        //private IList<TSODTO> GetAllRoleBaseTSO(int tsrId, int startingRecordNumber, int pageSize, string userId, out int totalRecords)
        //{
        //    LoginSession ls = new LoginSession();
        //    IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
        //    IList<TSODTO> objTSODTOList = null;
        //    totalRecords = 0;
        //    int userID = Convert.ToInt32(userId);
        //    try
        //    {
        //        // IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tsrId, x => x.TSOServiceDeliveryChains, x => x.TSR, x=> x.TSOStatus,x=>x.TeamLead,x=>x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
        //        IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.TSRId == tsrId || x.TSR.AccountManagerId== userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId== userID || x.TeamLeadId == userID, x => x.TSR, x => x.TSOServiceDeliveryChains, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
        //        if (objTSOList != null && objTSOList.Count > 0)
        //        {
        //            objTSODTOList = new List<TSODTO>();

        //            foreach (TSO objTSO in objTSOList)
        //            {
        //                UserDTO tl = Conversions.ToDTO<UserDTO, User>(objTSO.TeamLead);
        //                double? ActualEffort = 0;
        //                if (objTSO.TSOServiceDeliveryChains != null && objTSO.TSOServiceDeliveryChains.Count > 0)
        //                {
        //                    foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSO.TSOServiceDeliveryChains)
        //                    {
        //                        objBusinessLayer = new BusinessLayer(ls);
        //                        ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTSOServiceDeliveryChain.ServiceDeliveryChainId);

        //                        if (null != objServiceDeliveryChain)
        //                        {
        //                            objTSOServiceDeliveryChain.ServiceDeliveryChain = objServiceDeliveryChain;
        //                        }

        //                        IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = this.GetLastTSOServiceDeliveryChainList(objTSOServiceDeliveryChain.ID);
        //                        foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSOServiceDeliveryChainDTOList)
        //                        {
        //                            if (objTSOServiceDeliveryChainDTO != null)
        //                            {
        //                                ActualEffort = ActualEffort + (objTSOServiceDeliveryChainDTO.ActualEffort == null ? 0 : objTSOServiceDeliveryChainDTO.ActualEffort);
        //                            }
        //                        }
        //                    }
        //                }

        //                TSODTO objTSODTO = new TSODTO();
        //                objTSODTO = Conversions.ToDTO<TSODTO, TSO>(objTSO);

        //                var TSOServiceDeliveryChainsDTO = objTSODTO.TSOServiceDeliveryChains.ToList();
        //                foreach (TSOServiceDeliveryChainDTO item in TSOServiceDeliveryChainsDTO)
        //                {
        //                    //objBusinessLayer = new BusinessLayer(ls);
        //                    //var task = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x=>x.TSOServiceDeliveryChainId==item.ID);
        //                    //if (task != null)
        //                    //{
        //                    //    item.TaskStatus = GetTaskStatus(task.TaskStatusId);
        //                    //}
        //                    var task = this.GetLastTSOServiceDeliveryChain(item.ID);
        //                    if (task != null && task.TaskStatusId != 0)
        //                    {
        //                        item.TaskStatus = GetTaskStatus(task.TaskStatusId);
        //                    }
        //                }

        //                TSOStatusDTO objTSOStatusDTO = Conversions.ToDTO<TSOStatusDTO, TSOStatus>(objTSO.TSOStatus);
        //                objTSODTO.TSOStatus = objTSOStatusDTO;

        //                CoreServiceDTO objCoreServiceDTO = Conversions.ToDTO<CoreServiceDTO, CoreService>(objTSO.CoreService);
        //                objTSODTO.CoreService = objCoreServiceDTO;

        //                TSRDTO objSTRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSO.TSR);
        //                objTSODTO.TSR = objSTRDTO;

        //                OperationalRiskDTO objOperationalRiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objTSO.OperationalRisk);
        //                objTSODTO.OperationalRisk = objOperationalRiskDTO;

        //                objTSODTO.ActualEffort = ActualEffort;
        //                objTSODTO.TeamLead = tl;
        //                objTSODTOList.Add(objTSODTO);
        //            }

        //            totalRecords = objTSODTOList.Count;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
        //        throw;
        //    }
        //    finally
        //    {
        //        ls = null;
        //        objBusinessLayer.Dispose();
        //    }

        //    return objTSODTOList;
        //}


        /// <summary>
        /// Function to add new TSO 
        /// </summary>
        /// <param name="objTSO"></param>
        /// <returns></returns>
        private string AddNewTSO(TSODTO objTSODTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objTSODTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                TSO chkTSO = null;
                chkTSO = objBusinessLayer.TSORepository.GetSingle(x => x.Title == objTSODTO.Title);// && x.TSRId == objTSODTO.TSRId

                if (chkTSO != null)
                {
                    returnMessage = "Error - TSO title already exists.";
                }
                else
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    string strDefaultTSRStatusText = ConfigurationManager.AppSettings["DefaultTSOStatusText"].ToString();
                    TSOStatus objTSOStatus = objBusinessLayer.TSOStatusRepository.GetSingle(x => x.Name == strDefaultTSRStatusText);

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSODTO.TSOStatusID = objTSOStatus.ID;
                    TSO objTSO = Conversions.ToEntity<TSODTO, TSO>(objTSODTO);
                    objBusinessLayer.TSORepository.Add(objTSO);
                    objBusinessLayer = new BusinessLayer(ls);
                    TSR tsr =  objBusinessLayer.TSRRepository.GetSingle(x => x.ID == objTSO.TSRId, x => x.DeliveryManager);
                    if (null != objTSO && objTSO.ID > 0)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        TaskStatus objTaskStatus = objBusinessLayer.TaskStatusRepository.GetSingle(x => x.Name == "Created");

                        if (!String.IsNullOrWhiteSpace(objTSODTO.TSOServiceDeliveryChainArr))
                        {
                            if (objTSODTO.TSOServiceDeliveryChainArr.IndexOf(",") <= 0)
                            {
                                TSOServiceDeliveryChain objTSOServiceDeliveryChain = new TSOServiceDeliveryChain
                                {
                                    ServiceDeliveryChainId = int.Parse(objTSODTO.TSOServiceDeliveryChainArr),
                                    TSOId = objTSO.ID,
                                    PlannedStartDate = objTSO.StartDate,
                                    PlannedCompletionDate = objTSO.TargetCompletionDate,
                                    TaskStatusId = objTaskStatus.ID,
                                    Notes = String.Empty,
                                    //  ResponsiblePersonId = objTSO.TSR.DeliveryManagerId,
                                };

                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOServiceDeliveryChainRepository.Add(objTSOServiceDeliveryChain);

                                //Create a Task and set the default value

                                if (null != objTSOServiceDeliveryChain && objTSOServiceDeliveryChain.ID > 0)
                                {

                                    TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainActual = new TSOServiceDeliveryChainTaskActual
                                    {
                                        TSOServiceDeliveryChainTaskId = objTSOServiceDeliveryChain.ID,
                                        WeekNumber = GetWeekOfYear(DateTime.Now),
                                        Year = DateTime.Now.Year,
                                        ActualEffort = 0,
                                        ActualInput = 0,
                                        ActualOutcome = 0,
                                        ActualOutcomeTestSteps = 0,
                                        ActualProcessingTime = 0,
                                        ActualProductivity = 0,
                                        ActualReviewRounds = 0,
                                        DefectRaised = 0,
                                        DefectRejected = 0,
                                        Headcount = 0,
                                        IdleTimeDuration = 0,
                                        IdleTimeEffort = 0,
                                    };
                                    TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainPlanned = new TSOServiceDeliveryChainTaskPlanned
                                    {
                                        TSOServiceDeliveryChainTaskId = objTSOServiceDeliveryChain.ID,
                                        WeekNumber = GetWeekOfYear(DateTime.Now) + 1,
                                        Year = DateTime.Now.Year,
                                        PlannedEffort = 0,
                                        PlannedInput = 0,
                                        PlannedOutcome = 0,
                                        PlannedOutcomeTestSteps = 0,
                                        PlannedProcessingTime = 0,
                                        PlannedProductivity = 0,
                                        PlannedReviewRounds = 0
                                    };
                                    objBusinessLayer = new BusinessLayer(ls);
                                    objBusinessLayer.TSOServiceDeliveryChainTaskActualRepository.Add(objTSOServiceDeliveryChainActual);
                                    objBusinessLayer = new BusinessLayer(ls);
                                    objBusinessLayer.TSOServiceDeliveryChainTaskPlannedRepository.Add(objTSOServiceDeliveryChainPlanned);

                                    //if (objTSOServiceDeliveryChain.ServiceDeliveryChainId == 42)
                                    //{
                                    //    TSOServiceDeliveryChainActualRisk objTSOServiceDeliveryChainActualRisk = new TSOServiceDeliveryChainActualRisk
                                    //    {
                                    //        TSOServiceDeliveryChainId = objTSOServiceDeliveryChain.ID,
                                    //        WeekNumber = GetWeekOfYear(DateTime.Now),
                                    //        Year = DateTime.Now.Year,
                                    //        ActualOperationalRiskId = 3,
                                    //        ActualOperationalRiskIndicatorId = 1,
                                    //        ActualOperationalRiskDescription = "",
                                    //        ActualOperationalRiskMitigation = "",
                                    //        ResponsiblePersonId = tsr.DeliveryManagerId,
                                    //        DueDate = DateTime.Now,
                                    //        Impact = "Risk Impact",
                                    //        Resolution = "Risk Resolution",
                                    //        StatusId = 1,
                                    //        RaisedById = tsr.DeliveryManagerId,
                                    //    };

                                    //    //TSOServiceDeliveryChainPlannedRisk objTSOServiceDeliveryChainPlannedRisk = new TSOServiceDeliveryChainPlannedRisk
                                    //    //{
                                    //    //    TSOServiceDeliveryChainId = objTSOServiceDeliveryChain.ID,
                                    //    //    WeekNumber = GetWeekOfYear(DateTime.Now) +1 ,
                                    //    //    Year = DateTime.Now.Year,
                                    //    //    PlannedOperationalRiskId = 3,
                                    //    //    PlannedOperationalRiskIndicatorId = 1,
                                    //    //    PlannedOperationalRiskDescription = "",
                                    //    //    PlannedOperationalRiskMitigation = "",
                                    //    //};

                                    //    objBusinessLayer = new BusinessLayer(ls);
                                    //    objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.Add(objTSOServiceDeliveryChainActualRisk);
                                    //    //objBusinessLayer = new BusinessLayer(ls);
                                    //    //objBusinessLayer.TSOServiceDeliveryChainPlannedRiskRepository.Add(objTSOServiceDeliveryChainPlannedRisk);

                                    //    SetOperationRiskToTSO(objTSOServiceDeliveryChain.ID, objTSO.UpdatedBy);
                                    //};                              
                                                                       
                                }
                            }
                            else
                            {
                                //Add core services to TSO
                                foreach (string tsrCoreService in objTSODTO.TSOServiceDeliveryChainArr.Split(','))
                                {
                                    TSOServiceDeliveryChain objTSOServiceDeliveryChain = new TSOServiceDeliveryChain
                                    {
                                        ServiceDeliveryChainId = int.Parse(tsrCoreService),
                                        TSOId = objTSO.ID,
                                        PlannedStartDate = objTSO.StartDate,
                                        PlannedCompletionDate = objTSO.TargetCompletionDate,
                                        TaskStatusId = objTaskStatus.ID,
                                        Notes = String.Empty
                                    };

                                    objBusinessLayer = new BusinessLayer(ls);
                                    objBusinessLayer.TSOServiceDeliveryChainRepository.Add(objTSOServiceDeliveryChain);

                                    //Create a Task and set the default value

                                    if (null != objTSOServiceDeliveryChain && objTSOServiceDeliveryChain.ID > 0)
                                    {

                                        TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainActual = new TSOServiceDeliveryChainTaskActual
                                        {
                                            TSOServiceDeliveryChainTaskId = objTSOServiceDeliveryChain.ID,
                                            WeekNumber = GetWeekOfYear(DateTime.Now),
                                            Year = DateTime.Now.Year,
                                            ActualEffort = 0,
                                            ActualInput = 0,
                                            ActualOutcome = 0,
                                            ActualOutcomeTestSteps = 0,
                                            ActualProcessingTime = 0,
                                            ActualProductivity = 0,
                                            ActualReviewRounds = 0,
                                            DefectRaised = 0,
                                            DefectRejected = 0,
                                            Headcount = 0,
                                            IdleTimeDuration = 0,
                                            IdleTimeEffort = 0,
                                        };
                                        TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainPlanned = new TSOServiceDeliveryChainTaskPlanned
                                        {
                                            TSOServiceDeliveryChainTaskId = objTSOServiceDeliveryChain.ID,
                                            WeekNumber = GetWeekOfYear(DateTime.Now) + 1,
                                            Year = DateTime.Now.Year,
                                            PlannedEffort = 0,
                                            PlannedInput = 0,
                                            PlannedOutcome = 0,
                                            PlannedOutcomeTestSteps = 0,
                                            PlannedProcessingTime = 0,
                                            PlannedProductivity = 0,
                                            PlannedReviewRounds = 0
                                        };

                                        objBusinessLayer = new BusinessLayer(ls);
                                        objBusinessLayer.TSOServiceDeliveryChainTaskActualRepository.Add(objTSOServiceDeliveryChainActual);
                                        objBusinessLayer = new BusinessLayer(ls);
                                        objBusinessLayer.TSOServiceDeliveryChainTaskPlannedRepository.Add(objTSOServiceDeliveryChainPlanned);

                                        //if (objTSOServiceDeliveryChain.ServiceDeliveryChainId == 42)
                                        //{
                                        //    TSOServiceDeliveryChainActualRisk objTSOServiceDeliveryChainActualRisk = new TSOServiceDeliveryChainActualRisk
                                        //    {
                                        //        TSOServiceDeliveryChainId = objTSOServiceDeliveryChain.ID,
                                        //        WeekNumber = GetWeekOfYear(DateTime.Now),
                                        //        Year = DateTime.Now.Year,
                                        //        ActualOperationalRiskId = 3,
                                        //        ActualOperationalRiskIndicatorId = 1,
                                        //        ActualOperationalRiskDescription = "",
                                        //        ActualOperationalRiskMitigation = "",
                                        //        ResponsiblePersonId = tsr.DeliveryManagerId,
                                        //        DueDate = DateTime.Now,
                                        //        Impact = "Risk Impact",
                                        //        Resolution = "Risk Resolution",
                                        //        StatusId = 1,
                                        //        RaisedById= tsr.DeliveryManagerId,
                                        //    };

                                        //    //TSOServiceDeliveryChainPlannedRisk objTSOServiceDeliveryChainPlannedRisk = new TSOServiceDeliveryChainPlannedRisk
                                        //    //{
                                        //    //    TSOServiceDeliveryChainId = objTSOServiceDeliveryChain.ID,
                                        //    //    WeekNumber = GetWeekOfYear(DateTime.Now) + 1,
                                        //    //    Year = DateTime.Now.Year,
                                        //    //    PlannedOperationalRiskId = 3,
                                        //    //    PlannedOperationalRiskIndicatorId = 1,
                                        //    //    PlannedOperationalRiskDescription = "",
                                        //    //    PlannedOperationalRiskMitigation = "",
                                        //    //};

                                        //    objBusinessLayer = new BusinessLayer(ls);
                                        //    objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.Add(objTSOServiceDeliveryChainActualRisk);
                                        //    //objBusinessLayer = new BusinessLayer(ls);
                                        //    //objBusinessLayer.TSOServiceDeliveryChainPlannedRiskRepository.Add(objTSOServiceDeliveryChainPlannedRisk);

                                        //    SetOperationRiskToTSO(objTSOServiceDeliveryChain.ID, objTSO.UpdatedBy);
                                        //};



                                    }
                                }
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(objTSODTO.TSOProductivityInputsArr))
                        {
                            if (objTSODTO.TSOProductivityInputsArr.IndexOf(",") <= 0)
                            {
                                objBusinessLayer = new BusinessLayer(ls);

                                TSOProductivityInput objTSOProductivityInput = new TSOProductivityInput
                                {
                                    ProductivityInputId = int.Parse(objTSODTO.TSOProductivityInputsArr),
                                    TSOId = objTSO.ID
                                };

                                objBusinessLayer.TSOProductivityInputRepository.Add(objTSOProductivityInput);

                            }
                            else
                            {
                                //Add core services to TSO
                                foreach (string tsrInputType in objTSODTO.TSOProductivityInputsArr.Split(','))
                                {


                                    TSOProductivityInput objTSOProductivityInput = new TSOProductivityInput
                                    {
                                        ProductivityInputId = int.Parse(tsrInputType),
                                        TSOId = objTSO.ID
                                    };
                                    objBusinessLayer = new BusinessLayer(ls);
                                    objBusinessLayer.TSOProductivityInputRepository.Add(objTSOProductivityInput);


                                }
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(objTSODTO.TSOProductivityOutcomesArr))
                        {
                            if (objTSODTO.TSOProductivityOutcomesArr.IndexOf(",") <= 0)
                            {
                                objBusinessLayer = new BusinessLayer(ls);

                                TSOProductivityOutcome objTSOProductivityOutCome = new TSOProductivityOutcome
                                {
                                    ProductivityOutcomeId = int.Parse(objTSODTO.TSOProductivityOutcomesArr),
                                    TSOId = objTSO.ID
                                };

                                objBusinessLayer.TSOProductivityOutcomeRepository.Add(objTSOProductivityOutCome);

                            }
                            else
                            {
                                //Add core services to TSO
                                foreach (string tsrOutcomeType in objTSODTO.TSOProductivityOutcomesArr.Split(','))
                                {
                                    objBusinessLayer = new BusinessLayer(ls);

                                    TSOProductivityOutcome objTSOProductivityOutCome = new TSOProductivityOutcome
                                    {
                                        ProductivityOutcomeId = int.Parse(tsrOutcomeType),
                                        TSOId = objTSO.ID
                                    };

                                    objBusinessLayer.TSOProductivityOutcomeRepository.Add(objTSOProductivityOutCome);

                                }
                            }
                        }
                    }

                    UserController user = new UserController();
                    if (objTSODTO.TSR == null)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        TSR objTSR = objBusinessLayer.TSRRepository.GetByID(objTSODTO.TSRId);
                        if (null != objTSR)
                        {
                            objTSODTO.TSR = Conversions.ToDTO<TSRDTO, TSR>(objTSR);
                        }
                    }
                    if (objTSODTO.TSR.Client == null)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        Client objClient = objBusinessLayer.ClientRepository.GetByID(objTSODTO.TSR.ClientId);
                        if (null != objClient)
                        {
                            objTSODTO.TSR.Client = Conversions.ToDTO<ClientDTO, Client>(objClient);
                        }
                    }
                    System.Threading.Tasks.Task.Factory.StartNew(() => user.UserAssignmentAndSendMail(objTSO.TeamLeadId, "TSO", objTSODTO.Title, objTSO.ID.ToString(), objTSODTO.CreatedBy, objTSODTO.TSR.Client.Name));

                    returnMessage = objTSO.ID.ToString();
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return returnMessage;
        }

        private void SetOperationRiskToTSO(int tsoServiceDeliveryChainId, string UpdatedBy)
        {

            OperationalRisk operationalRisk = null;
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            TSOServiceDeliveryChainActualRisk actualRiskobj = null;

            List<OperationalRisk> opList = new List<OperationalRisk>();
            try
            {
                int week = GetWeekOfYear(DateTime.Now);
                TSOServiceDeliveryChain objTSOServiceDeliveryChain = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == tsoServiceDeliveryChainId, x => x.TSO);
                TSO tso = objTSOServiceDeliveryChain.TSO;
                objBusinessLayer = new BusinessLayer(ls);
                List<TSOServiceDeliveryChain> TSOServiceDeliveryChain = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSOId == tso.ID && x.ServiceDeliveryChainId == 42, x => x.TSOServiceDeliveryChainActualRisks.Select(y => y.ActualOperationalRisk)).ToList();
                foreach (var item in TSOServiceDeliveryChain)
                {
                    List<TSOServiceDeliveryChainActualRisk> actualRisk = item.TSOServiceDeliveryChainActualRisks.Where(x => x.WeekNumber == week).ToList();
                    actualRiskobj = actualRisk.OrderByDescending(x => x.ActualOperationalRisk.RiskNo).FirstOrDefault();
                    opList.Add(actualRiskobj.ActualOperationalRisk);
                }
                if (opList.Count() > 0)
                {
                    operationalRisk = opList.OrderByDescending(x => x.RiskNo).FirstOrDefault();
                    if (tso.OperationalRiskId != operationalRisk.ID)
                    {
                        tso.OperationalRiskId = operationalRisk.ID;
                        ls.LoginName = UpdatedBy;
                        tso.UpdatedBy = UpdatedBy;
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSORepository.Update(tso);
                    }
                }

                opList.Clear();
                objBusinessLayer = new BusinessLayer(ls);
                List<TSO> TSO = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tso.TSRId, x => x.OperationalRisk).ToList();
                foreach (var item in TSO)
                {
                    opList.Add(item.OperationalRisk);
                }
                if (opList.Count() > 0)
                {
                    operationalRisk = opList.OrderByDescending(x => x.RiskNo).FirstOrDefault();

                    objBusinessLayer = new BusinessLayer(ls);
                    TSR tsrobj = objBusinessLayer.TSRRepository.GetByID(tso.TSRId);

                    if (tsrobj.OperationalRiskId != operationalRisk.ID)
                    {
                        tsrobj.OperationalRiskId = operationalRisk.ID;
                        ls.LoginName = UpdatedBy;
                        tsrobj.UpdatedBy = UpdatedBy;
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSRRepository.Update(tsrobj);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

        }

        /// <summary>
        /// Function to update TSO 
        /// </summary>
        /// <param name="objTSO"></param>
        /// <returns></returns>
        private string ModifyTSO(int ID, TSODTO objTSODTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            bool statusChange = false;
            try
            {
                TSO chkTSO = null;
                chkTSO = objBusinessLayer.TSORepository.GetSingle(x => x.Title == objTSODTO.Title && x.TSRId == objTSODTO.TSRId && x.ID != ID);

                if (chkTSO != null)
                {
                    returnMessage = "Error: TSO title already exists.";
                }
                else
                {
                    TSODTO objTSODTOById = this.GetTSOById(ID, false);

                    if (objTSODTOById != null)
                    {
                        ls = new LoginSession();
                        ls.LoginName = objTSODTO.UpdatedBy;

                        objTSODTOById.Title = objTSODTO.Title;
                        objTSODTOById.TeamLeadId = objTSODTO.TeamLeadId;
                        objTSODTOById.Description = objTSODTO.Description;
                        //objTSODTOById.TSRId = objTSODTO.TSRId;
                        objTSODTOById.CoreServiceId = objTSODTO.CoreServiceId;
                        objTSODTOById.CoreService = null;
                        objTSODTOById.RelevantRepositoryId = objTSODTO.RelevantRepositoryId;
                        objTSODTOById.RelevantRepository = null;
                        objTSODTOById.StartDate = objTSODTO.StartDate;
                        objTSODTOById.TargetCompletionDate = objTSODTO.TargetCompletionDate;
                        objTSODTOById.EstimatedEffort = objTSODTO.EstimatedEffort;
                        objTSODTOById.PlannedOutcome = objTSODTO.PlannedOutcome;
                        objTSODTOById.PlannedEffort = objTSODTO.PlannedEffort;
                        objTSODTOById.PlannedEffort = objTSODTO.PlannedEffort;
                        objTSODTOById.OperationalRiskId = objTSODTO.OperationalRiskId;
                        objTSODTOById.TSOStatusID = objTSODTO.TSOStatusID;

                        objBusinessLayer = new BusinessLayer(ls);
                        chkTSO = objBusinessLayer.TSORepository.GetByID(ID);
                        if (chkTSO.TSOStatusID != objTSODTO.TSOStatusID)
                        {
                            statusChange = true;
                        }

                        objTSODTOById.TSR = null;
                        objTSODTOById.OperationalRisk = null;
                        objTSODTOById.SP_Id = objTSODTO.SP_Id;

                        TSO objTSO = Conversions.ToEntity<TSODTO, TSO>(objTSODTOById);
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSORepository.Update(objTSO);

                        if (null != objTSO && objTSO.ID > 0)
                        {
                            //Delete TSO Core Services
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSODTOById = this.GetTSOById(ID);

                            objBusinessLayer = new BusinessLayer(ls);
                            string arrExistingTSOServiceDeliveryChain = string.Empty;

                            List<int> arrTSOServiceDeliveryChainToDelete = new List<int>();
                            List<int> arrTSOServiceDeliveryChainToInsert = new List<int>();

                            //Get the Service Delivery chain which needs to be deleted
                            foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSODTOById.TSOServiceDeliveryChains)
                            {
                                bool Found = false;

                                if (objTSODTO.TSOServiceDeliveryChainArr.IndexOf(",") <= 0)
                                {
                                    if (objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId.ToString() == objTSODTO.TSOServiceDeliveryChainArr)
                                    {
                                        Found = true;
                                    }
                                }
                                else
                                {
                                    foreach (string strTSOServiceDeliveryChainId in objTSODTO.TSOServiceDeliveryChainArr.Split(','))
                                    {
                                        if (objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId.ToString() == strTSOServiceDeliveryChainId)
                                        {
                                            Found = true;
                                            break;
                                        }
                                    }
                                }

                                if (!Found)
                                {
                                    arrTSOServiceDeliveryChainToDelete.Add(objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId);
                                }
                            }

                            //Get the Service Delivery chain which needs to be added
                            if (objTSODTO.TSOServiceDeliveryChainArr.IndexOf(",") <= 0)
                            {
                                bool Found = false;
                                foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSODTOById.TSOServiceDeliveryChains)
                                {
                                    if (objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId.ToString() == objTSODTO.TSOServiceDeliveryChainArr)
                                    {
                                        Found = true;
                                        break;
                                    }
                                }

                                if (!Found)
                                {
                                    arrTSOServiceDeliveryChainToInsert.Add(int.Parse(objTSODTO.TSOServiceDeliveryChainArr));
                                }
                            }
                            else
                            {
                                foreach (string strTSOServiceDeliveryChainId in objTSODTO.TSOServiceDeliveryChainArr.Split(','))
                                {
                                    bool Found = false;

                                    foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSODTOById.TSOServiceDeliveryChains)
                                    {
                                        if (objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId.ToString() == strTSOServiceDeliveryChainId)
                                        {
                                            Found = true;
                                            break;
                                        }
                                    }

                                    if (!Found)
                                    {
                                        arrTSOServiceDeliveryChainToInsert.Add(int.Parse(strTSOServiceDeliveryChainId));
                                    }
                                }
                            }

                            //Delete the removed service chain
                            foreach (int deleteId in arrTSOServiceDeliveryChainToDelete)
                            {
                                objBusinessLayer = new BusinessLayer(ls);

                                TSOServiceDeliveryChain chkobj = null;
                                chkobj = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.TSOId == objTSO.ID && x.ServiceDeliveryChainId == deleteId);
                                if (chkobj != null)
                                {
                                    objBusinessLayer = new BusinessLayer(ls);
                                    objBusinessLayer.TSOServiceDeliveryChainRepository.Delete(x => x.ServiceDeliveryChainId == chkobj.ID);

                                    objBusinessLayer = new BusinessLayer(ls);
                                    objBusinessLayer.TSOServiceDeliveryChainRepository.Delete(x => x.TSOId == objTSO.ID && x.ServiceDeliveryChainId == deleteId);
                                }
                            }

                            //Insert new service delivery chain
                            foreach (int insertId in arrTSOServiceDeliveryChainToInsert)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                TSOServiceDeliveryChain tSRCoreService = new TSOServiceDeliveryChain { ServiceDeliveryChainId = insertId, TSOId = objTSO.ID };
                                objBusinessLayer.TSOServiceDeliveryChainRepository.Add(tSRCoreService);
                            }
                        }

                        if (statusChange)
                        {
                            UserController user = new UserController();
                            System.Threading.Tasks.Task.Factory.StartNew(() => user.TSOStatusUpdation(objTSODTOById.TeamLeadId, "TSO", objTSODTOById.Title, objTSODTOById.ID.ToString(), objTSODTOById.CreatedBy, objTSODTOById.TSOStatus.Name));
                            System.Threading.Tasks.Task.Factory.StartNew(() => user.TSOStatusUpdation(objTSODTOById.TSR.TestManagerId, "TSO", objTSODTOById.Title, objTSODTOById.ID.ToString(), objTSODTOById.CreatedBy, objTSODTOById.TSOStatus.Name));
                        }

                        returnMessage = "TSO updated successfully.";

                    }
                    else
                    {
                        returnMessage = "TSO do not exists.";
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                if (null != objBusinessLayer)
                    objBusinessLayer.Dispose();
            }
            return returnMessage;
        }

        /// <summary>
        /// RemoveTSO
        /// </summary>
        /// <param name="ID">int</param>
        private string RemoveTSO(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                TSODTO objTSODTOById = this.GetTSOById(ID);

                if (objTSODTOById != null)
                {
                    if (objTSODTOById.TSOServiceDeliveryChains != null && objTSODTOById.TSOServiceDeliveryChains.Count > 0)
                    {
                        returnMessage = "Error: Can not delete TSO as Service delivery tasks exists for it.";
                    }
                    else
                    {
                        ls = new LoginSession();
                        ls.LoginName = "Admin";

                        objBusinessLayer = new BusinessLayer(ls);


                        TSO objTSO = Conversions.ToEntity<TSODTO, TSO>(objTSODTOById);
                        objBusinessLayer.TSORepository.Delete(objTSO);
                        returnMessage = "TSO deleted successfully.";
                    }
                }
                else
                {
                    returnMessage = "TSO do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                if (null != objBusinessLayer)
                    objBusinessLayer.Dispose();
            }
            return returnMessage;
        }

        /// <summary>
        /// AddNewTask
        /// </summary>
        /// <param name="tsoServiceDeliveryChainTaskDTO">TSOServiceDeliveryChainDTO</param>
        /// <returns></returns>
        private string AddNewTask(TSOServiceDeliveryChainDTO tsoServiceDeliveryChainTaskDTO, TSOServiceDeliveryChainTaskActualDTO tsoServiceDeliveryChainTaskActualDTO, TSOServiceDeliveryChainTaskPlannedDTO tsoServiceDeliveryChainTaskPlannedDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = tsoServiceDeliveryChainTaskDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                TSOServiceDeliveryChain objTSOServiceDeliveryChain = Conversions.ToEntity<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(tsoServiceDeliveryChainTaskDTO);
                TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainActual = Conversions.ToEntity<TSOServiceDeliveryChainTaskActualDTO, TSOServiceDeliveryChainTaskActual>(tsoServiceDeliveryChainTaskActualDTO);
                TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainPlanned = Conversions.ToEntity<TSOServiceDeliveryChainTaskPlannedDTO, TSOServiceDeliveryChainTaskPlanned>(tsoServiceDeliveryChainTaskPlannedDTO);

                TSOServiceDeliveryChain existstask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == objTSOServiceDeliveryChain.ID, x => x.TSOServiceDeliveryChainActualTasks, x => x.TSOServiceDeliveryChainPlannedTasks);
                //x.tsoServiceDeliveryChainTaskActual.FirstOrDefault().WeekNumber == objTSOServiceDeliveryChain.WeekNumber
                TSOServiceDeliveryChainTaskActual existstaskactual = existstask.TSOServiceDeliveryChainActualTasks.FirstOrDefault(x => x.WeekNumber == objTSOServiceDeliveryChainActual.WeekNumber);
                int plannedweekNumber = objTSOServiceDeliveryChainActual.WeekNumber + 1;
                TSOServiceDeliveryChainTaskPlanned existstaskplanned = existstask.TSOServiceDeliveryChainPlannedTasks.FirstOrDefault(x => x.WeekNumber == plannedweekNumber);


                #region Fill data for missing Week
                //Check if there is any missing week
                //objBusinessLayer = new BusinessLayer(ls);
                //TSOServiceDeliveryChain objTSOServiceDeliveryChainLast = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.TSOServiceDeliveryChainId == objTSOServiceDeliveryChain.TSOServiceDeliveryChainId && x.WeekNumber < objTSOServiceDeliveryChain.WeekNumber, x => x.WeekNumber, true);


                //if (null != objTSOServiceDeliveryChainLast)
                //{
                //    int lastEnteredWeekNumber = objTSOServiceDeliveryChainLast.WeekNumber;
                //    int differenceWeekNumnber = objTSOServiceDeliveryChain.WeekNumber - lastEnteredWeekNumber;

                //    objBusinessLayer = new BusinessLayer(ls);
                //    TaskStatus objTaskStatus = objBusinessLayer.TaskStatusRepository.GetSingle(x => x.Name == "In Progress");

                //    for (int weekNumber = 1; weekNumber <= differenceWeekNumnber; weekNumber++)
                //    {
                //        lastEnteredWeekNumber++;
                //        int a = objTSOServiceDeliveryChain.WeekNumber - 1;
                //        //Check if missing week number is not current week number
                //        if (lastEnteredWeekNumber != a )
                //        {
                //            TSOServiceDeliveryChain objTSOServiceDeliveryChainMissing = Conversions.ToEntity<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(tsoServiceDeliveryChainTaskDTO);
                //            objTSOServiceDeliveryChainMissing.WeekNumber = lastEnteredWeekNumber;

                //            objTSOServiceDeliveryChainMissing.ActualEffort = 0;
                //            objTSOServiceDeliveryChainMissing.ActualInput = 0;
                //            objTSOServiceDeliveryChainMissing.ActualOutcome = 0;
                //            objTSOServiceDeliveryChainMissing.ActualOutcomeTestSteps = 0;
                //            objTSOServiceDeliveryChainMissing.ActualProcessingTime = 0;
                //            objTSOServiceDeliveryChainMissing.ActualProductivity = 0;
                //            objTSOServiceDeliveryChainMissing.ActualReviewRounds = 0;
                //            objTSOServiceDeliveryChainMissing.DefectRaised = 0;
                //            objTSOServiceDeliveryChainMissing.DefectRejected = 0;
                //            objTSOServiceDeliveryChainMissing.Headcount = 0;
                //            objTSOServiceDeliveryChainMissing.IdleTimeDuration = 0;
                //            objTSOServiceDeliveryChainMissing.IdleTimeEffort = 0;

                //            objTSOServiceDeliveryChainMissing.Notes = String.Empty;
                //            objTSOServiceDeliveryChainMissing.PlannedEffort = 0;
                //            objTSOServiceDeliveryChainMissing.PlannedInput = 0;
                //            objTSOServiceDeliveryChainMissing.PlannedOutcome = 0;
                //            objTSOServiceDeliveryChainMissing.PlannedOutcomeTestSteps = 0;
                //            objTSOServiceDeliveryChainMissing.PlannedProcessingTime = 0;
                //            objTSOServiceDeliveryChainMissing.PlannedProductivity = 0;
                //            objTSOServiceDeliveryChainMissing.PlannedReviewRounds = 0;
                //            objTSOServiceDeliveryChainMissing.TaskStatusId = objTaskStatus.ID;

                //            objBusinessLayer = new BusinessLayer(ls);
                //            objBusinessLayer.TSOServiceDeliveryChainRepository.Add(objTSOServiceDeliveryChain);
                //        }
                //    }
                //}
                #endregion

                if (existstask != null && existstaskactual != null && existstaskplanned != null)
                {
                    objTSOServiceDeliveryChain.CreatedOn = existstask.CreatedOn;
                    objTSOServiceDeliveryChain.UpdatedOn = DateTime.Today;
                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.TSOServiceDeliveryChainRepository.Update(objTSOServiceDeliveryChain);

                    if (existstaskactual != null)
                    {
                        objTSOServiceDeliveryChainActual.CreatedOn = existstaskactual.CreatedOn;
                        objTSOServiceDeliveryChainActual.UpdatedOn = DateTime.Today;
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSOServiceDeliveryChainTaskActualRepository.Update(objTSOServiceDeliveryChainActual);
                    }

                    if (existstaskplanned != null)
                    {
                        objTSOServiceDeliveryChainPlanned.CreatedOn = existstaskplanned.CreatedOn;
                        objTSOServiceDeliveryChainPlanned.UpdatedOn = DateTime.Today;
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSOServiceDeliveryChainTaskPlannedRepository.Update(objTSOServiceDeliveryChainPlanned);
                    }

                    returnMessage = "Task updated successfully";
                }
                else if(existstask != null && existstaskactual != null && existstaskplanned == null)
                {
                    objTSOServiceDeliveryChain.CreatedOn = existstask.CreatedOn;
                    objTSOServiceDeliveryChain.UpdatedOn = DateTime.Today;
                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.TSOServiceDeliveryChainRepository.Update(objTSOServiceDeliveryChain);

                    if (existstaskactual != null)
                    {
                        objTSOServiceDeliveryChainActual.CreatedOn = existstaskactual.CreatedOn;
                        objTSOServiceDeliveryChainActual.UpdatedOn = DateTime.Today;
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSOServiceDeliveryChainTaskActualRepository.Update(objTSOServiceDeliveryChainActual);
                    }

                    if (existstaskplanned == null)
                    {
                        objTSOServiceDeliveryChainPlanned.CreatedOn = existstaskactual.CreatedOn;
                        objTSOServiceDeliveryChainPlanned.UpdatedOn = DateTime.Today;
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSOServiceDeliveryChainTaskPlannedRepository.Add(objTSOServiceDeliveryChainPlanned);
                    }

                    returnMessage = "Task updated successfully";

                }
                else
                {
                    //int week = GetWeekOfYear(DateTime.Now);
                    //if (objTSOServiceDeliveryChainActual.WeekNumber < week)
                    //{
                    //    TTMLogger.Logger.LogError(String.Format("Error - {0}", "You can not create privious week data."));
                    //    throw new Exception("You can not create previous week data.");
                    //}

                    objTSOServiceDeliveryChain.CreatedOn = existstask.CreatedOn;
                    objTSOServiceDeliveryChain.UpdatedOn = DateTime.Today;
                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.TSOServiceDeliveryChainRepository.Update(objTSOServiceDeliveryChain);

                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.TSOServiceDeliveryChainTaskActualRepository.Add(objTSOServiceDeliveryChainActual);

                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.TSOServiceDeliveryChainTaskPlannedRepository.Add(objTSOServiceDeliveryChainPlanned);

                    returnMessage = "Task saved successfully";
                }

                foreach (InputOutcomeDTO objInOut in tsoServiceDeliveryChainTaskDTO.InputOutcome)
                {
                    switch (objInOut.Type)
                    {
                        case "ActualInput":
                            objBusinessLayer = new BusinessLayer(ls);
                            TSOProductivityInputActual ActualInput = objBusinessLayer.TSOProductivityInputActualRepository.GetSingle(x => x.TSOProductivityInputId == objInOut.ID && x.WeekNumber == objInOut.Week && x.Year == objInOut.Year && x.ChainID == objTSOServiceDeliveryChain.ID);
                            if (ActualInput != null)
                            {
                                ActualInput.InputValue = objInOut.Value;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOProductivityInputActualRepository.Update(ActualInput);
                            }
                            else
                            {
                                TSOProductivityInputActual AddActualInput = new TSOProductivityInputActual();
                                AddActualInput.InputValue = objInOut.Value;
                                AddActualInput.WeekNumber = objInOut.Week;
                                AddActualInput.Year = objInOut.Year;
                                AddActualInput.TSOProductivityInputId = objInOut.ID;
                                AddActualInput.ChainID = objTSOServiceDeliveryChain.ID;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOProductivityInputActualRepository.Add(AddActualInput);
                            }
                            break;
                        case "PlannedInput":
                            objBusinessLayer = new BusinessLayer(ls);
                            TSOProductivityInputPlanned PlannedInput = objBusinessLayer.TSOProductivityInputPlannedRepository.GetSingle(x => x.TSOProductivityInputId == objInOut.ID && x.WeekNumber == objInOut.Week + 1 && x.Year == objInOut.Year && x.ChainID == objTSOServiceDeliveryChain.ID);
                            if (PlannedInput != null)
                            {
                                PlannedInput.InputValue = objInOut.Value;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOProductivityInputPlannedRepository.Update(PlannedInput);
                            }
                            else
                            {
                                TSOProductivityInputPlanned AddPlannedInput = new TSOProductivityInputPlanned();
                                AddPlannedInput.InputValue = objInOut.Value;
                                AddPlannedInput.WeekNumber = objInOut.Week + 1;
                                AddPlannedInput.Year = objInOut.Year;
                                AddPlannedInput.TSOProductivityInputId = objInOut.ID;
                                AddPlannedInput.ChainID = objTSOServiceDeliveryChain.ID;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOProductivityInputPlannedRepository.Add(AddPlannedInput);
                            }
                            break;
                        case "ActualOutcome":
                            objBusinessLayer = new BusinessLayer(ls);
                            TSOProductivityOutcomeActual ActualOutcome = objBusinessLayer.TSOProductivityOutcomeActualRepository.GetSingle(x => x.TSOProductivityOutcomeId == objInOut.ID && x.WeekNumber == objInOut.Week && x.Year == objInOut.Year && x.ChainID == objTSOServiceDeliveryChain.ID);
                            if (ActualOutcome != null)
                            {
                                ActualOutcome.OutcomeValue = objInOut.Value;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOProductivityOutcomeActualRepository.Update(ActualOutcome);
                            }
                            else
                            {
                                TSOProductivityOutcomeActual AddActualOutcome = new TSOProductivityOutcomeActual();
                                AddActualOutcome.OutcomeValue = objInOut.Value;
                                AddActualOutcome.WeekNumber = objInOut.Week;
                                AddActualOutcome.Year = objInOut.Year;
                                AddActualOutcome.TSOProductivityOutcomeId = objInOut.ID;
                                AddActualOutcome.ChainID = objTSOServiceDeliveryChain.ID;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOProductivityOutcomeActualRepository.Add(AddActualOutcome);
                            }
                            break;
                        case "PlannedOutcome":
                            objBusinessLayer = new BusinessLayer(ls);
                            TSOProductivityOutcomePlanned PlannedOutcome = objBusinessLayer.TSOProductivityOutcomePlannedRepository.GetSingle(x => x.TSOProductivityOutcomeId == objInOut.ID && x.WeekNumber == objInOut.Week + 1 && x.Year == objInOut.Year && x.ChainID == objTSOServiceDeliveryChain.ID);
                            if (PlannedOutcome != null)
                            {
                                PlannedOutcome.OutcomeValue = objInOut.Value;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOProductivityOutcomePlannedRepository.Update(PlannedOutcome);
                            }
                            else
                            {
                                TSOProductivityOutcomePlanned AddPlannedOutcome = new TSOProductivityOutcomePlanned();
                                AddPlannedOutcome.OutcomeValue = objInOut.Value;
                                AddPlannedOutcome.WeekNumber = objInOut.Week + 1;
                                AddPlannedOutcome.Year = objInOut.Year;
                                AddPlannedOutcome.TSOProductivityOutcomeId = objInOut.ID;
                                AddPlannedOutcome.ChainID = objTSOServiceDeliveryChain.ID;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOProductivityOutcomePlannedRepository.Add(AddPlannedOutcome);
                            }
                            break;
                    }
                }
                ls = new LoginSession();
                ls.LoginName = tsoServiceDeliveryChainTaskDTO.CreatedBy;
                objBusinessLayer = new BusinessLayer(ls);

                TSOServiceDeliveryChain tSOServiceDeliveryChain = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == objTSOServiceDeliveryChain.ID, x => x.TSO, x => x.ServiceDeliveryChain);
                string name = tSOServiceDeliveryChain.ServiceDeliveryChain.Name;
                if (name == "A" || name == "D" || name == "I" || name == "E")
                {
                    ls = new LoginSession();
                    ls.LoginName = tsoServiceDeliveryChainTaskDTO.CreatedBy;
                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.TSORepository.Update(tSOServiceDeliveryChain.TSO);

                }

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return returnMessage;
        }

        /// <summary>
        /// GetLastTSOServiceDeliveryChain
        /// </summary>
        /// <param name="tsoServiceDeliveryChainId">int</param>
        /// <returns>TSOServiceDeliveryChainDTO</returns>
        private TSOServiceDeliveryChainDTO GetLastTSOServiceDeliveryChain(int tsoServiceDeliveryChainId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO = null;

            try
            {
                //IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetListWithOrder(x => x.TSOServiceDeliveryChainId == tsoServiceDeliveryChainId,x => x.WeekNumber, x => x.TSOServiceDeliveryChain.ServiceDeliveryChain);

                TSOServiceDeliveryChain objTSOServiceDeliveryChainList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == tsoServiceDeliveryChainId, x => x.TSOServiceDeliveryChainActualTasks, x => x.TSOServiceDeliveryChainPlannedTasks, x => x.ServiceDeliveryChain, x => x.TSO,
                     x => x.TSO.TSOProductivityInputs, x => x.TSO.TSOProductivityOutcomes, x => x.TSO.TSOProductivityInputs.Select(y => y.ProductivityInput), x => x.TSO.TSOProductivityOutcomes.Select(z => z.ProductivityOutcome), x => x.TSOProductivityInputActuals, x => x.TSOProductivityInputPlanneds, x => x.TSOProductivityOutcomeActuals, x => x.TSOProductivityOutcomePlanneds);

                if (null != objTSOServiceDeliveryChainList)
                {
                    objTSOServiceDeliveryChainDTO = new TSOServiceDeliveryChainDTO();

                    double ActualEffortCumulative = 0;
                    double? ActualOutcomeCumulative = 0;
                    int ActualOutcomeTestStepsCumulative = 0;
                    double ActualProcessingTimeCumulative = 0;
                    int DefectRaisedCumulative = 0;
                    int DefectRejectedCumulative = 0;
                    double? ActualInputCumulative = 0;

                    double PlannedEffortCumulative = 0;
                    double? PlannedOutcomeCumulative = 0;
                    int PlannedOutcomeTestStepsCumulative = 0;
                    double PlannedProcessingTimeCumulative = 0;
                    double PlannedInputCumulative = 0;
                    int week = GetWeekOfYear(DateTime.Now);
                    int year = DateTime.Now.Year;

                    //foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSOServiceDeliveryChainList)
                    //{
                    objTSOServiceDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChainList);
                    foreach (TSOServiceDeliveryChainTaskActualDTO objTSOServiceDeliveryChainActualDTO in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks)
                    {
                        if (objTSOServiceDeliveryChainActualDTO.WeekNumber < week)
                        {
                            ActualEffortCumulative += objTSOServiceDeliveryChainActualDTO.ActualEffort ?? 0;
                            ActualOutcomeTestStepsCumulative += objTSOServiceDeliveryChainActualDTO.ActualOutcomeTestSteps;
                            ActualProcessingTimeCumulative += objTSOServiceDeliveryChainActualDTO.ActualProcessingTime;
                            DefectRaisedCumulative += objTSOServiceDeliveryChainActualDTO.DefectRaised;
                            DefectRejectedCumulative += objTSOServiceDeliveryChainActualDTO.DefectRejected;
                        }
                        //objBusinessLayer = new BusinessLayer(ls);
                        //OperationalRisk rk= objBusinessLayer.OperationalRiskRepository.GetByID(objTSOServiceDeliveryChainActualDTO.ActualOperationalRiskId);
                        //OperationalRiskDTO objOperationalRiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(rk);
                        //objBusinessLayer = new BusinessLayer(ls);
                        //OperationalRiskIndicator rki = objBusinessLayer.OperationalRiskIndicatorRepository.GetByID(objTSOServiceDeliveryChainActualDTO.ActualOperationalRiskIndicatorId);
                        //OperationalRiskIndicatorDTO objOperationalRiskIndicatorDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(rki);

                        objTSOServiceDeliveryChainActualDTO.ActualProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainActualDTO.ActualProcessingTime, 2);

                        //objTSOServiceDeliveryChainActualDTO.ActualOperationalRiskIndicator = objOperationalRiskIndicatorDTO;
                        //objTSOServiceDeliveryChainActualDTO.ActualOperationalRisk = objOperationalRiskDTO;
                    }

                    foreach (TSOServiceDeliveryChainTaskPlannedDTO objTSOServiceDeliveryChainPlannedDTO in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks)
                    {
                        // Planned Cumulative
                        if (objTSOServiceDeliveryChainPlannedDTO.WeekNumber <= week)
                        {
                            PlannedEffortCumulative += objTSOServiceDeliveryChainPlannedDTO.PlannedEffort ?? 0;
                            PlannedOutcomeTestStepsCumulative += objTSOServiceDeliveryChainPlannedDTO.PlannedOutcomeTestSteps;
                            PlannedProcessingTimeCumulative += objTSOServiceDeliveryChainPlannedDTO.PlannedProcessingTime;
                        }


                        //objBusinessLayer = new BusinessLayer(ls);
                        //OperationalRisk rk = objBusinessLayer.OperationalRiskRepository.GetByID(objTSOServiceDeliveryChainPlannedDTO.PlannedOperationalRisk);
                        //OperationalRiskDTO objOperationalRiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(rk);
                        //objBusinessLayer = new BusinessLayer(ls);
                        //OperationalRiskIndicator rki = objBusinessLayer.OperationalRiskIndicatorRepository.GetByID(objTSOServiceDeliveryChainPlannedDTO.PlannedOperationalRiskIndicator);
                        //OperationalRiskIndicatorDTO objOperationalRiskIndicatorDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(rki);
                        objTSOServiceDeliveryChainPlannedDTO.PlannedProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainPlannedDTO.PlannedProcessingTime, 2);

                        //objTSOServiceDeliveryChainPlannedDTO.PlannedOperationalRiskIndicator = objOperationalRiskIndicatorDTO;
                        //objTSOServiceDeliveryChainPlannedDTO.PlannedOperationalRisk = objOperationalRiskDTO;
                    }
                    // }


                    List<InputOutcomeCumulativeDTO> inOutCumulativeList = new List<InputOutcomeCumulativeDTO>();
                    List<InputOutcomeDTO> inOutList = new List<InputOutcomeDTO>();
                    //foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChainTask in objTSOServiceDeliveryChainList)
                    //{
                    // objTSOServiceDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChainList);

                    //if (objTSOServiceDeliveryChainDTO.WeekNumber < week)
                    //{
                    foreach (TSOProductivityInputDTO input in objTSOServiceDeliveryChainDTO.TSO.TSOProductivityInputs)
                    {
                        foreach (TSOProductivityInputActualDTO actual in objTSOServiceDeliveryChainDTO.TSOProductivityInputActuals)
                        {
                            float fInputPlanned = objTSOServiceDeliveryChainDTO.TSOProductivityInputActuals.Where(x => x.WeekNumber < week && x.TSOProductivityInputId == input.ID && x.Year == year).Sum(y => y.InputValue);

                            if (inOutCumulativeList.Count == 0)
                            {
                                InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                inout.ID = input.ProductivityInputId;
                                inout.Value = fInputPlanned;
                                inout.Week = week;
                                inout.Type = "ActualInput";
                                inout.Year = year;
                                inout.TSOPID = input.ID;
                                inOutCumulativeList.Add(inout);
                            }
                            else
                            {
                                IEnumerable<InputOutcomeCumulativeDTO> isExist = inOutCumulativeList.Where(x => x.ID == input.ProductivityInputId && x.Type == "ActualInput" && x.TSOPID == input.ID);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                    inout.ID = input.ProductivityInputId;
                                    inout.Value = fInputPlanned;
                                    inout.Week = week;
                                    inout.Type = "ActualInput";
                                    inout.Year = 0;
                                    inout.TSOPID = input.ID;
                                    inOutCumulativeList.Add(inout);
                                }
                            }

                            if (actual.WeekNumber == week && input.ID == actual.TSOProductivityInputId)
                            {
                                IEnumerable<InputOutcomeDTO> isExist = inOutList.Where(x => x.ID == input.ProductivityInputId && x.Type == "ActualInput"
                                && x.TSOPID == input.ID && x.Value == actual.InputValue && x.Week == week && x.Year == year);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeDTO inoutNew = new InputOutcomeDTO();
                                    inoutNew.ID = input.ProductivityInputId;
                                    inoutNew.Week = week;
                                    inoutNew.Type = "ActualInput";
                                    inoutNew.Year = year;
                                    inoutNew.TSOPID = input.ID;
                                    inoutNew.Value = actual.InputValue;
                                    inOutList.Add(inoutNew);
                                }
                            }
                        }
                    }

                    foreach (TSOProductivityOutcomeDTO input in objTSOServiceDeliveryChainDTO.TSO.TSOProductivityOutcomes)
                    {
                        foreach (TSOProductivityOutcomeActualDTO Actuals in objTSOServiceDeliveryChainDTO.TSOProductivityOutcomeActuals)
                        {
                            float fInputPlanned = objTSOServiceDeliveryChainDTO.TSOProductivityOutcomeActuals.Where(x => x.WeekNumber < week && x.TSOProductivityOutcomeId == input.ID && x.Year == year).Sum(y => y.OutcomeValue);

                            if (inOutCumulativeList.Count == 0)
                            {
                                InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                inout.ID = input.ProductivityOutcomeId;
                                inout.Value = fInputPlanned;
                                inout.Week = week;
                                inout.Type = "ActualOutcome";
                                inout.Year = year;
                                inout.TSOPID = input.ID;
                                inOutCumulativeList.Add(inout);
                            }
                            else
                            {
                                IEnumerable<InputOutcomeCumulativeDTO> isExist = inOutCumulativeList.Where(x => x.ID == input.ProductivityOutcomeId && x.Type == "ActualOutcome" && x.TSOPID == input.ID);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                    inout.ID = input.ProductivityOutcomeId;
                                    inout.Value = fInputPlanned;
                                    inout.Week = week;
                                    inout.Type = "ActualOutcome";
                                    inout.Year = year;
                                    inout.TSOPID = input.ID;
                                    inOutCumulativeList.Add(inout);
                                }
                            }

                            if (Actuals.WeekNumber == week && input.ID == Actuals.TSOProductivityOutcomeId)
                            {
                                IEnumerable<InputOutcomeDTO> isExist = inOutList.Where(x => x.ID == input.ProductivityOutcomeId && x.Type == "ActualOutcome"
                                && x.TSOPID == input.ID && x.Value == Actuals.OutcomeValue && x.Week == week && x.Year == year);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeDTO inoutNew = new InputOutcomeDTO();
                                    inoutNew.ID = input.ProductivityOutcomeId;
                                    inoutNew.Week = week;
                                    inoutNew.Type = "ActualOutcome";
                                    inoutNew.Year = year;
                                    inoutNew.TSOPID = input.ID;
                                    inoutNew.Value = Actuals.OutcomeValue;
                                    inOutList.Add(inoutNew);
                                }
                            }
                        }
                    }
                    // }

                    //Planned Cumulative
                    //if (objTSOServiceDeliveryChainDTO.WeekNumber <= week)
                    //{
                    foreach (TSOProductivityInputDTO input in objTSOServiceDeliveryChainDTO.TSO.TSOProductivityInputs)
                    {
                        foreach (TSOProductivityInputPlannedDTO planned in objTSOServiceDeliveryChainDTO.TSOProductivityInputPlanneds)
                        {
                            float fInputPlanned = objTSOServiceDeliveryChainDTO.TSOProductivityInputPlanneds.Where(x => x.WeekNumber < week + 1 && x.TSOProductivityInputId == input.ID && x.Year == year).Sum(y => y.InputValue);

                            //PlannedInputCumulative += fInputPlanned;

                            if (inOutCumulativeList.Count == 0)
                            {
                                InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                inout.ID = input.ProductivityInputId;
                                inout.Value = fInputPlanned;
                                inout.Week = week;
                                inout.Type = "PlannedInput";
                                inout.Year = year;
                                inout.TSOPID = input.ID;
                                inOutCumulativeList.Add(inout);
                            }
                            else
                            {
                                IEnumerable<InputOutcomeCumulativeDTO> isExist = inOutCumulativeList.Where(x => x.ID == input.ProductivityInputId && x.Type == "PlannedInput" && x.TSOPID == input.ID);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                    inout.ID = input.ProductivityInputId;
                                    inout.Value = fInputPlanned;
                                    inout.Week = week;
                                    inout.Type = "PlannedInput";
                                    inout.Year = year;
                                    inout.TSOPID = input.ID;
                                    inOutCumulativeList.Add(inout);
                                }
                            }

                            if (planned.WeekNumber == week + 1 && input.ID == planned.TSOProductivityInputId)
                            {
                                IEnumerable<InputOutcomeDTO> isExist = inOutList.Where(x => x.ID == input.ProductivityInputId && x.Type == "PlannedInput"
                                && x.TSOPID == input.ID && x.Value == planned.InputValue && x.Week == week && x.Year == year);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeDTO inoutNew = new InputOutcomeDTO();
                                    inoutNew.ID = input.ProductivityInputId;
                                    inoutNew.Week = week;
                                    inoutNew.Type = "PlannedInput";
                                    inoutNew.Year = year;
                                    inoutNew.TSOPID = input.ID;
                                    inoutNew.Value = planned.InputValue;
                                    inOutList.Add(inoutNew);
                                }
                            }
                        }
                    }

                    foreach (TSOProductivityOutcomeDTO outcome in objTSOServiceDeliveryChainDTO.TSO.TSOProductivityOutcomes)
                    {
                        foreach (TSOProductivityOutcomePlannedDTO planned in objTSOServiceDeliveryChainDTO.TSOProductivityOutcomePlanneds)
                        {
                            float fInputPlanned = objTSOServiceDeliveryChainDTO.TSOProductivityOutcomePlanneds.Where(x => x.WeekNumber < week + 1 && x.TSOProductivityOutcomeId == outcome.ID && x.Year == year).Sum(y => y.OutcomeValue);

                            //PlannedOutcomeCumulative += fInputPlanned;

                            if (inOutCumulativeList.Count == 0)
                            {
                                InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                inout.ID = outcome.ProductivityOutcomeId;
                                inout.Value = fInputPlanned;
                                inout.Week = week;
                                inout.Type = "PlannedOutcome";
                                inout.Year = year;
                                inout.TSOPID = outcome.ID;
                                inOutCumulativeList.Add(inout);
                            }
                            else
                            {
                                IEnumerable<InputOutcomeCumulativeDTO> isExist = inOutCumulativeList.Where(x => x.ID == outcome.ProductivityOutcomeId && x.Type == "PlannedOutcome" && x.TSOPID == outcome.ID);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                    inout.ID = outcome.ProductivityOutcomeId;
                                    inout.Value = fInputPlanned;
                                    inout.Week = week;
                                    inout.Type = "PlannedOutcome";
                                    inout.Year = year;
                                    inout.TSOPID = outcome.ID;
                                    inOutCumulativeList.Add(inout);
                                }
                            }


                            if (planned.WeekNumber == week + 1 && outcome.ID == planned.TSOProductivityOutcomeId)
                            {
                                IEnumerable<InputOutcomeDTO> isExist = inOutList.Where(x => x.ID == outcome.ProductivityOutcomeId && x.Type == "PlannedInput"
                                && x.TSOPID == outcome.ID && x.Value == planned.OutcomeValue && x.Week == week && x.Year == year);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeDTO inoutNew = new InputOutcomeDTO();
                                    inoutNew.ID = outcome.ProductivityOutcomeId;
                                    inoutNew.Week = week;
                                    inoutNew.Type = "PlannedOutcome";
                                    inoutNew.Year = year;
                                    inoutNew.TSOPID = outcome.ID;
                                    inoutNew.Value = planned.OutcomeValue;
                                    inOutList.Add(inoutNew);
                                }
                            }
                        }
                    }
                    //}
                    //}

                    //Total of all input planned
                    ActualInputCumulative = objTSOServiceDeliveryChainDTO.TSOProductivityInputActuals.Where(x => x.WeekNumber <= week).Sum(y => y.InputValue);
                    ActualOutcomeCumulative = objTSOServiceDeliveryChainDTO.TSOProductivityOutcomeActuals.Where(x => x.WeekNumber <= week).Sum(y => y.OutcomeValue);

                    PlannedInputCumulative = objTSOServiceDeliveryChainDTO.TSOProductivityInputPlanneds.Where(x => x.WeekNumber < week + 2).Sum(y => y.InputValue);
                    PlannedOutcomeCumulative = objTSOServiceDeliveryChainDTO.TSOProductivityOutcomePlanneds.Where(x => x.WeekNumber < week + 2).Sum(y => y.OutcomeValue);

                    objTSOServiceDeliveryChainDTO.InputOutcomeCumulative = inOutCumulativeList;
                    objTSOServiceDeliveryChainDTO.InputOutcome = inOutList;
                    objTSOServiceDeliveryChainDTO.ActualEffortCumulative = ActualEffortCumulative;
                    objTSOServiceDeliveryChainDTO.ActualOutcomeCumulative = ActualOutcomeCumulative;
                    objTSOServiceDeliveryChainDTO.ActualOutcomeTestStepsCumulative = ActualOutcomeTestStepsCumulative;
                    objTSOServiceDeliveryChainDTO.ActualProcessingTimeCumulative = ActualProcessingTimeCumulative;
                    objTSOServiceDeliveryChainDTO.ActualInputCumulative = ActualInputCumulative;
                    objTSOServiceDeliveryChainDTO.DefectRaisedCumulative = DefectRaisedCumulative;
                    objTSOServiceDeliveryChainDTO.DefectRejectedCumulative = DefectRejectedCumulative;
                    if (objTSOServiceDeliveryChainDTO.TSO.TSR != null)
                    {
                        objTSOServiceDeliveryChainDTO.ResponsiblePerson = objTSOServiceDeliveryChainDTO.TSO.TSR.DeliveryManager;
                        objTSOServiceDeliveryChainDTO.ResponsiblePersonId = objTSOServiceDeliveryChainDTO.TSO.TSR.DeliveryManagerId;
                    }
                    else
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        TSR newTsr = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == objTSOServiceDeliveryChainDTO.TSO.TSRId, x => x.DeliveryManager);

                        objTSOServiceDeliveryChainDTO.ResponsiblePerson = Conversions.ToDTO<UserDTO, User>(newTsr.DeliveryManager);
                        objTSOServiceDeliveryChainDTO.ResponsiblePersonId = newTsr.DeliveryManagerId;
                    }

                    //Planned Cumulative
                    objTSOServiceDeliveryChainDTO.PlannedEffortCumulative = PlannedEffortCumulative;
                    objTSOServiceDeliveryChainDTO.PlannedOutcomeCumulative = PlannedOutcomeCumulative;
                    objTSOServiceDeliveryChainDTO.PlannedOutcomeTestStepsCumulative = PlannedOutcomeTestStepsCumulative;
                    objTSOServiceDeliveryChainDTO.PlannedProcessingTimeCumulative = PlannedProcessingTimeCumulative;
                    objTSOServiceDeliveryChainDTO.PlannedInputCumulative = PlannedInputCumulative;

                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objTSOServiceDeliveryChainDTO;
        }

        /// <summary>
        /// GetLastTSOServiceDeliveryChain
        /// </summary>
        /// <param name="tsoServiceDeliveryChainId">int</param>
        /// <returns>TSOServiceDeliveryChainDTO</returns>
        private IList<TSOServiceDeliveryChainDTO> GetLastTSOServiceDeliveryChainList(int tsoServiceDeliveryChainId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = new List<TSOServiceDeliveryChainDTO>();

            try
            {
                IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetListWithOrder(x => x.ID == tsoServiceDeliveryChainId, x => x.CreatedOn, x => x.TSOServiceDeliveryChainActualTasks, x => x.TSOServiceDeliveryChainPlannedTasks, x => x.TSOProductivityOutcomeActuals, x => x.TSOProductivityOutcomeActuals);

                if (null != objTSOServiceDeliveryChainList)
                {
                    foreach (TSOServiceDeliveryChain task in objTSOServiceDeliveryChainList)
                    {
                        //Not Needed new code
                        //objBusinessLayer = new BusinessLayer(ls);
                        //TSOServiceDeliveryChainActual objTSOServiceDeliveryChainActual = objBusinessLayer.TSOServiceDeliveryChainActualRepository.GetSingle(x => x.TSOServiceDeliveryChainId == task.ID);

                        //objBusinessLayer = new BusinessLayer(ls);
                        //TSOServiceDeliveryChainPlanned objTSOServiceDeliveryChainPlanned = objBusinessLayer.TSOServiceDeliveryChainPlannedRepository.GetSingle(x => x.TSOServiceDeliveryChainId == task.ID);

                        TSOServiceDeliveryChainDTO objTSOTaskDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(task);
                        objTSOServiceDeliveryChainDTOList.Add(objTSOTaskDTO);
                    }

                    double ActualEffortCumulative = 0;
                    int ActualOutcomeCumulative = 0;
                    int ActualOutcomeTestStepsCumulative = 0;
                    double ActualProcessingTimeCumulative = 0;
                    int DefectRaisedCumulative = 0;
                    int DefectRejectedCumulative = 0;

                    TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO = new TSOServiceDeliveryChainDTO();
                    foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSOServiceDeliveryChainList)
                    {
                        objTSOServiceDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
                        foreach (TSOServiceDeliveryChainTaskActualDTO objTSOServiceDeliveryChainActualDTO in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks)
                        {
                            ActualEffortCumulative += objTSOServiceDeliveryChainActualDTO.ActualEffort ?? 0;
                            ActualOutcomeCumulative += objTSOServiceDeliveryChainActualDTO.ActualOutcome;
                            ActualOutcomeTestStepsCumulative += objTSOServiceDeliveryChainActualDTO.ActualOutcomeTestSteps;
                            ActualProcessingTimeCumulative += objTSOServiceDeliveryChainActualDTO.ActualProcessingTime;
                            DefectRaisedCumulative += objTSOServiceDeliveryChainActualDTO.DefectRaised;
                            DefectRejectedCumulative += objTSOServiceDeliveryChainActualDTO.DefectRejected;

                            objTSOServiceDeliveryChainActualDTO.ActualProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainActualDTO.ActualProcessingTime, 2);
                        }

                        objTSOServiceDeliveryChainDTO.ActualEffortCumulative = ActualEffortCumulative;
                        objTSOServiceDeliveryChainDTO.ActualOutcomeCumulative = ActualOutcomeCumulative;
                        objTSOServiceDeliveryChainDTO.ActualOutcomeTestStepsCumulative = ActualOutcomeTestStepsCumulative;
                        objTSOServiceDeliveryChainDTO.ActualProcessingTimeCumulative = ActualProcessingTimeCumulative;
                        objTSOServiceDeliveryChainDTO.DefectRaisedCumulative = DefectRaisedCumulative;
                        objTSOServiceDeliveryChainDTO.DefectRejectedCumulative = DefectRejectedCumulative;
                        foreach (TSOServiceDeliveryChainTaskPlannedDTO objTSOServiceDeliveryChainPlannedDTO in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks)
                        {
                            objTSOServiceDeliveryChainPlannedDTO.PlannedProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainPlannedDTO.PlannedProcessingTime, 2);
                        }
                    }

                    //objTSOServiceDeliveryChainDTO.PlannedProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainDTO.PlannedProcessingTime, 2);
                    //objTSOServiceDeliveryChainDTO.ActualProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainDTO.ActualProcessingTime, 2);
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objTSOServiceDeliveryChainDTOList;
        }


        private int GetHighestOperationalRisk(int tsoServiceDeliveryChainId)
        {
            int operationalRisk=0;
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
                      

            try
            { int week=GetWeekOfYear(DateTime.Now);
                TSOServiceDeliveryChain objTSOServiceDeliveryChain= objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == tsoServiceDeliveryChainId, x => x.TSOServiceDeliveryChainActualRisks.Select(z => z.ActualOperationalRisk));
                List<TSOServiceDeliveryChainActualRisk> TSOServiceDeliveryChainActualRisk = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualRisks.Where(x => x.WeekNumber == week && x.StatusId!=3).ToList();
                if (TSOServiceDeliveryChainActualRisk.Count()!= 0)
                {
                    operationalRisk = TSOServiceDeliveryChainActualRisk.Max(x => x.ActualOperationalRisk.RiskNo);
                }
                else
                {
                    operationalRisk = 0;
                }


            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return operationalRisk;
        }

        private void SetOperationRiskToTSO(TSO tso, string UpdatedBy)
        {

            OperationalRisk operationalRisk = null;
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            TSOServiceDeliveryChainActualRisk actualRiskobj = null;

            List<OperationalRisk> opList = new List<OperationalRisk>();
            try
            {
                int week = GetWeekOfYear(DateTime.Now);             
                objBusinessLayer = new BusinessLayer(ls);
                TSO tsoobj = tso;//Conversions.ToEntity<TSODTO, TSO>(tso);
                List<TSOServiceDeliveryChain> TSOServiceDeliveryChain = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSOId == tso.ID && x.ServiceDeliveryChainId == 42, x => x.TSOServiceDeliveryChainActualRisks.Select(y => y.ActualOperationalRisk)).ToList();
                foreach (var item in TSOServiceDeliveryChain)
                {
                    List<TSOServiceDeliveryChainActualRisk> actualRisk = item.TSOServiceDeliveryChainActualRisks.Where(x => x.WeekNumber == week).ToList();
                    actualRiskobj = actualRisk.OrderByDescending(x => x.ActualOperationalRisk.RiskNo).FirstOrDefault();
                    if (actualRiskobj != null)
                    {
                        opList.Add(actualRiskobj.ActualOperationalRisk);
                    }
                }
                if (opList.Count() > 0)
                {
                    operationalRisk = opList.OrderByDescending(x => x.RiskNo).FirstOrDefault();
                    //if (tso.OperationalRiskId != operationalRisk.ID)
                    //{
                        tso.OperationalRiskId = operationalRisk.ID;
                        ls.LoginName = UpdatedBy;
                        tso.UpdatedBy = UpdatedBy;
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSORepository.Update(tsoobj);
                   // }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

        }
        #endregion

        #region internal functions

        /// <summary>
        /// GetTSOIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetTSOIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objTSOList = null;
            try
            {
                objTSOList = objBusinessLayer.TSORepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Title, Other = x.Description });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objTSOList;
        }

        #endregion

        [HttpGet, Route("v1/TSO/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objTSOList = null;

            try
            {
                objTSOList = this.GetTSOIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objTSOList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/TSO/GetById/{id}")]
        [ResponseType(typeof(TSO))]
        // [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager,Team Lead, Guest")]
        public JsonResult<TSODTO> GetById(int id)
        {
            JsonResult<TSODTO> result = null;
            TSODTO objTSODTO;

            try
            {
                objTSODTO = this.GetTSOById(id);

                result = Json(objTSODTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return result;
        }

        public TSODTO GetTSODTO(int id)
        {
            TSODTO objTSODTO;
            try
            {
                objTSODTO = this.GetTSOById(id);
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return objTSODTO;
        }

        /// <summary>
        /// GetAllPaged
        /// </summary>
        /// <param name="pageSize">int</param>
        /// <param name="startingRecordNumber">int</param>
        /// <returns>JsonResult<DataCollection>></returns>
        [HttpGet, Route("v1/TSO/GetAllPaged/{tsrId}/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<DataCollection> GetAllPaged(int tsrId, int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            string User = string.Empty;
            Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
            string userId = Request.Headers.GetValues("userid").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(userId))
            {
                userId = userId.Replace("~", "=").Replace("!", "+");
                User = objCryptorEngine.Decrypt(userId, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                User = usrInfo[usrInfo.Length - 2];
            }


            IList<TSODTO> objTSODTOList = this.GetAllTSO(tsrId, startingRecordNumber, pageSize, User, out totalRecords);
            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objTSODTOList)
                {
                    foreach (TSODTO objTSO in objTSODTOList)
                    {
                        objDataCollection.EntitySummary.Add(objTSO);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }

            return Json(objDataCollection, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// GetAllPaged
        /// </summary>
        /// <param name="pageSize">int</param>
        /// <param name="startingRecordNumber">int</param>
        /// <returns>JsonResult<DataCollection>></returns>
        [HttpGet, Route("v1/TSO/GetAllUserPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<DataCollection> GetAllUserPaged([FromUri] int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
            JsonResult<DataCollection> objReturn = null;
            string userId = Request.Headers.GetValues("userid").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(userId))
            {
                userId = userId.Replace("~", "=").Replace("!", "+");
                string User = objCryptorEngine.Decrypt(userId, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                User = usrInfo[usrInfo.Length - 2];

                IList<TSODTO> objTSODTOList = this.GetAllUserTSO(startingRecordNumber, pageSize, User, out totalRecords, isAdmin);

                DataCollection objDataCollection;

                try
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                    if (null != objTSODTOList)
                    {
                        foreach (TSODTO objTSO in objTSODTOList)
                        {
                            objDataCollection.EntitySummary.Add(objTSO);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                    throw;
                }

                objReturn = Json(objDataCollection, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return objReturn;
        }

        /// <summary>
        /// CreateTSO
        /// </summary>
        /// <param name="tsoDTO">TSODTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/TSO/CreateTSO")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Test Manager,TMO,Team Lead")]
        public JsonResult<string> CreateTSO([FromBody]TSODTO tsoDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewTSO(tsoDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (DbEntityValidationException eve)
            {
                StringBuilder objSB = new StringBuilder();
                foreach (var objEve in eve.EntityValidationErrors)
                {
                    objSB.Append(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", objEve.Entry.Entity.GetType().Name, objEve.Entry.State));
                    objSB.Append("\n");
                    foreach (var ve in objEve.ValidationErrors)
                    {
                        objSB.Append(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        objSB.Append("\n");
                    }
                }

                result = Json(String.Format("Error - {0}", objSB.ToString()), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// GetTSOCount
        /// </summary>
        /// <param name="strTitleOrClient"></param>
        /// <param name="status"></param>
        /// <param name="tsrId"></param>
        /// <returns></returns>
        [HttpGet, Route("v1/TSO/GetTSOCount/{strTitleOrClient}/{status}/{tsrId}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<string> GetTSOCount(string strTitleOrClient, int status, int tsrId = 0)
        {
            JsonResult<string> result = Json(string.Empty);
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                IList<TSO> objTSOList = null;
                Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
                string userId = Request.Headers.GetValues("userid").FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(userId))
                {
                    userId = userId.Replace("~", "=").Replace("!", "+");
                    string User = objCryptorEngine.Decrypt(userId, true);
                    bool isadmin = User.ToLower().Contains("admin");
                    string[] usrInfo = User.Split('#');
                    User = usrInfo[usrInfo.Length - 2];

                    int userID = Convert.ToInt32(User);
                    objBusinessLayer = new BusinessLayer(ls);
                    User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);

                    if (strTitleOrClient != "none")
                    {
                        if (status != 0)
                        {
                            if (tsrId > 0)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSOStatusID == status && x.TSRId == tsrId && (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                                else
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSOStatusID == status && x.TSRId == tsrId && (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                            }
                            else
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSOStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                                else
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSOStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);

                                }
                            }
                        }
                        else
                        {
                            if (tsrId > 0)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                                {

                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tsrId && (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                                else
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tsrId && (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                            }
                            else
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                                {

                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                                else
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => (x.Title.ToLower().Contains(strTitleOrClient) || x.TSR.Client.Name.ToLower().Contains(strTitleOrClient) || x.TSR.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);

                                }
                            }
                        }
                    }
                    else
                    {
                        if (status != 0)
                        {
                            if (tsrId > 0)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                                {

                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tsrId && x.TSOStatusID == status, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                                else
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tsrId && x.TSOStatusID == status && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                            }
                            else
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                                {

                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSOStatusID == status, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                                else
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSOStatusID == status && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID, x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSOServiceDeliveryChains, x => x.TSOStatus, x => x.TeamLead, x => x.CoreService, x => x.OperationalRisk, x => x.RelevantRepository);
                                }
                            }
                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            if (tsrId > 0)
                            {
                                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                                {

                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tsrId);
                                }
                                else
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tsrId && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID);
                                }
                            }
                            else
                            {
                                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetAll();
                                }
                                else
                                {
                                    objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == tsrId && x.TSR.AccountManagerId == userID || x.TSR.DeliveryManagerId == userID || x.TSR.TestManagerId == userID || x.TeamLeadId == userID);
                                }
                            }
                        }
                    }
                }

                result = Json(objTSOList.Count.ToString(), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (DbEntityValidationException eve)
            {
                StringBuilder objSB = new StringBuilder();
                foreach (var objEve in eve.EntityValidationErrors)
                {
                    objSB.Append(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", objEve.Entry.Entity.GetType().Name, objEve.Entry.State));
                    objSB.Append("\n");
                    foreach (var ve in objEve.ValidationErrors)
                    {
                        objSB.Append(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        objSB.Append("\n");
                    }
                }
                TTMLogger.Logger.LogError(String.Format("Error - {0}", objSB.ToString()));
                result = Json(String.Format("Error - {0}", objSB.ToString()), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateTSO
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="tsoDTO">TSODTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/TSO/UpdateTSO/{ID}")]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager,TMO,Team Lead")]
        public JsonResult<string> UpdateTSO(int ID, [FromBody]TSODTO tsoDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyTSO(ID, tsoDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteTSO
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [HttpDelete, Route("v1/TSO/DeleteTSO/{ID}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin")]
        public JsonResult<string> DeleteTSO(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveTSO(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// CreateTSO
        /// </summary>
        /// <param name="tsrDTO">TSODTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/TSO/CreateTask")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Team Lead,Test Manager,TMO")]
        public JsonResult<string> CreateTask([FromBody] CompleteTaskDTO data)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                TSOServiceDeliveryChainDTO tsoServiceDeliveryChainTaskDTO = new TSOServiceDeliveryChainDTO();
                TSOServiceDeliveryChainTaskActualDTO tsoServiceDeliveryChainTaskActualDTO = new TSOServiceDeliveryChainTaskActualDTO(); ;
                TSOServiceDeliveryChainTaskPlannedDTO tsoServiceDeliveryChainTaskPlannedDTO = new TSOServiceDeliveryChainTaskPlannedDTO();

                tsoServiceDeliveryChainTaskDTO.ID = data.ID;
                tsoServiceDeliveryChainTaskDTO.TSOId = data.TSOId;
                tsoServiceDeliveryChainTaskDTO.InputOutcome = data.InputOutcome;
                tsoServiceDeliveryChainTaskDTO.Notes = data.Notes;
                tsoServiceDeliveryChainTaskDTO.PlannedStartDate = data.PlannedStartDate;
                tsoServiceDeliveryChainTaskDTO.PlannedCompletionDate = data.PlannedCompletionDate;
                tsoServiceDeliveryChainTaskDTO.ServiceDeliveryChainId = data.ServiceDeliveryChainId;
                tsoServiceDeliveryChainTaskDTO.ServiceDeliveryChain = data.ServiceDeliveryChain;
                tsoServiceDeliveryChainTaskDTO.ServiceDeliveryChainMethodId = data.ServiceDeliveryChainMethodId;
                tsoServiceDeliveryChainTaskDTO.ServiceDeliveryChainMethod = data.ServiceDeliveryChainMethod;
                tsoServiceDeliveryChainTaskDTO.TaskStatus = data.TaskStatus;
                tsoServiceDeliveryChainTaskDTO.TaskStatusId = data.TaskStatusId;
                tsoServiceDeliveryChainTaskDTO.CreatedBy = data.CreatedBy;
                //if (data.ServiceDeliveryChainId == 42)
                //{
                //    tsoServiceDeliveryChainTaskDTO.ResponsiblePersonId = data.ResponsiblePersonId;
                //    tsoServiceDeliveryChainTaskDTO.ResponsiblePerson = data.ResponsiblePerson;
                //    tsoServiceDeliveryChainTaskDTO.DueDate = data.DueDate;
                //}


                tsoServiceDeliveryChainTaskActualDTO.ID = data.TaskActualID;
                tsoServiceDeliveryChainTaskActualDTO.TSOServiceDeliveryChainTaskId = tsoServiceDeliveryChainTaskDTO.ID;
                tsoServiceDeliveryChainTaskActualDTO.WeekNumber = data.WeekNumber;
                tsoServiceDeliveryChainTaskActualDTO.Year = DateTime.Now.Year;
                tsoServiceDeliveryChainTaskActualDTO.ActualEffort = data.ActualEffort;
                tsoServiceDeliveryChainTaskActualDTO.ActualInput = data.ActualInput;
                tsoServiceDeliveryChainTaskActualDTO.ActualOutcome = data.ActualOutcome;
                tsoServiceDeliveryChainTaskActualDTO.ActualOutcomeTestSteps = data.ActualOutcomeTestSteps;
                tsoServiceDeliveryChainTaskActualDTO.ActualProcessingTime = data.ActualProcessingTime;
                tsoServiceDeliveryChainTaskActualDTO.ActualProductivity = data.ActualProductivity;
                tsoServiceDeliveryChainTaskActualDTO.ActualReviewRounds = data.ActualReviewRounds;
                tsoServiceDeliveryChainTaskActualDTO.DefectRaised = data.DefectRaised;
                tsoServiceDeliveryChainTaskActualDTO.DefectRejected = data.DefectRejected;
                tsoServiceDeliveryChainTaskActualDTO.IdleTimeEffort = data.IdleTimeEffort;
                tsoServiceDeliveryChainTaskActualDTO.IdleTimeDuration = data.IdleTimeDuration;
                tsoServiceDeliveryChainTaskActualDTO.Headcount = data.Headcount;
                tsoServiceDeliveryChainTaskActualDTO.ActualOutcomeRatio = data.ActualOutcomeRatio;
                tsoServiceDeliveryChainTaskActualDTO.CreatedBy = data.CreatedBy;
                //if (data.ServiceDeliveryChainId == 42)
                //{
                //    tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRisk = data.ActualOperationalRisk;
                //    tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskId = data.ActualOperationalRiskId;
                //    tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskIndicator = data.ActualOperationalRiskIndicator;
                //    tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskIndicatorId = data.ActualOperationalRiskIndicatorId;
                //    tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskDescription = data.ActualOperationalRiskDescription;
                //    tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskMitigation = data.ActualOperationalRiskMitigation;                   
                //}

                tsoServiceDeliveryChainTaskPlannedDTO.ID = data.TaskPlannedId;
                tsoServiceDeliveryChainTaskPlannedDTO.PlannedEffort = data.PlannedEffort;
                if (data.PlannedProductivity == null)
                { tsoServiceDeliveryChainTaskPlannedDTO.PlannedProductivity = 0.0; }
                else
                {
                    tsoServiceDeliveryChainTaskPlannedDTO.PlannedProductivity = data.PlannedProductivity;
                }
                tsoServiceDeliveryChainTaskPlannedDTO.PlannedReviewRounds = data.PlannedReviewRounds;
                tsoServiceDeliveryChainTaskPlannedDTO.PlannedOutcome = data.PlannedOutcome;
                tsoServiceDeliveryChainTaskPlannedDTO.PlannedOutcomeTestSteps = data.PlannedOutcomeTestSteps;
                tsoServiceDeliveryChainTaskPlannedDTO.PlannedProcessingTime = data.PlannedProcessingTime;
                tsoServiceDeliveryChainTaskPlannedDTO.PlannedInput = data.PlannedInput;
                tsoServiceDeliveryChainTaskPlannedDTO.PlannedOutcomeTestSteps = data.PlannedOutcomeTestSteps;
                tsoServiceDeliveryChainTaskPlannedDTO.PlannedOutcomeRatio = data.PlannedOutcomeRatio;
                tsoServiceDeliveryChainTaskPlannedDTO.WeekNumber = data.WeekNumber + 1;
                tsoServiceDeliveryChainTaskPlannedDTO.Year = DateTime.Now.Year;
                tsoServiceDeliveryChainTaskPlannedDTO.CreatedBy = data.CreatedBy;
                tsoServiceDeliveryChainTaskPlannedDTO.TSOServiceDeliveryChainTaskId = tsoServiceDeliveryChainTaskDTO.ID;

                //if (data.ServiceDeliveryChainId == 42)
                //{
                //    tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRisk = data.PlannedOperationalRisk;
                //    tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskId = data.PlannedOperationalRiskId;
                //    tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskIndicatorId = data.PlannedOperationalRiskIndicatorId;
                //    tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskIndicator = data.PlannedOperationalRiskIndicator;
                //    tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskDescription = data.PlannedOperationalRiskDescription;
                //    tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskMitigation = data.PlannedOperationalRiskMitigation;                    
                //}

                result = Json(this.AddNewTask(tsoServiceDeliveryChainTaskDTO, tsoServiceDeliveryChainTaskActualDTO, tsoServiceDeliveryChainTaskPlannedDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (DbEntityValidationException eve)
            {
                StringBuilder objSB = new StringBuilder();
                foreach (var objEve in eve.EntityValidationErrors)
                {
                    objSB.Append(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", objEve.Entry.Entity.GetType().Name, objEve.Entry.State));
                    objSB.Append("\n");
                    foreach (var ve in objEve.ValidationErrors)
                    {
                        objSB.Append(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        objSB.Append("\n");
                    }
                }
                TTMLogger.Logger.LogError(String.Format("Error - {0}", objSB.ToString()));
                result = Json(String.Format("Error - {0}", objSB.ToString()), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        //[HttpGet, Route("v1/TSO/GetTaskByWeek/{TSOServiceDeliveryChainId}/{week}")]
        //[ResponseType(typeof(TSO))]
        //[SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,Team Lead,PQM,Guest")]
        //public JsonResult<TSOServiceDeliveryChainDTO> GetTaskByWeek(int TSOServiceDeliveryChainId, int week)
        //{
        //    JsonResult<TSOServiceDeliveryChainDTO> result = null;
        //    LoginSession ls = new LoginSession();
        //    IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
        //    TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO = null;
        //    try
        //    {
        //        int newweek = week + 1;
        //        objBusinessLayer = new BusinessLayer(ls);
        //        IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSOServiceDeliveryChainId == TSOServiceDeliveryChainId, x => x.tsoServiceDeliveryChainTaskActual, x => x.tsoServiceDeliveryChainTaskPlanned);
        //        objTSOServiceDeliveryChainList = objTSOServiceDeliveryChainList.Where(x => x.tsoServiceDeliveryChainTaskActual.FirstOrDefault().WeekNumber <= week && x.tsoServiceDeliveryChainTaskPlanned.FirstOrDefault().WeekNumber <= newweek).ToList();


        //        if (null != objTSOServiceDeliveryChainList)
        //        {
        //            objTSOServiceDeliveryChainDTO = new TSOServiceDeliveryChainDTO();

        //            double ActualEffortCumulative = 0;
        //            int ActualOutcomeCumulative = 0;
        //            int ActualOutcomeTestStepsCumulative = 0;
        //            double ActualProcessingTimeCumulative = 0;
        //            int DefectRaisedCumulative = 0;
        //            int DefectRejectedCumulative = 0;
        //            double ActualInputCumulative = 0;

        //            double PlannedEffortCumulative = 0;
        //            int PlannedOutcomeCumulative = 0;
        //            int PlannedOutcomeTestStepsCumulative = 0;
        //            double PlannedProcessingTimeCumulative = 0;
        //            double PlannedInputCumulative = 0;

        //            if (week > 0)
        //            {
        //                foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSOServiceDeliveryChainList)
        //                {
        //                    if (week == objTSOServiceDeliveryChain.tsoServiceDeliveryChainTaskActual.FirstOrDefault().WeekNumber)
        //                    {
        //                        objTSOServiceDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
        //                    }
        //                }
        //            }

        //            foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSOServiceDeliveryChainList)
        //            {
        //                TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTOForEach = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);

        //                if (objTSOServiceDeliveryChainDTOForEach.TSOServiceDeliveryChainActual.FirstOrDefault().WeekNumber < week)
        //                {
        //                    TSOServiceDeliveryChainActualDTO actualdata = objTSOServiceDeliveryChainDTOForEach.TSOServiceDeliveryChainActual.FirstOrDefault();
        //                     ActualEffortCumulative += actualdata.ActualEffort ?? 0;
        //                    ActualOutcomeCumulative += actualdata.ActualOutcome;
        //                    ActualOutcomeTestStepsCumulative += actualdata.ActualOutcomeTestSteps;
        //                    ActualProcessingTimeCumulative += actualdata.ActualProcessingTime;
        //                    DefectRaisedCumulative += actualdata.DefectRaised;
        //                    DefectRejectedCumulative += actualdata.DefectRejected;
        //                    ActualInputCumulative += actualdata.ActualInput;


        //                }
        //                if (objTSOServiceDeliveryChainDTOForEach.TSOServiceDeliveryChainPlanned.FirstOrDefault().WeekNumber <= week)
        //                {
        //                    TSOServiceDeliveryChainPlannedDTO planneddata = objTSOServiceDeliveryChainDTOForEach.TSOServiceDeliveryChainPlanned.FirstOrDefault();
        //                    PlannedEffortCumulative += planneddata.PlannedEffort ?? 0;
        //                    PlannedOutcomeCumulative += planneddata.PlannedOutcome;
        //                    PlannedOutcomeTestStepsCumulative += planneddata.PlannedOutcomeTestSteps;
        //                    PlannedProcessingTimeCumulative += planneddata.PlannedProcessingTime;
        //                    PlannedInputCumulative += planneddata.PlannedInput; ;
        //                }
        //            }

        //            objTSOServiceDeliveryChainDTO.ActualEffortCumulative = ActualEffortCumulative;
        //            objTSOServiceDeliveryChainDTO.ActualOutcomeCumulative = ActualOutcomeCumulative;
        //            objTSOServiceDeliveryChainDTO.ActualOutcomeTestStepsCumulative = ActualOutcomeTestStepsCumulative;
        //            objTSOServiceDeliveryChainDTO.ActualProcessingTimeCumulative = ActualProcessingTimeCumulative;
        //            objTSOServiceDeliveryChainDTO.DefectRaisedCumulative = DefectRaisedCumulative;
        //            objTSOServiceDeliveryChainDTO.DefectRejectedCumulative = DefectRejectedCumulative;
        //            objTSOServiceDeliveryChainDTO.ActualInputCumulative = ActualInputCumulative;

        //            objTSOServiceDeliveryChainDTO.PlannedEffortCumulative = PlannedEffortCumulative;
        //            objTSOServiceDeliveryChainDTO.PlannedOutcomeCumulative = PlannedOutcomeCumulative;
        //            objTSOServiceDeliveryChainDTO.PlannedOutcomeTestStepsCumulative = PlannedOutcomeTestStepsCumulative;
        //            objTSOServiceDeliveryChainDTO.PlannedProcessingTimeCumulative = PlannedProcessingTimeCumulative;
        //            objTSOServiceDeliveryChainDTO.PlannedInputCumulative = PlannedInputCumulative;

        //            //objTSOServiceDeliveryChainDTO.PlannedProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainDTO.PlannedProcessingTime, 2);
        //            //objTSOServiceDeliveryChainDTO.ActualProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainDTO.ActualProcessingTime, 2);
        //        }


        //        objTSOServiceDeliveryChainDTO.CurrentWeek = GetWeekOfYear(DateTime.Now);
        //        objTSOServiceDeliveryChainDTO.CurrentDate = DateTime.Now;

        //        result = Json(objTSOServiceDeliveryChainDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        //    }
        //    catch (Exception ex)
        //    {
        //        TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
        //        throw;
        //    }
        //    return result;
        //}

        [HttpGet, Route("v1/TSO/GetTaskByWeek/{TSOServiceDeliveryChainId}/{week}/{year}")]
        [ResponseType(typeof(TSO))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<CompleteTaskDTO> GetTaskByWeek(int TSOServiceDeliveryChainId, int week, int year)
        {
            JsonResult<CompleteTaskDTO> result = null;
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO = null;
            TSOServiceDeliveryChainTaskActualDTO objTSOServiceDeliveryChainActualDTO = null;
            TSOServiceDeliveryChainTaskPlannedDTO objTSOServiceDeliveryChainPlannedDTO = null;
            CompleteTaskDTO objCompleteTSOServiceDeliveryChainDTO = new CompleteTaskDTO();
            try
            {
                int newweek = week + 1;
                objBusinessLayer = new BusinessLayer(ls);
                //IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSOServiceDeliveryChainId == TSOServiceDeliveryChainId, x => x.tsoServiceDeliveryChainTaskActual, x => x.tsoServiceDeliveryChainTaskPlanned);
                //IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChains = objTSOServiceDeliveryChainList.Where(x => x.tsoServiceDeliveryChainTaskActual.FirstOrDefault().WeekNumber <= week && x.tsoServiceDeliveryChainTaskPlanned.FirstOrDefault().WeekNumber <= newweek).ToList();
                TSOServiceDeliveryChain objTSOServiceDeliveryChain = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == TSOServiceDeliveryChainId, x => x.TSOServiceDeliveryChainActualTasks, x => x.TSOServiceDeliveryChainPlannedTasks, x => x.ServiceDeliveryChain, x => x.TSO,
                    x => x.TSO.TSOProductivityInputs, x => x.TSO.TSOProductivityOutcomes, x => x.TSO.TSOProductivityInputs.Select(y => y.ProductivityInput), x => x.TSO.TSOProductivityOutcomes.Select(z => z.ProductivityOutcome), x => x.TSOProductivityInputActuals, x => x.TSOProductivityInputPlanneds, x => x.TSOProductivityOutcomeActuals, x => x.TSOProductivityOutcomePlanneds);
                List<TSOServiceDeliveryChainTaskActual> objTaskActuals = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.Where(x => x.WeekNumber <= week).ToList();
                List<TSOServiceDeliveryChainTaskPlanned> objTaskPlanneds = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.Where(x => x.WeekNumber <= newweek).ToList();

                if (null != objTSOServiceDeliveryChain)
                {
                    objTSOServiceDeliveryChainDTO = new TSOServiceDeliveryChainDTO();
                    objTSOServiceDeliveryChainActualDTO = new TSOServiceDeliveryChainTaskActualDTO();
                    objTSOServiceDeliveryChainPlannedDTO = new TSOServiceDeliveryChainTaskPlannedDTO();

                    double ActualEffortCumulative = 0;
                    float ActualOutcomeCumulative = 0;
                    int ActualOutcomeTestStepsCumulative = 0;
                    double ActualProcessingTimeCumulative = 0;
                    int DefectRaisedCumulative = 0;
                    int DefectRejectedCumulative = 0;
                    double ActualInputCumulative = 0;

                    double PlannedEffortCumulative = 0;
                    float PlannedOutcomeCumulative = 0;
                    int PlannedOutcomeTestStepsCumulative = 0;
                    double PlannedProcessingTimeCumulative = 0;
                    double PlannedInputCumulative = 0;

                    if (week > 0)
                    {
                        objTSOServiceDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
                        objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks.Clear();
                        objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks.Clear();
                        foreach (TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainActual in objTaskActuals)
                        {
                            if (week == objTSOServiceDeliveryChainActual.WeekNumber)
                            {
                                objTSOServiceDeliveryChainActualDTO = Conversions.ToDTO<TSOServiceDeliveryChainTaskActualDTO, TSOServiceDeliveryChainTaskActual>(objTSOServiceDeliveryChainActual);
                                objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks.Add(objTSOServiceDeliveryChainActualDTO);
                            }
                        }
                        foreach (TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainPlanned in objTaskPlanneds)
                        {
                            if (week + 1 == objTSOServiceDeliveryChainPlanned.WeekNumber)
                            {
                                objTSOServiceDeliveryChainPlannedDTO = Conversions.ToDTO<TSOServiceDeliveryChainTaskPlannedDTO, TSOServiceDeliveryChainTaskPlanned>(objTSOServiceDeliveryChainPlanned);
                                objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks.Add(objTSOServiceDeliveryChainPlannedDTO);
                            }
                        }
                    }

                    foreach (TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainActual in objTaskActuals)
                    {   // TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTOForEach = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
                        TSOServiceDeliveryChainTaskActualDTO objTSOServiceDeliveryChainActualDTOForEach = Conversions.ToDTO<TSOServiceDeliveryChainTaskActualDTO, TSOServiceDeliveryChainTaskActual>(objTSOServiceDeliveryChainActual);
                        if (objTSOServiceDeliveryChainActualDTOForEach.WeekNumber < week)
                        {
                            ActualEffortCumulative += objTSOServiceDeliveryChainActualDTOForEach.ActualEffort ?? 0;
                            ActualOutcomeTestStepsCumulative += objTSOServiceDeliveryChainActualDTOForEach.ActualOutcomeTestSteps;
                            ActualProcessingTimeCumulative += objTSOServiceDeliveryChainActualDTOForEach.ActualProcessingTime;
                            DefectRaisedCumulative += objTSOServiceDeliveryChainActualDTOForEach.DefectRaised;
                            DefectRejectedCumulative += objTSOServiceDeliveryChainActualDTOForEach.DefectRejected;
                            objTSOServiceDeliveryChainActualDTOForEach.ActualProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainActualDTOForEach.ActualProcessingTime, 2);
                        }
                    }

                    foreach (TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainPlanned in objTaskPlanneds)
                    {
                        TSOServiceDeliveryChainTaskPlannedDTO objTSOServiceDeliveryChainPlannedDTOForEach = Conversions.ToDTO<TSOServiceDeliveryChainTaskPlannedDTO, TSOServiceDeliveryChainTaskPlanned>(objTSOServiceDeliveryChainPlanned);
                        if (objTSOServiceDeliveryChainPlannedDTOForEach.WeekNumber < newweek)
                        {
                            PlannedEffortCumulative += objTSOServiceDeliveryChainPlannedDTOForEach.PlannedEffort ?? 0;
                            PlannedOutcomeTestStepsCumulative += objTSOServiceDeliveryChainPlannedDTOForEach.PlannedOutcomeTestSteps;
                            PlannedProcessingTimeCumulative += objTSOServiceDeliveryChainPlannedDTOForEach.PlannedProcessingTime;
                            objTSOServiceDeliveryChainPlannedDTOForEach.PlannedProcessingTime = Math.Round((Double)objTSOServiceDeliveryChainPlannedDTOForEach.PlannedProcessingTime, 2);
                        }
                    }



                    List<InputOutcomeCumulativeDTO> inOutCumulativeList = new List<InputOutcomeCumulativeDTO>();
                    List<InputOutcomeDTO> inOutList = new List<InputOutcomeDTO>();
                    objTSOServiceDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);

                    foreach (TSOProductivityInputDTO input in objTSOServiceDeliveryChainDTO.TSO.TSOProductivityInputs)
                    {
                        foreach (TSOProductivityInputActualDTO actual in objTSOServiceDeliveryChainDTO.TSOProductivityInputActuals)
                        {
                            float fInputPlanned = objTSOServiceDeliveryChainDTO.TSOProductivityInputActuals.Where(x => x.WeekNumber < week && x.TSOProductivityInputId == input.ID && x.Year == year).Sum(y => y.InputValue);

                            //ActualInputCumulative += fInputPlanned;

                            if (inOutCumulativeList.Count == 0)
                            {
                                InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                inout.ID = input.ProductivityInputId;
                                inout.Value = fInputPlanned;
                                inout.Week = week;
                                inout.Type = "ActualInput";
                                inout.Year = year;
                                inout.TSOPID = input.ID;
                                inOutCumulativeList.Add(inout);
                            }
                            else
                            {
                                IEnumerable<InputOutcomeCumulativeDTO> isExist = inOutCumulativeList.Where(x => x.ID == input.ProductivityInputId && x.Type == "ActualInput" && x.TSOPID == input.ID);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                    inout.ID = input.ProductivityInputId;
                                    inout.Value = fInputPlanned;
                                    inout.Week = week;
                                    inout.Type = "ActualInput";
                                    inout.Year = year;
                                    inout.TSOPID = input.ID;
                                    inOutCumulativeList.Add(inout);
                                }
                            }

                            if (actual.WeekNumber == week && input.ID == actual.TSOProductivityInputId)
                            {
                                IEnumerable<InputOutcomeDTO> isExist = inOutList.Where(x => x.ID == input.ProductivityInputId && x.Type == "ActualInput"
                                    && x.TSOPID == input.ID && x.Value == actual.InputValue && x.Week == week && x.Year == year);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeDTO inoutNew = new InputOutcomeDTO();
                                    inoutNew.ID = input.ProductivityInputId;
                                    inoutNew.Week = week;
                                    inoutNew.Type = "ActualInput";
                                    inoutNew.Year = year;
                                    inoutNew.TSOPID = input.ID;
                                    inoutNew.Value = actual.InputValue;
                                    inOutList.Add(inoutNew);
                                }
                            }
                        }
                    }

                    foreach (TSOProductivityOutcomeDTO input in objTSOServiceDeliveryChainDTO.TSO.TSOProductivityOutcomes)
                    {
                        foreach (TSOProductivityOutcomeActualDTO Actuals in objTSOServiceDeliveryChainDTO.TSOProductivityOutcomeActuals)
                        {
                            float fInputPlanned = objTSOServiceDeliveryChainDTO.TSOProductivityOutcomeActuals.Where(x => x.WeekNumber < week && x.TSOProductivityOutcomeId == input.ID && x.Year == year).Sum(y => y.OutcomeValue);

                            //ActualOutcomeCumulative += fInputPlanned;

                            if (inOutCumulativeList.Count == 0)
                            {
                                InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                inout.ID = input.ProductivityOutcomeId;
                                inout.Value = fInputPlanned;
                                inout.Week = week;
                                inout.Type = "ActualOutcome";
                                inout.Year = year;
                                inout.TSOPID = input.ID;
                                inOutCumulativeList.Add(inout);
                            }
                            else
                            {
                                IEnumerable<InputOutcomeCumulativeDTO> isExist = inOutCumulativeList.Where(x => x.ID == input.ProductivityOutcomeId && x.Type == "ActualOutcome" && x.TSOPID == input.ID);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                    inout.ID = input.ProductivityOutcomeId;
                                    inout.Value = fInputPlanned;
                                    inout.Week = week;
                                    inout.Type = "ActualOutcome";
                                    inout.Year = year;
                                    inout.TSOPID = input.ID;
                                    inOutCumulativeList.Add(inout);
                                }
                            }

                            if (Actuals.WeekNumber == week && input.ID == Actuals.TSOProductivityOutcomeId)
                            {
                                IEnumerable<InputOutcomeDTO> isExist = inOutList.Where(x => x.ID == input.ProductivityOutcomeId && x.Type == "ActualOutcome"
                                    && x.TSOPID == input.ID && x.Value == Actuals.OutcomeValue && x.Week == week && x.Year == year);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeDTO inoutNew = new InputOutcomeDTO();
                                    inoutNew.ID = input.ProductivityOutcomeId;
                                    inoutNew.Week = week;
                                    inoutNew.Type = "ActualOutcome";
                                    inoutNew.Year = year;
                                    inoutNew.TSOPID = input.ID;
                                    inoutNew.Value = Actuals.OutcomeValue;
                                    inOutList.Add(inoutNew);
                                }
                            }
                        }
                    }

                    // }

                    //Planned Cumulative
                    //if (objTSOServiceDeliveryChainTaskDTO.WeekNumber <= week)
                    //{
                    foreach (TSOProductivityInputDTO input in objTSOServiceDeliveryChainDTO.TSO.TSOProductivityInputs)
                    {
                        foreach (TSOProductivityInputPlannedDTO planned in objTSOServiceDeliveryChainDTO.TSOProductivityInputPlanneds)
                        {
                            float fInputPlanned = objTSOServiceDeliveryChainDTO.TSOProductivityInputPlanneds.Where(x => x.WeekNumber < week + 1 && x.TSOProductivityInputId == input.ID && x.Year == year).Sum(y => y.InputValue);

                            //PlannedInputCumulative += fInputPlanned;

                            if (inOutCumulativeList.Count == 0)
                            {
                                InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                inout.ID = input.ProductivityInputId;
                                inout.Value = fInputPlanned;
                                inout.Week = week;
                                inout.Type = "PlannedInput";
                                inout.Year = year;
                                inout.TSOPID = input.ID;
                                inOutCumulativeList.Add(inout);
                            }
                            else
                            {
                                IEnumerable<InputOutcomeCumulativeDTO> isExist = inOutCumulativeList.Where(x => x.ID == input.ProductivityInputId && x.Type == "PlannedInput" && x.TSOPID == input.ID);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                    inout.ID = input.ProductivityInputId;
                                    inout.Value = fInputPlanned;
                                    inout.Week = week;
                                    inout.Type = "PlannedInput";
                                    inout.Year = year;
                                    inout.TSOPID = input.ID;
                                    inOutCumulativeList.Add(inout);
                                }
                            }


                            if (planned.WeekNumber == week + 1 && input.ID == planned.TSOProductivityInputId)
                            {
                                IEnumerable<InputOutcomeDTO> isExist = inOutList.Where(x => x.ID == input.ProductivityInputId && x.Type == "PlannedInput"
                                    && x.TSOPID == input.ID && x.Value == planned.InputValue && x.Week == week && x.Year == year);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeDTO inoutNew = new InputOutcomeDTO();
                                    inoutNew.ID = input.ProductivityInputId;
                                    inoutNew.Week = week;
                                    inoutNew.Type = "PlannedInput";
                                    inoutNew.Year = year;
                                    inoutNew.TSOPID = input.ID;
                                    inoutNew.Value = planned.InputValue;
                                    inOutList.Add(inoutNew);
                                }
                            }
                        }
                    }

                    foreach (TSOProductivityOutcomeDTO outcome in objTSOServiceDeliveryChainDTO.TSO.TSOProductivityOutcomes)
                    {
                        foreach (TSOProductivityOutcomePlannedDTO planned in objTSOServiceDeliveryChainDTO.TSOProductivityOutcomePlanneds)
                        {
                            float fInputPlanned = objTSOServiceDeliveryChainDTO.TSOProductivityOutcomePlanneds.Where(x => x.WeekNumber < week + 1 && x.TSOProductivityOutcomeId == outcome.ID && x.Year == year).Sum(y => y.OutcomeValue);

                            //PlannedOutcomeCumulative += fInputPlanned;
                            if (inOutCumulativeList.Count == 0)
                            {

                                InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                inout.ID = outcome.ProductivityOutcomeId;
                                inout.Value = fInputPlanned;
                                inout.Week = week;
                                inout.Type = "PlannedOutcome";
                                inout.Year = year;
                                inout.TSOPID = outcome.ID;
                                inOutCumulativeList.Add(inout);
                            }
                            else
                            {
                                IEnumerable<InputOutcomeCumulativeDTO> isExist = inOutCumulativeList.Where(x => x.ID == outcome.ProductivityOutcomeId && x.Type == "PlannedOutcome" && x.TSOPID == outcome.ID);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeCumulativeDTO inout = new InputOutcomeCumulativeDTO();
                                    inout.ID = outcome.ProductivityOutcomeId;
                                    inout.Value = fInputPlanned;
                                    inout.Week = week;
                                    inout.Type = "PlannedOutcome";
                                    inout.Year = year;
                                    inout.TSOPID = outcome.ID;
                                    inOutCumulativeList.Add(inout);
                                }
                            }


                            if (planned.WeekNumber == week + 1 && outcome.ID == planned.TSOProductivityOutcomeId)
                            {
                                IEnumerable<InputOutcomeDTO> isExist = inOutList.Where(x => x.ID == outcome.ProductivityOutcomeId && x.Type == "PlannedOutcome"
                                    && x.TSOPID == outcome.ID && x.Value == planned.OutcomeValue && x.Week == week && x.Year == year);

                                if (isExist.Count() == 0)
                                {
                                    InputOutcomeDTO inoutNew = new InputOutcomeDTO();
                                    inoutNew.ID = outcome.ProductivityOutcomeId;
                                    inoutNew.Week = week;
                                    inoutNew.Type = "PlannedOutcome";
                                    inoutNew.Year = year;
                                    inoutNew.TSOPID = outcome.ID;
                                    inoutNew.Value = planned.OutcomeValue;
                                    inOutList.Add(inoutNew);
                                }
                            }
                        }
                    }

                    // }

                    //Total of all input planned
                    ActualInputCumulative = objTSOServiceDeliveryChainDTO.TSOProductivityInputActuals.Where(x => x.WeekNumber <= week && x.Year == year).Sum(y => y.InputValue);
                    ActualOutcomeCumulative = objTSOServiceDeliveryChainDTO.TSOProductivityOutcomeActuals.Where(x => x.WeekNumber <= week && x.Year == year).Sum(y => y.OutcomeValue);


                    PlannedInputCumulative = objTSOServiceDeliveryChainDTO.TSOProductivityInputPlanneds.Where(x => x.WeekNumber < week + 2 && x.Year == year).Sum(y => y.InputValue);
                    PlannedOutcomeCumulative = objTSOServiceDeliveryChainDTO.TSOProductivityOutcomePlanneds.Where(x => x.WeekNumber < week + 2 && x.Year == year).Sum(y => y.OutcomeValue);

                    // }

                    objTSOServiceDeliveryChainDTO.InputOutcomeCumulative = inOutCumulativeList;
                    objTSOServiceDeliveryChainDTO.InputOutcome = inOutList;

                    objTSOServiceDeliveryChainDTO.ActualEffortCumulative = ActualEffortCumulative;
                    objTSOServiceDeliveryChainDTO.ActualOutcomeCumulative = ActualOutcomeCumulative;
                    objTSOServiceDeliveryChainDTO.ActualOutcomeTestStepsCumulative = ActualOutcomeTestStepsCumulative;
                    objTSOServiceDeliveryChainDTO.ActualProcessingTimeCumulative = ActualProcessingTimeCumulative;
                    objTSOServiceDeliveryChainDTO.DefectRaisedCumulative = DefectRaisedCumulative;
                    objTSOServiceDeliveryChainDTO.DefectRejectedCumulative = DefectRejectedCumulative;
                    objTSOServiceDeliveryChainDTO.ActualInputCumulative = ActualInputCumulative;

                    objTSOServiceDeliveryChainDTO.PlannedEffortCumulative = PlannedEffortCumulative;
                    objTSOServiceDeliveryChainDTO.PlannedOutcomeCumulative = PlannedOutcomeCumulative;
                    objTSOServiceDeliveryChainDTO.PlannedOutcomeTestStepsCumulative = PlannedOutcomeTestStepsCumulative;
                    objTSOServiceDeliveryChainDTO.PlannedProcessingTimeCumulative = PlannedProcessingTimeCumulative;
                    objTSOServiceDeliveryChainDTO.PlannedInputCumulative = PlannedInputCumulative;
                }

                objTSOServiceDeliveryChainDTO.CurrentWeek = GetWeekOfYear(DateTime.Now);
                objTSOServiceDeliveryChainDTO.CurrentDate = DateTime.Now;

                MappedTASKForUI(objTSOServiceDeliveryChainDTO, objCompleteTSOServiceDeliveryChainDTO, objTSOServiceDeliveryChainActualDTO, objTSOServiceDeliveryChainPlannedDTO);

                result = Json(objCompleteTSOServiceDeliveryChainDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return result;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/TSO/GetLastTask/{tsoServiceDeliveryChainId}")]
        [ResponseType(typeof(TSO))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<CompleteTaskDTO> GetLastTask(int tsoServiceDeliveryChainId)
        {
            JsonResult<CompleteTaskDTO> result = null;
            TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO = null;
            CompleteTaskDTO objCompleteTask = new CompleteTaskDTO();
            try
            {
                objTSOServiceDeliveryChainDTO = this.GetLastTSOServiceDeliveryChain(tsoServiceDeliveryChainId);
                TSOServiceDeliveryChainTaskActualDTO objTaskActual = objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks.LastOrDefault();
                TSOServiceDeliveryChainTaskPlannedDTO objTaskPlanned = objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks.LastOrDefault();
                MappedTASKForUI(objTSOServiceDeliveryChainDTO, objCompleteTask, objTaskActual, objTaskPlanned);
                if (objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId== 42)
                {
                    int operationrisk = GetHighestOperationalRisk(tsoServiceDeliveryChainId);
                    objCompleteTask.OperationalRisk = operationrisk;
                }              

                if (null != objCompleteTask)
                {
                    result = Json(objCompleteTask, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
                else
                {
                    objCompleteTask = new CompleteTaskDTO();

                    result = Json(objCompleteTask, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }

                //if (null != objTSOServiceDeliveryChainDTO)
                //{
                //    result = Json(objTSOServiceDeliveryChainDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                //}
                //else
                //{
                //    objTSOServiceDeliveryChainDTO = new TSOServiceDeliveryChainDTO();

                //    result = Json(objTSOServiceDeliveryChainDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                //}
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return result;
        }

        private static void MappedTASKForUI(TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO, CompleteTaskDTO objCompleteTask, TSOServiceDeliveryChainTaskActualDTO objTaskActual, TSOServiceDeliveryChainTaskPlannedDTO objTaskPlanned)
        {
            objCompleteTask.ID = objTSOServiceDeliveryChainDTO.ID;
            objCompleteTask.Notes = objTSOServiceDeliveryChainDTO.Notes;
            objCompleteTask.PlannedStartDate = objTSOServiceDeliveryChainDTO.PlannedStartDate;
            objCompleteTask.PlannedCompletionDate = objTSOServiceDeliveryChainDTO.PlannedCompletionDate;
            if (objTaskActual != null)
            {
                objCompleteTask.ActualEffort = objTaskActual.ActualEffort;
                objCompleteTask.ActualProductivity = objTaskActual.ActualProductivity;
                objCompleteTask.ActualOutcome = objTaskActual.ActualOutcome;
                objCompleteTask.ActualOutcomeTestSteps = objTaskActual.ActualOutcomeTestSteps;
                objCompleteTask.ActualReviewRounds = objTaskActual.ActualReviewRounds;
                objCompleteTask.TaskActualID = objTaskActual.ID;
                objCompleteTask.IdleTimeEffort = objTaskActual.IdleTimeEffort;
                objCompleteTask.IdleTimeDuration = objTaskActual.IdleTimeDuration;
                objCompleteTask.Headcount = objTaskActual.Headcount;
                objCompleteTask.ActualOutcomeRatio = objTaskActual.ActualOutcomeRatio;
                objCompleteTask.ActualThroughput = objTaskActual.ActualThroughput;

                objCompleteTask.DefectDensity = objTaskActual.DefectDensity;
                objCompleteTask.DefectRejectionRatio = objTaskActual.DefectRejectionRatio;
                objCompleteTask.DefectRaised = objTaskActual.DefectRaised;
                objCompleteTask.DefectRejected = objTaskActual.DefectRejected;
                objCompleteTask.ActualProcessingTime = objTaskActual.ActualProcessingTime;
                objCompleteTask.ActualInput = objTaskActual.ActualInput;
                objCompleteTask.WeekNumber = objTaskActual.WeekNumber;

                //objCompleteTask.ActualOperationalRiskId = objTaskActual.ActualOperationalRiskId;
                //objCompleteTask.ActualOperationalRisk = objTaskActual.ActualOperationalRisk;
                //objCompleteTask.ActualOperationalRiskIndicatorId = objTaskActual.ActualOperationalRiskIndicatorId;
                //objCompleteTask.ActualOperationalRiskIndicator = objTaskActual.ActualOperationalRiskIndicator;
                //objCompleteTask.ActualOperationalRiskDescription = objTaskActual.ActualOperationalRiskDescription;
                //objCompleteTask.ActualOperationalRiskMitigation = objTaskActual.ActualOperationalRiskMitigation;
            }
            if (objTaskPlanned != null)
            {
                objCompleteTask.PlannedEffort = objTaskPlanned.PlannedEffort;
                objCompleteTask.PlannedProductivity = objTaskPlanned.PlannedProductivity;
                objCompleteTask.PlannedReviewRounds = objTaskPlanned.PlannedReviewRounds;
                objCompleteTask.PlannedOutcome = objTaskPlanned.PlannedOutcome;
                objCompleteTask.PlannedOutcomeTestSteps = objTaskPlanned.PlannedOutcomeTestSteps;
                objCompleteTask.TaskPlannedId = objTaskPlanned.ID;

                objCompleteTask.PlannedProcessingTime = objTaskPlanned.PlannedProcessingTime;
                objCompleteTask.PlannedInput = objTaskPlanned.PlannedInput;
                objCompleteTask.PlannedOutcomeTestSteps = objTaskPlanned.PlannedOutcomeTestSteps;
                objCompleteTask.PlannedOutcomeRatio = objTaskPlanned.PlannedOutcomeRatio;
                objCompleteTask.PlannedThroughput = objTaskPlanned.PlannedThroughput;

                //objCompleteTask.PlannedOperationalRiskId = objTaskPlanned.PlannedOperationalRiskId;
                //objCompleteTask.PlannedOperationalRisk = objTaskPlanned.PlannedOperationalRisk;
                //objCompleteTask.PlannedOperationalRiskIndicatorId = objTaskPlanned.PlannedOperationalRiskIndicatorId;
                //objCompleteTask.PlannedOperationalRiskIndicator = objTaskPlanned.PlannedOperationalRiskIndicator;
                //objCompleteTask.PlannedOperationalRiskDescription = objTaskPlanned.PlannedOperationalRiskDescription;
                //objCompleteTask.PlannedOperationalRiskMitigation = objTaskPlanned.PlannedOperationalRiskMitigation;
            }

            objCompleteTask.ServiceDeliveryChain = objTSOServiceDeliveryChainDTO.ServiceDeliveryChain;
            objCompleteTask.ServiceDeliveryChainId = objTSOServiceDeliveryChainDTO.ServiceDeliveryChainId;
            //objCompleteTask.TSOServiceDeliveryChain = objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChain;
            objCompleteTask.ServiceDeliveryChainMethodId = objTSOServiceDeliveryChainDTO.ServiceDeliveryChainMethodId;
            objCompleteTask.ServiceDeliveryChainMethod = objTSOServiceDeliveryChainDTO.ServiceDeliveryChainMethod;
            objCompleteTask.TaskStatus = objTSOServiceDeliveryChainDTO.TaskStatus;
            objCompleteTask.TaskStatusId = objTSOServiceDeliveryChainDTO.TaskStatusId;

            //objCompleteTask.ResponsiblePersonId = objTSOServiceDeliveryChainDTO.ResponsiblePersonId;
            //objCompleteTask.DueDate = objTSOServiceDeliveryChainDTO.DueDate;
            //objCompleteTask.ResponsiblePerson = objTSOServiceDeliveryChainDTO.ResponsiblePerson;



            objCompleteTask.CreatedBy = objTSOServiceDeliveryChainDTO.CreatedBy;


            objCompleteTask.CreatedBy = objTSOServiceDeliveryChainDTO.CreatedBy;
            objCompleteTask.CreatedOn = objTSOServiceDeliveryChainDTO.CreatedOn;
            objCompleteTask.UpdatedBy = objTSOServiceDeliveryChainDTO.UpdatedBy;

            objCompleteTask.UpdatedOn = objTSOServiceDeliveryChainDTO.UpdatedOn;
            objCompleteTask.Version = objTSOServiceDeliveryChainDTO.Version;
            objCompleteTask.ObjectSate = objTSOServiceDeliveryChainDTO.ObjectSate;
            objCompleteTask.CurrentWeek = GetWeekOfYear(DateTime.Now);
            objCompleteTask.CurrentDate = DateTime.Now;

            objCompleteTask.ActualEffortCumulative = objTSOServiceDeliveryChainDTO.ActualEffortCumulative;
            objCompleteTask.ActualOutcomeCumulative = objTSOServiceDeliveryChainDTO.ActualOutcomeCumulative;
            objCompleteTask.ActualOutcomeTestStepsCumulative = objTSOServiceDeliveryChainDTO.ActualOutcomeTestStepsCumulative;
            objCompleteTask.ActualProcessingTimeCumulative = objTSOServiceDeliveryChainDTO.ActualProcessingTimeCumulative;
            objCompleteTask.ActualInputCumulative = objTSOServiceDeliveryChainDTO.ActualInputCumulative;
            objCompleteTask.DefectRaisedCumulative = objTSOServiceDeliveryChainDTO.DefectRaisedCumulative;
            objCompleteTask.DefectRejectedCumulative = objTSOServiceDeliveryChainDTO.DefectRejectedCumulative;
            objCompleteTask.PlannedEffortCumulative = objTSOServiceDeliveryChainDTO.PlannedEffortCumulative;
            objCompleteTask.PlannedOutcomeCumulative = objTSOServiceDeliveryChainDTO.PlannedOutcomeCumulative;
            objCompleteTask.PlannedOutcomeTestStepsCumulative = objTSOServiceDeliveryChainDTO.PlannedOutcomeTestStepsCumulative;
            objCompleteTask.PlannedInputCumulative = objTSOServiceDeliveryChainDTO.PlannedInputCumulative;
            objCompleteTask.InputOutcomeCumulative = objTSOServiceDeliveryChainDTO.InputOutcomeCumulative;
            objCompleteTask.InputOutcome = objTSOServiceDeliveryChainDTO.InputOutcome;
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetWeekOfYear(DateTime time)
        {
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }


        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


        #region TSO and TASK Dump 
        /// <summary>
        /// GetTSODump
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet, Route("v1/TSO/GetTSODump")]
        [ResponseType(typeof(HttpResponseMessage))]
        //[SessionAuthorize(Roles = "Admin")]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public HttpResponseMessage GetTSODump()
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            var tsoTable = objBusinessLayer.TSOServiceDeliveryChainRepository.ExecuteStoredProcedure("sp_GetTSOData");

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                if (null != tsoTable)
                {
                    StringBuilder sb = new StringBuilder();
                    //Set the Headings and Orders                    
                    sb.Append("TSOID").Append(",");                                             //ID
                    sb.Append("Title").Append(",");                                             //Title
                    sb.Append("Client").Append(",");                                            //Client
                    sb.Append("TSO Status").Append(",");                                        //TSO Status
                    sb.Append("Related TSR").Append(",");                                       //Related TSR
                    sb.Append("Related Tasks").Append(",");                                     //Related Tasks	
                    sb.Append("Engagement Model").Append(",");                                  //Engagement Model
                    sb.Append("Pricing Model").Append(",");                                     //Pricing Model	
                    sb.Append("Project Model").Append(",");                                     //Project Model	
                    sb.Append("Client Region").Append(",");                                     //Client Region
                    sb.Append("Account").Append(",");                                           //Account	
                    sb.Append("ERP order number").Append(",");                                  //ERP order number	
                    sb.Append("Solution centre").Append(",");                                   //Solution centre	
                    sb.Append("Core Service").Append(",");                                  //Core Service	
                    sb.Append("Service Delivery Chain").Append(",");                            //Service Delivery Chain
                    sb.Append("Operational risk").Append(",");                                  //Operational risk
                    sb.Append("Start Date").Append(",");                                        //Start Date
                    sb.Append("Target Completion Date").Append(",");                            //Target Completion Date	
                    sb.Append("Estimated effort").Append(",");                                  //Estimated effort
                    sb.Append("Planned effort").Append(",");                                    //Planned effort	
                    sb.Append("Actual effort").Append(",");                                     //Actual effort	
                    sb.Append("Account Manager").Append(",");                                   //Account manager	
                    sb.Append("Delivery manager").Append(",");                                  //Delivery manager	
                    sb.Append("Test Manager").Append(",");                                      //Test manager	
                    sb.Append("Team Lead").Append(",");                                         //Team Lead	
                    sb.Append("Vertical").Append(",");                                          //Vertical	
                    sb.Append("Practice").Append(",");                                          //Practice	
                    sb.Append("Market Offering").Append(",");                                   //Market offering	
                    sb.Append("Relevant repositories").Append(",");                             //Relevant repositories
                    sb.Append("Modified").Append(",");                                          //Modified	
                    sb.Append("Modified By").Append(",");                                       //Modified By	
                    sb.Append("Description").Append(",");                                       //Description	
                    sb.Append("\n");

                    for (int tsoTableRow = 0; tsoTableRow < tsoTable.Rows.Count; tsoTableRow++)
                    {
                        // Append data                 
                        sb.Append(tsoTable.Rows[tsoTableRow]["TSOID"].ToString()).Append(","); //ID
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Title"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Title"].ToString())).Append(",");                             //Title
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Client"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Client"].ToString())).Append(",");                 //Client
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["TSOStatus"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["TSOStatus"].ToString())).Append(",");           //TSR Status
                        sb.Append(string.Format("=\"{0}\"", tsoTable.Rows[tsoTableRow]["Related_TSR"].ToString())).Append(",");                                                                       //Related TSO
                        if (tsoTable.Rows[tsoTableRow]["Related_Tasks"].ToString() == null)
                            sb.Append(string.Empty).Append(",");                         // CHECK FOR RELATED TASK
                        else
                        {


                            if (tsoTable.Rows[tsoTableRow]["Related_Tasks"] != null)
                            {
                                bool isSingleNum = tsoTable.Rows[tsoTableRow]["Related_Tasks"].ToString().IndexOf(", ") > 0;
                                if (!isSingleNum)
                                    sb.Append(string.Format("=\"{0}\"", tsoTable.Rows[tsoTableRow]["Related_Tasks"].ToString()));
                                else
                                    sb.Append(string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Related_Tasks"].ToString()));
                                sb.Append(",");
                            }
                            else
                                sb.Append(",");
                        }
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Engagement_Model"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Engagement_Model"].ToString())).Append(",");         //Engagement Model
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Pricing_Model"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Pricing_Model"].ToString())).Append(",");     //Pricing Model	
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Project_Model"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Project_Model"].ToString())).Append(",");       // Project Model
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Client_Region"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Client_Region"].ToString())).Append(",");     // Client Region
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Account"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Account"].ToString())).Append(",");                         // Account

                        long n;
                        bool isNumeric = long.TryParse(tsoTable.Rows[tsoTableRow]["ERPordernumber"].ToString(), out n);
                        if (!isNumeric)
                            sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["ERPordernumber"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["ERPordernumber"].ToString())).Append(",");           //ERP order number
                        else
                            sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["ERPordernumber"].ToString()) ? string.Empty : string.Format("=\"{0}\"", tsoTable.Rows[tsoTableRow]["ERPordernumber"].ToString())).Append(",");           //ERP order number
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Solution_Center"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Solution_Center"].ToString())).Append(","); //Solution centre	
                        if (tsoTable.Rows[tsoTableRow]["Core_Service"].ToString() == null)
                            sb.Append(string.Empty).Append(",");
                        else
                        {
                            if (tsoTable.Rows[tsoTableRow]["Core_Service"].ToString() == "N.A.")
                                sb.Append("");
                            else
                                sb.Append(string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Core_Service"].ToString()));
                            sb.Append(",");                             // <--- CHECK FOR CORE SERVICES  //Core Service (s)
                        }
                        if (tsoTable.Rows[tsoTableRow]["ServiceDeliveryChains"].ToString() == null)
                            sb.Append(string.Empty).Append(",");
                        else
                        {
                            if (tsoTable.Rows[tsoTableRow]["ServiceDeliveryChains"].ToString() != null)
                            {
                                sb.Append(string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["ServiceDeliveryChains"].ToString()));
                            }
                            sb.Append(",");
                        }
                        sb.Append(tsoTable.Rows[tsoTableRow]["RiskNo"].ToString()).Append(",");                                                       //Operational risk
                        sb.Append(tsoTable.Rows[tsoTableRow]["StartDate"].ToString()).Append(",");                                            //Start date
                                                                                                                                              /* sb.Append(objTSO.ActualStartDate.ToString("dd-MMM-yyyy")).Append(","); */                                     //Actual start date
                        sb.Append(tsoTable.Rows[tsoTableRow]["TargetCompletionDate"].ToString()).Append(",");                                 //Target Completion Date
                        sb.Append(tsoTable.Rows[tsoTableRow]["EstimatedEffort"].ToString()).Append(",");                                                              //Estimated effort
                        sb.Append(tsoTable.Rows[tsoTableRow]["PlannedEffort"].ToString()).Append(",");                                                                //Planned effort
                        sb.Append(tsoTable.Rows[tsoTableRow]["ActualEffort"].ToString()).Append(",");                                                                 //Actual effort
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Account_Manger"].ToString()) ? string.Empty : tsoTable.Rows[tsoTableRow]["Account_Manger"].ToString()).Append(","); //Account Manager	
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Delivery_Manager"].ToString()) ? string.Empty : tsoTable.Rows[tsoTableRow]["Delivery_Manager"].ToString()).Append(",");// Delivery manager
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Test_Manager"].ToString()) ? string.Empty : tsoTable.Rows[tsoTableRow]["Test_Manager"].ToString()).Append(",");       // Test Manager	
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Test_Lead"].ToString()) ? string.Empty : tsoTable.Rows[tsoTableRow]["Test_Lead"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Vertical"].ToString() ?? "") ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Vertical"].ToString())).Append(",");      //Vertical	
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Practice"].ToString() ?? "") ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["Practice"].ToString())).Append(",");       //Practice	

                        string strMarketOffering = tsoTable.Rows[tsoTableRow]["MarketOffering"].ToString() == null ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["MarketOffering"].ToString());
                        sb.Append(strMarketOffering).Append(",");                    // Market Offering
                        string strReleventRepository;
                        if (tsoTable.Rows[tsoTableRow]["RelevantRepository"].ToString() == null)
                            strReleventRepository = string.Empty;
                        else
                        {
                            strReleventRepository = string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["RelevantRepository"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsoTable.Rows[tsoTableRow]["RelevantRepository"].ToString());
                        }
                        sb.Append(strReleventRepository).Append(",");      //Relevant repositories	
                        sb.Append(tsoTable.Rows[tsoTableRow]["Modified_On"].ToString()).Append(",");                                           // Modified	
                        sb.Append(string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Modified_By"].ToString()) ? string.Empty : tsoTable.Rows[tsoTableRow]["Modified_By"].ToString()).Append(",");                     // Modified By	                       
                        if (string.IsNullOrEmpty(tsoTable.Rows[tsoTableRow]["Description"].ToString()))
                            sb.Append(string.Empty);
                        else
                        {
                            StringBuilder sbDescription = new StringBuilder();
                            if (tsoTable.Rows[tsoTableRow]["Description"].ToString().IndexOfAny(new char[] { '"', ',' }) != -1)
                                sbDescription.AppendFormat("\"{0}\"", tsoTable.Rows[tsoTableRow]["Description"].ToString().Replace("\"", "\"\""));
                            else
                            {
                                sbDescription.Append(tsoTable.Rows[tsoTableRow]["Description"].ToString());
                                sb.Append(String.Format("\"{0}\"", sbDescription));
                            }
                            sb.Append(",");
                        }
                        sb.Append("\n");
                    }

                    result.Content = new StringContent(sb.ToString(), Encoding.UTF8);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment"); //attachment will force download
                    result.Content.Headers.ContentDisposition.FileName = "TSOExport.csv";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(ex.Message);
                throw ex;
            }
            return result;
        }

        [HttpGet, Route("v1/TSO/GetServiceDeliveryChainTasksDump")]
        [ResponseType(typeof(HttpResponseMessage))]
        //[SessionAuthorize(Roles = "Admin")]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public HttpResponseMessage GetServiceDeliveryChainTasksDump()
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            var taskTable = objBusinessLayer.TSOServiceDeliveryChainRepository.ExecuteStoredProcedure("sp_GetTaskData");
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                if (null != taskTable)
                {
                    StringBuilder sb = new StringBuilder();
                    //Set the Headings and Orders                    
                    sb.Append("Task ID").Append(",");
                    sb.Append("Title").Append(",");
                    sb.Append("Related TSR").Append(",");
                    sb.Append("Related TSO").Append(",");
                    sb.Append("Task Status").Append(",");
                    sb.Append("Client").Append(",");
                    sb.Append("Engagement Model").Append(",");
                    sb.Append("Pricing Model").Append(",");
                    sb.Append("Project Model").Append(",");
                    sb.Append("Client Region").Append(",");
                    sb.Append("Account").Append(",");
                    sb.Append("ERP order number").Append(",");
                    sb.Append("Solution centre").Append(",");
                    sb.Append("Core Service").Append(",");
                    sb.Append("Method").Append(",");
                    sb.Append("Start Date").Append(",");
                    sb.Append("Target Completion Date").Append(",");
                    sb.Append("Actual effort").Append(",");
                    sb.Append("Planned effort").Append(",");
                    sb.Append("Actual Input").Append(",");
                    sb.Append("Planned Input").Append(",");
                    sb.Append("Actual Outcome").Append(",");
                    sb.Append("Planned Outcome").Append(",");
                    sb.Append("Actual Outcome (Test Steps)").Append(",");
                    sb.Append("Planned Outcome (Test Steps)").Append(",");
                    sb.Append("Actual Processing Time").Append(",");
                    sb.Append("Planned Processing time").Append(",");
                    sb.Append("Actual Productivity").Append(",");
                    sb.Append("Planned Productivity").Append(",");
                    sb.Append("Actual Outcome Ratio").Append(",");
                    sb.Append("Planned Outcome Ratio").Append(",");
                    sb.Append("Actual Throughput").Append(",");
                    sb.Append("Planned Throughput").Append(",");
                    sb.Append("Actual Review Rounds").Append(",");
                    sb.Append("Planned Review Rounds").Append(",");
                    sb.Append("Defects Raised").Append(",");
                    sb.Append("Defects Rejected").Append(",");
                    sb.Append("Defect Density").Append(",");
                    sb.Append("Defect Rejection Ratio").Append(",");
                    sb.Append("Headcount").Append(",");
                    sb.Append("Idle Time(Effort)").Append(",");
                    sb.Append("Idle Time (Duration)").Append(",");
                    sb.Append("Account Manager").Append(",");
                    sb.Append("Delivery manager").Append(",");
                    sb.Append("Test Manager").Append(",");
                    sb.Append("Team Lead").Append(",");
                    sb.Append("Vertical").Append(",");
                    sb.Append("Practice").Append(",");
                    sb.Append("Modified").Append(",");
                    sb.Append("Modified By").Append(",");
                    sb.Append("Notes").Append(",");
                    sb.Append("Week Number");
                    sb.Append("\n");

                    for (int taskTableRow = 0; taskTableRow < taskTable.Rows.Count; taskTableRow++)
                    {
                        // Append data                 
                        sb.Append(taskTable.Rows[taskTableRow]["Task_ID"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["Task_Title"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["Task_Title"].ToString() + "( " + taskTable.Rows[taskTableRow]["Task_Description"].ToString() + " )")).Append(",");
                        sb.Append(string.Format("=\"{0}\"", taskTable.Rows[taskTableRow]["Related_TSR"].ToString())).Append(",");
                        sb.Append(string.Format("=\"{0}\"", taskTable.Rows[taskTableRow]["Related_TSO"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["TaskStatus"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["TaskStatus"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["Client_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["Client_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["Engagement_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["Engagement_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["PricingModel_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["PricingModel_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["ProjectModel_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["ProjectModel_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["ClientRegion_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["ClientRegion_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["Account_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["Account_Name"].ToString())).Append(",");
                        long lngERPNumner;
                        bool isNumeric = long.TryParse(taskTable.Rows[taskTableRow]["ERPordernumber"].ToString(), out lngERPNumner);
                        if (!isNumeric)
                        {
                            sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["ERPordernumber"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["ERPordernumber"].ToString())).Append(",");
                        }
                        else
                        {
                            sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["ERPordernumber"].ToString()) ? string.Empty : string.Format("=\"{0}\"", taskTable.Rows[taskTableRow]["ERPordernumber"].ToString())).Append(",");
                        }
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["SolutionCenter_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["SolutionCenter_Name"].ToString())).Append(",");
                        if (taskTable.Rows[taskTableRow]["CoreServiceName"].ToString() == null)
                            sb.Append(string.Empty).Append(",");
                        else
                        {
                            sb.Append(string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["CoreServiceName"].ToString()));
                            sb.Append(",");
                        }
                        sb.Append(taskTable.Rows[taskTableRow]["Method"].ToString() == null ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["Method"].ToString())).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedStartDate"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedCompletionDate"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualEffort"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedEffort"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualInput"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedInput"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualOutcome"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedOutcome"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualOutcomeTestSteps"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedOutcomeTestSteps"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualProcessingTime"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedProcessingTime"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualProductivity"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedProductivity"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualOutcomeRatio"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedOutcomeRatio"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualThroughput"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedThroughput"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["ActualReviewRounds"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["PlannedReviewRounds"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["DefectRaised"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["DefectRejected"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["DefectDensity"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["DefectRejectionRatio"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["Headcount"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["IdleTimeEffort"].ToString()).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["IdleTimeDuration"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["AccountManager_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["AccountManager_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["DeliveryManager_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["DeliveryManager_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["TestManager_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["TestManager_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["TeamLead_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["TeamLead_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["Vertical_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["Vertical_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["Practice_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", taskTable.Rows[taskTableRow]["Practice_Name"].ToString())).Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["UpdatedOn"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["UpdatedBy"].ToString()) ? string.Empty : taskTable.Rows[taskTableRow]["UpdatedBy"].ToString()).Append(",");
                        if (string.IsNullOrEmpty(taskTable.Rows[taskTableRow]["Task_Notes"].ToString()))
                            sb.Append(string.Empty);
                        else
                        {
                            StringBuilder sbNotes = new StringBuilder();
                            if (taskTable.Rows[taskTableRow]["Task_Notes"].ToString().IndexOfAny(new char[] { '"', ',' }) != -1)
                                sbNotes.AppendFormat("\"{0}\"", taskTable.Rows[taskTableRow]["Task_Notes"].ToString().Replace("\"", "\"\""));
                            else
                            {
                                sbNotes.Append(taskTable.Rows[taskTableRow]["Task_Notes"].ToString());
                                sb.Append(String.Format("\"{0}\"", sbNotes));
                            }

                        }
                        sb.Append(",");
                        sb.Append(taskTable.Rows[taskTableRow]["WeekNumber"].ToString());
                        sb.Append("\n");
                    }

                    result.Content = new StringContent(sb.ToString(), Encoding.UTF8);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment"); //attachment will force download
                    result.Content.Headers.ContentDisposition.FileName = "TSOExport.csv";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(ex.Message);
                throw ex;
            }
            return result;
        }

        private IList<TSOServiceDeliveryChainDTO> GetAllServiceDeliveryChainTask()
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            IList<TSOServiceDeliveryChainDTO> objTSODTOList = null;
            try
            {
                IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetAll(x => x.ServiceDeliveryChain, x => x.TSO.TSR,
                    x => x.TSO.TeamLead, x => x.TSO.TSR.Client,
                    x => x.TSO.TSR.ClientRegion, x => x.TSO.TSR.SolutionCentre,
                    x => x.TSO.TSR.TSRCoreServices, x => x.TSO.CoreService,
                    x => x.TSO.TSR.OperationalRisk,
                    x => x.TSO.TSR.AccountManager, x => x.TSO.TSR.DeliveryManager,
                    x => x.TSO.TSR.TestManager, x => x.TSO.TSR.Vertical,
                    x => x.TSO.TSR.Practice, x => x.TSO.TSR.Engagement,
                    x => x.TaskStatus, x => x.TSO.TSR.PricingModel,
                    x => x.TSO.TSR.ProjectModel,
                    x => x.ServiceDeliveryChainMethod, x => x.ServiceDeliveryChain);
                if (objTSOServiceDeliveryChainList != null && objTSOServiceDeliveryChainList.Count > 0)
                {
                    objTSODTOList = new List<TSOServiceDeliveryChainDTO>();
                    foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSOServiceDeliveryChainList)
                    {
                        var objTSOServiceDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);

                        objTSODTOList.Add(objTSOServiceDeliveryChainDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objTSODTOList;

        }

        [AllowAnonymous]
        [HttpGet, Route("v1/TSO/GetServiceDeliveryChainTasksAPI")]
        public IHttpActionResult GetServiceDeliveryChainTasksAPI()
        {
            //var MaxJsonLength = 1024000;
            var MaxJsonLength = 2147483644;
            var RecursionLimit = 100;

            List<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = GetAllServiceDeliveryChainTask().ToList();

            List<TSOServiceDeliveryChainTaskDumpDTO> objTSOServiceDeliveryChainDTOListResult = new List<TSOServiceDeliveryChainTaskDumpDTO>();
            try
            {
                if (null != objTSOServiceDeliveryChainDTOList)
                {
                    //    objTSOServiceDeliveryChainDTOList = objTSOServiceDeliveryChainDTOList.OrderBy(x => x.WeekNumber).ThenBy(x => x.TSOServiceDeliveryChainId).ToList();
                    //    while (objTSOServiceDeliveryChainDTOList.Count != 0)
                    //    {
                    //        int tsoSDCId = objTSOServiceDeliveryChainDTOList[0].TSOServiceDeliveryChainId;
                    //        var currentTSOCIdList = objTSOServiceDeliveryChainDTOList.Where(x => x.TSOServiceDeliveryChainId == tsoSDCId).ToList();
                    //        for (int taskCount = 0; taskCount < currentTSOCIdList.Count; taskCount++)
                    //        {

                    //            TSOServiceDeliveryChainDTO sdcTask = currentTSOCIdList[taskCount];
                    //            TSOServiceDeliveryChainDTO sdcTaskPrevious;

                    //            double? PlannedEffort = 0.00;
                    //            double PlannedInput = 0.00;
                    //            double PlannedProcessingTime = 0.00;
                    //            double PlannedOutcome = 0.00;
                    //            double PlannedOutcomeTestSteps = 0.00;
                    //            double PlannedOutcomeRatio = 0.00;
                    //            double PlannedThroughput = 0.00;
                    //            double? PlannedProductivity = 0.00;
                    //            double PlannedReviewRounds = 0.00;

                    //            if (taskCount != 0)
                    //            {
                    //                sdcTaskPrevious = currentTSOCIdList[taskCount - 1];
                    //                if (sdcTask.TSOServiceDeliveryChainId == sdcTaskPrevious.TSOServiceDeliveryChainId)
                    //                {
                    //                    PlannedEffort = sdcTaskPrevious.PlannedEffort;
                    //                    PlannedInput = sdcTaskPrevious.PlannedInput;
                    //                    PlannedProcessingTime = sdcTaskPrevious.PlannedProcessingTime;
                    //                    PlannedOutcome = sdcTaskPrevious.PlannedOutcome;
                    //                    PlannedOutcomeTestSteps = sdcTaskPrevious.PlannedOutcomeTestSteps;
                    //                    PlannedReviewRounds = sdcTaskPrevious.PlannedReviewRounds;
                    //                    PlannedOutcomeRatio = PlannedInput != 0 ? PlannedOutcome / PlannedInput : 0.0;
                    //                    PlannedThroughput = PlannedProcessingTime != 0 ? PlannedOutcome / PlannedProcessingTime : 0;
                    //                    PlannedProductivity = PlannedEffort != 0 ? PlannedOutcome / PlannedEffort : 0;
                    //                }
                    //            }
                    //            TSOServiceDeliveryChainDumpDTO task = new TSOServiceDeliveryChainDumpDTO();
                    //            // Append data                 
                    //            task.ID = sdcTask.ID;
                    //            task.Title = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.ServiceDeliveryChain.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.ServiceDeliveryChain.Name) + "( " + sdcTask.TSOServiceDeliveryChain.ServiceDeliveryChain.Description + " )";
                    //            task.Client = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Client.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Client.Name);
                    //            task.TaskStatus = string.IsNullOrEmpty(sdcTask.TaskStatus.Name) ? string.Empty : string.Format("{0}", sdcTask.TaskStatus.Name);
                    //            task.RelatedTSR = string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSRId);
                    //            task.RelatedTSO = string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSOId);
                    //            task.EnagementModel = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Engagement.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Engagement.Name);
                    //            task.PricingModel = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.PricingModel.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.PricingModel.Name);
                    //            task.ProjectModel = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.ProjectModel.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.ProjectModel.Name);
                    //            task.ClientRegion = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.ClientRegion.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.ClientRegion.Name);
                    //            task.Account = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Account) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Account);
                    //            task.ERPOrderNumber = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.ERPordernumber) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.ERPordernumber);
                    //            task.SolutionCenter = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre.Name);
                    //            task.CoreService = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.CoreService.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.CoreService.Name);
                    //            task.Method = sdcTask.ServiceDeliveryChainMethod == null ? string.Empty : string.Format("{0}", sdcTask.ServiceDeliveryChainMethod.Name);
                    //            task.PlannedProcessingTime = string.Format("{0:0.00}", PlannedProcessingTime);
                    //            task.ActualProcessingTime = string.Format("{0:0.00}", sdcTask.ActualProcessingTime);
                    //            task.PlannedEffort = string.Format("{0:0.00}", PlannedEffort);
                    //            task.ActualEffort = string.Format("{0:0.00}", sdcTask.ActualEffort);
                    //            task.PlannedInput = string.Format("{0}", PlannedInput);
                    //            task.ActualInput = string.Format("{0}", sdcTask.ActualInput);
                    //            task.PlannedOutcome = string.Format("{0}", PlannedOutcome);
                    //            task.ActualOutcome = string.Format("{0}", sdcTask.ActualOutcome);
                    //            task.PlannedOutcomeTestSteps = string.Format("{0}", PlannedOutcomeTestSteps);
                    //            task.ActualOutcomeTestSteps = string.Format("{0}", sdcTask.ActualOutcomeTestSteps);
                    //            sdcTask.CalculateFields();
                    //            task.PlannedOutcomeRatio = string.Format("{0:0.00}", PlannedOutcomeRatio);
                    //            task.ActualOutcomeRatio = string.Format("{0:0.00}", sdcTask.ActualOutcomeRatio);
                    //            task.PlannedThroughput = string.Format("{0:0.00}", PlannedThroughput);
                    //            task.ActualThroughput = string.Format("{0:0.00}", sdcTask.ActualThroughput);
                    //            task.PlannedProductivity = string.Format("{0:0.00}", PlannedProductivity);
                    //            task.ActualProductivity = string.Format("{0:0.00}", sdcTask.ActualProductivity);
                    //            task.PlannedReviewRounds = string.Format("{0}", PlannedReviewRounds);
                    //            task.ActualReviewRounds = string.Format("{0}", sdcTask.ActualReviewRounds);
                    //            task.DefectRaised = string.Format("{0}", sdcTask.DefectRaised);
                    //            task.DefectRejected = string.Format("{0}", sdcTask.DefectRejected);
                    //            task.DefectDensity = string.Format("{0}", sdcTask.DefectDensity);
                    //            task.DefectRejectionRatio = string.Format("{0}", sdcTask.DefectRejectionRatio);
                    //            task.Headcount = string.Format("{0}", sdcTask.Headcount);
                    //            task.IdleTimeDuration = string.Format("{0:0.00}", sdcTask.IdleTimeDuration);
                    //            task.IdleTimeEffort = string.Format("{0:0.00}", sdcTask.IdleTimeEffort);
                    //            task.StartDate = sdcTask.PlannedStartDate.ToString("dd-MMM-yyyy");
                    //            task.TargetCompletionDate = sdcTask.PlannedCompletionDate.ToString("dd-MMM-yyyy");
                    //            task.AccountManager = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.AccountManager.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.AccountManager.Name);
                    //            task.DeliveryManager = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.DeliveryManager.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.DeliveryManager.Name);
                    //            task.TestManager = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.TestManager.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.TestManager.Name);
                    //            task.TeamLead = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TeamLead.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TeamLead.Name);
                    //            task.Vertical = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Vertical.Name ?? "") ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Vertical.Name);
                    //            task.Practice = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Practice.Name ?? "") ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Practice.Name);
                    //            task.Modified = sdcTask.UpdatedOn.ToString("dd-MMM-yyyy");
                    //            task.ModifiedBy = string.IsNullOrEmpty(sdcTask.UpdatedBy) ? string.Empty : sdcTask.UpdatedBy;
                    //            task.Notes = string.IsNullOrEmpty(sdcTask.Notes ?? "") ? string.Empty : string.Format("{0}", sdcTask.Notes);
                    //            task.WeekNumber = string.Format("{0}", sdcTask.WeekNumber);

                    //            objTSOServiceDeliveryChainDTOListResult.Add(task);
                    //        }
                    //        {
                    //            TSOServiceDeliveryChainDTO sdcTask = currentTSOCIdList[currentTSOCIdList.Count - 1];

                    //            double? PlannedEffort = 0;
                    //            double PlannedInput = 0;
                    //            double PlannedProcessingTime = 0;
                    //            double PlannedOutcome = 0;
                    //            double PlannedOutcomeTestSteps = 0;
                    //            double PlannedOutcomeRatio = 0;
                    //            double PlannedThroughput = 0;
                    //            double? PlannedProductivity = 0;
                    //            double PlannedReviewRounds = 0;


                    //            PlannedEffort = sdcTask.PlannedEffort;
                    //            PlannedInput = sdcTask.PlannedInput;
                    //            PlannedProcessingTime = sdcTask.PlannedProcessingTime;
                    //            PlannedOutcome = sdcTask.PlannedOutcome;
                    //            PlannedOutcomeTestSteps = sdcTask.PlannedOutcomeTestSteps;
                    //            PlannedReviewRounds = sdcTask.PlannedReviewRounds;
                    //            PlannedOutcomeRatio = PlannedInput != 0 ? PlannedOutcome / PlannedInput : 0;
                    //            PlannedThroughput = PlannedProcessingTime != 0 ? PlannedOutcome / PlannedProcessingTime : 0;
                    //            PlannedProductivity = PlannedEffort != 0 ? PlannedOutcome / PlannedEffort : 0;

                    //            TSOServiceDeliveryChainDumpDTO task = new TSOServiceDeliveryChainDumpDTO();

                    //            // Append data                 
                    //            task.ID = sdcTask.ID;
                    //            task.Title = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.ServiceDeliveryChain.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.ServiceDeliveryChain.Name) + "( " + sdcTask.TSOServiceDeliveryChain.ServiceDeliveryChain.Description + " )";
                    //            task.Client = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Client.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Client.Name);
                    //            task.TaskStatus = string.IsNullOrEmpty(sdcTask.TaskStatus.Name) ? string.Empty : string.Format("{0}", sdcTask.TaskStatus.Name);
                    //            task.RelatedTSR = string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSRId);
                    //            task.RelatedTSO = string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSOId);
                    //            task.EnagementModel = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Engagement.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Engagement.Name);
                    //            task.PricingModel = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.PricingModel.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.PricingModel.Name);
                    //            task.ProjectModel = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.ProjectModel.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.ProjectModel.Name);
                    //            task.ClientRegion = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.ClientRegion.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.ClientRegion.Name);
                    //            task.Account = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Account) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Account);
                    //            task.ERPOrderNumber = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.ERPordernumber) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.ERPordernumber);
                    //            task.SolutionCenter = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre.Name);
                    //            task.CoreService = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.CoreService.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.CoreService.Name);
                    //            task.Method = sdcTask.ServiceDeliveryChainMethod == null ? string.Empty : string.Format("{0}", sdcTask.ServiceDeliveryChainMethod.Name);
                    //            task.PlannedProcessingTime = string.Format("{0:0.00}", PlannedProcessingTime);
                    //            task.ActualProcessingTime = string.Format("{0:0.00}", 0);
                    //            task.PlannedEffort = string.Format("{0:0.00}", PlannedEffort);
                    //            task.ActualEffort = string.Format("{0:0.00}", 0);
                    //            task.PlannedInput = string.Format("{0}", PlannedInput);
                    //            task.ActualInput = string.Format("{0}", 0);
                    //            task.PlannedOutcome = string.Format("{0}", PlannedOutcome);
                    //            task.ActualOutcome = string.Format("{0}", 0);
                    //            task.PlannedOutcomeTestSteps = string.Format("{0}", PlannedOutcomeTestSteps);
                    //            task.ActualOutcomeTestSteps = string.Format("{0}", 0);
                    //            sdcTask.CalculateFields();
                    //            task.PlannedOutcomeRatio = string.Format("{0:0.00}", PlannedOutcomeRatio);
                    //            task.ActualOutcomeRatio = string.Format("{0:0.00}", 0);
                    //            task.PlannedThroughput = string.Format("{0:0.00}", PlannedThroughput);
                    //            task.ActualThroughput = string.Format("{0:0.00}", 0);
                    //            task.PlannedProductivity = string.Format("{0:0.00}", PlannedProductivity);
                    //            task.ActualProductivity = string.Format("{0:0.00}", 0);
                    //            task.PlannedReviewRounds = string.Format("{0}", PlannedReviewRounds);
                    //            task.ActualReviewRounds = string.Format("{0}", 1);
                    //            task.DefectRaised = string.Format("{0}", 0);
                    //            task.DefectRejected = string.Format("{0}", 0);
                    //            task.DefectDensity = string.Format("{0}", 0);
                    //            task.DefectRejectionRatio = string.Format("{0}", 0);
                    //            task.Headcount = string.Format("{0}", 0);
                    //            task.IdleTimeDuration = string.Format("{0:0.00}", 0);
                    //            task.IdleTimeEffort = string.Format("{0:0.00}", 0);


                    //            task.StartDate = sdcTask.PlannedStartDate.ToString("dd-MMM-yyyy");
                    //            task.TargetCompletionDate = sdcTask.PlannedCompletionDate.ToString("dd-MMM-yyyy");
                    //            task.AccountManager = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.AccountManager.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.AccountManager.Name);
                    //            task.DeliveryManager = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.DeliveryManager.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.DeliveryManager.Name);
                    //            task.TestManager = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.TestManager.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.TestManager.Name);
                    //            task.TeamLead = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TeamLead.Name) ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TeamLead.Name);
                    //            task.Vertical = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Vertical.Name ?? "") ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Vertical.Name);
                    //            task.Practice = string.IsNullOrEmpty(sdcTask.TSOServiceDeliveryChain.TSO.TSR.Practice.Name ?? "") ? string.Empty : string.Format("{0}", sdcTask.TSOServiceDeliveryChain.TSO.TSR.Practice.Name);
                    //            task.Modified = sdcTask.UpdatedOn.ToString("dd-MMM-yyyy");
                    //            task.ModifiedBy = string.IsNullOrEmpty(sdcTask.UpdatedBy) ? string.Empty : sdcTask.UpdatedBy;
                    //            task.Notes = string.IsNullOrEmpty(sdcTask.Notes ?? "") ? string.Empty : string.Format("{0}", sdcTask.Notes);
                    //            task.WeekNumber = string.Format("{0}", (sdcTask.WeekNumber + 1));

                    //            objTSOServiceDeliveryChainDTOListResult.Add(task);
                    //        }
                    //        objTSOServiceDeliveryChainDTOList.RemoveAll(x => x.TSOServiceDeliveryChainId == tsoSDCId);
                    //    }
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = MaxJsonLength, RecursionLimit = RecursionLimit };
                return Ok(serializer.Serialize(objTSOServiceDeliveryChainDTOListResult));

                //return Json(objTSOServiceDeliveryChainDTOListResult, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion


        /// <summary>
        /// SearchByTitle
        /// </summary>
        /// <param name="startingRecordNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [ResponseType(typeof(DataCollection))]
        [HttpGet, Route("v1/TSO/SearchByTitleOrClient/{startingRecordNumber}/{pageSize}/{Title}/{Status}")]
        public JsonResult<DataCollection> SearchByTitleOrClient(int startingRecordNumber, int pageSize, string Title, int Status)
        {
            int totalRecords = 0; DataCollection objDataCollection;
            Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
            string userId = Request.Headers.GetValues("userid").FirstOrDefault();
            string User = string.Empty;
            if (!String.IsNullOrWhiteSpace(userId))
            {
                userId = userId.Replace("~", "=").Replace("!", "+");
                User = objCryptorEngine.Decrypt(userId, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                User = usrInfo[usrInfo.Length - 2];
            }
            Title = GetSpecialChar(Title);
            // IList<TSODTO> objTSOList = this.GetTSOByTitleOrClient(startingRecordNumber, pageSize, Title, Status, out totalRecords);
            IList<TSODTO> objTSOList = this.GetTSOByTitleOrClient(startingRecordNumber, pageSize, Title, Status, User, out totalRecords);
            try
            {
                if (objTSOList.Count != 0)
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };

                    foreach (TSODTO objTSOType in objTSOList)
                    {
                        objDataCollection.EntitySummary.Add(objTSOType);
                    }
                }
                else
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, Message = "No Records Found", EntitySummary = new List<IBaseObject>() };

                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }

            return Json(objDataCollection, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// GetTSRByTitleOrClient
        /// </summary>
        /// <param name="startingRecordNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="title"></param>
        /// <param name="status"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        [ResponseType(typeof(DataCollection))]
        [HttpGet, Route("v1/TSO/GetTSOByAdvanceSearch/{pageNumber}/{pageSize}/{tsrId}/{strcenter}/{strclient}/{strid}/{strtitle}/{strstatus}/{strpractice}")]
        public JsonResult<DataCollection> GetTSOByAdvanceSearch(int pageNumber, int pageSize, int tsrId, string strcenter, string strclient, int strid, string strtitle, int strstatus, string strpractice)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            DataCollection objDataCollection;
            IList<TSO> objTSOList = null;
            int totalRecords = pageSize; ;
            IList<TSODTO> objTSODTOList = new List<TSODTO>();
            try
            {
                strclient = GetSpecialChar(strclient);
                strtitle = GetSpecialChar(strtitle);
                strcenter = GetSpecialChar(strcenter);
                strpractice = GetSpecialChar(strpractice);

                string sql = "Select * from TSO as tso " +
                    "inner join TSR as tsr on tso.TSRId = tsr.id inner join SolutionCentre as sc on tsr.SolutionCentreId = sc.Id " +
                    "inner join Practice as p on tsr.PracticeId = p.Id inner join Client as cli on tsr.ClientId = cli.Id where";

                if (strid > 0)
                {
                    sql = sql + " tso.ID = " + strid;
                }

                if (tsrId > 0)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.TSRId = " + tsrId;
                    }
                    else
                    {
                        sql = sql + " tso.TSRId = " + tsrId;
                    }
                }

                if (strcenter != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and sc.Name like '%" + strcenter + "%'";
                    }
                    else
                    {
                        sql = sql + " sc.Name like '%" + strcenter + "%'";
                    }
                }

                if (strclient != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and cli.Name like '%" + strclient + "%'";
                    }
                    else
                    {
                        sql = sql + " cli.Name like '%" + strclient + "%'";
                    }
                }

                if (strtitle != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.Title like '%" + strtitle + "%'";
                    }
                    else
                    {
                        sql = sql + " tso.Title like '%" + strtitle + "%'";
                    }
                }

                if (strpractice != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and p.Name like '%" + strpractice + "%'";
                    }
                    else
                    {
                        sql = sql + " p.Name like '%" + strpractice + "%'";
                    }
                }

                if (strstatus > -1)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.TSOStatusID = " + strstatus;
                    }
                    else
                    {
                        sql = sql + " tso.TSOStatusID = " + strstatus;
                    }
                }

                string userId = Request.Headers.GetValues("userid").FirstOrDefault();
                userId = userId.Replace("~", "=").Replace("!", "+");
                Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
                string User = objCryptorEngine.Decrypt(userId, true);
                bool isAdmin = User.ToLower().Contains("admin");
                bool isguest = User.ToLower().Contains("guest");
                string[] usrInfo = User.Split('#');
                User = usrInfo[usrInfo.Length - 2];
                int userID = Convert.ToInt32(User);

                if (!isAdmin && !isguest)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and (tso.TeamLeadId = " + userID + " or tsr.AccountManagerId = " + userID + " or tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + ")";
                    }
                    else
                    {
                        sql = sql + " (tso.TeamLeadId = " + userID + " or tsr.AccountManagerId = " + userID + " or tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + ")";
                    }
                }
                int rows = pageNumber - 1;
                sql = sql + " ORDER BY tso.id OFFSET " + (rows < 0 ? 0 : rows) + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

                objTSOList = objBusinessLayer.TSORepository.GetWithRawSql(sql);



                TSODTO objTSODTO = new TSODTO();
                foreach (TSO objTSO in objTSOList)
                {
                    double? ActualEffort = 0;
                    double? ActualOutcome = 0;
                    TSR objTSR = new TSR();
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == objTSO.TSRId,
                    x => x.Practice, x => x.SolutionCentre, x => x.ClientRegion, x => x.Client,
                    x => x.TSRCoreServices, x => x.TSRReleventRepositories);

                    objTSO.TSR = objTSR;
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSO.TSOServiceDeliveryChains = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSOId == objTSO.ID, x => x.TSOServiceDeliveryChainActualTasks,
                    x => x.TSO.TSOProductivityOutcomes, x => x.TSO.TSOProductivityOutcomes.Select(z => z.ProductivityOutcome),
                    x => x.TSOProductivityOutcomeActuals, x => x.TSOProductivityOutcomePlanneds).ToList();

                    foreach (TSOServiceDeliveryChain item in objTSO.TSOServiceDeliveryChains)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        ServiceDeliveryChain chain = objBusinessLayer.ServiceDeliveryChainRepository.GetSingle(x => x.ID == item.ServiceDeliveryChainId);
                        if (null != chain)
                        {
                            item.ServiceDeliveryChain = chain;
                        }

                        IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = this.GetLastTSOServiceDeliveryChainList(item.ID);
                        foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO in objTSOServiceDeliveryChainDTOList)
                        {
                            if (objTSOServiceDeliveryChainDTO != null)
                            {
                                foreach (var task in objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks)
                                {
                                    ActualEffort = ActualEffort + (task.ActualEffort == null ? 0 : task.ActualEffort);
                                }
                            }
                        }

                        foreach (TSOProductivityOutcomeActual objTSOProductivityOutcomeDTO in item.TSOProductivityOutcomeActuals)
                        {
                            ActualOutcome += objTSOProductivityOutcomeDTO.OutcomeValue;
                        }
                    }


                    objBusinessLayer = new BusinessLayer(ls);
                    objTSO.RelevantRepository = objBusinessLayer.RelevantRepositoryRepository.GetSingle(x => x.ID == objTSO.RelevantRepositoryId);

                    objTSODTO = Conversions.ToDTO<TSODTO, TSO>(objTSO);

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSODTO.TeamLead = Conversions.ToDTO<UserDTO, User>(objBusinessLayer.UserRepository.GetSingle(x => x.ID == objTSO.TeamLeadId));

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSODTO.TSOStatus = Conversions.ToDTO<TSOStatusDTO, TSOStatus>(objBusinessLayer.TSOStatusRepository.GetSingle(x => x.ID == objTSO.TSOStatusID));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSODTO.OperationalRisk = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objBusinessLayer.OperationalRiskRepository.GetSingle(x => x.ID == objTSR.OperationalRiskId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSODTO.CoreService = Conversions.ToDTO<CoreServiceDTO, CoreService>(objBusinessLayer.CoreServiceRepository.GetSingle(x => x.ID == objTSO.CoreServiceId));

                    objTSODTO.ActualEffort = ActualEffort;
                    objTSODTO.ActualOutcome = (ActualOutcome == null) ? 0 : ActualOutcome;
                    objTSODTO.OutomeCompletion = (objTSODTO.PlannedOutcome == 0) ? 0 : ((objTSODTO.ActualOutcome / objTSODTO.PlannedOutcome) * 100);

                    objTSODTOList.Add(objTSODTO);
                }
                totalRecords = objTSOList.Count();
                if (objTSOList.Count != 0)
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, Message = "No Records Found", EntitySummary = new List<IBaseObject>() };

                    foreach (TSODTO objTSRType in objTSODTOList)
                    {
                        objDataCollection.EntitySummary.Add(objTSRType);
                    }
                }
                else
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, Message = "No Records Found", EntitySummary = new List<IBaseObject>() };

                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }
            return Json(objDataCollection, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }); ;
        }

        /// <summary>
        /// CreateTSR
        /// </summary>
        /// <param name="tsrDTO">TSRDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpGet, Route("v1/TSO/GetTSOByAdvanceSearchCount/{tsrId}/{strcenter}/{strclient}/{strid}/{strtitle}/{strstatus}/{strpractice}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<string> GetTSOByAdvanceSearchCount(int tsrId, string strcenter, string strclient, int strid, string strtitle, int strstatus, string strpractice)
        {
            JsonResult<string> result = Json(string.Empty);
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                strclient = GetSpecialChar(strclient);
                strtitle = GetSpecialChar(strtitle);
                strcenter = GetSpecialChar(strcenter);
                strpractice = GetSpecialChar(strpractice);

                IList<TSO> objTSOList;// = objBusinessLayer.TSRRepository.GetAll(x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                string sql = "Select * from TSO as tso " +
                   "inner join TSR as tsr on tso.TSRId = tsr.id inner join SolutionCentre as sc on tsr.SolutionCentreId = sc.Id " +
                   "inner join Practice as p on tsr.PracticeId = p.Id inner join Client as cli on tsr.ClientId = cli.Id where";

                if (strid > 0)
                {
                    sql = sql + " tso.ID = " + strid;
                }

                if (tsrId > 0)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.TSRId = " + tsrId;
                    }
                    else
                    {
                        sql = sql + " tso.TSRId = " + tsrId;
                    }
                }

                if (strcenter != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and sc.Name like '%" + strcenter + "%'";
                    }
                    else
                    {
                        sql = sql + " sc.Name like '%" + strcenter + "%'";
                    }
                }

                if (strclient != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and cli.Name like '%" + strclient + "%'";
                    }
                    else
                    {
                        sql = sql + " cli.Name like '%" + strclient + "%'";
                    }
                }

                if (strtitle != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.Title like '%" + strtitle + "%'";
                    }
                    else
                    {
                        sql = sql + " tso.Title like '%" + strtitle + "%'";
                    }
                }

                if (strpractice != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and p.Name like '%" + strpractice + "%'";
                    }
                    else
                    {
                        sql = sql + " p.Name like '%" + strpractice + "%'";
                    }
                }

                if (strstatus > -1)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.TSOStatusID = " + strstatus;
                    }
                    else
                    {
                        sql = sql + " tso.TSOStatusID = " + strstatus;
                    }
                }

                string userId = Request.Headers.GetValues("userid").FirstOrDefault();
                userId = userId.Replace("~", "=").Replace("!", "+");
                Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
                string User = objCryptorEngine.Decrypt(userId, true);
                bool isAdmin = User.ToLower().Contains("admin");
                bool isguest = User.ToLower().Contains("guest");
                string[] usrInfo = User.Split('#');
                User = usrInfo[usrInfo.Length - 2];
                int userID = Convert.ToInt32(User);
                if (!isAdmin && !isguest)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and (tso.TeamLeadId = " + userID + " or tsr.AccountManagerId = " + userID + " or tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + ")";
                    }
                    else
                    {
                        sql = sql + " (tso.TeamLeadId = " + userID + " or tsr.AccountManagerId = " + userID + " or tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + ")";
                    }
                }

                objTSOList = objBusinessLayer.TSORepository.GetWithRawSql(sql);
                result = Json(objTSOList.Count.ToString(), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (DbEntityValidationException eve)
            {
                StringBuilder objSB = new StringBuilder();
                foreach (var objEve in eve.EntityValidationErrors)
                {
                    objSB.Append(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", objEve.Entry.Entity.GetType().Name, objEve.Entry.State));
                    objSB.Append("\n");
                    foreach (var ve in objEve.ValidationErrors)
                    {
                        objSB.Append(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        objSB.Append("\n");
                    }
                }
                TTMLogger.Logger.LogError(String.Format("Error - {0}", objSB.ToString()));
                result = Json(String.Format("Error - {0}", objSB.ToString()), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        public string GetSpecialChar(string Title)
        {
            if (Title.IndexOf("_undefindAmpersand_") > -1)
                Title = Title.Replace("_undefindAmpersand_", "&");
            if (Title.IndexOf("_undefindAsterisk_") > -1)
                Title = Title.Replace("_undefindAsterisk_", "*");
            if (Title.IndexOf("_undefindCaret_") > -1)
                Title = Title.Replace("_undefindCaret_", "^");
            if (Title.IndexOf("_undefindLessthan_") > -1)
                Title = Title.Replace("_undefindLessthan_", "<");
            if (Title.IndexOf("_undefindGreaterthan_") > -1)
                Title = Title.Replace("_undefindGreaterthan_", ">");
            if (Title.IndexOf("_undefindQuestionmark_") > -1)
                Title = Title.Replace("_undefindQuestionmark_", "?");
            if (Title.IndexOf("_undefindFullstop_") > -1)
                Title = Title.Replace("_undefindFullstop_", ".");
            if (Title.IndexOf("_undefindSlash_") > -1)
                Title = Title.Replace("_undefindSlash_", "/");
            if (Title.IndexOf("_undefindColon_") > -1)
                Title = Title.Replace("_undefindColon_", ":");
            if (Title.IndexOf("_undefindBackslash_") > -1)
                Title = Title.Replace("_undefindBackslash_", "\\");
            if (Title.IndexOf("'") > -1)
                Title = Title.Replace("'", "''''");
            return Title;
        }
    }
}