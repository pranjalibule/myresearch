﻿/******************************************************************************
 *                          © 2017 SQS India                                  *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * TRB  05May2018   Created the class
 * 
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
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    /// <summary>
    /// ServiceDeliveryChainTask
    /// </summary>
    [RoutePrefix("api")]
    public class ServiceDeliveryChainTaskController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// Get All Tasks using tsoId
        /// </summary>
        /// <param name="tsoId"></param>
        /// <param name="startingRecordNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        private IList<TSOServiceDeliveryChainDTO> GetAllTSOServiceDeliveryChain(int tsoId, int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSOServiceDeliveryChainDTO> objTaskSDTOList = null;
            totalRecords = 0;
            try
            {

                IList<TSOServiceDeliveryChain> objTask = null;
                objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.TSOId == tsoId,
                    x => x.ServiceDeliveryChain,
                    x => x.TSOServiceDeliveryChainActualTasks,
                    x => x.TSOServiceDeliveryChainPlannedTasks,
                    x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                    x => x.TSO.TSR.SolutionCentre

                    );




                objTaskSDTOList = new List<TSOServiceDeliveryChainDTO>();
                objBusinessLayer = new BusinessLayer(ls);
                foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTask)
                {
                    TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainTaskActual = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.OrderBy(x => x.WeekNumber).ToList().LastOrDefault();
                    TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainTaskPlanned = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.OrderBy(x => x.WeekNumber).ToList().LastOrDefault();

                    if (null != objTSOServiceDeliveryChainTaskActual && null != objTSOServiceDeliveryChainTaskPlanned)
                    {
                        var objTSOSDeliveryChainActualDTO = Conversions.ToDTO<TSOServiceDeliveryChainTaskActualDTO, TSOServiceDeliveryChainTaskActual>(objTSOServiceDeliveryChainTaskActual);
                        var objTSOSDeliveryChainPlannedDTO = Conversions.ToDTO<TSOServiceDeliveryChainTaskPlannedDTO, TSOServiceDeliveryChainTaskPlanned>(objTSOServiceDeliveryChainTaskPlanned);
                        var objTSOSDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainActualTasks.Clear();
                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks.Clear();

                        objTSOSDeliveryChainDTO.TaskStatus = GetTaskStatus(objTSOSDeliveryChainDTO.TaskStatusId);

                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainActualTasks.Add(objTSOSDeliveryChainActualDTO);
                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks.Add(objTSOSDeliveryChainPlannedDTO);
                        objTaskSDTOList.Add(objTSOSDeliveryChainDTO);
                    }
                    else
                    {
                        var objTSOSDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
                        objTSOSDeliveryChainDTO.TaskStatus = "not started";
                        objTaskSDTOList.Add(objTSOSDeliveryChainDTO);
                    }
                }
                totalRecords = objTaskSDTOList.Count;

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

            return objTaskSDTOList;
        }


        /// <summary>
        /// Get All Tasks
        /// </summary>
        /// <param name="startingRecordNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        private IList<TSOServiceDeliveryChainDTO> GetAllUserTASKS(int startingRecordNumber, int pageSize, string userId, out int totalRecords)
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            totalRecords = 0;
            IList<TSOServiceDeliveryChainDTO> objTaskSDTOList = null;

            try
            {
                IList<TSOServiceDeliveryChain> objTask = null;
                int userID = Convert.ToInt32(userId);
                objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);
                //Note - Currently we are showing all record in TSO dashboard as per discussion
                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords,
                     x => x.ServiceDeliveryChain,
                    x => x.TSOServiceDeliveryChainActualTasks,
                    x => x.TSOServiceDeliveryChainPlannedTasks,
                    x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                    x => x.TSO.TSR.SolutionCentre

                    );
                }
                else
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords,
                    x => x.TSO.TSR.AccountManagerId == userID || x.TSO.TSR.DeliveryManagerId == userID || x.TSO.TSR.TestManagerId == userID || x.TSO.TeamLeadId == userID,
                    x => x.ServiceDeliveryChain,
                    x => x.TSOServiceDeliveryChainActualTasks,
                    x => x.TSOServiceDeliveryChainPlannedTasks,
                    x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                    x => x.TSO.TSR.SolutionCentre
                    );


                    objBusinessLayer = new BusinessLayer(ls);
                    List<TSRTMOUser> tmoUsers = objBusinessLayer.TSRTMOUserRepository.GetList(x => x.UserId == userID, x => x.TSR, x => x.TSR.TSRStatus, x => x.TSR.TestManager, x => x.TSR.Client, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.Engagement, x => x.TSR.OperationalRisk, x => x.TSR.Vertical, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.PricingModel, x => x.TSR.ClientRegion, x => x.TSR.TSRReleventRepositories, x => x.TSR.TSRCoreServices, x => x.TSR.TSRTMOUsers).ToList();
                    foreach (var item in tmoUsers)
                    {
                        if (objTask.Count < pageSize)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            if (!objTask.Any(x => x.TSO.TSRId == item.TSRId))
                            {
                                IList<TSOServiceDeliveryChain> objSTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSO.TSRId == item.TSRId,
                                x => x.ServiceDeliveryChain,
                                x => x.TSOServiceDeliveryChainActualTasks,
                                x => x.TSOServiceDeliveryChainPlannedTasks,
                                x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                                x => x.TSO.TSR.SolutionCentre
                                ).ToList();

                                foreach (TSOServiceDeliveryChain objT in objSTask)
                                {
                                    objTask.Add(objT);
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                objTaskSDTOList = new List<TSOServiceDeliveryChainDTO>();

                foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTask)
                {

                    TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainTaskActual = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.OrderBy(x => x.WeekNumber).ToList().LastOrDefault();
                    TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainTaskPlanned = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.OrderBy(x => x.WeekNumber).ToList().LastOrDefault();

                    if (null != objTSOServiceDeliveryChainTaskActual && null != objTSOServiceDeliveryChainTaskPlanned)
                    {
                        var objTSOSDeliveryChainActualDTO = Conversions.ToDTO<TSOServiceDeliveryChainTaskActualDTO, TSOServiceDeliveryChainTaskActual>(objTSOServiceDeliveryChainTaskActual);
                        var objTSOSDeliveryChainPlannedDTO = Conversions.ToDTO<TSOServiceDeliveryChainTaskPlannedDTO, TSOServiceDeliveryChainTaskPlanned>(objTSOServiceDeliveryChainTaskPlanned);
                        var objTSOSDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainActualTasks.Clear();
                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks.Clear();

                        objTSOSDeliveryChainDTO.TaskStatus = GetTaskStatus(objTSOSDeliveryChainDTO.TaskStatusId);

                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainActualTasks.Add(objTSOSDeliveryChainActualDTO);
                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks.Add(objTSOSDeliveryChainPlannedDTO);

                        objBusinessLayer = new BusinessLayer(ls);
                        objTSOSDeliveryChainDTO.TSO = Conversions.ToDTO<TSODTO, TSO>(objBusinessLayer.TSORepository.GetSingle(x => x.ID == objTSOSDeliveryChainDTO.TSOId,
                            x => x.TSR, x => x.TSR.Client, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TeamLead, x => x.TSOStatus,
                            x => x.TSOServiceDeliveryChains, x => x.TSOProductivityInputs, x => x.TSOProductivityOutcomes, x => x.OperationalRisk,
                            x => x.CoreService, x => x.RelevantRepository));
                        objTaskSDTOList.Add(objTSOSDeliveryChainDTO);
                    }
                    else
                    {
                        var objTSOSDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
                        objTSOSDeliveryChainDTO.TaskStatus = "not started";
                        objTaskSDTOList.Add(objTSOSDeliveryChainDTO);
                    }
                }
                totalRecords = objTaskSDTOList.Count;

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


            return objTaskSDTOList;
        }


        /// <summary>
        /// GetTSOTASKById
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="loadEntities">bool</param>
        /// <returns>Tasks</returns>
        private TSOServiceDeliveryChainDTO GetTSOTASKById(int id, bool loadEntities = true)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            TSOServiceDeliveryChainDTO objTaskDTOList = null;
            try
            {
                TSOServiceDeliveryChain objTask = null;

                if (loadEntities)
                {
                    objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == id,
                        x => x.ServiceDeliveryChain,                                               
                        x => x.TSO,
                        x => x.TSO.TSR,
                        x => x.TSO.TeamLead,
                        x => x.TSO.TSR.Client,
                        x => x.ServiceDeliveryChainMethod
                        );
                }             
                var objTSOServiceDeliveryChainTaskDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTask);

               objTaskDTOList = objTSOServiceDeliveryChainTaskDTO;          
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

            return objTaskDTOList;
        }


        #endregion


        /// <summary>
        /// GetAllUserPaged
        /// </summary>
        /// <param name="pageSize">int</param>
        /// <param name="startingRecordNumber">int</param>
        /// <returns>JsonResult<DataCollection>></returns>
        [HttpGet, Route("v1/ServiceDeliveryChainTask/GetAllUserPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<DataCollection> GetAllUserPaged(int startingRecordNumber, int pageSize)
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


                IList<TSOServiceDeliveryChainDTO> objTSOServiceDeliveryChainDTOList = this.GetAllUserTASKS(startingRecordNumber, pageSize, User, out totalRecords);

                DataCollection objDataCollection;

                try
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                    if (null != objTSOServiceDeliveryChainDTOList)
                    {
                        foreach (TSOServiceDeliveryChainDTO objTASK in objTSOServiceDeliveryChainDTOList)
                        {
                            objDataCollection.EntitySummary.Add(objTASK);
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
        /// GetAllPaged
        /// </summary>
        /// <param name="pageSize">int</param>
        /// <param name="startingRecordNumber">int</param>
        /// <returns>JsonResult<DataCollection>></returns>
        [HttpGet, Route("v1/ServiceDeliveryChainTask/GetAllPaged/{tsoId}/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<DataCollection> GetAllPaged(int tsoId, int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            //IList<TSOServiceDeliveryChainTaskDTO> objTASKDTOList = this.GetAllTask(tsoId, startingRecordNumber, pageSize, out totalRecords);
            IList<TSOServiceDeliveryChainDTO> objTASK1DTOList = this.GetAllTSOServiceDeliveryChain(tsoId, startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objTASK1DTOList)
                {
                    foreach (TSOServiceDeliveryChainDTO objTASK in objTASK1DTOList)
                    {
                        objDataCollection.EntitySummary.Add(objTASK);
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
        /// GetTASKCount
        /// </summary>
        /// <param name="strTitleOrClient"></param>
        /// <param name="status"></param>
        /// <param name="tsoId"></param>
        /// <returns></returns>
        [HttpGet, Route("v1/ServiceDeliveryChainTask/GetTASKCount/{strTitleOrClient}/{status}/{tsoId}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<string> GetTASKCount(string strTitleOrClient, int status, int tsoId = 0)
        {
            JsonResult<string> result = Json(string.Empty);
            LoginSession ls = new LoginSession();
            IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainTaskList = null;
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                IList<TSOServiceDeliveryChain> objTask = null;

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

                    if (tsoId > 0)
                    {

                        objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSOId == tsoId,
                            x => x.ServiceDeliveryChain, x => x.TSOServiceDeliveryChainActualTasks, x => x.TSOServiceDeliveryChainPlannedTasks,
                            x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR, x => x.TSO.TSR.SolutionCentre);


                        if (objTask != null)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTask)
                            {
                                var actualTask = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.OrderBy(x => x.WeekNumber).LastOrDefault();
                                var plannedTask = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.OrderBy(x => x.WeekNumber + 1).LastOrDefault();
                                objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.Clear();
                                objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.Clear();
                                objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.Add(actualTask);
                                objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.Add(plannedTask);
                            }
                        }


                    }
                    else
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);
                        //Note - Currently we are showing all record in TSO dashboard as per discussion
                        if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetAll(
                                         x => x.ServiceDeliveryChain, x => x.TSOServiceDeliveryChainActualTasks, x => x.TSOServiceDeliveryChainPlannedTasks, x => x.TSO,
                                         x => x.TSO.TSR.Client, x => x.TSO.TSR, x => x.TSO.TSR.SolutionCentre);

                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSO.TSR.AccountManagerId == userID || x.TSO.TSR.DeliveryManagerId == userID || x.TSO.TSR.TestManagerId == userID || x.TSO.TeamLeadId == userID,
                                                                     x => x.ServiceDeliveryChain,
                                                                     x => x.TSOServiceDeliveryChainActualTasks,
                                                                     x => x.TSOServiceDeliveryChainPlannedTasks,
                                                                     x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                                                                     x => x.TSO.TSR.SolutionCentre);
                        }
                        if (objTask != null)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            //foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTask)
                            //{
                            //    objTSOServiceDeliveryChainTaskList = objTSOServiceDeliveryChain.TSOServiceDeliveryChainTasks.ToList();
                            //}
                            objTask.ToList();
                        }

                    }
                    result = Json(objTask.Count().ToString(), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
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
        [HttpGet, Route("v1/ServiceDeliveryChainTask/GetById/{id}")]
        [ResponseType(typeof(TSOServiceDeliveryChainDTO))]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager,Team Lead, Guest")]
        public JsonResult<TSOServiceDeliveryChainDTO> GetById(int id)
        {

            JsonResult<TSOServiceDeliveryChainDTO> result = null;
            TSOServiceDeliveryChainDTO objTSOTasksDTO;
            try
            {
                objTSOTasksDTO = this.GetTSOTASKById(id);

                result = Json(objTSOTasksDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return result;
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
        [HttpGet, Route("v1/ServiceDeliveryChainTask/GetTASKByAdvanceSearch/{pageNumber}/{pageSize}/{tsoId}/{strcenter}/{strclient}/{strid}/{strtitle}/{strstatus}/{strpractice}")]
        public JsonResult<DataCollection> GetTASKByAdvanceSearch(int pageNumber, int pageSize, int tsoId, string strcenter, string strclient, double strid, string strtitle, int strstatus, string strpractice)
        {
            LoginSession ls = new LoginSession();
            JsonResult<DataCollection> objReturn = null;
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            DataCollection objDataCollection;
            IList<TSOServiceDeliveryChain> objTaskList = null;
            int totalRecords = pageSize; ;
            IList<TSOServiceDeliveryChainDTO> objChainTaskDTOList = new List<TSOServiceDeliveryChainDTO>();
            IList<TSOServiceDeliveryChainDTO> objTaskDTOList = new List<TSOServiceDeliveryChainDTO>();

            strclient = GetSpecialChar(strclient);
            strtitle = GetSpecialChar(strtitle);
            strcenter = GetSpecialChar(strcenter);
            strpractice = GetSpecialChar(strpractice);

            try
            {
                string sql = "Select chain.*, task.*, tso.*, sc.*, tsr.* FROM (SELECT TSOServiceDeliveryChainTaskId, MAX(WeekNumber) AS WeekNumber FROM TSOServiceDeliveryChainTaskActual " +
                             "GROUP BY TSOServiceDeliveryChainTaskId) AS m " +
                             "INNER JOIN TSOServiceDeliveryChainTaskActual AS task ON task.TSOServiceDeliveryChainTaskId = m.TSOServiceDeliveryChainTaskId AND task.WeekNumber = m.WeekNumber " +
                             "INNER JOIN TSOServiceDeliveryChain as chain  on chain.ID = m.TSOServiceDeliveryChainTaskId inner join TSO as tso on chain.TSOId = tso.id " +
                             "inner join TSR as tsr on tso.TSRId = tsr.id inner join ServiceDeliveryChain as serv on chain.ServiceDeliveryChainId = serv.id " +
                             "inner join SolutionCentre as sc on tsr.SolutionCentreId = sc.Id inner join Client as cli on tsr.ClientId = cli.Id where";


                if (strid > 0)
                {
                    sql = sql + " chain.ID = " + strid;
                }

                if (tsoId > 0)
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.Id = " + tsoId;
                    }
                    else
                    {
                        sql = sql + " tso.Id = " + tsoId;
                    }
                }

                if (strpractice != "none")
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.Title like '%" + strpractice + "%'";
                    }
                    else
                    {
                        sql = sql + " tso.Title like '%" + strpractice + "%'";
                    }
                }

                if (strclient != "none")
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
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
                    strtitle = strtitle.Replace("(", "");
                    strtitle = strtitle.Replace(")", "");
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and serv.Description like '%" + strtitle + "%'";
                    }
                    else
                    {
                        sql = sql + " serv.Description like '%" + strtitle + "%'";
                    }
                }

                if (strcenter != "none")
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and sc.Name like '%" + strcenter + "%'";
                    }
                    else
                    {
                        sql = sql + " sc.Name like '%" + strcenter + "%'";
                    }
                }

                if (strstatus > -1)
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and chain.TaskStatusId = " + strstatus;
                    }
                    else
                    {
                        sql = sql + " chain.TaskStatusId = " + strstatus;
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
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and (tso.TeamLeadId = " + userID + " or tsr.AccountManagerId = " + userID + " or tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + ")";
                    }
                    else
                    {
                        sql = sql + " (tso.TeamLeadId = " + userID + " or tsr.AccountManagerId = " + userID + " or tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + ")";
                    }
                }

                int rows = pageNumber - 1;
                sql = sql + " ORDER BY chain.id OFFSET " + (rows < 0 ? 0 : rows) + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";


                //if (strstatus > 0 || strid > 0)
                //{
                objTaskList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetWithRawSql(sql);
                foreach (TSOServiceDeliveryChain objTask in objTaskList)
                {
                    //objBusinessLayer = new BusinessLayer(ls);
                    //objTask.TSOServiceDeliveryChain = objBusinessLayer.TSOServiceDeliveryChainRepository.GetByID(objTask.TSOServiceDeliveryChainId);

                    objBusinessLayer = new BusinessLayer(ls);
                    objTask.ServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetByID(objTask.ServiceDeliveryChainId);

                    objBusinessLayer = new BusinessLayer(ls);
                    objTask.TSO = objBusinessLayer.TSORepository.GetSingle(x => x.ID == objTask.TSOId,
                    x => x.TeamLead,
                    x => x.OperationalRisk,
                    x => x.TSOServiceDeliveryChains,
                    x => x.TSOStatus,
                    x => x.TSR);

                    objBusinessLayer = new BusinessLayer(ls);
                    objTask.TSO.TSR.Client = objBusinessLayer.ClientRepository.GetByID(objTask.TSO.TSR.ClientId);

                    objTask.TSO.TSR.SolutionCentre = objBusinessLayer.SolutionCentreRepository.GetByID(objTask.TSO.TSR.SolutionCentreId);

                    //objBusinessLayer = new BusinessLayer(ls);
                    //List<TSOServiceDeliveryChainTask> objTSOServiceDeliveryChainTaskList = objTask.TSOServiceDeliveryChainTasks.Where(x => x.TSOServiceDeliveryChainId == objTask.ID).OrderBy(x => x.WeekNumber).ToList();//  == currentWeek);
                    // TSOServiceDeliveryChainTask objtsoTask = objTSOServiceDeliveryChainTaskList.LastOrDefault();
                    TSOServiceDeliveryChainDTO objTSOSDeliveryChainDTO;
                    bool canAdd = true;
                    if (null != objTask)
                    {
                        objTSOSDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTask);

                        objBusinessLayer = new BusinessLayer(ls);
                        TaskStatus taskStatus = objBusinessLayer.TaskStatusRepository.GetByID(objTask.TaskStatusId);

                        objTSOSDeliveryChainDTO.TaskStatuss = Conversions.ToDTO<TaskStatusDTO, TaskStatus>(taskStatus);



                        if (objChainTaskDTOList.Count > 0)
                        {
                            foreach (TSOServiceDeliveryChainDTO task in objChainTaskDTOList)
                            {
                                if (task.ID == objTSOSDeliveryChainDTO.ID)
                                {
                                    canAdd = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        objTSOSDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTask);

                        if (objChainTaskDTOList.Count > 0)
                        {
                            foreach (TSOServiceDeliveryChainDTO task in objChainTaskDTOList)
                            {
                                if (task.ID == objTSOSDeliveryChainDTO.ID)
                                {
                                    canAdd = false;
                                }
                            }
                        }
                    }


                    if (canAdd)
                    {
                        objTaskDTOList.Add(objTSOSDeliveryChainDTO);
                    }
                }
                totalRecords = objTaskList.Count();

                try
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };

                    if (objTaskList.Count == 0)
                        objDataCollection = new DataCollection { TotalRecords = totalRecords, Message = "No Record Found", EntitySummary = new List<IBaseObject>() };


                    if (null != objChainTaskDTOList)
                    {
                        foreach (TSOServiceDeliveryChainDTO objTASK in objTaskDTOList)
                        {
                            objDataCollection.EntitySummary.Add(objTASK);
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
            return Json(objDataCollection, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// CreateTSR
        /// </summary>
        /// <param name="tsrDTO">TSRDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpGet, Route("v1/ServiceDeliveryChainTask/GetTASKByAdvanceSearchCount/{tsoId}/{strcenter}/{strclient}/{strid}/{strtitle}/{strstatus}/{strpractice}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<string> GetTASKByAdvanceSearchCount(int tsoId, string strcenter, string strclient, int strid, string strtitle, int strstatus, string strpractice)
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

                IList<TSOServiceDeliveryChain> objTaskList;// = objBusinessLayer.TSRRepository.GetAll(x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);               


                string sql = "Select chain.*, task.*, tso.*, sc.*, tsr.* FROM (SELECT TSOServiceDeliveryChainTaskId, MAX(WeekNumber) AS WeekNumber FROM TSOServiceDeliveryChainTaskActual " +
                              "GROUP BY TSOServiceDeliveryChainTaskId) AS m " +
                              "INNER JOIN TSOServiceDeliveryChainTaskActual AS task ON task.TSOServiceDeliveryChainTaskId = m.TSOServiceDeliveryChainTaskId AND task.WeekNumber = m.WeekNumber " +
                              "INNER JOIN TSOServiceDeliveryChain as chain  on chain.ID = m.TSOServiceDeliveryChainTaskId inner join TSO as tso on chain.TSOId = tso.id " +
                              "inner join TSR as tsr on tso.TSRId = tsr.id inner join ServiceDeliveryChain as serv on chain.ServiceDeliveryChainId = serv.id " +
                              "inner join SolutionCentre as sc on tsr.SolutionCentreId = sc.Id inner join Client as cli on tsr.ClientId = cli.Id where";

                if (strid > 0)
                {
                    sql = sql + " chain.ID = " + strid;
                }

                if (tsoId > 0)
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.Id = " + tsoId;
                    }
                    else
                    {
                        sql = sql + " tso.Id = " + tsoId;
                    }
                }

                if (strpractice != "none")
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and tso.Title like '%" + strpractice + "%'";
                    }
                    else
                    {
                        sql = sql + " tso.Title like '%" + strpractice + "%'";
                    }
                }

                if (strclient != "none")
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
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
                    strtitle = strtitle.Replace("(", "");
                    strtitle = strtitle.Replace(")", "");
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and serv.Description like '%" + strtitle + "%'";
                    }
                    else
                    {
                        sql = sql + " serv.Description like '%" + strtitle + "%'";
                    }
                }

                if (strcenter != "none")
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and sc.Name like '%" + strcenter + "%'";
                    }
                    else
                    {
                        sql = sql + " sc.Name like '%" + strcenter + "%'";
                    }
                }

                if (strstatus > -1)
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and chain.TaskStatusId = " + strstatus;
                    }
                    else
                    {
                        sql = sql + " chain.TaskStatusId = " + strstatus;
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
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and (tso.TeamLeadId = " + userID + " or tsr.AccountManagerId = " + userID + " or tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + ")";
                    }
                    else
                    {
                        sql = sql + " (tso.TeamLeadId = " + userID + " or tsr.AccountManagerId = " + userID + " or tsr.DeliveryManagerId = " + userID + " or tsr.TestManagerId = " + userID + ")";
                    }
                }

                objTaskList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetWithRawSql(sql);

                result = Json(objTaskList.Count.ToString(), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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


        private TSOServiceDeliveryChain GetTSRByTSOServiceDeliveryChainId(int id, bool loadEntities = true)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            
            TSOServiceDeliveryChain objTSO = null;
            try
            {
               

                if (loadEntities)
                {
                    objTSO = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID== id,
                    x => x.TSO,
                    x => x.TSO.TSR,
                    x => x.TSO.TSR.DeliveryManager,
                     x => x.ServiceDeliveryChain
                   );
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

            return objTSO;
        }
        [HttpGet, Route("v1/ServiceDeliveryChainTask/GetTSR/{tsoServiceDeliveryChainId}")]
        [ResponseType(typeof(TSOServiceDeliveryChain))]
        // [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager,Team Lead, Guest")]
        public JsonResult<TSOServiceDeliveryChainDTO> GetTSR(int tsoServiceDeliveryChainId)
        {
            JsonResult<TSOServiceDeliveryChainDTO> result = null;
            //TSR objTSR;

            try
            {
                TSOServiceDeliveryChain TSOServiceDeliveryChain = GetTSRByTSOServiceDeliveryChainId(tsoServiceDeliveryChainId);
               // objTSR = TSOServiceDeliveryChain.TSO.TSR;
                var tsrDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(TSOServiceDeliveryChain);
                result = Json(tsrDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return result;
        }

        //[HttpGet, Route("v1/ServiceDeliveryChainTask/GetServiceDeliveryChainTask/{tsoServiceDeliveryChainId}")]
        //[ResponseType(typeof(TSOServiceDeliveryChain))]
        //// [SessionAuthorize(Roles = "Admin,Delivery Manager,Test Manager,Team Lead, Guest")]
        //public JsonResult<TSOServiceDeliveryChainDTO> GetServiceDeliveryChainTask(int tsoServiceDeliveryChainId)
        //{
        //    JsonResult<TSRDTO> result = null;
        //    TSR objTSR;

        //    try
        //    {
        //        TSOServiceDeliveryChain TSOServiceDeliveryChain = GetTSRByTSOServiceDeliveryChainId(tsoServiceDeliveryChainId);
        //        objTSR = TSOServiceDeliveryChain.TSO.TSR;
        //        var tsrDTO = Conversions.ToDTO<TSRDTO, TSR>(objTSR);
        //        result = Json(tsrDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        //    }
        //    catch (Exception ex)
        //    {
        //        TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
        //        throw;
        //    }
        //    return result;
        //}
    }
}