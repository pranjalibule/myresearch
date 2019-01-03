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
    using RoleAttribute;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    /// <summary>
    /// RiskStatusController
    /// </summary>
    [RoutePrefix("api")]
    public class RiskStatusController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetRiskStatusById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>RiskStatus</returns>
        private RiskStatusDTO GetRiskStatusById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            RiskStatusDTO objRiskStatusDTO = null;

            try
            {
                RiskStatus objRiskStatus = objBusinessLayer.RiskStatusRepository.GetByID(id);
                if (null != objRiskStatus)
                {
                    objRiskStatusDTO = new RiskStatusDTO();
                    objRiskStatusDTO = Conversions.ToDTO<RiskStatusDTO, RiskStatus>(objRiskStatus);
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

            return objRiskStatusDTO;
        }

        /// <summary>
        /// GetAllRiskStatus
        /// </summary>
        /// <returns>IList<RiskStatus>></returns>
        private IList<RiskStatusDTO> GetAllRiskStatus(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<RiskStatusDTO> objRiskStatusDTOList = null;
            try
            {
                IList<RiskStatus> objRiskStatusList = objBusinessLayer.RiskStatusRepository.GetAll(startingRecordNumber, pageSize, x => x.DisplayOrder, false, out totalRecords);
                if (objRiskStatusList != null && objRiskStatusList.Count > 0)
                {
                    objRiskStatusDTOList = new List<RiskStatusDTO>();

                    foreach (RiskStatus objRiskStatus in objRiskStatusList)
                    {
                        RiskStatusDTO objRiskStatusDTO = new RiskStatusDTO();
                        objRiskStatusDTO = Conversions.ToDTO<RiskStatusDTO, RiskStatus>(objRiskStatus);

                        objRiskStatusDTOList.Add(objRiskStatusDTO);
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

            return objRiskStatusDTOList;
        }

        /// <summary>
        /// Function to add new RiskStatus 
        /// </summary>
        /// <param name="objRiskStatus"></param>
        /// <returns></returns>
        private string AddNewRiskStatus(RiskStatusDTO objRiskStatusDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objRiskStatusDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objRiskStatusDTO.Name, null);
                if (result != true)
                {
                    RiskStatus objRiskStatus = Conversions.ToEntity<RiskStatusDTO, RiskStatus>(objRiskStatusDTO);
                    objBusinessLayer.RiskStatusRepository.Add(objRiskStatus);
                    returnMessage = "Risk Status added successfully.";
                }
                else
                {
                    returnMessage = "This " + objRiskStatusDTO.Name + " Risk Status already exists.";
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
            IList<RiskStatus> CheckNameExists = null;

            if (null != ID)
            {
                CheckNameExists = objBusinessLayer.RiskStatusRepository.GetList(x => x.Name == Name && x.ID == ID);
            }
            else
            {
                CheckNameExists = objBusinessLayer.RiskStatusRepository.GetList(x => x.Name == Name);
            }

            if (CheckNameExists.Count >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Function to update RiskStatus 
        /// </summary>
        /// <param name="objRiskStatus"></param>
        /// <returns>string</returns>
        private string ModifyRiskStatus(int ID, RiskStatusDTO objRiskStatusDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                RiskStatusDTO objRiskStatusDTOById = this.GetRiskStatusById(ID);

                if (objRiskStatusDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objRiskStatusDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objRiskStatusDTO.Name, ID);
                    if (result == true)
                    {
                        objRiskStatusDTOById.Name = objRiskStatusDTO.Name;
                        objRiskStatusDTOById.Description = objRiskStatusDTO.Description;
                        objRiskStatusDTOById.DisplayOrder = objRiskStatusDTO.DisplayOrder;
                        returnMessage = "Risk Status updated successfully.";
                    }
                    else
                    {
                        objRiskStatusDTOById.Description = objRiskStatusDTO.Description;
                        returnMessage = "This " + objRiskStatusDTO.Name + " Risk Status already exists.";
                    }
                    RiskStatus objRiskStatus = Conversions.ToEntity<RiskStatusDTO, RiskStatus>(objRiskStatusDTOById);

                    //objRiskStatus = Conversions.ToEntity<RiskStatusDTO, RiskStatus>(objRiskStatusDTO, objRiskStatus);
                    objBusinessLayer.RiskStatusRepository.Update(objRiskStatus);


                }
                else
                {
                    returnMessage = "Risk Status do not exists.";
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
        /// DeleteRiskStatus
        /// </summary>
        /// <param name="objRiskStatusDTO">RiskStatusDTO</param>
        private string RemoveRiskStatus(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                RiskStatusDTO objRiskStatusDTOById = this.GetRiskStatusById(ID);

                if (objRiskStatusDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    RiskStatus objRiskStatus = Conversions.ToEntity<RiskStatusDTO, RiskStatus>(objRiskStatusDTOById);
                    objBusinessLayer.RiskStatusRepository.Delete(objRiskStatus);
                    returnMessage = "Risk Status deleted successfully.";

                }
                else
                {
                    returnMessage = "Risk Status do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified Risk Status mapped with Task.");
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
        /// GetRiskStatusIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetRiskStatusIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objRiskStatusList = null;
            try
            {
                objRiskStatusList = objBusinessLayer.RiskStatusRepository.GetPartial(x => new IDName { ID = x.ID, Name = x.Name }, x => x.DisplayOrder, null);
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

            return objRiskStatusList;
        }

        #endregion

        [HttpGet, Route("v1/RiskStatus/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objRiskStatusList = null;

            try
            {
                objRiskStatusList = this.GetRiskStatusIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objRiskStatusList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/RiskStatus/GetById/{id}")]
        [ResponseType(typeof(RiskStatus))]
        public JsonResult<RiskStatusDTO> GetById(int id)
        {
            JsonResult<RiskStatusDTO> result = null;
            RiskStatusDTO objRiskStatusDTO;

            try
            {
                objRiskStatusDTO = this.GetRiskStatusById(id);

                result = Json(objRiskStatusDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpGet, Route("v1/RiskStatus/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        // comment : When we come to Risk Status html page, error shown for userid. need to work on it.
        //[SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,Team Lead,PQM,Guest")]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<RiskStatusDTO> objRiskStatusDTOList = this.GetAllRiskStatus(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objRiskStatusDTOList)
                {
                    foreach (RiskStatusDTO objRiskStatus in objRiskStatusDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objRiskStatus);
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
        /// CreateRiskStatus
        /// </summary>
        /// <param name="RiskStatusDTO">RiskStatusDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/RiskStatus/CreateRiskStatus")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin")]
        public JsonResult<string> CreateRiskStatus([FromBody]RiskStatusDTO RiskStatusDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewRiskStatus(RiskStatusDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateRiskStatus
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="userTypeDTO">UserTypeDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/RiskStatus/UpdateRiskStatus/{ID}")]
        [SessionAuthorize(Roles = "Admin,Test Manager,TMO,Team Lead")]
        public JsonResult<string> UpdateRiskStatus(int ID, [FromBody]RiskStatusDTO RiskStatusDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyRiskStatus(ID, RiskStatusDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteRiskStatus
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/RiskStatus/DeleteRiskStatus/{ID}")]
        [SessionAuthorize(Roles = "Admin")]
        public JsonResult<string> DeleteRiskStatus(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveRiskStatus(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
