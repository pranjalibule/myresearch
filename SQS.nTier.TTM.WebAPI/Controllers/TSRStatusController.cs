/******************************************************************************
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
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    /// <summary>
    /// TSRStatusController
    /// </summary>
    [RoutePrefix("api")]
    public class TSRStatusController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetTSRStatusById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>TSRStatus</returns>
        private TSRStatusDTO GetTSRStatusById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            TSRStatusDTO objTSRStatusDTO = null;

            try
            {
                TSRStatus objTSRStatus = objBusinessLayer.TSRStatusRepository.GetByID(id);
                if (null != objTSRStatus)
                {
                    objTSRStatusDTO = new TSRStatusDTO();
                    objTSRStatusDTO = Conversions.ToDTO<TSRStatusDTO, TSRStatus>(objTSRStatus);
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

            return objTSRStatusDTO;
        }

        /// <summary>
        /// GetAllTSRStatus
        /// </summary>
        /// <returns>IList<TSRStatus>></returns>
        private IList<TSRStatusDTO> GetAllTSRStatus(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TSRStatusDTO> objTSRStatusDTOList = null;
            try
            {
                IList<TSRStatus> objTSRStatusList = objBusinessLayer.TSRStatusRepository.GetAll(startingRecordNumber, pageSize, x => x.DisplayOrder, false, out totalRecords);
                if (objTSRStatusList != null && objTSRStatusList.Count > 0)
                {
                    objTSRStatusDTOList = new List<TSRStatusDTO>();

                    foreach (TSRStatus objTSRStatus in objTSRStatusList)
                    {
                        TSRStatusDTO objTSRStatusDTO = new TSRStatusDTO();
                        objTSRStatusDTO = Conversions.ToDTO<TSRStatusDTO, TSRStatus>(objTSRStatus);

                        objTSRStatusDTOList.Add(objTSRStatusDTO);
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

            return objTSRStatusDTOList;
        }

        /// <summary>
        /// Function to add new TSRStatus 
        /// </summary>
        /// <param name="objTSRStatusDTO">TSRStatusDTO</param>
        /// <returns>string</returns>
        private string AddNewTSRStatus(TSRStatusDTO objTSRStatusDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objTSRStatusDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objTSRStatusDTO.Name, null);
                if (result == true)
                {
                    TSRStatus objTSRStatus = Conversions.ToEntity<TSRStatusDTO, TSRStatus>(objTSRStatusDTO);
                    objBusinessLayer.TSRStatusRepository.Add(objTSRStatus);
                    returnMessage = "TSRStatus added successfully.";
                }
                else
                {
                    returnMessage = "This " + objTSRStatusDTO.Name + " TSR Status already exists.";
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
            var CheckNameExists = objBusinessLayer.TSRStatusRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update TSRStatus 
        /// </summary>
        /// <param name="objTSRStatusDTO">TSRStatusDTO</param>
        /// <returns>string</returns>
        private string ModifyTSRStatus(int ID, TSRStatusDTO objTSRStatusDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                TSRStatusDTO objTSRStatusDTOById = this.GetTSRStatusById(ID);

                if (objTSRStatusDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objTSRStatusDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objTSRStatusDTO.Name, ID);
                    if (result == true)
                    {
                        objTSRStatusDTOById.Name = objTSRStatusDTO.Name;
                        objTSRStatusDTOById.Description = objTSRStatusDTO.Description;
                        returnMessage = "TSRStatus updated successfully.";
                    }
                    else
                    {
                        objTSRStatusDTOById.Description = objTSRStatusDTO.Description;
                        returnMessage = "This " + objTSRStatusDTO.Name + " TSR Status already exists.";
                    }
                    TSRStatus objTSRStatus = Conversions.ToEntity<TSRStatusDTO, TSRStatus>(objTSRStatusDTOById);

                    objBusinessLayer.TSRStatusRepository.Update(objTSRStatus);


                }
                else
                {
                    returnMessage = "TSRStatus do not exists.";
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
        /// RemoveTSRStatus
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>string</returns>
        private string RemoveTSRStatus(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                TSRStatusDTO objTSRStatusDTOById = this.GetTSRStatusById(ID);

                if (objTSRStatusDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    TSRStatus objTSRStatus = Conversions.ToEntity<TSRStatusDTO, TSRStatus>(objTSRStatusDTOById);
                    objBusinessLayer.TSRStatusRepository.Delete(objTSRStatus);
                    returnMessage = "TSRStatus deleted successfully.";
                }
                else
                {
                    returnMessage = "TSRStatus do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified tsr status mapped with TSR / TSO.");
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
        /// GetTSRStatusIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetTSRStatusIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objTSRStatusList = null;
            try
            {
                objTSRStatusList = objBusinessLayer.TSRStatusRepository.GetPartial(x => new IDName { ID = x.ID, Name = x.Name }, x => x.DisplayOrder, null);
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

            return objTSRStatusList;
        }

        #endregion

        [HttpGet, Route("v1/TSRStatus/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objTSRStatusList = null;

            try
            {
                objTSRStatusList = this.GetTSRStatusIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objTSRStatusList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/TSRStatus/GetById/{id}")]
        [ResponseType(typeof(TSRStatus))]
        public JsonResult<TSRStatusDTO> GetById(int id)
        {
            JsonResult<TSRStatusDTO> result = null;
            TSRStatusDTO objTSRStatusDTO;

            try
            {
                objTSRStatusDTO = this.GetTSRStatusById(id);

                result = Json(objTSRStatusDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/TSRStatus/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<TSRStatusDTO> objTSRStatusDTOList = this.GetAllTSRStatus(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objTSRStatusDTOList)
                {
                    foreach (TSRStatusDTO objTSRStatus in objTSRStatusDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objTSRStatus);
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
        /// CreateTSRStatus
        /// </summary>
        /// <param name="tsrStatusDTO">TSRStatusDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/TSRStatus/CreateTSRStatus")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateTSRStatus([FromBody]TSRStatusDTO tsrStatusDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewTSRStatus(tsrStatusDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateTSRStatus
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="tsrStatusDTO">TSRStatusDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/TSRStatus/UpdateTSRStatus/{ID}")]
        [ResponseType(typeof(string))]
        public JsonResult<string> UpdateTSRStatus(int ID, [FromBody]TSRStatusDTO tsrStatusDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyTSRStatus(ID, tsrStatusDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteTSRStatus
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/TSRStatus/DeleteTSRStatus/{ID}")]
        public JsonResult<string> DeleteTSRStatus(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.RemoveTSRStatus(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// <param name="disposing">bool</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
