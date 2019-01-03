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
    /// RoleController
    /// </summary>
    [RoutePrefix("api")]
    public class EngagementController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetEngagementById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>Engagement</returns>
        private EngagementDTO GetEngagementById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            EngagementDTO objEngagementDTO = null;

            try
            {
                Engagement objEngagement = objBusinessLayer.EngagementRepository.GetByID(id);
                if (null != objEngagement)
                {
                    objEngagementDTO = new EngagementDTO();
                    objEngagementDTO = Conversions.ToDTO<EngagementDTO, Engagement>(objEngagement);
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
        /// GetEngagements
        /// </summary>
        /// <returns>IList<Engagements>></returns>
        private IList<EngagementDTO> GetAllEngagements(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<EngagementDTO> objEngagementDTOList = null;
            try
            {
                IList<Engagement> objEngagementList = objBusinessLayer.EngagementRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objEngagementList != null && objEngagementList.Count > 0)
                {
                    objEngagementDTOList = new List<EngagementDTO>();

                    foreach (Engagement objEngagement in objEngagementList)
                    {
                        EngagementDTO objRoleDTO = new EngagementDTO();
                        objRoleDTO = Conversions.ToDTO<EngagementDTO, Engagement>(objEngagement);

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
        /// Function to add new Role 
        /// </summary>
        /// <param name="objRole"></param>
        /// <returns></returns>
        private string AddNewEngagement(EngagementDTO objRoleDTO)
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
                    Engagement objRole = Conversions.ToEntity<EngagementDTO, Engagement>(objRoleDTO);
                    objBusinessLayer.EngagementRepository.Add(objRole);
                    returnMessage = "Engagement added successfully.";
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
            var CheckNameExists = objBusinessLayer.EngagementRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        private string ModifyEngagement(int ID, EngagementDTO objRoleDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                EngagementDTO objRoleDTOById = this.GetEngagementById(ID);

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
                        returnMessage = "Engagement updated successfully.";
                    }
                    else
                    {
                        objRoleDTOById.Description = objRoleDTO.Description;
                        returnMessage = "This " + objRoleDTO.Name + " record already exists.";
                    }
                    Engagement objRole = Conversions.ToEntity<EngagementDTO, Engagement>(objRoleDTOById);
                    objBusinessLayer.EngagementRepository.Update(objRole);
                }
                else
                {
                    returnMessage = "Engagement do not exists.";
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
        private string RemoveEngagement(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                EngagementDTO objRoleDTOById = this.GetEngagementById(ID);

                if (objRoleDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    Engagement objRole = Conversions.ToEntity<EngagementDTO, Engagement>(objRoleDTOById);
                    objBusinessLayer.EngagementRepository.Delete(objRole);
                    returnMessage = "Engagement deleted successfully.";

                }
                else
                {
                    returnMessage = "Engagement do not exists.";
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
        internal IList<IDName> GetEngagementsIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objRolesList = null;
            try
            {
                objRolesList = objBusinessLayer.EngagementRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

        [HttpGet, Route("v1/Engagement/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objRolesList = null;

            try
            {
                objRolesList = this.GetEngagementsIDNameList();
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
        [HttpGet, Route("v1/Engagement/GetById/{id}")]
        [ResponseType(typeof(Engagement))]
        public JsonResult<EngagementDTO> GetById(int id)
        {
            JsonResult<EngagementDTO> result = null;
            EngagementDTO objRolesDTO;

            try
            {
                objRolesDTO = this.GetEngagementById(id);

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
        [HttpGet, Route("v1/Engagement/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<EngagementDTO> objRoleDTOList = this.GetAllEngagements(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objRoleDTOList)
                {
                    foreach (EngagementDTO objRole in objRoleDTOList)
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
        [HttpPost, Route("v1/Engagement/CreateEngagement")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateEngagement([FromBody]EngagementDTO roleDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewEngagement(roleDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpPut, Route("v1/Engagement/UpdateEngagement/{ID}")]
        public JsonResult<string> UpdateEngagement(int ID, [FromBody]EngagementDTO roleDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyEngagement(ID, roleDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpDelete, Route("v1/Engagement/DeleteEngagement/{ID}")]
        public JsonResult<string> DeleteEngagement(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveEngagement(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
