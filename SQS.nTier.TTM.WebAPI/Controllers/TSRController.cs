﻿/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 04Oct2017 Created the class
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
    /// <summary>
    /// TSRController
    /// </summary>
    [RoutePrefix("api")]
    public class TSRController : ApiController
    {
        //public object TTMLogger { get; private set; }

        public TSRDTO GetTSR(int id)
        {
            TSRDTO tsrDto = this.GetTSRById(id);
            return tsrDto;
        }

        #region Private Functions

        /// <summary>
        /// GetTSRById
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="loadEntities">bool</param>
        /// <returns>TSR</returns>
        private TSRDTO GetTSRById(int id, bool loadEntities = true)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            TSRDTO objTSRDTO = null;

            try
            {
                TSR objTSR = null;


                if (loadEntities)
                {
                    objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == id,
                    x => x.DeliveryManager,
                    x => x.TestManager,
                    x => x.AccountManager,
                    x => x.Vertical,
                    x => x.Practice,
                    x => x.SolutionCentre,
                    x => x.ClientRegion,
                    x => x.MarketOffering,
                    x => x.OperationalRisk,
                    x => x.TSRCoreServices,
                    x => x.TSRReleventRepositories,
                    x => x.TSRFiles,
                    x => x.TSOes,
                    x => x.TSRStatus,
                    x => x.TSRTMOUsers
                  );

                }
                else
                {
                    objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == id,
                    x => x.TSRCoreServices,
                    x => x.TSRReleventRepositories,
                    x => x.TSRFiles,
                    x => x.TSOes);
                }

                if (null != objTSR)
                {
                    objTSRDTO = new TSRDTO();
                    objTSRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSR);
                    objTSRDTO.FilePaths = new List<string>();

                    String fileUploadPath = ConfigurationManager.AppSettings["fileUploadPath"].ToString();

                    foreach (TSRFileUpload files in objTSR.TSRFiles)
                    {
                        string ext;
                        if (files.Path.Contains("."))
                            ext = files.Path.Substring(files.Path.LastIndexOf("."));
                        else
                            ext = "";
                        objTSRDTO.FilePaths.Add(fileUploadPath + files.GUID + ext);
                    }
                }

                if (objTSR != null)
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == objTSR.ID, x => x.TSOServiceDeliveryChains, x => x.TSR);
                    int tsoCC = 0;
                    int taskCC = 0;
                    int taskCount = 0;
                    bool canClose = false;
                    objTSRDTO.ChildTSOPlannedEffortSum = 0;
                    double ChildTSOPlannedEffortSum = 0;
                    foreach (TSO tso in objTSOList)
                    {
                        ChildTSOPlannedEffortSum = ChildTSOPlannedEffortSum + (tso.PlannedEffort ?? 0);
                        if (GetTSOStatus(tso.TSOStatusID).ToLower() == "cancelled" || GetTSOStatus(tso.TSOStatusID).ToLower() == "closed")
                        {
                            tsoCC++;
                        }
                        if (tso.TSOServiceDeliveryChains != null && tso.TSOServiceDeliveryChains.Count > 0)
                        {
                            foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in tso.TSOServiceDeliveryChains)
                            {
                                IBusinessLayer oldobjBusinessLayer = new BusinessLayer(ls);
                                TSOServiceDeliveryChain objTSOServiceDeliveryChainTask = oldobjBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == objTSOServiceDeliveryChain.ID, x => x.TSOServiceDeliveryChainActualTasks, x => x.TSOServiceDeliveryChainPlannedTasks);

                                if (objTSOServiceDeliveryChainTask != null)
                                {
                                    oldobjBusinessLayer = new BusinessLayer(ls);
                                    //IList<TSOServiceDeliveryChainTaskActual> objTSOServiceDeliveryChainTaskActualList = oldobjBusinessLayer.TSOServiceDeliveryChainTaskActualRepository.GetList(x => x.TSOServiceDeliveryChainTaskId == objTSOServiceDeliveryChainTask.ID);
                                    foreach (var objTSOServiceDeliveryChainTaskActual in objTSOServiceDeliveryChainTask.TSOServiceDeliveryChainActualTasks)
                                    {
                                        // objTSRDTO.ActualEffort = objTSRDTO.ActualEffort + (objTSOServiceDeliveryChainTask.ActualEffort ?? 0);
                                        objTSRDTO.ActualEffort = objTSRDTO.ActualEffort + objTSOServiceDeliveryChainTaskActual.ActualEffort;
                                    }
                                    TSOController tsoController = new TSOController();
                                    if (objTSOServiceDeliveryChainTask.TaskStatusId > 0)
                                    {
                                        taskCount++;
                                        if (tsoController.GetTaskStatus(objTSOServiceDeliveryChainTask.TaskStatusId) == "cancelled"
                                        || tsoController.GetTaskStatus(objTSOServiceDeliveryChainTask.TaskStatusId) == "closed")
                                        {
                                            taskCC++;
                                        }
                                    }
                                }
                            }

                            if (taskCC == taskCount)
                            {
                                canClose = true;
                            }
                        }
                    }
                    if (objTSRDTO.TSRTMOUsers.Count() > 0)
                    {
                        foreach (var item in objTSRDTO.TSRTMOUsers)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == item.UserId);
                            UserDTO objuserDTO = Conversions.ToDTO<UserDTO, User>(objUser);
                            item.User = objuserDTO;

                        }
                    }

                    if (tsoCC == objTSOList.Count)
                    {
                        canClose = true;
                    }
                    objTSRDTO.CanClose = canClose;
                    objTSRDTO.ChildTSOPlannedEffortSum = ChildTSOPlannedEffortSum;
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



            objTSRDTO.ActualEffort = Math.Round((Double)objTSRDTO.ActualEffort, 2);

            return objTSRDTO;
        }

        private string GetTSOStatus(int statusId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            string status = null;
            try
            {
                TSOStatus objTSOStatus = objBusinessLayer.TSOStatusRepository.GetSingle(x => x.ID == statusId);
                status = objTSOStatus.Name;
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

        /// <summary>
        /// GetAllTSR
        /// </summary>
        /// <returns>IList<TSR>></returns>
        private IList<TSRDTO> GetAllTSR(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            IList<TSRDTO> objTSRDTOList = null;
            try
            {
                IList<TSR> objTSRList;
                string userId = Request.Headers.GetValues("userid").FirstOrDefault();
                userId = userId.Replace("~", "=").Replace("!", "+");
                Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
                string User = objCryptorEngine.Decrypt(userId, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                User = usrInfo[usrInfo.Length - 2];
                int userID = Convert.ToInt32(User);
                objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);

                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRList = objBusinessLayer.TSRRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices, x => x.TSRTMOUsers);
                }
                else
                {

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRList = objBusinessLayer.TSRRepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices, x => x.TSRTMOUsers);

                    objBusinessLayer = new BusinessLayer(ls);
                    IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TeamLeadId == userID, x => x.TSR);
                    foreach (TSO objTSO in objTSOList)
                    {
                        if (objTSRList.Count < pageSize)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            if (!objTSRList.Any(x => x.ID == objTSO.TSRId))
                            {
                                TSR objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == objTSO.TSRId, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices, x => x.TSRTMOUsers);
                                //objTSRList.Add(objTSR);
                                objTSRList.Add(objTSR);
                            }
                        }
                        else
                        {
                            break;
                        }

                    }

                    objBusinessLayer = new BusinessLayer(ls);
                    List<TSRTMOUser> tmoUsers = objBusinessLayer.TSRTMOUserRepository.GetList(x => x.UserId == userID, x => x.TSR, x => x.TSR.TSRStatus, x => x.TSR.TestManager, x => x.TSR.Client, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.Engagement, x => x.TSR.OperationalRisk, x => x.TSR.Vertical, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.PricingModel, x => x.TSR.ClientRegion, x => x.TSR.TSRReleventRepositories, x => x.TSR.TSRCoreServices, x => x.TSR.TSRTMOUsers).ToList();
                    foreach (var item in tmoUsers)
                    {
                        if (objTSRList.Count < pageSize)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            if (!objTSRList.Any(x => x.ID == item.TSRId))
                            {
                                TSR objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == item.TSRId, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices, x => x.TSRTMOUsers);
                                objTSRList.Add(objTSR);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (objTSRList != null && objTSRList.Count > 0)
                {
                    objTSRDTOList = new List<TSRDTO>();

                    foreach (TSR objTSR in objTSRList)
                    {
                        if (objTSR != null)
                        {
                            double ActualEffort = 0;
                            //double PlannedEffort = 0;
                            foreach (var item in objTSR.TSRReleventRepositories)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                item.RelevantRepository = objBusinessLayer.RelevantRepositoryRepository.GetByID(item.RelevantRepositoryId);
                                objTSR.TSTRelevantRepositoriesArr += item.RelevantRepository.Name + ",";
                            }

                            if (objTSR.TSTRelevantRepositoriesArr != null)
                            {
                                objTSR.TSTRelevantRepositoriesArr = objTSR.TSTRelevantRepositoriesArr.Remove(objTSR.TSTRelevantRepositoriesArr.Length - 1);
                            }


                            foreach (var item in objTSR.TSRCoreServices)
                            {
                                objBusinessLayer = new BusinessLayer(ls);
                                item.CoreService = objBusinessLayer.CoreServiceRepository.GetByID(item.CoreServiceId);
                                objTSR.TSRCoreServicesArr += item.CoreService.Name + ",";
                            }
                            if (objTSR.TSRCoreServicesArr != null)
                            {
                                objTSR.TSRCoreServicesArr = objTSR.TSRCoreServicesArr.Remove(objTSR.TSRCoreServicesArr.Length - 1);
                            }

                            objBusinessLayer = new BusinessLayer(ls);
                            ProjectModel pm = objBusinessLayer.ProjectModelRepository.GetByID(objTSR.ProjectModelID);
                            //ProjectModel

                            objBusinessLayer = new BusinessLayer(ls);
                            IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == objTSR.ID, x => x.TSOServiceDeliveryChains, x => x.TSR);

                            if (objTSOList != null && objTSOList.Count > 0)
                            {
                                foreach (TSO objTSO in objTSOList)
                                {
                                    //PlannedEffort = PlannedEffort + objTSO.PlannedEffort;
                                    if (objTSO.TSOServiceDeliveryChains != null && objTSO.TSOServiceDeliveryChains.Count > 0)
                                    {
                                        foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSO.TSOServiceDeliveryChains)
                                        {
                                            //objBusinessLayer = new BusinessLayer(ls);
                                            //IList<TSOServiceDeliveryChainTask> objTSOServiceDeliveryChainTaskList = objBusinessLayer.TSOServiceDeliveryChainTaskRepository.GetList(x => x.TSOServiceDeliveryChainId == objTSOServiceDeliveryChain.ID);//.FirstOrDefault();

                                            //if (objTSOServiceDeliveryChainTaskList != null && objTSOServiceDeliveryChainTaskList.Count > 0)
                                            //{
                                            //    foreach (TSOServiceDeliveryChainTask objTSOServiceDeliveryChainTask in objTSOServiceDeliveryChainTaskList)
                                            //    {
                                            //        ActualEffort += (double)(objTSOServiceDeliveryChainTask.ActualEffort ?? 0);
                                            //    }
                                            //}

                                            objBusinessLayer = new BusinessLayer(ls);
                                            ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTSOServiceDeliveryChain.ServiceDeliveryChainId);

                                            if (null != objServiceDeliveryChain)
                                            {
                                                objTSOServiceDeliveryChain.ServiceDeliveryChain = objServiceDeliveryChain;
                                            }

                                            IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainTaskDTOList = this.GetLastTSOServiceDeliveryChainTaskList(objTSOServiceDeliveryChain.ID);
                                            foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainTaskDTO in objTSOServiceDeliveryChainTaskDTOList)
                                            {
                                                foreach (var item in objTSOServiceDeliveryChainTaskDTO.TSOServiceDeliveryChainActualTasks)
                                                {
                                                    if (item != null)
                                                    {
                                                        ActualEffort = ActualEffort + (item.ActualEffort ?? 0);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            TSRDTO objTSRDTO = new TSRDTO();
                            objTSRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSR);
                            if (pm != null)
                            {
                                objTSRDTO.ProjectModelName = pm.Name;
                            }
                            if (objTSOList.Count > 0)
                            {
                                objTSRDTO.ActualEffort = ActualEffort;
                                //objTSRDTO.Plannedeffort = PlannedEffort;
                            }
                            objTSRDTOList.Add(objTSRDTO);
                        }
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

            return objTSRDTOList;
        }

        /// <summary>
        /// GetAllTSR
        /// </summary>
        /// <returns>IList<TSR>></returns>
        private IList<TSRDTO> GetAllTSR()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSRDTO> objTSRDTOList = null;
            try
            {
                IList<TSR> objTSRList = objBusinessLayer.TSRRepository.GetAll(x => x.TSRStatus, x => x.TSOes, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices, x => x.TSRFiles);

                if (objTSRList != null && objTSRList.Count > 0)
                {
                    objTSRDTOList = new List<TSRDTO>();

                    foreach (TSR objTSR in objTSRList)
                    {
                        double ActualEffort = 0.00;

                        objBusinessLayer = new BusinessLayer(ls);
                        IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == objTSR.ID, x => x.TSOServiceDeliveryChains, x => x.TSR);

                        if (objTSOList != null && objTSOList.Count > 0)
                        {
                            foreach (TSO objTSO in objTSOList)
                            {
                                //PlannedEffort = PlannedEffort + (float)objTSO.PlannedEffort;
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

                                        IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainTaskDTOList = this.GetLastTSOServiceDeliveryChainTaskList(objTSOServiceDeliveryChain.ID);
                                        foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainTaskDTO in objTSOServiceDeliveryChainTaskDTOList)
                                        {
                                            foreach (var item in objTSOServiceDeliveryChainTaskDTO.TSOServiceDeliveryChainActualTasks)
                                            {
                                                if (item != null)
                                                {
                                                    ActualEffort = ActualEffort + (item.ActualEffort ?? 0);
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        TSRDTO objTSRDTO = new TSRDTO();
                        objTSRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSR);
                        if (objTSOList.Count > 0)
                        {
                            objTSRDTO.ActualEffort = (double)ActualEffort;
                        }
                        objTSRDTOList.Add(objTSRDTO);
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

            return objTSRDTOList;
        }

        /// <summary>
        /// Function to add new TSR 
        /// </summary>
        /// <param name="objTSR"></param>
        /// <returns></returns>
        private string AddNewTSR(TSRDTO objTSRDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objTSRDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                TSR objTSR = null;
                objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.Title == objTSRDTO.Title);

                if (objTSR != null)
                {
                    returnMessage = "Error: TSR title already exists.";
                }
                else
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    string strDefaultTSRStatusText = ConfigurationManager.AppSettings["DefaultTSRStatusText"].ToString();
                    TSRStatus objTSRStatus = objBusinessLayer.TSRStatusRepository.GetSingle(x => x.Name == strDefaultTSRStatusText);

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.TSRStatusID = objTSRStatus.ID;
                    objTSRDTO.ProjectModelID = objTSRDTO.ProjectModelID;
                    objTSR = Conversions.ToEntity<TSRDTO, TSR>(objTSRDTO);
                    objBusinessLayer.TSRRepository.Add(objTSR);

                    if (null != objTSR && objTSR.ID > 0)
                    {
                        if (!String.IsNullOrWhiteSpace(objTSRDTO.TSRCoreServicesArr))
                        {
                            //Add core services to TSR
                            foreach (string tsrCoreService in objTSRDTO.TSRCoreServicesArr.Split(','))
                            {
                                objBusinessLayer = new BusinessLayer(ls);

                                TSRCoreService tSRCoreService = new TSRCoreService { CoreServiceId = int.Parse(tsrCoreService), TSRId = objTSR.ID };

                                objBusinessLayer.TSRCoreServicesRepository.Add(tSRCoreService);
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(objTSRDTO.TSRTMOArr))
                        {
                            //Add TMO to TSR
                            foreach (string tsrtmoUsers in objTSRDTO.TSRTMOArr.Split(','))
                            {
                                objBusinessLayer = new BusinessLayer(ls);

                                TSRTMOUser tSRTMOUSer = new TSRTMOUser { UserId = int.Parse(tsrtmoUsers), TSRId = objTSR.ID };

                                objBusinessLayer.TSRTMOUserRepository.Add(tSRTMOUSer);
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(objTSRDTO.TSTRelevantRepositoriesArr))
                        {
                            //Add relevant repositories to TSR
                            foreach (string tsrRelevantRepository in objTSRDTO.TSTRelevantRepositoriesArr.Split(','))
                            {
                                objBusinessLayer = new BusinessLayer(ls);

                                TSRRelevantRepository tsrReleventRepority = new TSRRelevantRepository { RelevantRepositoryId = int.Parse(tsrRelevantRepository), TSRId = objTSR.ID };

                                objBusinessLayer.TSRRelevantRepositoriesRepository.Add(tsrReleventRepority);
                            }
                        }
                        else
                        {
                            //objBusinessLayer = new BusinessLayer(ls);
                            //RelevantRepository tr = objBusinessLayer.RelevantRepositoryRepository.GetSingle(x => x.Name == "N.A.");
                            //objBusinessLayer = new BusinessLayer(ls);
                            //TSRRelevantRepository tsrReleventRepority = new TSRRelevantRepository { RelevantRepositoryId = tr.ID, TSRId = objTSR.ID };
                            //objBusinessLayer.TSRRelevantRepositoriesRepository.Add(tsrReleventRepority);
                        }
                    }

                    if (objTSRDTO.Client == null)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        Client objClient = objBusinessLayer.ClientRepository.GetByID(objTSRDTO.ClientId);
                        if (null != objClient)
                        {
                            objTSRDTO.Client = Conversions.ToDTO<ClientDTO, Client>(objClient);
                        }
                    }

                    UserController user = new UserController();
                    System.Threading.Tasks.Task.Factory.StartNew(() => user.UserAssignmentAndSendMail(objTSR.TestManagerId, "TSR", objTSRDTO.Title, objTSR.ID.ToString(), objTSRDTO.CreatedBy, objTSRDTO.Client.Name));
                    System.Threading.Tasks.Task.Factory.StartNew(() => user.UserAssignmentAndSendMail(objTSR.DeliveryManagerId, "TSR", objTSRDTO.Title, objTSR.ID.ToString(), objTSRDTO.CreatedBy, objTSRDTO.Client.Name));
                    System.Threading.Tasks.Task.Factory.StartNew(() => user.UserAssignmentAndSendMail(objTSR.AccountManagerId, "TSR", objTSRDTO.Title, objTSR.ID.ToString(), objTSRDTO.CreatedBy, objTSRDTO.Client.Name));

                    returnMessage = objTSR.ID.ToString();
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
        /// Function to update TSR 
        /// </summary>
        /// <param name="objTSR"></param>
        /// <returns></returns>
        private string ModifyTSR(int ID, TSRDTO objTSRDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                TSR chkTSR = null;
                chkTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == ID);

                //objBusinessLayer = new BusinessLayer(ls);
                //if (objBusinessLayer.TSRRepository.GetSingle(x => x.ID != ID && x.Title == objTSRDTO.Title) != null)
                //{
                //returnMessage = "Error: TSR title already exists.";
                //}
                //else
                {
                    TSRDTO objTSRDTOById = this.GetTSRById(ID, false);

                    if (objTSRDTOById != null)
                    {
                        ls = new LoginSession();
                        ls.LoginName = objTSRDTO.UpdatedBy;

                        objTSRDTOById.Title = objTSRDTO.Title;
                        objTSRDTOById.DeliveryManagerId = objTSRDTO.DeliveryManagerId;
                        objTSRDTOById.Description = objTSRDTO.Description;
                        objTSRDTOById.ERPOrderDescription = objTSRDTO.ERPOrderDescription;
                        objTSRDTOById.TestManagerId = objTSRDTO.TestManagerId;
                        objTSRDTOById.VerticalId = objTSRDTO.VerticalId;
                        objTSRDTOById.PracticeId = objTSRDTO.PracticeId;
                        objTSRDTOById.SolutionCentreId = objTSRDTO.SolutionCentreId;
                        objTSRDTOById.ClientRegionId = objTSRDTO.ClientRegionId;
                        objTSRDTOById.Account = objTSRDTO.Account;
                        objTSRDTOById.AccountManagerId = objTSRDTO.AccountManagerId;
                        objTSRDTOById.ERPordernumber = objTSRDTO.ERPordernumber;
                        objTSRDTOById.MarketOfferingId = objTSRDTO.MarketOfferingId;
                        objTSRDTOById.StartDate = objTSRDTO.StartDate;
                        objTSRDTOById.TargetCompletionDate = objTSRDTO.TargetCompletionDate;
                        //objTSRDTOById.ActualStartDate = objTSRDTO.ActualStartDate;
                        //objTSRDTOById.ActualCompletionDate = objTSRDTO.ActualCompletionDate;
                        objTSRDTOById.Estimatedeffort = objTSRDTO.Estimatedeffort;
                        objTSRDTOById.Plannedeffort = objTSRDTO.Plannedeffort;
                        objTSRDTOById.OperationalRiskId = objTSRDTO.OperationalRiskId;
                        objTSRDTOById.ProjectModelID = objTSRDTO.ProjectModelID;
                        objTSRDTOById.ClientId = objTSRDTO.ClientId;
                        objTSRDTOById.EngagementId = objTSRDTO.EngagementId;
                        objTSRDTOById.PricingModelId = objTSRDTO.PricingModelId;
                        objTSRDTOById.TSRStatusID = objTSRDTO.TSRStatusID;
                        objTSRDTOById.SP_Id = objTSRDTO.SP_Id;
                        //objTSRDTOById.Name = objTSRDTO.Name;
                        //objTSRDTOById.Description = objTSRDTO.Description;

                        TSR objTSR = Conversions.ToEntity<TSRDTO, TSR>(objTSRDTOById);
                        objBusinessLayer = new BusinessLayer(ls);
                        objBusinessLayer.TSRRepository.Update(objTSR);

                        if (null != objTSR && objTSR.ID > 0)
                        {
                            //Delete TSR Core Services
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRDTOById = this.GetTSRById(ID);

                            objBusinessLayer = new BusinessLayer(ls);
                            objBusinessLayer.TSRCoreServicesRepository.Delete(x => x.TSRId == objTSRDTOById.ID);

                            if (!String.IsNullOrWhiteSpace(objTSRDTO.TSRCoreServicesArr))
                            {
                                if (objTSRDTO.TSRCoreServicesArr.IndexOf(",") <= 0)
                                {
                                    objBusinessLayer = new BusinessLayer(ls);

                                    TSRCoreService tSRCoreService = new TSRCoreService { CoreServiceId = int.Parse(objTSRDTO.TSRCoreServicesArr), TSRId = objTSR.ID };

                                    objBusinessLayer.TSRCoreServicesRepository.Add(tSRCoreService);
                                }
                                else
                                {
                                    //Add core services to TSR
                                    foreach (string tsrCoreService in objTSRDTO.TSRCoreServicesArr.Split(','))
                                    {
                                        objBusinessLayer = new BusinessLayer(ls);

                                        TSRCoreService tSRCoreService = new TSRCoreService { CoreServiceId = int.Parse(tsrCoreService), TSRId = objTSR.ID };

                                        objBusinessLayer.TSRCoreServicesRepository.Add(tSRCoreService);
                                    }
                                }
                            }


                            // Delete TMO User Repository
                            objBusinessLayer = new BusinessLayer(ls);
                            objBusinessLayer.TSRTMOUserRepository.Delete(x => x.TSRId == objTSRDTOById.ID);

                            if (!String.IsNullOrWhiteSpace(objTSRDTO.TSRTMOArr))
                            {
                                //Add TMO to TSR
                                foreach (string tsrtmoUsers in objTSRDTO.TSRTMOArr.Split(','))
                                {
                                    objBusinessLayer = new BusinessLayer(ls);

                                    TSRTMOUser tSRTMOUSer = new TSRTMOUser { UserId = int.Parse(tsrtmoUsers), TSRId = objTSR.ID };

                                    objBusinessLayer.TSRTMOUserRepository.Add(tSRTMOUSer);
                                }
                            }
                            //Delete TSR Relevant Repository
                            objBusinessLayer = new BusinessLayer(ls);
                            objBusinessLayer.TSRRelevantRepositoriesRepository.Delete(x => x.TSRId == objTSR.ID);

                            if (!String.IsNullOrWhiteSpace(objTSRDTO.TSTRelevantRepositoriesArr))
                            {
                                if (objTSRDTO.TSTRelevantRepositoriesArr.IndexOf(",") <= 0)
                                {
                                    objBusinessLayer = new BusinessLayer(ls);

                                    TSRRelevantRepository tsrReleventRepority = new TSRRelevantRepository { RelevantRepositoryId = int.Parse(objTSRDTO.TSTRelevantRepositoriesArr), TSRId = objTSR.ID };

                                    objBusinessLayer.TSRRelevantRepositoriesRepository.Add(tsrReleventRepority);
                                }
                                else
                                {
                                    //Add relevant repositories to TSR
                                    foreach (string tsrRelevantRepository in objTSRDTO.TSTRelevantRepositoriesArr.Split(','))
                                    {
                                        objBusinessLayer = new BusinessLayer(ls);

                                        TSRRelevantRepository tsrReleventRepority = new TSRRelevantRepository { RelevantRepositoryId = int.Parse(tsrRelevantRepository), TSRId = objTSR.ID };

                                        objBusinessLayer.TSRRelevantRepositoriesRepository.Add(tsrReleventRepority);
                                    }
                                }
                            }

                        }



                        if (objTSRDTO.Client == null)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            Client objClient = objBusinessLayer.ClientRepository.GetByID(objTSRDTO.ClientId);
                            if (null != objClient)
                            {
                                objTSRDTO.Client = Conversions.ToDTO<ClientDTO, Client>(objClient);
                            }
                        }

                        UserController user = new UserController();
                        if (chkTSR.TestManagerId != objTSRDTOById.TestManagerId)
                        {
                            System.Threading.Tasks.Task.Factory.StartNew(() => user.UserAssignmentAndSendMail(objTSR.TestManagerId, "TSR", objTSRDTO.Title, objTSR.ID.ToString(), objTSRDTO.UpdatedBy, objTSRDTO.Client.Name));
                        }
                        if (chkTSR.DeliveryManagerId != objTSRDTOById.DeliveryManagerId)
                        {
                            System.Threading.Tasks.Task.Factory.StartNew(() => user.UserAssignmentAndSendMail(objTSR.DeliveryManagerId, "TSR", objTSRDTO.Title, objTSR.ID.ToString(), objTSRDTO.UpdatedBy, objTSRDTO.Client.Name));
                        }
                        if (chkTSR.AccountManagerId != objTSRDTOById.AccountManagerId)
                        {
                            System.Threading.Tasks.Task.Factory.StartNew(() => user.UserAssignmentAndSendMail(objTSR.AccountManagerId, "TSR", objTSRDTO.Title, objTSR.ID.ToString(), objTSRDTO.UpdatedBy, objTSRDTO.Client.Name));
                        }

                        returnMessage = objTSR.ID.ToString();
                    }
                    else
                    {
                        returnMessage = "TSR do not exists.";
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw ex.InnerException;
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
        /// RemoveTSR
        /// </summary>
        /// <param name="ID">int</param>
        private string RemoveTSR(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                TSRDTO objTSRDTOById = this.GetTSRById(ID, false);

                if (objTSRDTOById != null)
                {
                    if (objTSRDTOById.TSOes != null && objTSRDTOById.TSOes.Count > 0)
                    {
                        returnMessage = "Error: Can not delete TSR as tso's exists for it.";
                    }
                    else
                    {
                        ls = new LoginSession();
                        ls.LoginName = "Admin";

                        TSR objTSR = Conversions.ToEntity<TSRDTO, TSR>(objTSRDTOById);
                        if (objTSR.TSRCoreServices.Count > 0)
                        {
                            //Delete TSR Core Service if exist
                            objBusinessLayer = new BusinessLayer(ls);
                            objBusinessLayer.TSRCoreServicesRepository.Delete(x => x.TSRId == ID);
                        }

                        if (objTSR.TSRReleventRepositories.Count > 0)
                        {
                            //Delete TSR Relevant Repository if exist
                            objBusinessLayer = new BusinessLayer(ls);
                            objBusinessLayer.TSRRelevantRepositoriesRepository.Delete(x => x.TSRId == ID);
                        }

                        if (objTSR.TSRFiles.Count > 0)
                        {
                            //Delete TSR Relevant Repository if exist
                            //objBusinessLayer = new BusinessLayer(ls);
                            RemoveTSRFile(ID.ToString());
                            //objBusinessLayer.TSRFileUploadRepository.Delete(x => x.TSRId == ID);
                        }

                        //now Delete TSR
                        objBusinessLayer = new BusinessLayer(ls);
                        objTSR = objBusinessLayer.TSRRepository.GetByID(ID);
                        objBusinessLayer.TSRRepository.Delete(objTSR);
                        returnMessage = "TSR deleted successfully.";
                    }
                }
                else
                {
                    returnMessage = "TSR do not exists.";
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
        /// GetTSRByTitleOrClient
        /// </summary>
        /// <param name="startingRecordNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="title"></param>
        /// <param name="status"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        private IList<TSRDTO> GetTSRByTitleOrClient(int startingRecordNumber, int pageSize, string strTitleOrClient, int status, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSR> objTSRList = null; totalRecords = 0;
            IList<TSRDTO> objTSRDTOList = new List<TSRDTO>();
            try
            {
                string userId = Request.Headers.GetValues("userid").FirstOrDefault();
                userId = userId.Replace("~", "=").Replace("!", "+");
                Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
                string User = objCryptorEngine.Decrypt(userId, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                User = usrInfo[usrInfo.Length - 2];
                int userID = Convert.ToInt32(User);
                objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);

                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRList = objBusinessLayer.TSRRepository.GetAll(x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                }
                else
                {
                    if (strTitleOrClient != "none")
                    {
                        if (status != 0)
                        {
                            //objBusinessLayer = new BusinessLayer(ls);
                            //objTSRList = objBusinessLayer.TSRRepository.GetList(x => x.TSRStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRList = objBusinessLayer.TSRRepository.GetList(startingRecordNumber, pageSize, x => x.TSRStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);


                        }
                        else
                        {
                            //objBusinessLayer = new BusinessLayer(ls);
                            //objTSRList = objBusinessLayer.TSRRepository.GetList(x => ((x.Title.ToLower().Contains(strTitleOrClient)) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRList = objBusinessLayer.TSRRepository.GetList(startingRecordNumber, pageSize, x => ((x.Title.ToLower().Contains(strTitleOrClient)) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                        }
                    }
                    else
                    {
                        if (status != 0)
                        {
                            //objBusinessLayer = new BusinessLayer(ls);
                            //objTSRList = objBusinessLayer.TSRRepository.GetList(x => x.TSRStatusID == status, x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRList = objBusinessLayer.TSRRepository.GetList(startingRecordNumber, pageSize, x => x.TSRStatusID == status && x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRList = objBusinessLayer.TSRRepository.GetList(startingRecordNumber, pageSize, x => x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                        }
                    }

                    objBusinessLayer = new BusinessLayer(ls);
                    IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TeamLeadId == userID);
                    foreach (TSO objTSO in objTSOList)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        if (!objTSRList.Any(x => x.ID == objTSO.TSRId))
                        {
                            TSR objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == objTSO.TSRId, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                            objTSRList.Add(objTSR);
                        }

                    }

                    //if (strTitleOrClient != "none")
                    //{
                    //    if (status != 0)
                    //    {
                    //        objTSRList = objBusinessLayer.TSRRepository.GetList(startingRecordNumber, pageSize, x => x.TSRStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TSOes, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                    //    }
                    //    else
                    //    {
                    //        objTSRList = objBusinessLayer.TSRRepository.GetList(startingRecordNumber, pageSize, x => ((x.Title.ToLower().Contains(strTitleOrClient)) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TSOes, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                    //    }
                    //}
                    //else
                    //{
                    //    if (status != 0)
                    //    {
                    //        objTSRList = objBusinessLayer.TSRRepository.GetList(startingRecordNumber, pageSize, x => x.TSRStatusID == status, x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TSOes, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                    //    }
                    //    else
                    //    {
                    //        objTSRList = objBusinessLayer.TSRRepository.GetList(startingRecordNumber, pageSize, x => x.ID != 0, x => x.ID, false, out totalRecords, x => x.TSRStatus, x => x.TSOes, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                    //    }
                    //}
                }

                TSRDTO objTSRDTO = new TSRDTO();
                foreach (TSR objTSR in objTSRList)
                {


                    TSRStatusDTO objTSRStatusDTO = Conversions.ToDTO<TSRStatusDTO, TSRStatus>(objTSR.TSRStatus);
                    objTSRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSR);
                    objTSRDTO.TSRStatus = objTSRStatusDTO;

                    objTSRDTO.TestManager = Conversions.ToDTO<UserDTO, User>(objTSR.TestManager);
                    objTSRDTO.AccountManager = Conversions.ToDTO<UserDTO, User>(objTSR.AccountManager);
                    objTSRDTO.DeliveryManager = Conversions.ToDTO<UserDTO, User>(objTSR.DeliveryManager);

                    objTSRDTO.Engagement = Conversions.ToDTO<EngagementDTO, Engagement>(objTSR.Engagement);
                    objTSRDTO.OperationalRisk = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objTSR.OperationalRisk);
                    objTSRDTO.Vertical = Conversions.ToDTO<VerticalDTO, Vertical>(objTSR.Vertical);
                    objTSRDTO.Practice = Conversions.ToDTO<PracticeDTO, Practice>(objTSR.Practice);
                    objTSRDTO.SolutionCentre = Conversions.ToDTO<SolutionCentreDTO, SolutionCentre>(objTSR.SolutionCentre);
                    objTSRDTO.Client = Conversions.ToDTO<ClientDTO, Client>(objTSR.Client);
                    objTSRDTO.ClientRegion = Conversions.ToDTO<ClientRegionDTO, ClientRegion>(objTSR.ClientRegion);
                    objTSRDTO.PricingModel = Conversions.ToDTO<PricingModelDTO, PricingModel>(objTSR.PricingModel);
                    objTSRDTO.MarketOffering = Conversions.ToDTO<MarketOfferingDTO, MarketOffering>(objTSR.MarketOffering);

                    foreach (var item in objTSR.TSRReleventRepositories)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        item.RelevantRepository = objBusinessLayer.RelevantRepositoryRepository.GetByID(item.RelevantRepositoryId);
                        objTSR.TSTRelevantRepositoriesArr += item.RelevantRepository.Name + ",";
                    }

                    if (objTSR.TSTRelevantRepositoriesArr != null)
                    {
                        objTSRDTO.TSTRelevantRepositoriesArr = objTSR.TSTRelevantRepositoriesArr.Remove(objTSR.TSTRelevantRepositoriesArr.Length - 1);
                    }


                    foreach (var item in objTSR.TSRCoreServices)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        item.CoreService = objBusinessLayer.CoreServiceRepository.GetByID(item.CoreServiceId);
                        objTSR.TSRCoreServicesArr += item.CoreService.Name + ",";
                    }
                    objTSRDTO.TSRCoreServicesArr = objTSR.TSRCoreServicesArr.Remove(objTSR.TSRCoreServicesArr.Length - 1);

                    objBusinessLayer = new BusinessLayer(ls);
                    ProjectModel pm = objBusinessLayer.ProjectModelRepository.GetByID(objTSR.ProjectModelID);
                    if (pm != null)
                    {
                        objTSRDTO.ProjectModelName = pm.Name;
                    }

                    // objTSRDTO.TSRReleventRepositories = Conversions.ToDTO<TSRRelevantRepositoryDTO, RelevantRepository>(objTSR.TSRReleventRepositories);
                    // objTSRDTO.TSRCoreServices = Conversions.ToDTO<TSRCoreServiceDTO, TSRCoreService>(objTSR.TSRCoreServices);
                    double ActualEffort = 0;
                    //float PlannedEffort = 0;
                    objBusinessLayer = new BusinessLayer(ls);
                    IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == objTSR.ID, x => x.TSOServiceDeliveryChains, x => x.TSR);

                    if (objTSOList != null && objTSOList.Count > 0)
                    {

                        foreach (TSO objTSO in objTSOList)
                        {
                            //PlannedEffort = PlannedEffort + (float)objTSO.PlannedEffort;
                            if (objTSO.TSOServiceDeliveryChains != null && objTSO.TSOServiceDeliveryChains.Count > 0)
                            {
                                foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSO.TSOServiceDeliveryChains)
                                {
                                    //IBusinessLayer oldobjBusinessLayer = new BusinessLayer(ls);
                                    //TSOServiceDeliveryChainTask objTSOServiceDeliveryChainTask = oldobjBusinessLayer.TSOServiceDeliveryChainTaskRepository.GetList(x => x.TSOServiceDeliveryChainId == objTSOServiceDeliveryChain.ID).FirstOrDefault();

                                    //if (objTSOServiceDeliveryChainTask != null)
                                    //{
                                    //    ActualEffort = ActualEffort + (objTSOServiceDeliveryChainTask.ActualEffort ?? 0);
                                    //}
                                    objBusinessLayer = new BusinessLayer(ls);
                                    ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTSOServiceDeliveryChain.ServiceDeliveryChainId);

                                    if (null != objServiceDeliveryChain)
                                    {
                                        objTSOServiceDeliveryChain.ServiceDeliveryChain = objServiceDeliveryChain;
                                    }

                                    IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainTaskDTOList = this.GetLastTSOServiceDeliveryChainTaskList(objTSOServiceDeliveryChain.ID);
                                    foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainTaskDTO in objTSOServiceDeliveryChainTaskDTOList)
                                    {
                                        foreach (var item in objTSOServiceDeliveryChainTaskDTO.TSOServiceDeliveryChainActualTasks)
                                        {
                                            if (item != null)
                                            {
                                                ActualEffort = ActualEffort + (item.ActualEffort ?? 0);
                                            }
                                        }
                                    }
                                    //TSOServiceDeliveryChainTaskDTO objTSOServiceDeliveryChainTaskDTO = this.GetLastTSOServiceDeliveryChainTask(objTSOServiceDeliveryChain.ID);
                                    //if (objTSOServiceDeliveryChainTaskDTO != null)
                                    //{
                                    //    ActualEffort = ActualEffort + (objTSOServiceDeliveryChainTaskDTO.ActualEffort ?? 0);
                                    //}
                                }
                            }
                        }
                    }
                    if (objTSOList.Count > 0)
                    {
                        objTSRDTO.ActualEffort = ActualEffort;
                        //objTSRDTO.Plannedeffort = PlannedEffort;
                    }

                    objTSRDTOList.Add(objTSRDTO);
                }
                totalRecords = objTSRList.Count();

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
            return objTSRDTOList;
        }

        /// <summary>
        /// GetLastTSOServiceDeliveryChainTask
        /// </summary>
        /// <param name="tsoServiceDeliveryChainId">int</param>
        /// <returns>TSOServiceDeliveryChainTaskDTO</returns>
        private TSOServiceDeliveryChainDTO GetLastTSOServiceDeliveryChainTask(int tsoServiceDeliveryChainId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainTaskDTO = null;

            try
            {
                //int totalRecords = 0;
                //TSOServiceDeliveryChainTask objTSOServiceDeliveryChainTask = objBusinessLayer.TSOServiceDeliveryChainTaskRepository.GetList(0, 1, x => x.TSOServiceDeliveryChainId == tsoServiceDeliveryChainId, x => x.ID, true, out totalRecords).FirstOrDefault();

                IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainTaskList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetListWithOrder(x => x.ID == tsoServiceDeliveryChainId, x => x.CreatedOn, y => y.TSOServiceDeliveryChainActualTasks);

                if (null != objTSOServiceDeliveryChainTaskList)
                {
                    objTSOServiceDeliveryChainTaskDTO = new TSOServiceDeliveryChainDTO();

                    double ActualEffortCumulative = 0;
                    int ActualOutcomeCumulative = 0;
                    int ActualOutcomeTestStepsCumulative = 0;
                    double ActualProcessingTimeCumulative = 0;
                    int DefectRaisedCumulative = 0;
                    int DefectRejectedCumulative = 0;

                    foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChainTask in objTSOServiceDeliveryChainTaskList)
                    {
                        objTSOServiceDeliveryChainTaskDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChainTask);
                        foreach (var item in objTSOServiceDeliveryChainTaskDTO.TSOServiceDeliveryChainActualTasks)
                        {
                            ActualEffortCumulative += item.ActualEffort ?? 0;
                            ActualOutcomeCumulative += item.ActualOutcome;
                            ActualOutcomeTestStepsCumulative += item.ActualOutcomeTestSteps;
                            ActualProcessingTimeCumulative += item.ActualProcessingTime;
                            DefectRaisedCumulative += item.DefectRaised;
                            DefectRejectedCumulative += item.DefectRejected;
                        }

                    }

                    objTSOServiceDeliveryChainTaskDTO.ActualEffortCumulative = ActualEffortCumulative;
                    objTSOServiceDeliveryChainTaskDTO.ActualOutcomeCumulative = ActualOutcomeCumulative;
                    objTSOServiceDeliveryChainTaskDTO.ActualOutcomeTestStepsCumulative = ActualOutcomeTestStepsCumulative;
                    objTSOServiceDeliveryChainTaskDTO.ActualProcessingTimeCumulative = ActualProcessingTimeCumulative;
                    objTSOServiceDeliveryChainTaskDTO.DefectRaisedCumulative = DefectRaisedCumulative;
                    objTSOServiceDeliveryChainTaskDTO.DefectRejectedCumulative = DefectRejectedCumulative;
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

            return objTSOServiceDeliveryChainTaskDTO;
        }


        /// <summary>
        /// GetLastTSOServiceDeliveryChainTask
        /// </summary>
        /// <param name="tsoServiceDeliveryChainId">int</param>
        /// <returns>TSOServiceDeliveryChainTaskDTO</returns>
        private IList<TSOServiceDeliveryChainDTO> GetLastTSOServiceDeliveryChainTaskList(int tsoServiceDeliveryChainId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainTaskDTOList = new List<TSOServiceDeliveryChainDTO>();

            try
            {
                IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainTaskList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetListWithOrder(x => x.ID == tsoServiceDeliveryChainId, x => x.CreatedOn, y => y.TSOServiceDeliveryChainActualTasks);

                if (null != objTSOServiceDeliveryChainTaskList)
                {
                    foreach (TSOServiceDeliveryChain task in objTSOServiceDeliveryChainTaskList)
                    {
                        TSOServiceDeliveryChainDTO objTSOTaskDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(task);
                        objTSOServiceDeliveryChainTaskDTOList.Add(objTSOTaskDTO);
                    }

                    double ActualEffortCumulative = 0;
                    int ActualOutcomeCumulative = 0;
                    int ActualOutcomeTestStepsCumulative = 0;
                    double ActualProcessingTimeCumulative = 0;
                    int DefectRaisedCumulative = 0;
                    int DefectRejectedCumulative = 0;

                    TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainTaskDTO = new TSOServiceDeliveryChainDTO();
                    foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChainTask in objTSOServiceDeliveryChainTaskList)
                    {
                        objTSOServiceDeliveryChainTaskDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChainTask);
                        foreach (var item in objTSOServiceDeliveryChainTaskDTO.TSOServiceDeliveryChainActualTasks)
                        {
                            ActualEffortCumulative += item.ActualEffort ?? 0;
                            ActualOutcomeCumulative += item.ActualOutcome;
                            ActualOutcomeTestStepsCumulative += item.ActualOutcomeTestSteps;
                            ActualProcessingTimeCumulative += item.ActualProcessingTime;
                            DefectRaisedCumulative += item.DefectRaised;
                            DefectRejectedCumulative += item.DefectRejected;
                            item.ActualProcessingTime = Math.Round((Double)item.ActualProcessingTime, 2);
                        }
                        foreach (var item in objTSOServiceDeliveryChainTaskDTO.TSOServiceDeliveryChainPlannedTasks)
                        {
                            item.PlannedProcessingTime = Math.Round((Double)item.PlannedProcessingTime, 2);
                        }

                        objTSOServiceDeliveryChainTaskDTO.ActualEffortCumulative = ActualEffortCumulative;
                        objTSOServiceDeliveryChainTaskDTO.ActualOutcomeCumulative = ActualOutcomeCumulative;
                        objTSOServiceDeliveryChainTaskDTO.ActualOutcomeTestStepsCumulative = ActualOutcomeTestStepsCumulative;
                        objTSOServiceDeliveryChainTaskDTO.ActualProcessingTimeCumulative = ActualProcessingTimeCumulative;
                        objTSOServiceDeliveryChainTaskDTO.DefectRaisedCumulative = DefectRaisedCumulative;
                        objTSOServiceDeliveryChainTaskDTO.DefectRejectedCumulative = DefectRejectedCumulative;

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

            return objTSOServiceDeliveryChainTaskDTOList;
        }


        #endregion

        #region internal functions

        /// <summary>
        /// GetTSRIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetTSRIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objTSRList = null;
            try
            {
                objTSRList = objBusinessLayer.TSRRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Title, Other = x.Description });
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

            return objTSRList;
        }

        #endregion

        [HttpGet, Route("v1/TSR/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objTSRList = null;

            try
            {
                objTSRList = this.GetTSRIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw ex;
            }
            return Json(objTSRList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/TSR/GetById/{id}")]
        [ResponseType(typeof(TSR))]
        // [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager")]
        public JsonResult<TSRDTO> GetById(int id)
        {
            JsonResult<TSRDTO> result = null;
            TSRDTO objTSRDTO;

            try
            {
                objTSRDTO = this.GetTSRById(id);

                result = Json(objTSRDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// GetAllPaged
        /// </summary>
        /// <param name="pageSize">int</param>
        /// <param name="startingRecordNumber">int</param>
        /// <returns>JsonResult<DataCollection>></returns>
        [HttpGet, Route("v1/TSR/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<TSRDTO> objTSRDTOList = this.GetAllTSR(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;
            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objTSRDTOList)
                {
                    foreach (TSRDTO objTSR in objTSRDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objTSR);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(ex.Message);
                throw ex;
            }
            return Json(objDataCollection, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// GetTSRDump
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet, Route("v1/TSR/GetTSRDump")]
        [ResponseType(typeof(HttpResponseMessage))]
        //[SessionAuthorize(Roles = "Admin")]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public HttpResponseMessage GetTSRDump()
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            var tsrTable = objBusinessLayer.TSOServiceDeliveryChainRepository.ExecuteStoredProcedure("sp_GetTSRData");

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                if (null != tsrTable)
                {
                    StringBuilder sb = new StringBuilder();
                    //Set the Headings and Orders                    
                    sb.Append("TSRID").Append(",");
                    sb.Append("Title").Append(",");
                    sb.Append("Client").Append(",");
                    sb.Append("TSR Status").Append(",");
                    sb.Append("Related TSO").Append(",");
                    sb.Append("Engagement Model").Append(",");
                    sb.Append("Pricing Model").Append(",");
                    sb.Append("Project Model").Append(",");
                    sb.Append("Client Region").Append(",");
                    sb.Append("Account").Append(",");
                    sb.Append("ERP order number").Append(",");
                    sb.Append("Solution centre").Append(",");
                    sb.Append("Core Service").Append(",");
                    sb.Append("TMO Assigned").Append(",");
                    sb.Append("Estimated effort").Append(",");
                    sb.Append("Planned effort").Append(",");
                    sb.Append("Actual effort").Append(",");
                    sb.Append("Operational risk").Append(",");
                    sb.Append("Start date").Append(",");
                    sb.Append("Target Completion Date").Append(",");
                    sb.Append("Account Manager").Append(",");
                    sb.Append("Delivery manager").Append(",");
                    sb.Append("Test Manager").Append(",");
                    sb.Append("Vertical").Append(",");
                    sb.Append("Practice").Append(",");
                    sb.Append("Market Offering").Append(",");
                    sb.Append("Relevant repositories").Append(",");
                    sb.Append("Version").Append(",");
                    sb.Append("Modified").Append(",");
                    sb.Append("Modified By").Append(",");
                    sb.Append("Description").Append(",");
                    sb.Append("Attached Filename(s)").Append(",");
                    sb.Append("\n");

                    for (int tsrTableRow = 0; tsrTableRow < tsrTable.Rows.Count; tsrTableRow++)
                    {
                        // Append data ID
                        sb.Append(tsrTable.Rows[tsrTableRow]["TSR_ID"].ToString()).Append(",");
                        // Append Title
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Title"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Title"].ToString())).Append(",");

                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Client"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Client"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["TSR_Status"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["TSR_Status"].ToString())).Append(",");
                        if (tsrTable.Rows[tsrTableRow]["Related_TSO"] == null)
                            sb.Append(string.Empty).Append(",");
                        else
                        {
                            if (tsrTable.Rows[tsrTableRow]["Related_TSO"] != null)
                            {
                                bool isSingleNum = tsrTable.Rows[tsrTableRow]["Related_TSO"].ToString().IndexOf(", ") > 0;
                                if (!isSingleNum)
                                    sb.Append(string.Format("=\"{0}\"", tsrTable.Rows[tsrTableRow]["Related_TSO"].ToString()));
                                else
                                    sb.Append(string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Related_TSO"].ToString()));
                                sb.Append(",");
                            }
                            else
                                sb.Append(",");

                        }
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Engagement_Model"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Engagement_Model"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Pricing_Model"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Pricing_Model"].ToString())).Append(",");
                        sb.Append(tsrTable.Rows[tsrTableRow]["Project_Model"] == null ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Project_Model"].ToString())).Append(",");

                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["ClientRegion"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["ClientRegion"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Account"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Account"].ToString())).Append(",");

                        long n;
                        bool isNumeric = long.TryParse(tsrTable.Rows[tsrTableRow]["ERPordernumber"].ToString(), out n);
                        if (!isNumeric)
                            sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["ERPordernumber"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["ERPordernumber"].ToString())).Append(",");           //ERP order number
                        else
                            sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["ERPordernumber"].ToString()) ? string.Empty : string.Format("=\"{0}\"", tsrTable.Rows[tsrTableRow]["ERPordernumber"].ToString())).Append(",");           //ERP order number
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Solution_Center"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Solution_Center"].ToString())).Append(",");
                        if (tsrTable.Rows[tsrTableRow]["CoreServices"] == null)
                            sb.Append(string.Empty).Append(",");
                        else
                        {
                            if (tsrTable.Rows[tsrTableRow]["CoreServices"] != null)
                            {
                                sb.Append(string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["CoreServices"].ToString()));
                            }
                            sb.Append(",");
                        }
                        if (tsrTable.Rows[tsrTableRow]["TMO"] == null)
                            sb.Append(string.Empty).Append(",");
                        else
                        {
                            if (tsrTable.Rows[tsrTableRow]["TMO"] != null)
                            {
                                sb.Append(string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["TMO"].ToString()));
                            }
                            sb.Append(",");
                        }
                        sb.Append(tsrTable.Rows[tsrTableRow]["Estimatedeffort"].ToString()).Append(",");
                        sb.Append(tsrTable.Rows[tsrTableRow]["Plannedeffort"].ToString()).Append(",");
                        sb.Append(tsrTable.Rows[tsrTableRow]["ActualEffort"].ToString()).Append(",");
                        sb.Append(tsrTable.Rows[tsrTableRow]["RiskNo"].ToString()).Append(",");
                        sb.Append(tsrTable.Rows[tsrTableRow]["StartDate"].ToString()).Append(",");
                        sb.Append(tsrTable.Rows[tsrTableRow]["TargetCompletionDate"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["AccountManager"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["AccountManager"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["DeliveryManager"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["DeliveryManager"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["TestManager"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["TestManager"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Vertical"].ToString() ?? "") ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Vertical"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Practice"].ToString() ?? "") ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["Practice"].ToString())).Append(",");
                        string strMarketOffering = tsrTable.Rows[tsrTableRow]["MarketOffering"].ToString() == null ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["MarketOffering"].ToString());
                        //if (strMarketOffering.Contains("N.A."))
                        //    strMarketOffering = strMarketOffering.Replace("N.A.", string.Empty);
                        sb.Append(strMarketOffering).Append(",");
                        if (tsrTable.Rows[tsrTableRow]["ReleventRepositories"] == null)
                            sb.Append(string.Empty).Append(",");
                        else
                        {
                            if (tsrTable.Rows[tsrTableRow]["ReleventRepositories"] != null)
                            {
                                sb.Append(string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["ReleventRepositories"].ToString()));
                            }
                            sb.Append(",");
                        }
                        sb.Append(tsrTable.Rows[tsrTableRow]["Version"].ToString()).Append(",");
                        sb.Append(tsrTable.Rows[tsrTableRow]["Modified"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["ModifiedBy"].ToString()) ? string.Empty : string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["ModifiedBy"].ToString())).Append(",");
                        if (string.IsNullOrEmpty(tsrTable.Rows[tsrTableRow]["Description"].ToString()))
                            sb.Append(string.Empty);
                        else
                        {
                            StringBuilder sbDescription = new StringBuilder();
                            if (tsrTable.Rows[tsrTableRow]["Description"].ToString().IndexOfAny(new char[] { '"', ',' }) != -1)
                                sbDescription.AppendFormat("\"{0}\"", tsrTable.Rows[tsrTableRow]["Description"].ToString().Replace("\"", "\"\""));
                            else
                            {
                                sbDescription.Append(tsrTable.Rows[tsrTableRow]["Description"].ToString());
                                sb.Append(string.Format("\"{0}\"", sbDescription));
                            }
                        }
                        sb.Append(",");
                        if (tsrTable.Rows[tsrTableRow]["TSRFiles"] == null)
                        {
                            sb.Append(string.Empty);
                        }
                        else
                        {
                            sb.Append(string.Format("\"{0}\"", tsrTable.Rows[tsrTableRow]["TSRFiles"].ToString()));
                        }
                        sb.Append("\n");
                    }

                    result.Content = new StringContent(sb.ToString(), Encoding.UTF8);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = "TSRExport.csv";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(ex.Message);
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// CreateTSR
        /// </summary>
        /// <param name="tsrDTO">TSRDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/TSR/CreateTSR")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager,TMO,")]
        public JsonResult<string> CreateTSR([FromBody]TSRDTO tsrDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewTSR(tsrDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// CreateTSR
        /// </summary>
        /// <param name="tsrDTO">TSRDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpGet, Route("v1/TSR/GetTSRCount/{strTitleOrClient}/{status}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<string> GetTSRCount(string strTitleOrClient, int status)
        {
            JsonResult<string> result = Json(string.Empty);
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                IList<TSR> objTSRList;// = objBusinessLayer.TSRRepository.GetAll(x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                string userId = Request.Headers.GetValues("userid").FirstOrDefault();
                userId = userId.Replace("~", "=").Replace("!", "+");
                Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
                string User = objCryptorEngine.Decrypt(userId, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                User = usrInfo[usrInfo.Length - 2];
                int userID = Convert.ToInt32(User);
                objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);

                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRList = objBusinessLayer.TSRRepository.GetAll(x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                }
                else
                {
                    if (strTitleOrClient != "none")
                    {
                        if (status != 0)
                        {
                            //objBusinessLayer = new BusinessLayer(ls);
                            //objTSRList = objBusinessLayer.TSRRepository.GetList(x => x.TSRStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRList = objBusinessLayer.TSRRepository.GetList(x => x.TSRStatusID == status && (x.Title.ToLower().Contains(strTitleOrClient) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);


                        }
                        else
                        {
                            //objBusinessLayer = new BusinessLayer(ls);
                            //objTSRList = objBusinessLayer.TSRRepository.GetList(x => ((x.Title.ToLower().Contains(strTitleOrClient)) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)), x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRList = objBusinessLayer.TSRRepository.GetList(x => ((x.Title.ToLower().Contains(strTitleOrClient)) || x.Client.Name.ToLower().Contains(strTitleOrClient) || x.SolutionCentre.Name.ToLower().Contains(strTitleOrClient)) && x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                        }
                    }
                    else
                    {
                        if (status != 0)
                        {
                            //objBusinessLayer = new BusinessLayer(ls);
                            //objTSRList = objBusinessLayer.TSRRepository.GetList(x => x.TSRStatusID == status, x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);

                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRList = objBusinessLayer.TSRRepository.GetList(x => x.TSRStatusID == status && x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTSRList = objBusinessLayer.TSRRepository.GetList(x => x.AccountManagerId == userID || x.DeliveryManagerId == userID || x.TestManagerId == userID, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                        }
                    }

                    objBusinessLayer = new BusinessLayer(ls);
                    IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TeamLeadId == userID);
                    foreach (TSO objTSO in objTSOList)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        if (!objTSRList.Any(x => x.ID == objTSO.TSRId))
                        {
                            TSR objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == objTSO.TSRId, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                            objTSRList.Add(objTSR);
                        }

                    }
                }

                result = Json(objTSRList.Count.ToString(), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// UpdateTSR
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="tsrDTO">TSRDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/TSR/UpdateTSR/{ID}")]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager,TMO,")]
        public JsonResult<string> UpdateTSR(int ID, [FromBody]TSRDTO tsrDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyTSR(ID, tsrDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteTSR
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [HttpDelete, Route("v1/TSR/DeleteTSR/{ID}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin")]
        public JsonResult<string> DeleteTSR(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveTSR(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// TSRFileUpload
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>Files</returns>
        [HttpPost, Route("v1/TSR/TSRFileUpload")]
        //[ResponseType(typeof(string))]
        public string TSRFileUpload()//int id,
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            TSRDTO objTSRDTO = null;
            string returnMessage = string.Empty;
            try
            {
                TSR objTSR = null;
                // objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == id);


                if (objTSR != null)
                {
                    returnMessage = "Error: TSR title already exists.";
                }
                else
                {
                    TSRDTO objTSRDTOById = this.GetTSRById(Convert.ToInt32(System.Web.HttpContext.Current.Request.Params["id"]), false);

                    if (objTSRDTOById != null)
                    {
                        ls = new LoginSession();
                        ls.LoginName = objTSRDTOById.UpdatedBy;
                        var keys = System.Web.HttpContext.Current.Request.Files.AllKeys;
                        for (int file = 0; file < keys.Length; file++)
                        {
                            bool tobeAdded = true;
                            var files = System.Web.HttpContext.Current.Request.Files[keys[file]];
                            System.Web.HttpPostedFileBase filebase = new System.Web.HttpPostedFileWrapper(files);
                            var fileName = System.IO.Path.GetFileName(filebase.FileName);

                            IList<TSRFileUpload> tsrFileUploads = objBusinessLayer.TSRFileUploadRepository.GetList(x => x.TSRId == objTSRDTOById.ID);
                            foreach (TSRFileUpload singlefile in tsrFileUploads)
                            {
                                if (singlefile.Path == fileName)
                                {
                                    tobeAdded = false;
                                }
                            }

                            if (tobeAdded)
                            {
                                string ext = System.IO.Path.GetExtension(filebase.FileName);
                                Guid obj = Guid.NewGuid();
                                string FilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Attachments\\" + obj.ToString() + ext;

                                files.SaveAs(FilePath);

                                //System.IO.File.Copy(FilePath, files.ToString());

                                objTSR = Conversions.ToEntity<TSRDTO, TSR>(objTSRDTO);

                                objBusinessLayer = new BusinessLayer(ls);

                                TSRFileUpload tsrFileUpload = new TSRFileUpload
                                {
                                    Path = fileName,
                                    TSRId = objTSRDTOById.ID,
                                    GUID = obj.ToString()
                                };
                                objBusinessLayer.TSRFileUploadRepository.Add(tsrFileUpload);
                            }
                        }
                        returnMessage = "TSR created successfully.";
                    }
                    else
                    {
                        returnMessage = "TSR do not exists.";
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
                //objBusinessLayer.Dispose();
            }

            return returnMessage;
        }

        /// <summary>
        /// RemoveTSRFile
        /// </summary>
        /// <param name="guidString">int</param>
        [HttpPost, Route("v1/TSR/RemoveTSRFile/{guidString}")]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager,TMO")]
        public string RemoveTSRFile(string guidString)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                objBusinessLayer = new BusinessLayer(ls);
                Guid guidOutput;
                bool isValid = Guid.TryParse(guidString, out guidOutput);
                if (isValid)
                {
                    objBusinessLayer.TSRFileUploadRepository.Delete(x => x.GUID == guidString);
                }
                else
                {
                    objBusinessLayer.TSRFileUploadRepository.Delete(x => x.TSRId == Convert.ToInt32(guidString));
                }

                string[] files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Attachments\\");

                foreach (string file in files)
                {
                    if (file.Contains(guidString))
                    {
                        System.IO.File.Delete(file);
                    }
                }

                //now Delete TSR
                objBusinessLayer = new BusinessLayer(ls);
                returnMessage = "TSR deleted successfully.";
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
        /// SearchByTitle
        /// </summary>
        /// <param name="startingRecordNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [ResponseType(typeof(DataCollection))]
        [HttpGet, Route("v1/TSR/SearchByTitleOrClient/{startingRecordNumber}/{pageSize}/{Title}/{Status}")]
        public JsonResult<DataCollection> SearchByTitleOrClient(int startingRecordNumber, int pageSize, string Title, int Status)
        {
            int totalRecords = pageSize;
            DataCollection objDataCollection;

            Title = GetSpecialChar(Title);
            IList<TSRDTO> objTSRList = this.GetTSRByTitleOrClient(startingRecordNumber, pageSize, Title.Trim(), Status, out totalRecords);
            try
            {
                if (objTSRList.Count != 0)
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, Message = "No Records Found", EntitySummary = new List<IBaseObject>() };

                    foreach (TSRDTO objTSRType in objTSRList)
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
        [HttpGet, Route("v1/TSR/GetTSRByAdvanceSearch/{pageNumber}/{pageSize}/{strcenter}/{strclient}/{strid}/{strtitle}/{strstatus}/{strpractice}")]
        public JsonResult<DataCollection> GetTSRByAdvanceSearch(int pageNumber, int pageSize, string strcenter, string strclient, int strid, string strtitle, int strstatus, string strpractice)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            DataCollection objDataCollection;
            IList<TSR> objTSRList = null;
            int totalRecords = pageSize; ;
            IList<TSRDTO> objTSRDTOList = new List<TSRDTO>();
            try
            {
                strclient = GetSpecialChar(strclient);
                strtitle = GetSpecialChar(strtitle);
                strcenter = GetSpecialChar(strcenter);
                strpractice = GetSpecialChar(strpractice);

                string sql = "Select DISTINCT tsr.*  from TSR as tsr inner join Practice as p on tsr.PracticeId = p.Id " +
                    "inner join SolutionCentre as sc on tsr.SolutionCentreId = sc.Id left join TSO as tso on tsr.ID = tso.TSRId " +
                    "inner join Client as cli on tsr.ClientId = cli.Id where";

                if (strid > 0)
                {
                    sql = sql + " tsr.ID = " + strid;
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
                        sql = sql + " and tsr.Title like '%" + strtitle + "%'";
                    }
                    else
                    {
                        sql = sql + " tsr.Title like '%" + strtitle + "%'";
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
                        sql = sql + " and tsr.TSRStatusID = " + strstatus;
                    }
                    else
                    {
                        sql = sql + " tsr.TSRStatusID = " + strstatus;
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
                        sql = sql + " and (tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + " or tsr.AccountManagerId = " + userID + " or tso.TeamLeadId = " + userID + ")";
                    }
                    else
                    {
                        sql = sql + " (tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + " or tsr.AccountManagerId = " + userID + " or tso.TeamLeadId = " + userID + ")";
                    }
                }

                int rows = pageNumber - 1;
                sql = sql + " ORDER BY tsr.id OFFSET " + (rows < 0 ? 0 : rows) + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

                objTSRList = objBusinessLayer.TSRRepository.GetWithRawSql(sql);
                //if (!isAdmin)
                //{
                //    objBusinessLayer = new BusinessLayer(ls);
                //    IList<TSO> objTSOListTsr = objBusinessLayer.TSORepository.GetList(x => x.TeamLeadId == userID);
                //    foreach (TSO objTSO in objTSOListTsr)
                //    {
                //        objBusinessLayer = new BusinessLayer(ls);
                //        if (!objTSRList.Any(x => x.ID == objTSO.TSRId))
                //        {
                //            TSR objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == objTSO.TSRId, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                //            objTSRList.Add(objTSR);
                //        }

                //    }
                //}

                TSRDTO objTSRDTO = new TSRDTO();
                foreach (TSR objTSR in objTSRList)
                {


                    //TSRStatusDTO objTSRStatusDTO = Conversions.ToDTO<TSRStatusDTO, TSRStatus>(objTSR.TSRStatus);
                    //objTSRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSR);
                    //objTSRDTO.TSRStatus = objTSRStatusDTO;
                    //objTSRDTO.TestManager = Conversions.ToDTO<UserDTO, User>(objBusinessLayer.UserRepository.GetSingle(x => x.ID == objTSR.TestManagerId));
                    //objTSRDTO.AccountManager = Conversions.ToDTO<UserDTO, User>(objTSR.AccountManager);
                    //objTSRDTO.DeliveryManager = Conversions.ToDTO<UserDTO, User>(objTSR.DeliveryManager);
                    //objTSRDTO.Engagement = Conversions.ToDTO<EngagementDTO, Engagement>(objTSR.Engagement);
                    //objTSRDTO.Vertical = Conversions.ToDTO<EngagementDTO, Vertical>(objTSR.Vertical);

                    objTSRDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSR);

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.TestManager = Conversions.ToDTO<UserDTO, User>(objBusinessLayer.UserRepository.GetSingle(x => x.ID == objTSR.TestManagerId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.AccountManager = Conversions.ToDTO<UserDTO, User>(objBusinessLayer.UserRepository.GetSingle(x => x.ID == objTSR.AccountManagerId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.DeliveryManager = Conversions.ToDTO<UserDTO, User>(objBusinessLayer.UserRepository.GetSingle(x => x.ID == objTSR.DeliveryManagerId));

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.Engagement = Conversions.ToDTO<EngagementDTO, Engagement>(objBusinessLayer.EngagementRepository.GetSingle(x => x.ID == objTSR.EngagementId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.Vertical = Conversions.ToDTO<VerticalDTO, Vertical>(objBusinessLayer.VerticalRepository.GetSingle(x => x.ID == objTSR.VerticalId));

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.TSRStatus = Conversions.ToDTO<TSRStatusDTO, TSRStatus>(objBusinessLayer.TSRStatusRepository.GetSingle(x => x.ID == objTSR.TSRStatusID));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.OperationalRisk = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objBusinessLayer.OperationalRiskRepository.GetSingle(x => x.ID == objTSR.OperationalRiskId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.Practice = Conversions.ToDTO<PracticeDTO, Practice>(objBusinessLayer.PracticeRepository.GetSingle(x => x.ID == objTSR.PracticeId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.SolutionCentre = Conversions.ToDTO<SolutionCentreDTO, SolutionCentre>(objBusinessLayer.SolutionCentreRepository.GetSingle(x => x.ID == objTSR.SolutionCentreId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.Client = Conversions.ToDTO<ClientDTO, Client>(objBusinessLayer.ClientRepository.GetSingle(x => x.ID == objTSR.ClientId));

                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.ClientRegion = Conversions.ToDTO<ClientRegionDTO, ClientRegion>(objBusinessLayer.ClientRegionRepository.GetSingle(x => x.ID == objTSR.ClientRegionId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.PricingModel = Conversions.ToDTO<PricingModelDTO, PricingModel>(objBusinessLayer.PricingModelRepository.GetSingle(x => x.ID == objTSR.PricingModelId));
                    objBusinessLayer = new BusinessLayer(ls);
                    objTSRDTO.MarketOffering = Conversions.ToDTO<MarketOfferingDTO, MarketOffering>(objBusinessLayer.MarketOfferingRepository.GetSingle(x => x.ID == objTSR.MarketOfferingId));
                    //objTSRDTO.ClientRegion = Conversions.ToDTO<ClientRegionDTO, ClientRegion>(objTSR.ClientRegion);
                    //objTSRDTO.PricingModel = Conversions.ToDTO<PricingModelDTO, PricingModel>(objTSR.PricingModel);
                    //objTSRDTO.MarketOffering = Conversions.ToDTO<MarketOfferingDTO, MarketOffering>(objTSR.MarketOffering);

                    objBusinessLayer = new BusinessLayer(ls);
                    List<TSRRelevantRepository> tsrRele = objBusinessLayer.TSRRelevantRepositoriesRepository.GetList(x => x.TSRId == objTSR.ID).ToList();
                    foreach (TSRRelevantRepository item in tsrRele)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        item.RelevantRepository = objBusinessLayer.RelevantRepositoryRepository.GetByID(item.RelevantRepositoryId);
                        objTSR.TSTRelevantRepositoriesArr += " " + item.RelevantRepository.Name + ",";
                    }

                    if (objTSR.TSTRelevantRepositoriesArr != null)
                    {
                        objTSRDTO.TSTRelevantRepositoriesArr = objTSR.TSTRelevantRepositoriesArr.Remove(objTSR.TSTRelevantRepositoriesArr.Length - 1).Trim();
                    }

                    objBusinessLayer = new BusinessLayer(ls);
                    List<TSRCoreService> tsrCore = objBusinessLayer.TSRCoreServicesRepository.GetList(x => x.TSRId == objTSR.ID).ToList();
                    foreach (TSRCoreService item in tsrCore)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        item.CoreService = objBusinessLayer.CoreServiceRepository.GetByID(item.CoreServiceId);
                        objTSR.TSRCoreServicesArr += " " + item.CoreService.Name + ",";
                    }

                    if (objTSR.TSRCoreServicesArr != null)
                    {
                        objTSRDTO.TSRCoreServicesArr = objTSR.TSRCoreServicesArr.Remove(objTSR.TSRCoreServicesArr.Length - 1).Trim();
                    }


                    objBusinessLayer = new BusinessLayer(ls);
                    ProjectModel pm = objBusinessLayer.ProjectModelRepository.GetByID(objTSR.ProjectModelID);
                    if (pm != null)
                    {
                        objTSRDTO.ProjectModelName = pm.Name;
                    }

                    // objTSRDTO.TSRReleventRepositories = Conversions.ToDTO<TSRRelevantRepositoryDTO, RelevantRepository>(objTSR.TSRReleventRepositories);
                    // objTSRDTO.TSRCoreServices = Conversions.ToDTO<TSRCoreServiceDTO, TSRCoreService>(objTSR.TSRCoreServices);
                    double ActualEffort = 0;
                    //float PlannedEffort = 0;
                    objBusinessLayer = new BusinessLayer(ls);
                    IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TSRId == objTSR.ID, x => x.TSOServiceDeliveryChains, x => x.TSR);

                    if (objTSOList != null && objTSOList.Count > 0)
                    {

                        foreach (TSO objTSO in objTSOList)
                        {
                            //PlannedEffort = PlannedEffort + (float)objTSO.PlannedEffort;
                            if (objTSO.TSOServiceDeliveryChains != null && objTSO.TSOServiceDeliveryChains.Count > 0)
                            {
                                foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTSO.TSOServiceDeliveryChains)
                                {
                                    //IBusinessLayer oldobjBusinessLayer = new BusinessLayer(ls);
                                    //TSOServiceDeliveryChainTask objTSOServiceDeliveryChainTask = oldobjBusinessLayer.TSOServiceDeliveryChainTaskRepository.GetList(x => x.TSOServiceDeliveryChainId == objTSOServiceDeliveryChain.ID).FirstOrDefault();

                                    //if (objTSOServiceDeliveryChainTask != null)
                                    //{
                                    //    ActualEffort = ActualEffort + (objTSOServiceDeliveryChainTask.ActualEffort ?? 0);
                                    //}
                                    objBusinessLayer = new BusinessLayer(ls);
                                    ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTSOServiceDeliveryChain.ServiceDeliveryChainId);

                                    if (null != objServiceDeliveryChain)
                                    {
                                        objTSOServiceDeliveryChain.ServiceDeliveryChain = objServiceDeliveryChain;
                                    }

                                    IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainTaskDTOList = this.GetLastTSOServiceDeliveryChainTaskList(objTSOServiceDeliveryChain.ID);
                                    foreach (TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainTaskDTO in objTSOServiceDeliveryChainTaskDTOList)
                                    {
                                        foreach (var item in objTSOServiceDeliveryChainTaskDTO.TSOServiceDeliveryChainActualTasks)
                                        {
                                            if (item != null)
                                            {
                                                ActualEffort = ActualEffort + (item.ActualEffort ?? 0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (objTSOList.Count > 0)
                    {
                        objTSRDTO.ActualEffort = ActualEffort;
                        //objTSRDTO.Plannedeffort = PlannedEffort;
                    }

                    objTSRDTOList.Add(objTSRDTO);
                }
                totalRecords = objTSRList.Count();
                if (objTSRList.Count != 0)
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, Message = "No Records Found", EntitySummary = new List<IBaseObject>() };

                    foreach (TSRDTO objTSRType in objTSRDTOList)
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
        [HttpGet, Route("v1/TSR/GetTSRByAdvanceSearchCount/{strcenter}/{strclient}/{strid}/{strtitle}/{strstatus}/{strpractice}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<string> GetTSRByAdvanceSearchCount(string strcenter, string strclient, int strid, string strtitle, int strstatus, string strpractice)
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

                IList<TSR> objTSRList;// = objBusinessLayer.TSRRepository.GetAll(x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                string sql = "Select DISTINCT tsr.*  from TSR as tsr inner join Practice as p on tsr.PracticeId = p.Id " +
                    "inner join SolutionCentre as sc on tsr.SolutionCentreId = sc.Id left join TSO as tso on tsr.ID = tso.TSRId " +
                    "inner join Client as cli on tsr.ClientId = cli.Id where";

                if (strid > 0)
                {
                    sql = sql + " tsr.ID = " + strid;
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
                        sql = sql + " and tsr.Title like '%" + strtitle + "%'";
                    }
                    else
                    {
                        sql = sql + " tsr.Title like '%" + strtitle + "%'";
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
                        sql = sql + " and tsr.TSRStatusID = " + strstatus;
                    }
                    else
                    {
                        sql = sql + " tsr.TSRStatusID = " + strstatus;
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
                        sql = sql + " and tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + "  or tsr.AccountManagerId = " + userID + " or tso.TeamLeadId = " + userID + ")";
                    }
                    else
                    {
                        sql = sql + " tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + "  or tsr.AccountManagerId = " + userID + " or tso.TeamLeadId = " + userID + ")";
                    }
                }
                objTSRList = objBusinessLayer.TSRRepository.GetWithRawSql(sql);
                //if (!isAdmin)
                //{
                //    objBusinessLayer = new BusinessLayer(ls);
                //    IList<TSO> objTSOList = objBusinessLayer.TSORepository.GetList(x => x.TeamLeadId == userID);
                //    foreach (TSO objTSO in objTSOList)
                //    {
                //        objBusinessLayer = new BusinessLayer(ls);
                //        if (!objTSRList.Any(x => x.ID == objTSO.TSRId))
                //        {
                //            TSR objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.ID == objTSO.TSRId, x => x.TSRStatus, x => x.TestManager, x => x.Client, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.SolutionCentre, x => x.Practice, x => x.PricingModel, x => x.ClientRegion, x => x.TSRReleventRepositories, x => x.TSRCoreServices);
                //            objTSRList.Add(objTSR);
                //        }

                //    }
                //}

                result = Json(objTSRList.Count.ToString(), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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