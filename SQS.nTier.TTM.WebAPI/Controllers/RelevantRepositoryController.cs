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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    /// <summary>
    /// RelevantRepositoryController
    /// </summary>
    [RoutePrefix("api")]
    public class RelevantRepositoryController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetRelevantRepositoryById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>RelevantRepository</returns>
        private RelevantRepositoryDTO GetRelevantRepositoryById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            RelevantRepositoryDTO objRelevantRepositoryDTO = null;

            try
            {
                RelevantRepository objRelevantRepository = objBusinessLayer.RelevantRepositoryRepository.GetByID(id);
                if (null != objRelevantRepository)
                {
                    objRelevantRepositoryDTO = new RelevantRepositoryDTO();
                    objRelevantRepositoryDTO = Conversions.ToDTO<RelevantRepositoryDTO, RelevantRepository>(objRelevantRepository);
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

            return objRelevantRepositoryDTO;
        }

        /// <summary>
        /// GetAllRelevantRepository
        /// </summary>
        /// <returns>IList<RelevantRepository>></returns>
        private IList<RelevantRepositoryDTO> GetAllRelevantRepository(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<RelevantRepositoryDTO> objRelevantRepositoryDTOList = null;
            try
            {
                IList<RelevantRepository> objRelevantRepositoryList = objBusinessLayer.RelevantRepositoryRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objRelevantRepositoryList != null && objRelevantRepositoryList.Count > 0)
                {
                    objRelevantRepositoryDTOList = new List<RelevantRepositoryDTO>();

                    foreach (RelevantRepository objRelevantRepository in objRelevantRepositoryList)
                    {
                        RelevantRepositoryDTO objRelevantRepositoryDTO = new RelevantRepositoryDTO();
                        objRelevantRepositoryDTO = Conversions.ToDTO<RelevantRepositoryDTO, RelevantRepository>(objRelevantRepository);

                        objRelevantRepositoryDTOList.Add(objRelevantRepositoryDTO);
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

            return objRelevantRepositoryDTOList;
        }

        /// <summary>
        /// Function to add new RelevantRepository 
        /// </summary>
        /// <param name="objRelevantRepositoryDTO">RelevantRepositoryDTO</param>
        /// <returns>string</returns>
        private string AddNewRelevantRepository(RelevantRepositoryDTO objRelevantRepositoryDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objRelevantRepositoryDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objRelevantRepositoryDTO.Name, null);
                if (result == true)
                {
                    RelevantRepository objRelevantRepository = Conversions.ToEntity<RelevantRepositoryDTO, RelevantRepository>(objRelevantRepositoryDTO);
                    objBusinessLayer.RelevantRepositoryRepository.Add(objRelevantRepository);
                    returnMessage = "Relevant Repository added successfully.";
                }
                else
                {
                    returnMessage = "This " + objRelevantRepositoryDTO.Name + " record already exists.";
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
        private bool CheckNameExists(string Name, int? ID)
        {
            LoginSession ls = new LoginSession();
            ls.LoginName = "Admin";
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            var CheckNameExists = objBusinessLayer.RelevantRepositoryRepository.GetList(x => x.Name == Name && x.ID == ID);
            if (CheckNameExists.Count <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Function to update RelevantRepository 
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="objRelevantRepositoryDTO">RelevantRepositoryDTO</param>
        /// <returns>string</returns>
        private string ModifyRelevantRepository(int ID, RelevantRepositoryDTO objRelevantRepositoryDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                RelevantRepositoryDTO objRelevantRepositoryDTOById = this.GetRelevantRepositoryById(ID);

                if (objRelevantRepositoryDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objRelevantRepositoryDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objRelevantRepositoryDTO.Name, ID);
                    if (result == true)
                    {
                        objRelevantRepositoryDTOById.Name = objRelevantRepositoryDTO.Name;
                        objRelevantRepositoryDTOById.Description = objRelevantRepositoryDTO.Description;
                        returnMessage = "Relevant Repository updated successfully.";
                    }
                    else
                    {
                        objRelevantRepositoryDTOById.Description = objRelevantRepositoryDTO.Description;
                        returnMessage = "This " + objRelevantRepositoryDTO.Name + " record already exists.";
                    }
                    RelevantRepository objRelevantRepository = Conversions.ToEntity<RelevantRepositoryDTO, RelevantRepository>(objRelevantRepositoryDTOById);

                    //objRelevantRepository = Conversions.ToEntity<RelevantRepositoryDTO, RelevantRepository>(objRelevantRepositoryDTO, objRelevantRepository);
                    objBusinessLayer.RelevantRepositoryRepository.Update(objRelevantRepository);


                }
                else
                {
                    returnMessage = "Relevant Repository do not exists.";
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
        /// RemoveRelevantRepository
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>string</returns>
        private string RemoveRelevantRepository(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                RelevantRepositoryDTO objRelevantRepositoryDTOById = this.GetRelevantRepositoryById(ID);

                if (objRelevantRepositoryDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    RelevantRepository objRelevantRepository = Conversions.ToEntity<RelevantRepositoryDTO, RelevantRepository>(objRelevantRepositoryDTOById);
                    objBusinessLayer.RelevantRepositoryRepository.Delete(objRelevantRepository);
                    returnMessage = "Relevant Repository deleted successfully.";

                }
                else
                {
                    returnMessage = "Relevant Repository do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified relevant repository mapped with TSR / TSO.");
                else
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

        #endregion

        #region internal functions

        /// <summary>
        /// GetRelevantRepositoryIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetRelevantRepositoryIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objRelevantRepositoryList = null;
            try
            {
                objRelevantRepositoryList = objBusinessLayer.RelevantRepositoryRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

            return objRelevantRepositoryList;
        }

        /// <summary>
        /// GetRelevantRepositoryIDNameList
        /// </summary>
        /// <param name="tsrId"></param>
        /// <returns></returns>
        private IList<IDName> GetRelevantRepositoryIDNameList(int tsrId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objRelevantRepositoryIDNameList = null;

            IList<RelevantRepository> objRelevantRepositoryList = null;
            try
            {
                objRelevantRepositoryList = objBusinessLayer.RelevantRepositoryRepository.GetList(x => x.TSTReleventRepositories.Any(y => y.TSRId == tsrId));

                if (null != objRelevantRepositoryList && objRelevantRepositoryList.Count > 0)
                {
                    objRelevantRepositoryIDNameList = new List<IDName>();

                    objRelevantRepositoryIDNameList = objRelevantRepositoryList.Select(x => new IDName { ID = x.ID, Name = x.Name }).ToList<IDName>();
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

            return objRelevantRepositoryIDNameList;
        }

        #endregion

        [HttpGet, Route("v1/RelevantRepository/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objRelevantRepositoryList = null;

            try
            {
                objRelevantRepositoryList = this.GetRelevantRepositoryIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objRelevantRepositoryList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        [HttpGet, Route("v1/RelevantRepository/GetAllIDNameByTSRId/{tsrId}")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDNameByTSRId(int tsrId)
        {
            IList<IDName> objCoreServiceList = null;

            try
            {
                objCoreServiceList = this.GetRelevantRepositoryIDNameList(tsrId);
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw ex;
            }
            return Json(objCoreServiceList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/RelevantRepository/GetById/{id}")]
        [ResponseType(typeof(RelevantRepository))]
        public JsonResult<RelevantRepositoryDTO> GetById(int id)
        {
            JsonResult<RelevantRepositoryDTO> result = null;
            RelevantRepositoryDTO objRelevantRepositoryDTO;

            try
            {
                objRelevantRepositoryDTO = this.GetRelevantRepositoryById(id);

                result = Json(objRelevantRepositoryDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return result;
        }

        /// <summary>
        /// GetAllPaged
        /// </summary>
        /// <param name="pageSize">int</param>
        /// <param name="startingRecordNumber">int</param>
        /// <returns>JsonResult<DataCollection>></returns>
        [HttpGet, Route("v1/RelevantRepository/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<RelevantRepositoryDTO> objRelevantRepositoryDTOList = this.GetAllRelevantRepository(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objRelevantRepositoryDTOList)
                {
                    foreach (RelevantRepositoryDTO objRelevantRepository in objRelevantRepositoryDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objRelevantRepository);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw ex;
            }
            return Json(objDataCollection, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// CreateRelevantRepository
        /// </summary>
        /// <param name="relevantRepositoryDTO">RelevantRepositoryDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/RelevantRepository/CreateRelevantRepository")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateRelevantRepository([FromBody]RelevantRepositoryDTO relevantRepositoryDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewRelevantRepository(relevantRepositoryDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateRelevantRepository
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="relevantRepositoryDTO">RelevantRepositoryDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/RelevantRepository/UpdateRelevantRepository/{ID}")]
        public JsonResult<string> UpdateRelevantRepository(int ID, [FromBody]RelevantRepositoryDTO relevantRepositoryDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyRelevantRepository(ID, relevantRepositoryDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteRelevantRepository
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/RelevantRepository/DeleteRelevantRepository/{ID}")]
        public JsonResult<string> DeleteRelevantRepository(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveRelevantRepository(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">bool</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
