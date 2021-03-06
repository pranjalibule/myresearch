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
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    /// <summary>
    /// RoleController
    /// </summary>
    [RoutePrefix("api")]
    public class PricingModelController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetPricingModelById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>PricingModel</returns>
        private PricingModelDTO GetPricingModelById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            PricingModelDTO objEngagementDTO = null;

            try
            {
                PricingModel objEngagement = objBusinessLayer.PricingModelRepository.GetByID(id);
                if (null != objEngagement)
                {
                    objEngagementDTO = new PricingModelDTO();
                    objEngagementDTO = Conversions.ToDTO<PricingModelDTO, PricingModel>(objEngagement);
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

            return objEngagementDTO;
        }

        /// <summary>
        /// GetAllPricingModels
        /// </summary>
        /// <returns>IList<PricingModels>></returns>
        private IList<PricingModelDTO> GetAllPricingModels(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<PricingModelDTO> objEngagementDTOList = null;
            try
            {
                IList<PricingModel> objEngagementList = objBusinessLayer.PricingModelRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objEngagementList != null && objEngagementList.Count > 0)
                {
                    objEngagementDTOList = new List<PricingModelDTO>();

                    foreach (PricingModel objEngagement in objEngagementList)
                    {
                        PricingModelDTO objRoleDTO = new PricingModelDTO();
                        objRoleDTO = Conversions.ToDTO<PricingModelDTO, PricingModel>(objEngagement);

                        objEngagementDTOList.Add(objRoleDTO);
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

            return objEngagementDTOList;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="objRoleDTO"></param>
        /// <returns></returns>
        private string AddNewPricingModel(PricingModelDTO objRoleDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objRoleDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objRoleDTO.Name, null);
                if (result == true)
                {
                    PricingModel objRole = Conversions.ToEntity<PricingModelDTO, PricingModel>(objRoleDTO);
                    objBusinessLayer.PricingModelRepository.Add(objRole);
                    returnMessage = "PricingModel added successfully.";
                }
                else
                {
                    returnMessage = "This " + objRoleDTO.Name + " record already exists.";
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
            var CheckNameExists = objBusinessLayer.PricingModelRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update Role 
        /// </summary>
        /// <param name="objRole"></param>
        /// <returns>string</returns>
        private string ModifyPricingModel(int ID, PricingModelDTO objRoleDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                PricingModelDTO objRoleDTOById = this.GetPricingModelById(ID);

                if (objRoleDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objRoleDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objRoleDTO.Name, ID);
                    if (result == true)
                    {
                        objRoleDTOById.Name = objRoleDTO.Name;
                        objRoleDTOById.Description = objRoleDTO.Description;
                        returnMessage = "Pricing Model updated successfully.";
                    }
                    else
                    {
                        objRoleDTOById.Description = objRoleDTO.Description;
                        returnMessage = "This " + objRoleDTO.Name + " record already exists.";
                    }
                    PricingModel objRole = Conversions.ToEntity<PricingModelDTO, PricingModel>(objRoleDTOById);
                    objBusinessLayer.PricingModelRepository.Update(objRole);


                }
                else
                {
                    returnMessage = "Pricing Model do not exists.";
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
        /// DeleteRole
        /// </summary>
        /// <param name="objRoleDTO">RoleDTO</param>
        private string RemovePricingModel(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                PricingModelDTO objRoleDTOById = this.GetPricingModelById(ID);

                if (objRoleDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    PricingModel objRole = Conversions.ToEntity<PricingModelDTO, PricingModel>(objRoleDTOById);
                    objBusinessLayer.PricingModelRepository.Delete(objRole);
                    returnMessage = "Pricing Model deleted successfully.";

                }
                else
                {
                    returnMessage = "Pricing Model do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified role mapped with TSR / TSO.");
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
        /// GetRolegIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetRicingModelsIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objRolesList = null;
            try
            {
                objRolesList = objBusinessLayer.PricingModelRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

            return objRolesList;
        }

        #endregion

        [HttpGet, Route("v1/PricingModel/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objRolesList = null;

            try
            {
                objRolesList = this.GetRicingModelsIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objRolesList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/PricingModel/GetById/{id}")]
        [ResponseType(typeof(Engagement))]
        public JsonResult<PricingModelDTO> GetById(int id)
        {
            JsonResult<PricingModelDTO> result = null;
            PricingModelDTO objRolesDTO;

            try
            {
                objRolesDTO = this.GetPricingModelById(id);

                result = Json(objRolesDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpGet, Route("v1/PricingModel/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<PricingModelDTO> objRoleDTOList = this.GetAllPricingModels(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objRoleDTOList)
                {
                    foreach (PricingModelDTO objRole in objRoleDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objRole);
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
        /// CreateRole
        /// </summary>
        /// <param name="rolDTO">RoleDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/PricingModel/CreatePricingModel")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreatePricingModel([FromBody]PricingModelDTO roleDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewPricingModel(roleDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateRole
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="userTypeDTO">UserTypeDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/PricingModel/UpdatePricingModel/{ID}")]
        public JsonResult<string> UpdatePricingModel(int ID, [FromBody]PricingModelDTO roleDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyPricingModel(ID, roleDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteRole
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/PricingModel/DeleteEngagement/{ID}")]
        public JsonResult<string> DeleteEngagement(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemovePricingModel(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
