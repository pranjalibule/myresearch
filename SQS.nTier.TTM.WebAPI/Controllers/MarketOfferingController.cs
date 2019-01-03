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
    /// MarketOfferingController
    /// </summary>
    [RoutePrefix("api")]
    public class MarketOfferingController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetMarketOfferingById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>MarketOffering</returns>
        private MarketOfferingDTO GetMarketOfferingById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            MarketOfferingDTO objMarketOfferingDTO = null;

            try
            {
                MarketOffering objMarketOffering = objBusinessLayer.MarketOfferingRepository.GetByID(id);
                if (null != objMarketOffering)
                {
                    objMarketOfferingDTO = new MarketOfferingDTO();
                    objMarketOfferingDTO = Conversions.ToDTO<MarketOfferingDTO, MarketOffering>(objMarketOffering);
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

            return objMarketOfferingDTO;
        }

        /// <summary>
        /// GetAllMarketOffering
        /// </summary>
        /// <returns>IList<MarketOffering>></returns>
        private IList<MarketOfferingDTO> GetAllMarketOffering(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<MarketOfferingDTO> objMarketOfferingDTOList = null;
            try
            {
                IList<MarketOffering> objMarketOfferingList = objBusinessLayer.MarketOfferingRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objMarketOfferingList != null && objMarketOfferingList.Count > 0)
                {
                    objMarketOfferingDTOList = new List<MarketOfferingDTO>();

                    foreach (MarketOffering objMarketOffering in objMarketOfferingList)
                    {
                        MarketOfferingDTO objMarketOfferingDTO = new MarketOfferingDTO();
                        objMarketOfferingDTO = Conversions.ToDTO<MarketOfferingDTO, MarketOffering>(objMarketOffering);

                        objMarketOfferingDTOList.Add(objMarketOfferingDTO);
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

            return objMarketOfferingDTOList;
        }

        /// <summary>
        /// Function to add new MarketOffering 
        /// </summary>
        /// <param name="objMarketOffering"></param>
        /// <returns></returns>
        private string AddNewMarketOffering(MarketOfferingDTO objMarketOfferingDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objMarketOfferingDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objMarketOfferingDTO.Name, null);
                if (result == true)
                {
                    MarketOffering objMarketOffering = Conversions.ToEntity<MarketOfferingDTO, MarketOffering>(objMarketOfferingDTO);
                    objBusinessLayer.MarketOfferingRepository.Add(objMarketOffering);
                    returnMessage = "Market Offering added successfully.";
                }
                else
                {
                    returnMessage = "This " + objMarketOfferingDTO.Name + " record already exists.";
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
            var CheckNameExists = objBusinessLayer.MarketOfferingRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update MarketOffering 
        /// </summary>
        /// <param name="objMarketOffering"></param>
        /// <returns>string</returns>
        private string ModifyMarketOffering(int ID, MarketOfferingDTO objMarketOfferingDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                MarketOfferingDTO objMarketOfferingDTOById = this.GetMarketOfferingById(ID);

                if (objMarketOfferingDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objMarketOfferingDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objMarketOfferingDTO.Name, ID);
                    if (result == true)
                    {
                        objMarketOfferingDTOById.Name = objMarketOfferingDTO.Name;
                        objMarketOfferingDTOById.Description = objMarketOfferingDTO.Description;
                        returnMessage = "Market Offering updated successfully.";
                    }
                    else
                    {
                        objMarketOfferingDTOById.Description = objMarketOfferingDTO.Description;
                        returnMessage = "This " + objMarketOfferingDTO.Name + " record already exists.";
                    }
                    MarketOffering objMarketOffering = Conversions.ToEntity<MarketOfferingDTO, MarketOffering>(objMarketOfferingDTOById);

                    //objMarketOffering = Conversions.ToEntity<MarketOfferingDTO, MarketOffering>(objMarketOfferingDTO, objMarketOffering);
                    objBusinessLayer.MarketOfferingRepository.Update(objMarketOffering);
                }
                else
                {
                    returnMessage = "Market Offering do not exists.";
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
        /// DeleteMarketOffering
        /// </summary>
        /// <param name="objMarketOfferingDTO">MarketOfferingDTO</param>
        private string RemoveMarketOffering(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                MarketOfferingDTO objMarketOfferingDTOById = this.GetMarketOfferingById(ID);

                if (objMarketOfferingDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    MarketOffering objMarketOffering = Conversions.ToEntity<MarketOfferingDTO, MarketOffering>(objMarketOfferingDTOById);
                    objBusinessLayer.MarketOfferingRepository.Delete(objMarketOffering);
                    returnMessage = "Market Offering deleted successfully.";

                }
                else
                {
                    returnMessage = "Market Offering do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified market offering mapped with TSR / TSO.");
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
        /// GetMarketOfferingIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetMarketOfferingIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objMarketOfferingList = null;
            try
            {
                objMarketOfferingList = objBusinessLayer.MarketOfferingRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

            return objMarketOfferingList;
        }

        #endregion

        [HttpGet, Route("v1/MarketOffering/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objMarketOfferingList = null;

            try
            {
                objMarketOfferingList = this.GetMarketOfferingIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objMarketOfferingList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/MarketOffering/GetById/{id}")]
        [ResponseType(typeof(MarketOffering))]
        public JsonResult<MarketOfferingDTO> GetById(int id)
        {
            JsonResult<MarketOfferingDTO> result = null;
            MarketOfferingDTO objMarketOfferingDTO;

            try
            {
                objMarketOfferingDTO = this.GetMarketOfferingById(id);

                result = Json(objMarketOfferingDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpGet, Route("v1/MarketOffering/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<MarketOfferingDTO> objMarketOfferingDTOList = this.GetAllMarketOffering(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objMarketOfferingDTOList)
                {
                    foreach (MarketOfferingDTO objMarketOffering in objMarketOfferingDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objMarketOffering);
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
        /// CreateMarketOffering
        /// </summary>
        /// <param name="marketOfferingDTO">MarketOfferingDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/MarketOffering/CreateMarketOffering")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateMarketOffering([FromBody]MarketOfferingDTO marketOfferingDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewMarketOffering(marketOfferingDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateMarketOffering
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="userTypeDTO">UserTypeDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/MarketOffering/UpdateMarketOffering/{ID}")]
        public JsonResult<string> UpdateMarketOffering(int ID, [FromBody]MarketOfferingDTO marketOfferingDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyMarketOffering(ID, marketOfferingDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteMarketOffering
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/MarketOffering/DeleteMarketOffering/{ID}")]
        public JsonResult<string> DeleteMarketOffering(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveMarketOffering(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
