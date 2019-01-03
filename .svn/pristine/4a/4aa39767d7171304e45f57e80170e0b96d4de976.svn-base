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
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    /// <summary>
    /// CoreServiceController
    /// </summary>
    [RoutePrefix("api")]
    public class CoreServiceController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetCoreServiceById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>CoreService</returns>
        private CoreServiceDTO GetCoreServiceById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            CoreServiceDTO objCoreServiceDTO = null;

            try
            {
                CoreService objCoreService = objBusinessLayer.CoreServiceRepository.GetByID(id);
                if (null != objCoreService)
                {
                    objCoreServiceDTO = new CoreServiceDTO();
                    objCoreServiceDTO = Conversions.ToDTO<CoreServiceDTO, CoreService>(objCoreService);
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

            return objCoreServiceDTO;
        }

        /// <summary>
        /// GetAllCoreService
        /// </summary>
        /// <returns>IList<CoreService>></returns>
        private IList<CoreServiceDTO> GetAllCoreService(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<CoreServiceDTO> objCoreServiceDTOList = null;
            try
            {
                IList<CoreService> objCoreServiceList = objBusinessLayer.CoreServiceRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objCoreServiceList != null && objCoreServiceList.Count > 0)
                {
                    objCoreServiceDTOList = new List<CoreServiceDTO>();

                    foreach (CoreService objCoreService in objCoreServiceList)
                    {
                        CoreServiceDTO objCoreServiceDTO = new CoreServiceDTO();
                        objCoreServiceDTO = Conversions.ToDTO<CoreServiceDTO, CoreService>(objCoreService);

                        objCoreServiceDTOList.Add(objCoreServiceDTO);
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

            return objCoreServiceDTOList;
        }

        /// <summary>
        /// Function to add new CoreService 
        /// </summary>
        /// <param name="objCoreService"></param>
        /// <returns></returns>
        private string AddNewCoreService(CoreServiceDTO objCoreServiceDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objCoreServiceDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objCoreServiceDTO.Name, null);
                if (result == true)
                {
                    CoreService objCoreService = Conversions.ToEntity<CoreServiceDTO, CoreService>(objCoreServiceDTO);
                    objBusinessLayer.CoreServiceRepository.Add(objCoreService);
                    returnMessage = "Core Service added successfully.";
                }
                else
                {
                    returnMessage = "This " + objCoreServiceDTO.Name + " record already exists.";
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
            var CheckNameExists = objBusinessLayer.CoreServiceRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update CoreService 
        /// </summary>
        /// <param name="objCoreService"></param>
        /// <returns></returns>
        private string ModifyCoreService(int ID, CoreServiceDTO objCoreServiceDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                CoreServiceDTO objCoreServiceDTOById = this.GetCoreServiceById(ID);

                if (objCoreServiceDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objCoreServiceDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objCoreServiceDTO.Name, ID);
                    if (result == true)
                    {
                        objCoreServiceDTOById.Name = objCoreServiceDTO.Name;
                        objCoreServiceDTOById.Description = objCoreServiceDTO.Description;
                        returnMessage = "Core Service updated successfully.";
                    }
                    else
                    {
                        objCoreServiceDTOById.Description = objCoreServiceDTO.Description;
                        returnMessage = "This " + objCoreServiceDTO.Name + " record already exists.";
                    }
                    CoreService objCoreService = Conversions.ToEntity<CoreServiceDTO, CoreService>(objCoreServiceDTOById);
                    objBusinessLayer.CoreServiceRepository.Update(objCoreService);

                }
                else
                {
                    returnMessage = "Core Service do not exists.";
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
        /// DeleteCoreService
        /// </summary>
        /// <param name="objCoreServiceDTO">CoreServiceDTO</param>
        private string RemoveCoreService(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                CoreServiceDTO objCoreServiceDTOById = this.GetCoreServiceById(ID);

                if (objCoreServiceDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    CoreService objCoreService = Conversions.ToEntity<CoreServiceDTO, CoreService>(objCoreServiceDTOById);
                    objBusinessLayer.CoreServiceRepository.Delete(objCoreService);
                    returnMessage = "Core Service deleted successfully.";

                }
                else
                {
                    returnMessage = "Core Service do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified core service mapped with TSR / TSO.");
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
        /// GetCoreServiceIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetCoreServiceIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objCoreServiceList = null;
            try
            {
                objCoreServiceList = objBusinessLayer.CoreServiceRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

            return objCoreServiceList;
        }

        /// <summary>
        /// GetCoreServiceIDNameListByTSRId
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetCoreServiceIDNameListByTSRId(int tsrId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objCoreServiceIDNameList = null;

            IList<CoreService> objCoreServiceList = null;
            try
            {
                objCoreServiceList = objBusinessLayer.CoreServiceRepository.GetList(x => x.TSRCoreServices.Any(y => y.TSRId == tsrId));

                if (null != objCoreServiceList && objCoreServiceList.Count > 0)
                {
                    objCoreServiceIDNameList = new List<IDName>();

                    objCoreServiceIDNameList = objCoreServiceList.Select(x => new IDName { ID = x.ID, Name = x.Name }).ToList<IDName>();
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

            return objCoreServiceIDNameList;
        }

        #endregion

        [HttpGet, Route("v1/CoreService/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objCoreServiceList = null;

            try
            {
                objCoreServiceList = this.GetCoreServiceIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objCoreServiceList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        [HttpGet, Route("v1/CoreService/GetAllIDNameByTSRId/{tsrId}")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDNameByTSRId(int tsrId)
        {
            IList<IDName> objCoreServiceList = null;

            try
            {
                objCoreServiceList = this.GetCoreServiceIDNameListByTSRId(tsrId);
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objCoreServiceList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/CoreService/GetById/{id}")]
        [ResponseType(typeof(CoreService))]
        public JsonResult<CoreServiceDTO> GetById(int id)
        {
            JsonResult<CoreServiceDTO> result = null;
            CoreServiceDTO objCoreServiceDTO;

            try
            {
                objCoreServiceDTO = this.GetCoreServiceById(id);

                result = Json(objCoreServiceDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpGet, Route("v1/CoreService/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<CoreServiceDTO> objCoreServiceDTOList = this.GetAllCoreService(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection = null;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };

                if (null != objCoreServiceDTOList)
                {
                    foreach (CoreServiceDTO objCoreService in objCoreServiceDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objCoreService);
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
        /// CreateCoreService
        /// </summary>
        /// <param name="coreServiceDTO">CoreServiceDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/CoreService/CreateCoreService")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateCoreService([FromBody]CoreServiceDTO coreServiceDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewCoreService(coreServiceDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateCoreService
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="coreServiceDTO">CoreServiceDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/CoreService/UpdateCoreService/{ID}")]
        public JsonResult<string> UpdateCoreService(int ID, [FromBody]CoreServiceDTO coreServiceDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyCoreService(ID, coreServiceDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteCoreService
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/CoreService/DeleteCoreService/{ID}")]
        [ResponseType(typeof(string))]
        public JsonResult<string> DeleteCoreService(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveCoreService(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
