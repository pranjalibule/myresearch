﻿using Newtonsoft.Json;
using SQS.nTier.TTM.BAL;
using SQS.nTier.TTM.DAL;
using SQS.nTier.TTM.DTO;
using SQS.nTier.TTM.GenericFramework;
using SQS.nTier.TTM.GenericFramework.Utility;
using SQS.nTier.TTM.WebAPI.Common;
using SQS.nTier.TTM.WebAPI.RoleAttribute;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace SQS.nTier.TTM.WebAPI.Controllers
{
    [RoutePrefix("api")]
    public class TSOServiceDeliveryChainRiskController : ApiController
    {
        #region Private Functions

        private IList<TSOServiceDeliveryChainDTO> GetAllTSOServiceDeliveryChainRisk(int taskid, int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSOServiceDeliveryChainDTO> objTaskSDTOList = null;

            List<TSOServiceDeliveryChainActualRisk> objARList = new List<TSOServiceDeliveryChainActualRisk>();
            // List<TSOServiceDeliveryChainPlannedRisk> objPRList = new List<TSOServiceDeliveryChainPlannedRisk>();
            List<TSOServiceDeliveryChainActualRiskDTO> objARDTOList = new List<TSOServiceDeliveryChainActualRiskDTO>();
            //List<TSOServiceDeliveryChainPlannedRiskDTO> objPRDTOList = new List<TSOServiceDeliveryChainPlannedRiskDTO>();
            totalRecords = 0;
            try
            {

                IList<TSOServiceDeliveryChain> objTask = null;
                objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.ID == taskid,
                    x => x.ServiceDeliveryChain,
                    x => x.TSOServiceDeliveryChainActualRisks,
                    // x => x.TSOServiceDeliveryChainPlannedRisks,
                    x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                    x => x.TSO.TSR.SolutionCentre, x => x.TSO.TSR.Practice
                    );



                int week = GetWeek();
                objTaskSDTOList = new List<TSOServiceDeliveryChainDTO>();
                objBusinessLayer = new BusinessLayer(ls);
                foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTask)
                {
                    // List<TSOServiceDeliveryChainActualRisk> objTSOServiceDeliveryChainTaskActual = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualRisks.Where(x => x.WeekNumber == week).ToList();
                    List<TSOServiceDeliveryChainActualRisk> objTSOServiceDeliveryChainTaskActual = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualRisks.OrderByDescending(x => x.WeekNumber).ToList();
                    // List<TSOServiceDeliveryChainPlannedRisk> objTSOServiceDeliveryChainTaskPlanned = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedRisks.OrderBy(x => x.WeekNumber == week + 1).ToList();
                    //TSOServiceDeliveryChainActualRisk objTSOServiceDeliveryChainTaskActual = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualRisks.OrderBy(x => x.WeekNumber).ToList().LastOrDefault();
                    //TSOServiceDeliveryChainPlannedRisk objTSOServiceDeliveryChainTaskPlanned = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedRisks.OrderBy(x => x.WeekNumber).ToList().LastOrDefault();
                    foreach (var item in objTSOServiceDeliveryChainTaskActual)
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        TSOServiceDeliveryChainActualRisk objTSOServiceDeliveryChainTaskActuals = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetSingle(x => x.ID == item.ID, x => x.ResponsiblePerson, x => x.ActualOperationalRiskIndicator, x => x.ActualOperationalRisk, x => x.Status);
                        objARList.Add(objTSOServiceDeliveryChainTaskActuals);
                    }
                    //foreach (var item in objTSOServiceDeliveryChainTaskPlanned)
                    //{
                    //    objBusinessLayer = new BusinessLayer(ls);
                    //    TSOServiceDeliveryChainPlannedRisk objTSOServiceDeliveryChainTaskPlanneds = objBusinessLayer.TSOServiceDeliveryChainPlannedRiskRepository.GetSingle(x => x.ID == item.ID, x => x.PlannedOperationalRisk, x => x.PlannedOperationalRiskIndicator);
                    //    objPRList.Add(objTSOServiceDeliveryChainTaskPlanneds);
                    //}

                    if (null != objARList)// && null != objPRList
                    {
                        foreach (var item in objARList)
                        {
                            var objResponsibleDTO = Conversions.ToDTO<UserDTO, User>(item.ResponsiblePerson);
                            var objOperationalARiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(item.ActualOperationalRisk);
                            var objOperationalAIndicatorDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(item.ActualOperationalRiskIndicator);
                            var objStatusDTO = Conversions.ToDTO<RiskStatusDTO, RiskStatus>(item.Status);

                            var objTSOSDeliveryChainActualDTO = Conversions.ToDTO<TSOServiceDeliveryChainActualRiskDTO, TSOServiceDeliveryChainActualRisk>(item);
                            objTSOSDeliveryChainActualDTO.ActualOperationalRisk = objOperationalARiskDTO;
                            objTSOSDeliveryChainActualDTO.ActualOperationalRiskIndicator = objOperationalAIndicatorDTO;
                            objTSOSDeliveryChainActualDTO.ResponsiblePerson = objResponsibleDTO;
                            objTSOSDeliveryChainActualDTO.Status = objStatusDTO;


                            objARDTOList.Add(objTSOSDeliveryChainActualDTO);
                        }

                        //foreach (var item in objPRList)
                        //{
                        //    //planned
                        //    var objOperationalPRiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(item.PlannedOperationalRisk);
                        //    var objOperationalPIndicatorDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(item.PlannedOperationalRiskIndicator);

                        //    var objTSOSDeliveryChainPlannedDTO = Conversions.ToDTO<TSOServiceDeliveryChainPlannedRiskDTO, TSOServiceDeliveryChainPlannedRisk>(item);
                        //    objTSOSDeliveryChainPlannedDTO.PlannedOperationalRisk = objOperationalPRiskDTO;
                        //    objTSOSDeliveryChainPlannedDTO.PlannedOperationalRiskIndicator = objOperationalPIndicatorDTO;
                        //    objPRDTOList.Add(objTSOSDeliveryChainPlannedDTO);
                        //}


                        var objTSOSDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
                        objTSOSDeliveryChainDTO.TSOServiceDeliveryChainActualRisks.Clear();
                        //  objTSOSDeliveryChainDTO.TSOServiceDeliveryChainPlannedRisks.Clear();
                        foreach (var item in objARDTOList)
                        {
                            objTSOSDeliveryChainDTO.TSOServiceDeliveryChainActualRisks.Add(item);
                        }

                        //foreach (var item in objPRDTOList)
                        //{
                        //    objTSOSDeliveryChainDTO.TSOServiceDeliveryChainPlannedRisks.Add(item);
                        //}

                        objTaskSDTOList.Add(objTSOSDeliveryChainDTO);
                    }
                    else
                    {
                        var objTSOSDeliveryChainDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTSOServiceDeliveryChain);
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



        private IList<TSOServiceDeliveryChainDTO> GetAllUserTASKS(int startingRecordNumber, int pageSize, string userId, out int totalRecords)
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            totalRecords = 0;
            IList<TSOServiceDeliveryChainDTO> objTaskSDTOList = null;
            int week = GetWeek();
            List<TSOServiceDeliveryChainActualRisk> objARList = new List<TSOServiceDeliveryChainActualRisk>();
            // List<TSOServiceDeliveryChainPlannedRisk> objPRList = new List<TSOServiceDeliveryChainPlannedRisk>();
            List<TSOServiceDeliveryChainActualRiskDTO> objARDTOList = new List<TSOServiceDeliveryChainActualRiskDTO>();
            // List<TSOServiceDeliveryChainPlannedRiskDTO> objPRDTOList = new List<TSOServiceDeliveryChainPlannedRiskDTO>();
            try
            {
                IList<TSOServiceDeliveryChainActualRisk> objTask = null;
                int userID = Convert.ToInt32(userId);
                objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);
                //Note - Currently we are showing all record in TSO dashboard as per discussion
                if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    // objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords, x => x.WeekNumber == week,
                    // x => x.TSOServiceDeliveryChain,
                    // x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,                 
                    // x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR,
                    // x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre,
                    // x => x.TSOServiceDeliveryChain.TSO.TSR.Practice,
                    // x=>x.ResponsiblePerson,
                    // x=>x.ActualOperationalRisk,
                    // x=>x.ActualOperationalRiskIndicator
                    //); 

                    objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords,
                     x => x.TSOServiceDeliveryChain,
                     x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,
                     x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR,
                     x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre,
                     x => x.TSOServiceDeliveryChain.TSO.TSR.Practice,
                     x => x.ResponsiblePerson,
                     x => x.ActualOperationalRisk,
                     x => x.ActualOperationalRiskIndicator,
                      x => x.Status
                    ).OrderByDescending(x => x.WeekNumber).ToList();
                }
                else
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    // objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords,
                    // x => x.TSOServiceDeliveryChain.TSO.TSR.AccountManagerId == userID || x.TSOServiceDeliveryChain.TSO.TSR.DeliveryManagerId == userID || x.TSOServiceDeliveryChain.TSO.TSR.TestManagerId == userID || x.TSOServiceDeliveryChain.TSO.TeamLeadId == userID && x.WeekNumber==week,
                    //  x => x.TSOServiceDeliveryChain,
                    //x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,
                    //x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR,
                    //x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre,
                    //x => x.TSOServiceDeliveryChain.TSO.TSR.Practice,
                    //x => x.ResponsiblePerson,
                    //x => x.ActualOperationalRisk,
                    //x => x.ActualOperationalRiskIndicator
                    // );

                    objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetAllWithWhere(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords,
                    x => x.TSOServiceDeliveryChain.TSO.TSR.AccountManagerId == userID || x.TSOServiceDeliveryChain.TSO.TSR.DeliveryManagerId == userID || x.TSOServiceDeliveryChain.TSO.TSR.TestManagerId == userID || x.TSOServiceDeliveryChain.TSO.TeamLeadId == userID,
                     x => x.TSOServiceDeliveryChain,
                   x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,
                   x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR,
                   x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre,
                   x => x.TSOServiceDeliveryChain.TSO.TSR.Practice,
                   x => x.ResponsiblePerson,
                   x => x.ActualOperationalRisk,
                   x => x.ActualOperationalRiskIndicator,
                    x => x.Status
                    ).OrderByDescending(x => x.WeekNumber).ToList();


                    objBusinessLayer = new BusinessLayer(ls);
                    List<TSRTMOUser> tmoUsers = objBusinessLayer.TSRTMOUserRepository.GetList(x => x.UserId == userID, x => x.TSR, x => x.TSR.TSRStatus, x => x.TSR.TestManager, x => x.TSR.Client, x => x.TSR.AccountManager, x => x.TSR.DeliveryManager, x => x.TSR.Engagement, x => x.TSR.OperationalRisk, x => x.TSR.Vertical, x => x.TSR.SolutionCentre, x => x.TSR.Practice, x => x.TSR.PricingModel, x => x.TSR.ClientRegion, x => x.TSR.TSRReleventRepositories, x => x.TSR.TSRCoreServices, x => x.TSR.TSRTMOUsers).ToList();
                    foreach (var item in tmoUsers)
                    {
                        if (objTask.Count < pageSize)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            if (!objTask.Any(x => x.TSOServiceDeliveryChain.TSO.TSRId == item.TSRId))
                            {
                                IList<TSOServiceDeliveryChainActualRisk> objSTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetList(x => x.TSOServiceDeliveryChain.TSO.TSRId == item.TSRId,
                                 x => x.TSOServiceDeliveryChain,
                          x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,
                   x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR,
                   x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre,
                   x => x.TSOServiceDeliveryChain.TSO.TSR.Practice,
                   x => x.ResponsiblePerson,
                   x => x.ActualOperationalRisk,
                   x => x.ActualOperationalRiskIndicator,
                   x => x.Status
                                ).OrderByDescending(x => x.WeekNumber).ToList();

                                foreach (TSOServiceDeliveryChainActualRisk objT in objSTask)
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

                foreach (TSOServiceDeliveryChainActualRisk objActulRisk in objTask)
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    TSOServiceDeliveryChain Task = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == objActulRisk.TSOServiceDeliveryChainId,
                    x => x.ServiceDeliveryChain,
                    x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                    x => x.TSO.TSR.SolutionCentre, x => x.TSO.TSR.Practice
                    );


                    TSOServiceDeliveryChainActualRiskDTO objActRiskDTO = Conversions.ToDTO<TSOServiceDeliveryChainActualRiskDTO, TSOServiceDeliveryChainActualRisk>(objActulRisk);

                    //objBusinessLayer = new BusinessLayer(ls);
                    //OperationalRiskIndicator objOperationalAIndicator = objBusinessLayer.OperationalRiskIndicatorRepository.GetByID(objActulRisk.ActualOperationalRiskIndicatorId);

                    //objBusinessLayer = new BusinessLayer(ls);
                    //User objUser = objBusinessLayer.UserRepository.GetByID(objActulRisk.ResponsiblePerson);

                    //objBusinessLayer = new BusinessLayer(ls);
                    //OperationalRisk objOperationalARisk = objBusinessLayer.OperationalRiskRepository.GetByID(objActulRisk.ActualOperationalRiskId);


                    var objResponsibleDTO = Conversions.ToDTO<UserDTO, User>(objActulRisk.ResponsiblePerson);
                    var objOperationalAIndicatorDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objActulRisk.ActualOperationalRiskIndicator);
                    var objOperationalARiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objActulRisk.ActualOperationalRisk);
                    var objStatusDTO = Conversions.ToDTO<RiskStatusDTO, RiskStatus>(objActulRisk.Status);
                    objActRiskDTO.ResponsiblePerson = objResponsibleDTO;
                    objActRiskDTO.ActualOperationalRisk = objOperationalARiskDTO;
                    objActRiskDTO.ActualOperationalRiskIndicator = objOperationalAIndicatorDTO;
                    objActRiskDTO.Status = objStatusDTO;

                    TSOServiceDeliveryChainDTO objTaskDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(Task);
                    objTaskDTO.TSOServiceDeliveryChainActualRisks.Add(objActRiskDTO);



                    //  objBusinessLayer = new BusinessLayer(ls);
                    //  TSOServiceDeliveryChainPlannedRisk objPlanRisk = objBusinessLayer.TSOServiceDeliveryChainPlannedRiskRepository.GetSingle(x => x.TSOServiceDeliveryChainId == objActulRisk.TSOServiceDeliveryChainId && x.WeekNumber == (objActulRisk.WeekNumber + 1) && x.PlannedOperationalRiskIndicatorId == objActulRisk.ActualOperationalRiskIndicatorId,x=>x.PlannedOperationalRisk);

                    //objBusinessLayer = new BusinessLayer(ls);
                    //OperationalRiskIndicator objOperationalPIndicator = objBusinessLayer.OperationalRiskIndicatorRepository.GetByID(objPlanRisk.PlannedOperationalRiskIndicatorId);
                    //var objOperationalPIndicatorDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objOperationalPIndicator);

                    //TSOServiceDeliveryChainPlannedRiskDTO objPlanRiskDTO = Conversions.ToDTO<TSOServiceDeliveryChainPlannedRiskDTO, TSOServiceDeliveryChainPlannedRisk>(objPlanRisk);
                    //objPlanRiskDTO.PlannedOperationalRiskIndicator = objOperationalPIndicatorDTO;
                    //objTaskDTO.TSOServiceDeliveryChainPlannedRisks.Add(objPlanRiskDTO);

                    objTaskSDTOList.Add(objTaskDTO);
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

        #endregion


        /// <summary>
        /// GetAllUserPaged
        /// </summary>
        /// <param name="pageSize">int</param>
        /// <param name="startingRecordNumber">int</param>
        /// <returns>JsonResult<DataCollection>></returns>
        [HttpGet, Route("v1/TSOServiceDeliveryChainRisk/GetAllUserPaged/{startingRecordNumber}/{pageSize}")]
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
        [HttpGet, Route("v1/TSOServiceDeliveryChainRisk/GetAllPaged/{taskid}/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<DataCollection> GetAllPaged(int taskid, int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            //IList<TSOServiceDeliveryChainTaskDTO> objTASKDTOList = this.GetAllTask(tsoId, startingRecordNumber, pageSize, out totalRecords);
            IList<TSOServiceDeliveryChainDTO> objTASK1DTOList = this.GetAllTSOServiceDeliveryChainRisk(taskid, startingRecordNumber, pageSize, out totalRecords);

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



        [HttpPost, Route("v1/TSOServiceDeliveryChainRisk/CreateRiskForTask")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Team Lead,Test Manager,TMO")]
        public JsonResult<string> CreateRiskForTask([FromBody] RiskDTO data)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                TSOServiceDeliveryChainActualRiskDTO tsoServiceDeliveryChainTaskActualDTO = new TSOServiceDeliveryChainActualRiskDTO();
                //TSOServiceDeliveryChainPlannedRiskDTO tsoServiceDeliveryChainTaskPlannedDTO = new TSOServiceDeliveryChainPlannedRiskDTO();

                tsoServiceDeliveryChainTaskActualDTO.ID = data.ID;
                tsoServiceDeliveryChainTaskActualDTO.TSOServiceDeliveryChainId = data.TSOServiceDeliveryChainId;
                tsoServiceDeliveryChainTaskActualDTO.ResponsiblePersonId = data.ResponsiblePersonId;
                tsoServiceDeliveryChainTaskActualDTO.ResponsiblePerson = data.ResponsiblePerson;
                tsoServiceDeliveryChainTaskActualDTO.DueDate = data.DueDate;
                tsoServiceDeliveryChainTaskActualDTO.CreatedBy = data.CreatedBy;
                tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRisk = data.ActualOperationalRisk;
                tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskId = data.ActualOperationalRiskId;
                tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskIndicator = data.ActualOperationalRiskIndicator;
                tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskIndicatorId = data.ActualOperationalRiskIndicatorId;
                tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskDescription = data.ActualOperationalRiskDescription;
                tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskMitigation = data.ActualOperationalRiskMitigation;
                tsoServiceDeliveryChainTaskActualDTO.WeekNumber = data.WeekNumber;
                tsoServiceDeliveryChainTaskActualDTO.Year = DateTime.Now.Year;

                tsoServiceDeliveryChainTaskActualDTO.Impact = data.Impact;
                tsoServiceDeliveryChainTaskActualDTO.Resolution = data.Resolution;
                tsoServiceDeliveryChainTaskActualDTO.RaisedBy = data.RaisedBy;
                tsoServiceDeliveryChainTaskActualDTO.RaisedById = data.RaisedById;
                tsoServiceDeliveryChainTaskActualDTO.StatusId = data.StatusId;


                //tsoServiceDeliveryChainTaskPlannedDTO.ID = data.PlannedRiskId;
                //tsoServiceDeliveryChainTaskPlannedDTO.TSOServiceDeliveryChainId = data.TSOServiceDeliveryChainId;
                //tsoServiceDeliveryChainTaskPlannedDTO.CreatedBy = data.CreatedBy;
                //tsoServiceDeliveryChainTaskPlannedDTO.WeekNumber = data.WeekNumber + 1;
                //tsoServiceDeliveryChainTaskPlannedDTO.Year = DateTime.Now.Year;
                //tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRisk = data.PlannedOperationalRisk;
                //tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskId = data.PlannedOperationalRiskId;
                //tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskIndicatorId = data.PlannedOperationalRiskIndicatorId;
                //tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskIndicator = data.PlannedOperationalRiskIndicator;
                //tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskDescription = data.PlannedOperationalRiskDescription;
                //tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskMitigation = data.PlannedOperationalRiskMitigation;

                result = Json(this.AddNewRisk(tsoServiceDeliveryChainTaskActualDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                //, tsoServiceDeliveryChainTaskPlannedDTO
                SetOperationRiskToTSO(data.TSOServiceDeliveryChainId, data.CreatedBy);

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

        private void SetOperationRiskToTSO(int tsoServiceDeliveryChainId, string UpdatedBy)
        {

            OperationalRisk operationalRisk = null;
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            TSOServiceDeliveryChainActualRisk actualRiskobj = null;

            List<OperationalRisk> opList = new List<OperationalRisk>();
            try
            {
                int week = GetWeek();
                TSOServiceDeliveryChain objTSOServiceDeliveryChain = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == tsoServiceDeliveryChainId, x => x.TSO);
                TSO tso = objTSOServiceDeliveryChain.TSO;
                objBusinessLayer = new BusinessLayer(ls);
                List<TSOServiceDeliveryChain> TSOServiceDeliveryChain = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSOId == tso.ID && x.ServiceDeliveryChainId == 42, x => x.TSOServiceDeliveryChainActualRisks.Select(y => y.ActualOperationalRisk)).ToList();
                foreach (var item in TSOServiceDeliveryChain)
                {
                    List<TSOServiceDeliveryChainActualRisk> actualRisk = item.TSOServiceDeliveryChainActualRisks.Where(x => x.WeekNumber == week).ToList();
                    actualRiskobj = actualRisk.Where(x=>x.StatusId!=3).OrderByDescending(x => x.ActualOperationalRisk.RiskNo).FirstOrDefault();
                    if (actualRiskobj != null)
                    {
                        opList.Add(actualRiskobj.ActualOperationalRisk);
                    }
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
                else
                {
                    tso.OperationalRiskId =3;
                    ls.LoginName = UpdatedBy;
                    tso.UpdatedBy = UpdatedBy;
                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.TSORepository.Update(tso);
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
        private string AddNewRisk(TSOServiceDeliveryChainActualRiskDTO tsoServiceDeliveryChainTaskActualDTO)//, TSOServiceDeliveryChainPlannedRiskDTO tsoServiceDeliveryChainTaskPlannedDTO
        {
            string returnMessage = string.Empty;
            LoginSession ls = new LoginSession();
            ls.LoginName = tsoServiceDeliveryChainTaskActualDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                TSOServiceDeliveryChainActualRisk objTSOServiceDeliveryChainActual = Conversions.ToEntity<TSOServiceDeliveryChainActualRiskDTO, TSOServiceDeliveryChainActualRisk>(tsoServiceDeliveryChainTaskActualDTO);
                // TSOServiceDeliveryChainPlannedRisk objTSOServiceDeliveryChainPlanned = Conversions.ToEntity<TSOServiceDeliveryChainPlannedRiskDTO, TSOServiceDeliveryChainPlannedRisk>(tsoServiceDeliveryChainTaskPlannedDTO);

                objBusinessLayer = new BusinessLayer(ls);
                TSOServiceDeliveryChain existstask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == tsoServiceDeliveryChainTaskActualDTO.TSOServiceDeliveryChainId, x => x.TSOServiceDeliveryChainActualRisks);//, x => x.TSOServiceDeliveryChainPlannedRisks

                TSOServiceDeliveryChainActualRisk existstaskactual = existstask.TSOServiceDeliveryChainActualRisks.FirstOrDefault(x => x.WeekNumber == objTSOServiceDeliveryChainActual.WeekNumber && x.ActualOperationalRiskIndicatorId == tsoServiceDeliveryChainTaskActualDTO.ActualOperationalRiskIndicatorId);
                // && x.ID== tsoServiceDeliveryChainTaskActualDTO.ID
                // &&  x.ID == tsoServiceDeliveryChainTaskPlannedDTO.ID
                //int plannedweekNumber = objTSOServiceDeliveryChainActual.WeekNumber + 1;
                //TSOServiceDeliveryChainPlannedRisk existstaskplanned = existstask.TSOServiceDeliveryChainPlannedRisks.FirstOrDefault(x => x.WeekNumber == plannedweekNumber && x.PlannedOperationalRiskIndicatorId == tsoServiceDeliveryChainTaskPlannedDTO.PlannedOperationalRiskIndicatorId);


                if (existstaskactual != null)// && existstaskplanned != null
                {
                    if (existstaskactual != null)
                    {
                        if (objTSOServiceDeliveryChainActual.ID != 0)
                        {
                            //objBusinessLayer = new BusinessLayer(ls);
                            //TSOServiceDeliveryChainActualRisk existstaskactualW = existstask.TSOServiceDeliveryChainActualRisks.FirstOrDefault(x => x.WeekNumber == objTSOServiceDeliveryChainActual.WeekNumber && x.ActualOperationalRiskIndicatorId == objTSOServiceDeliveryChainActual.ActualOperationalRiskIndicatorId);
                            //if (existstaskactualW != null)
                            //{
                            //    returnMessage = "Error - Risk with same category is already available.";
                            //}
                            //else
                            //{

                                objTSOServiceDeliveryChainActual.CreatedOn = existstaskactual.CreatedOn;
                                objTSOServiceDeliveryChainActual.UpdatedOn = DateTime.Today;
                                objBusinessLayer = new BusinessLayer(ls);
                                objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.Update(objTSOServiceDeliveryChainActual);
                                returnMessage = "Risk updated successfully";
                          //  }
                        }
                        else
                        {
                            returnMessage = "Error - Risk with same category is already available.";
                        }
                    }

                    //if (existstaskplanned != null)
                    //{
                    //    if (objTSOServiceDeliveryChainPlanned.ID != 0)
                    //    {
                    //        objTSOServiceDeliveryChainPlanned.CreatedOn = existstaskplanned.CreatedOn;
                    //        objTSOServiceDeliveryChainPlanned.UpdatedOn = DateTime.Today;
                    //        objBusinessLayer = new BusinessLayer(ls);
                    //        objBusinessLayer.TSOServiceDeliveryChainPlannedRiskRepository.Update(objTSOServiceDeliveryChainPlanned);
                    //        returnMessage = "Risk updated successfully";
                    //    }
                    //    else
                    //    {
                    //        returnMessage = "Error - Risk with same indicator is already available.";
                    //    }
                    //}


                }
                else
                {
                    //int week = GetWeekOfYear(DateTime.Now);
                    //if (objTSOServiceDeliveryChainActual.WeekNumber < week)
                    //{
                    //    TTMLogger.Logger.LogError(String.Format("Error - {0}", "You can not create privious week data."));
                    //    throw new Exception("You can not create previous week data.");
                    //}                   

                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.Add(objTSOServiceDeliveryChainActual);

                    //objBusinessLayer = new BusinessLayer(ls);
                    //objBusinessLayer.TSOServiceDeliveryChainPlannedRiskRepository.Add(objTSOServiceDeliveryChainPlanned);

                    returnMessage = "Risk saved successfully";
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
        /// GetTSRByTitleOrClient
        /// </summary>
        /// <param name="startingRecordNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="title"></param>
        /// <param name="status"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        [ResponseType(typeof(DataCollection))]
        [HttpGet, Route("v1/TSOServiceDeliveryChainRisk/GetRISKByAdvanceSearch/{pageNumber}/{pageSize}/{strcenter}/{strclient}/{strid}/{strtitle}/{strstatus}/{strpractice}/{strweek}/{strriskstatus}")]
        public JsonResult<DataCollection> GetRISKByAdvanceSearch(int pageNumber, int pageSize, string strcenter, string strclient, double strid, string strtitle, int strstatus, string strpractice, int strweek, int strriskstatus)
        {
            LoginSession ls = new LoginSession();
            JsonResult<DataCollection> objReturn = null;
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            DataCollection objDataCollection;
            IList<TSOServiceDeliveryChainActualRisk> objTaskList = null;
            IList<TSOServiceDeliveryChainDTO> objTaskSDTOList = new List<TSOServiceDeliveryChainDTO>();
            int totalRecords = pageSize;
            strclient = GetSpecialChar(strclient);
            strtitle = GetSpecialChar(strtitle);
            strcenter = GetSpecialChar(strcenter);
            strpractice = GetSpecialChar(strpractice);

            try
            {
                string sql = "Select task.* FROM (SELECT TSOServiceDeliveryChainId, MAX(WeekNumber) " +
                             "AS WeekNumber FROM TSOServiceDeliveryChainActualRisk GROUP BY TSOServiceDeliveryChainId) AS m " +
                             "INNER JOIN TSOServiceDeliveryChainActualRisk AS task ON task.TSOServiceDeliveryChainId = m.TSOServiceDeliveryChainId AND task.WeekNumber = m.WeekNumber " +
                             "INNER JOIN TSOServiceDeliveryChain as chain  on chain.ID = m.TSOServiceDeliveryChainId " +
                             "INNER JOIN OperationalRiskIndicator as Ind on Ind.Id = task.ActualOperationalRiskIndicatorId " +
                             "INNER JOIN OperationalRisk as risk on risk.Id = task.ActualOperationalRiskId " +
                             "inner join TSO as tso on chain.TSOId = tso.id inner join TSR as tsr on tso.TSRId = tsr.id inner join Practice as p on tsr.PracticeId = p.Id " +
                             "inner join ServiceDeliveryChain as serv on chain.ServiceDeliveryChainId = serv.id " +
                             "inner join SolutionCentre as sc on tsr.SolutionCentreId = sc.Id " +
                             "inner join Client as cli on tsr.ClientId = cli.Id  where";


                if (strid > 0)
                {
                    sql = sql + " task.ID = " + strid;
                }

                if (strtitle != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and Ind.Name like '%" + strtitle + "%'";
                    }
                    else
                    {
                        sql = sql + " Ind.Name like '%" + strtitle + "%'";
                    }
                }

                if (strstatus > -1)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and risk.Id = " + strstatus;
                    }
                    else
                    {
                        sql = sql + " risk.Id = " + strstatus;
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

                if (strweek > -1)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and task.WeekNumber = " + strweek;
                    }
                    else
                    {
                        sql = sql + " task.WeekNumber = " + strweek;
                    }
                }

                if (strriskstatus > -1)
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and task.StatusId = " + strriskstatus;
                    }
                    else
                    {
                        sql = sql + " task.StatusId = " + strriskstatus;
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
                objTaskList = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetWithRawSql(sql);
                foreach (TSOServiceDeliveryChainActualRisk objActulRisk in objTaskList)
                {
                    objBusinessLayer = new BusinessLayer(ls);
                    TSOServiceDeliveryChain objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetSingle(x => x.ID == objActulRisk.TSOServiceDeliveryChainId,
                    x => x.ServiceDeliveryChain,
                    x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                    x => x.TSO.TSR.SolutionCentre, x => x.TSO.TSR.Practice
                    );


                    TSOServiceDeliveryChainActualRiskDTO objActRiskDTO = Conversions.ToDTO<TSOServiceDeliveryChainActualRiskDTO, TSOServiceDeliveryChainActualRisk>(objActulRisk);

                    objBusinessLayer = new BusinessLayer(ls);
                    OperationalRiskIndicator objOperationalAIndicator = objBusinessLayer.OperationalRiskIndicatorRepository.GetByID(objActulRisk.ActualOperationalRiskIndicatorId);

                    objBusinessLayer = new BusinessLayer(ls);
                    User objUser = objBusinessLayer.UserRepository.GetByID(objActulRisk.ResponsiblePersonId);

                    objBusinessLayer = new BusinessLayer(ls);
                    OperationalRisk objOperationalARisk = objBusinessLayer.OperationalRiskRepository.GetByID(objActulRisk.ActualOperationalRiskId);

                    objBusinessLayer = new BusinessLayer(ls);
                    RiskStatus objStatus = objBusinessLayer.RiskStatusRepository.GetByID(objActulRisk.StatusId);


                    var objResponsibleDTO = Conversions.ToDTO<UserDTO, User>(objUser);
                    var objOperationalAIndicatorDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objOperationalAIndicator);
                    var objOperationalARiskDTO = Conversions.ToDTO<OperationalRiskDTO, OperationalRisk>(objOperationalARisk);
                    var objRiskStatusDTO = Conversions.ToDTO<RiskStatusDTO, RiskStatus>(objStatus);

                    objActRiskDTO.ResponsiblePerson = objResponsibleDTO;
                    objActRiskDTO.ActualOperationalRisk = objOperationalARiskDTO;
                    objActRiskDTO.ActualOperationalRiskIndicator = objOperationalAIndicatorDTO;
                    objActRiskDTO.Status = objRiskStatusDTO;

                    TSOServiceDeliveryChainDTO objTaskDTO = Conversions.ToDTO<TSOServiceDeliveryChainDTO, TSOServiceDeliveryChain>(objTask);
                    objTaskDTO.TSOServiceDeliveryChainActualRisks.Add(objActRiskDTO);



                    //objBusinessLayer = new BusinessLayer(ls);
                    //TSOServiceDeliveryChainPlannedRisk objPlanRisk = objBusinessLayer.TSOServiceDeliveryChainPlannedRiskRepository.GetSingle(x => x.TSOServiceDeliveryChainId == objActulRisk.TSOServiceDeliveryChainId && x.WeekNumber == (objActulRisk.WeekNumber + 1) && x.PlannedOperationalRiskIndicatorId == objActulRisk.ActualOperationalRiskIndicatorId);

                    //objBusinessLayer = new BusinessLayer(ls);
                    //OperationalRiskIndicator objOperationalPIndicator = objBusinessLayer.OperationalRiskIndicatorRepository.GetByID(objPlanRisk.PlannedOperationalRiskIndicatorId);
                    //var objOperationalPIndicatorDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objOperationalPIndicator);

                    //TSOServiceDeliveryChainPlannedRiskDTO objPlanRiskDTO = Conversions.ToDTO<TSOServiceDeliveryChainPlannedRiskDTO, TSOServiceDeliveryChainPlannedRisk>(objPlanRisk);
                    //objPlanRiskDTO.PlannedOperationalRiskIndicator = objOperationalPIndicatorDTO;
                    //objTaskDTO.TSOServiceDeliveryChainPlannedRisks.Add(objPlanRiskDTO);

                    objTaskSDTOList.Add(objTaskDTO);
                }
                totalRecords = objTaskSDTOList.Count();

                try
                {
                    objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };

                    if (objTaskSDTOList.Count == 0)
                        objDataCollection = new DataCollection { TotalRecords = totalRecords, Message = "No Record Found", EntitySummary = new List<IBaseObject>() };


                    if (null != objTaskSDTOList)
                    {
                        foreach (TSOServiceDeliveryChainDTO objTASK in objTaskSDTOList)
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
        [HttpGet, Route("v1/TSOServiceDeliveryChainRisk/GetRISKByAdvanceSearchCount/{strcenter}/{strclient}/{strid}/{strtitle}/{strstatus}/{strpractice}/{strweek}/{strriskstatus}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<string> GetRISKByAdvanceSearchCount(string strcenter, string strclient, int strid, string strtitle, int strstatus, string strpractice, int strweek, int strriskstatus)
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

                IList<TSOServiceDeliveryChainActualRisk> objTaskList;// = objBusinessLayer.TSRRepository.GetAll(x => x.TSRStatus, x => x.TestManager, x => x.AccountManager, x => x.DeliveryManager, x => x.Engagement, x => x.OperationalRisk, x => x.Vertical, x => x.Practice, x => x.SolutionCentre, x => x.Client, x => x.ClientRegion, x => x.PricingModel, x => x.MarketOffering, x => x.OperationalRisk, x => x.TSRReleventRepositories, x => x.TSRCoreServices);               


                string sql = "Select task.* FROM (SELECT TSOServiceDeliveryChainId, MAX(WeekNumber) " +
                             "AS WeekNumber FROM TSOServiceDeliveryChainActualRisk GROUP BY TSOServiceDeliveryChainId) AS m " +
                             "INNER JOIN TSOServiceDeliveryChainActualRisk AS task ON task.TSOServiceDeliveryChainId = m.TSOServiceDeliveryChainId AND task.WeekNumber = m.WeekNumber " +
                             "INNER JOIN TSOServiceDeliveryChain as chain  on chain.ID = m.TSOServiceDeliveryChainId " +
                             "INNER JOIN OperationalRiskIndicator as Ind on Ind.Id = task.ActualOperationalRiskIndicatorId " +
                             "INNER JOIN OperationalRisk as risk on risk.Id = task.ActualOperationalRiskId " +
                             "inner join TSO as tso on chain.TSOId = tso.id inner join TSR as tsr on tso.TSRId = tsr.id inner join Practice as p on tsr.PracticeId = p.Id " +
                             "inner join ServiceDeliveryChain as serv on chain.ServiceDeliveryChainId = serv.id " +
                             "inner join SolutionCentre as sc on tsr.SolutionCentreId = sc.Id " +
                             "inner join Client as cli on tsr.ClientId = cli.Id  where";


                if (strid > 0)
                {
                    sql = sql + " task.ID = " + strid;
                }

                if (strtitle != "none")
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and Ind.Name like '%" + strtitle + "%'";
                    }
                    else
                    {
                        sql = sql + " Ind.Name like '%" + strtitle + "%'";
                    }
                }

                if (strstatus > -1)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and risk.Id = " + strstatus;
                    }
                    else
                    {
                        sql = sql + " risk.Id = " + strstatus;
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

                if (strweek > -1)
                {
                    if (sql.IndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and task.WeekNumber = " + strweek;
                    }
                    else
                    {
                        sql = sql + " task.WeekNumber = " + strweek;
                    }
                }

                if (strriskstatus > -1)
                {
                    if (sql.LastIndexOf("where") != sql.Length - 5)
                    {
                        sql = sql + " and task.StatusId = " + strriskstatus;
                    }
                    else
                    {
                        sql = sql + " task.StatusId = " + strriskstatus;
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

                objTaskList = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetWithRawSql(sql);

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

        private RiskDTO GetLastTSOServiceDeliveryChainRisk(int tsoServiceDeliveryChainActualRiskId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            RiskDTO objRiskDTO = new RiskDTO();

            try
            {
                TSOServiceDeliveryChainActualRisk objActualTaskRisk = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetSingle(x => x.ID == tsoServiceDeliveryChainActualRiskId, x => x.TSOServiceDeliveryChain, x => x.TSOServiceDeliveryChain.ServiceDeliveryChain, x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR, x => x.ActualOperationalRisk, x => x.ActualOperationalRiskIndicator, x => x.ResponsiblePerson, x => x.RaisedBy, x => x.Status);
                objBusinessLayer = new BusinessLayer(ls);
                // TSOServiceDeliveryChainPlannedRisk objPlannedTaskRisk = objBusinessLayer.TSOServiceDeliveryChainPlannedRiskRepository.GetSingle(x => x.TSOServiceDeliveryChainId == objActualTaskRisk.TSOServiceDeliveryChainId && x.WeekNumber == objActualTaskRisk.WeekNumber + 1 && x.PlannedOperationalRiskIndicatorId == objActualTaskRisk.ActualOperationalRiskIndicatorId, x => x.PlannedOperationalRisk, x => x.PlannedOperationalRiskIndicator);
                TSOServiceDeliveryChainActualRiskDTO objActualRiskDTO = Conversions.ToDTO<TSOServiceDeliveryChainActualRiskDTO, TSOServiceDeliveryChainActualRisk>(objActualTaskRisk);
                //TSOServiceDeliveryChainPlannedRiskDTO objPlannedRiskDTO = Conversions.ToDTO<TSOServiceDeliveryChainPlannedRiskDTO, TSOServiceDeliveryChainPlannedRisk>(objPlannedTaskRisk);

                objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetByID(objActualRiskDTO.RaisedById);
                var objUserDTO = Conversions.ToDTO<UserDTO, User>(objUser);

                RiskStatus objStatus = objBusinessLayer.RiskStatusRepository.GetByID(objActualRiskDTO.StatusId);
                var objStatusDTO = Conversions.ToDTO<RiskStatusDTO, RiskStatus>(objStatus);

                objRiskDTO.ID = objActualRiskDTO.ID;
                objRiskDTO.ActualOperationalRisk = objActualRiskDTO.ActualOperationalRisk;
                objRiskDTO.ActualOperationalRiskDescription = objActualRiskDTO.ActualOperationalRiskDescription;
                objRiskDTO.ActualOperationalRiskId = objActualRiskDTO.ActualOperationalRiskId;
                objRiskDTO.ActualOperationalRiskIndicatorId = objActualRiskDTO.ActualOperationalRiskIndicatorId;
                objRiskDTO.ActualOperationalRiskIndicator = objActualRiskDTO.ActualOperationalRiskIndicator;
                objRiskDTO.ActualOperationalRiskMitigation = objActualRiskDTO.ActualOperationalRiskMitigation;
                objRiskDTO.DueDate = objActualRiskDTO.DueDate;
                objRiskDTO.ResponsiblePerson = objActualRiskDTO.ResponsiblePerson;
                objRiskDTO.ResponsiblePersonId = objActualRiskDTO.ResponsiblePersonId;
                objRiskDTO.TSOServiceDeliveryChain = objActualRiskDTO.TSOServiceDeliveryChain;
                objRiskDTO.TSOServiceDeliveryChainId = objActualRiskDTO.TSOServiceDeliveryChainId;

                //objRiskDTO.PlannedRiskId = objPlannedRiskDTO.ID;
                //objRiskDTO.PlannedOperationalRiskId = objPlannedRiskDTO.PlannedOperationalRiskId;
                //objRiskDTO.PlannedOperationalRisk = objPlannedRiskDTO.PlannedOperationalRisk;
                //objRiskDTO.PlannedOperationalRiskIndicatorId = objPlannedRiskDTO.PlannedOperationalRiskIndicatorId;
                //objRiskDTO.PlannedOperationalRiskIndicator = objPlannedRiskDTO.PlannedOperationalRiskIndicator;
                //objRiskDTO.PlannedOperationalRiskDescription = objPlannedRiskDTO.PlannedOperationalRiskDescription;
                //objRiskDTO.PlannedOperationalRiskMitigation = objPlannedRiskDTO.PlannedOperationalRiskMitigation;

                objRiskDTO.Impact = objActualRiskDTO.Impact;
                objRiskDTO.Resolution = objActualRiskDTO.Resolution;
                objRiskDTO.RaisedBy = objUserDTO;
                objRiskDTO.RaisedById = objActualRiskDTO.RaisedById;
                objRiskDTO.CreatedOn = objActualRiskDTO.CreatedOn;
                objRiskDTO.Status = objStatusDTO;
                objRiskDTO.StatusId = objActualRiskDTO.StatusId;
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

            return objRiskDTO;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/TSOServiceDeliveryChainRisk/GetLastTaskRisk/{tsoServiceDeliveryChainActualRiskId}")]
        [ResponseType(typeof(TSO))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<RiskDTO> GetLastTaskRisk(int tsoServiceDeliveryChainActualRiskId)
        {
            JsonResult<RiskDTO> result = null;
            // TSOServiceDeliveryChainDTO objTSOServiceDeliveryChainDTO = null;
            RiskDTO objCompleteTask = null;
            try
            {
                objCompleteTask = this.GetLastTSOServiceDeliveryChainRisk(tsoServiceDeliveryChainActualRiskId);
                //TSOServiceDeliveryChainTaskActualDTO objTaskActual = objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainActualTasks.LastOrDefault();
                //TSOServiceDeliveryChainTaskPlannedDTO objTaskPlanned = objTSOServiceDeliveryChainDTO.TSOServiceDeliveryChainPlannedTasks.LastOrDefault();

                //MappedTASKForUI(objTSOServiceDeliveryChainDTO, objCompleteTask, objTaskActual, objTaskPlanned);

                if (null != objCompleteTask)
                {
                    result = Json(objCompleteTask, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
                else
                {
                    objCompleteTask = new RiskDTO();

                    result = Json(objCompleteTask, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return result;
        }

        /// <summary>
        /// GetTASKCount
        /// </summary>
        /// <param name="strTitleOrClient"></param>
        /// <param name="status"></param>
        /// <param name="tsoId"></param>
        /// <returns></returns>
        [HttpGet, Route("v1/TSOServiceDeliveryChainRisk/GetTASKRiskCount/{strTitleOrClient}/{status}/{taskid}")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<string> GetTASKRiskCount(string strTitleOrClient, int status, int taskid = 0)
        {
            JsonResult<string> result = Json(string.Empty);
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                // IList<TSOServiceDeliveryChain> objTask = null;
                IList<TSOServiceDeliveryChainActualRisk> objTask = null;
                List<TSOServiceDeliveryChain> taskList = new List<TSOServiceDeliveryChain>();
                List<TSOServiceDeliveryChainActualRisk> ActualRiskList = new List<TSOServiceDeliveryChainActualRisk>();
                Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
                int week = GetWeek();
                string userId = Request.Headers.GetValues("userid").FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(userId))
                {
                    userId = userId.Replace("~", "=").Replace("!", "+");
                    string User = objCryptorEngine.Decrypt(userId, true);
                    bool isadmin = User.ToLower().Contains("admin");
                    string[] usrInfo = User.Split('#');
                    User = usrInfo[usrInfo.Length - 2];
                    int userID = Convert.ToInt32(User);

                    if (taskid > 0)
                    {
                        //objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.ID == taskid,
                        //    x => x.ServiceDeliveryChain, x => x.TSOServiceDeliveryChainActualRisks, x => x.TSOServiceDeliveryChainPlannedRisks,
                        //    x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR, x => x.TSO.TSR.SolutionCentre, x => x.TSO.TSR.Practice);

                        //objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetList(x => x.ID == taskid && x.WeekNumber==week,
                        //    x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,
                        //    x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR, x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre, x => x.TSOServiceDeliveryChain.TSO.TSR.Practice);

                        objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetList(x => x.ID == taskid,
                               x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,
                               x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR, x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre, x => x.TSOServiceDeliveryChain.TSO.TSR.Practice).OrderByDescending(x => x.WeekNumber).ToList();
                        //if (objTask != null)
                        //{
                        //    objBusinessLayer = new BusinessLayer(ls);
                        //    foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in objTask)
                        //    {
                        //        //var actualTask = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.OrderBy(x => x.WeekNumber).LastOrDefault();
                        //        //var plannedTask = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.OrderBy(x => x.WeekNumber + 1).LastOrDefault();
                        //        //objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.Clear();
                        //        //objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.Clear();
                        //        //objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualTasks.Add(actualTask);
                        //        //objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedTasks.Add(plannedTask);                              
                        //        List<TSOServiceDeliveryChainActualRisk> objTSOServiceDeliveryChainTaskActual = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualRisks.Where(x => x.WeekNumber == week).ToList();
                        //        List<TSOServiceDeliveryChainPlannedRisk> objTSOServiceDeliveryChainTaskPlanned = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedRisks.OrderBy(x => x.WeekNumber == week + 1).ToList();
                        //    }
                        //}

                        //foreach (var item in objTask)
                        //{
                        //    ActualRiskList.AddRange(item.TSOServiceDeliveryChainActualRisks);
                        //}
                    }
                    else
                    {
                        objBusinessLayer = new BusinessLayer(ls);
                        User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);
                        //Note - Currently we are showing all record in Risk dashboard as per discussion
                        if (objUser.Role.Name == "Admin" || objUser.Role.Name == "Guest")
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            //objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetAll(
                            //             x => x.ServiceDeliveryChain, x => x.TSOServiceDeliveryChainActualRisks, x => x.TSOServiceDeliveryChainPlannedRisks, x => x.TSO,
                            //             x => x.TSO.TSR.Client, x => x.TSO.TSR, x => x.TSO.TSR.SolutionCentre, x => x.TSO.TSR.Practice);

                            //objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetList(x=>x.WeekNumber==week,
                            //             x => x.TSOServiceDeliveryChain, x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,x => x.TSOServiceDeliveryChain.TSO,
                            //              x => x.TSOServiceDeliveryChain.TSO.TSR, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre, x => x.TSOServiceDeliveryChain.TSO.TSR.Practice);

                            objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetAll(x => x.TSOServiceDeliveryChain, x => x.TSOServiceDeliveryChain.ServiceDeliveryChain, x => x.TSOServiceDeliveryChain.TSO,
                                             x => x.TSOServiceDeliveryChain.TSO.TSR, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre, x => x.TSOServiceDeliveryChain.TSO.TSR.Practice).OrderByDescending(x => x.WeekNumber).ToList();

                        }
                        else
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            //objTask = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSO.TSR.AccountManagerId == userID || x.TSO.TSR.DeliveryManagerId == userID || x.TSO.TSR.TestManagerId == userID || x.TSO.TeamLeadId == userID,
                            //                                         x => x.ServiceDeliveryChain,
                            //                                         x => x.TSOServiceDeliveryChainActualRisks,
                            //                                         x => x.TSOServiceDeliveryChainPlannedRisks,
                            //                                         x => x.TSO, x => x.TSO.TSR.Client, x => x.TSO.TSR,
                            //                                         x => x.TSO.TSR.SolutionCentre, x => x.TSO.TSR.Practice);

                            //objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetList(x => x.TSOServiceDeliveryChain.TSO.TSR.AccountManagerId == userID || x.TSOServiceDeliveryChain.TSO.TSR.DeliveryManagerId == userID || x.TSOServiceDeliveryChain.TSO.TSR.TestManagerId == userID || x.TSOServiceDeliveryChain.TSO.TeamLeadId == userID && x.WeekNumber==week,
                            //                                         x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,                                                                   
                            //                                         x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR,
                            //                                         x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre, x => x.TSOServiceDeliveryChain.TSO.TSR.Practice);

                            objTask = objBusinessLayer.TSOServiceDeliveryChainActualRiskRepository.GetList(x => x.TSOServiceDeliveryChain.TSO.TSR.AccountManagerId == userID || x.TSOServiceDeliveryChain.TSO.TSR.DeliveryManagerId == userID || x.TSOServiceDeliveryChain.TSO.TSR.TestManagerId == userID || x.TSOServiceDeliveryChain.TSO.TeamLeadId == userID,
                                                                    x => x.TSOServiceDeliveryChain.ServiceDeliveryChain,
                                                                    x => x.TSOServiceDeliveryChain.TSO, x => x.TSOServiceDeliveryChain.TSO.TSR.Client, x => x.TSOServiceDeliveryChain.TSO.TSR,
                                                                    x => x.TSOServiceDeliveryChain.TSO.TSR.SolutionCentre, x => x.TSOServiceDeliveryChain.TSO.TSR.Practice).OrderByDescending(x => x.WeekNumber).ToList();
                        }
                        //taskList = objTask.Where(x => x.ServiceDeliveryChainId == 42).ToList();
                        //foreach (TSOServiceDeliveryChain objTSOServiceDeliveryChain in taskList)
                        //{

                        //    List<TSOServiceDeliveryChainActualRisk> objTSOServiceDeliveryChainTaskActual = objTSOServiceDeliveryChain.TSOServiceDeliveryChainActualRisks.Where(x => x.WeekNumber == week).ToList();
                        //    List<TSOServiceDeliveryChainPlannedRisk> objTSOServiceDeliveryChainTaskPlanned = objTSOServiceDeliveryChain.TSOServiceDeliveryChainPlannedRisks.OrderBy(x => x.WeekNumber == week + 1).ToList();
                        //}

                        //foreach (var item in objTask)
                        //{
                        //    ActualRiskList.AddRange(item.TSOServiceDeliveryChainActualRisks);
                        //}

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

        private int GetWeek()
        {
            DateTime inputDate = DateTime.Now;
            var d = inputDate;
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(
                d,
                CalendarWeekRule.FirstDay,
                DayOfWeek.Monday);

            return weekNum;
        }

        [HttpGet, Route("v1/TSOServiceDeliveryChainRisk/GetRisksDump")]
        [ResponseType(typeof(HttpResponseMessage))]
        //[SessionAuthorize(Roles = "Admin")]
        [SessionAuthorize(Roles = "Admin,Delivery Manager,Account Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public HttpResponseMessage GetRisksDump()
        {
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            var riskTable = objBusinessLayer.TSOServiceDeliveryChainRepository.ExecuteStoredProcedure("sp_GetRiskData");
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                if (null != riskTable)
                {
                    StringBuilder sb = new StringBuilder();
                    //Set the Headings and Orders    
                    sb.Append("ID").Append(",");
                    sb.Append("Task Title").Append(",");
                    sb.Append("Related TSR").Append(",");
                    sb.Append("TSR Name").Append(",");
                    sb.Append("Related TSO").Append(",");
                    sb.Append("TSO Name").Append(",");
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
                    sb.Append("Planned Start Date").Append(",");
                    sb.Append("Planned Completion Date").Append(",");
                    sb.Append("Category").Append(",");
                    sb.Append("Severity").Append(",");

                    sb.Append("Description").Append(",");
                    sb.Append("Mitigation").Append(",");
                    sb.Append("Impact").Append(",");
                    sb.Append("Resolution").Append(",");
                    sb.Append("DueDate").Append(",");
                    sb.Append("Responsible Person").Append(",");
                    sb.Append("Raised By").Append(",");
                    sb.Append("Risk Status").Append(",");
                    sb.Append("Account Manager").Append(",");
                    sb.Append("Delivery manager").Append(",");
                    sb.Append("Test Manager").Append(",");
                    sb.Append("Team Lead").Append(",");
                    sb.Append("Vertical").Append(",");
                    sb.Append("Practice").Append(",");
                    sb.Append("Modified").Append(",");
                    sb.Append("Modified By").Append(",");
                    sb.Append("TSR Status").Append(",");
                    sb.Append("TSO Status").Append(",");
                    sb.Append("TSR Operational Risk").Append(",");
                    sb.Append("TSO Operational Risk").Append(",");
                    sb.Append("Week Number").Append(",");
                    sb.Append("Year");
                    sb.Append("\n");

                    for (int taskTableRow = 0; taskTableRow < riskTable.Rows.Count; taskTableRow++)
                    {
                        // Append data 
                        sb.Append(string.Format("=\"{0}\"", riskTable.Rows[taskTableRow]["ID"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["Task_Title"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Task_Title"].ToString() + "( " + riskTable.Rows[taskTableRow]["Task_Description"].ToString() + " )")).Append(",");
                        sb.Append(string.Format("=\"{0}\"", riskTable.Rows[taskTableRow]["Related_TSR"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["TSR_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["TSR_Name"].ToString())).Append(",");
                        sb.Append(string.Format("=\"{0}\"", riskTable.Rows[taskTableRow]["Related_TSO"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["TSO_Title"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["TSO_Title"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["TaskStatus"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["TaskStatus"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["Client_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Client_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["Engagement_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Engagement_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["PricingModel_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["PricingModel_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["ProjectModel_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["ProjectModel_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["ClientRegion_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["ClientRegion_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["Account_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Account_Name"].ToString())).Append(",");
                        long lngERPNumner;
                        bool isNumeric = long.TryParse(riskTable.Rows[taskTableRow]["ERPordernumber"].ToString(), out lngERPNumner);
                        if (!isNumeric)
                        {
                            sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["ERPordernumber"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["ERPordernumber"].ToString())).Append(",");
                        }
                        else
                        {
                            sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["ERPordernumber"].ToString()) ? string.Empty : string.Format("=\"{0}\"", riskTable.Rows[taskTableRow]["ERPordernumber"].ToString())).Append(",");
                        }
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["SolutionCenter_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["SolutionCenter_Name"].ToString())).Append(",");
                        if (riskTable.Rows[taskTableRow]["CoreServiceName"].ToString() == null)
                            sb.Append(string.Empty).Append(",");
                        else
                        {
                            sb.Append(string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["CoreServiceName"].ToString()));
                            sb.Append(",");
                        }
                        sb.Append(riskTable.Rows[taskTableRow]["Method"].ToString() == null ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Method"].ToString())).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["PlannedStartDate"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["PlannedCompletionDate"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["OperationalRiskIndicatorName"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["Severity"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["ActualOperationalRiskDescription"].ToString() == null ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["ActualOperationalRiskDescription"].ToString())).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["ActualOperationalRiskMitigation"].ToString() == null ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["ActualOperationalRiskMitigation"].ToString())).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["Impact"].ToString() == null ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Impact"].ToString())).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["Resolution"].ToString() == null ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Resolution"].ToString())).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["DueDate"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["ResponsiblePerson"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["ResponsiblePerson"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["RaisedBy"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["RaisedBy"].ToString())).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["Risk_Status"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["AccountManager_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["AccountManager_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["DeliveryManager_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["DeliveryManager_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["TestManager_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["TestManager_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["TeamLead_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["TeamLead_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["Vertical_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Vertical_Name"].ToString())).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["Practice_Name"].ToString()) ? string.Empty : string.Format("\"{0}\"", riskTable.Rows[taskTableRow]["Practice_Name"].ToString())).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["UpdatedOn"].ToString()).Append(",");
                        sb.Append(string.IsNullOrEmpty(riskTable.Rows[taskTableRow]["UpdatedBy"].ToString()) ? string.Empty : riskTable.Rows[taskTableRow]["UpdatedBy"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["TSR_Status"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["TSO_Status"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["TSO_OperationalRisk"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["TSR_OperationalRisk"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["WeekNumber"].ToString()).Append(",");
                        sb.Append(riskTable.Rows[taskTableRow]["Year"].ToString());
                        sb.Append("\n");
                    }

                    result.Content = new StringContent(sb.ToString(), Encoding.UTF8);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment"); //attachment will force download
                    result.Content.Headers.ContentDisposition.FileName = "RiskExport.csv";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(ex.Message);
                throw ex;
            }
            return result;
        }
    }
}
