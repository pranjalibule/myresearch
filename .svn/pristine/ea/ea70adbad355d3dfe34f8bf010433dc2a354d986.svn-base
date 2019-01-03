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
    public class MethodController : ApiController
    {
        #region Private Functions


        private ServiceDeliveryChainMethodDTO GetMethodById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            ServiceDeliveryChainMethodDTO objRoleDTO = null;

            try
            {
                ServiceDeliveryChainMethod objMethod = objBusinessLayer.MethodRepository.GetByID(id);
                if (null != objMethod)
                {
                    objRoleDTO = new ServiceDeliveryChainMethodDTO();
                    objRoleDTO = Conversions.ToDTO<ServiceDeliveryChainMethodDTO, ServiceDeliveryChainMethod>(objMethod);
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

            return objRoleDTO;
        }


        private IList<ServiceDeliveryChainMethodDTO> GetAllMethods(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<ServiceDeliveryChainMethodDTO> objRoleDTOList = null;
            try
            {
                IList<ServiceDeliveryChainMethod> objRolesList = objBusinessLayer.MethodRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objRolesList != null && objRolesList.Count > 0)
                {
                    objRoleDTOList = new List<ServiceDeliveryChainMethodDTO>();

                    foreach (ServiceDeliveryChainMethod objRole in objRolesList)
                    {
                        ServiceDeliveryChainMethodDTO objRoleDTO = new ServiceDeliveryChainMethodDTO();
                        objRoleDTO = Conversions.ToDTO<ServiceDeliveryChainMethodDTO, ServiceDeliveryChainMethod>(objRole);

                        objRoleDTOList.Add(objRoleDTO);
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

            return objRoleDTOList;
        }

        /// <summary>
        /// Function to add new Role 
        /// </summary>
        /// <param name="objRole"></param>
        /// <returns></returns>
        private string AddNewMethod(ServiceDeliveryChainMethodDTO objMethodDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objMethodDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objMethodDTO.Name, null);
                if (result == true)
                {
                    ServiceDeliveryChainMethod objRole = Conversions.ToEntity<ServiceDeliveryChainMethodDTO, ServiceDeliveryChainMethod>(objMethodDTO);
                    objBusinessLayer.MethodRepository.Add(objRole);
                    returnMessage = "Method added successfully.";
                }
                else
                {
                    returnMessage = "This " + objMethodDTO.Name + " method already exists.";
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
            var CheckNameExists = objBusinessLayer.MethodRepository.GetList(x => x.Name == Name && x.ID == ID);
            if (CheckNameExists.Count <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string ModifyMethod(int ID, ServiceDeliveryChainMethodDTO objRoleDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                ServiceDeliveryChainMethodDTO objRoleDTOById = this.GetMethodById(ID);

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
                        //objRoleDTOById.ServiceChainId = objRoleDTO.ServiceChainId;
                        returnMessage = "Method updated successfully.";
                    }
                    else
                    {
                        objRoleDTOById.Description = objRoleDTO.Description;
                        returnMessage = "This " + objRoleDTO.Name + " method already exists.";
                    }
                    ServiceDeliveryChainMethod objRole = Conversions.ToEntity<ServiceDeliveryChainMethodDTO, ServiceDeliveryChainMethod>(objRoleDTOById);

                    objBusinessLayer.MethodRepository.Update(objRole);


                }
                else
                {
                    returnMessage = "Method do not exists.";
                    TTMLogger.Logger.LogInfo(returnMessage);
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


        private string RemoveMethod(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                ServiceDeliveryChainMethodDTO objRoleDTOById = this.GetMethodById(ID);

                if (objRoleDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    ServiceDeliveryChainMethod objRole = Conversions.ToEntity<ServiceDeliveryChainMethodDTO, ServiceDeliveryChainMethod>(objRoleDTOById);
                    objBusinessLayer.RoleRepository.Delete(objRole);
                    returnMessage = "Method deleted successfully.";

                }
                else
                {
                    returnMessage = "Method do not exists.";
                    TTMLogger.Logger.LogInfo(returnMessage);
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
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
        /// GetMethodIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetMethodsIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objRolesList = null;
            try
            {
                objRolesList = objBusinessLayer.MethodRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

        internal IList<IDName> GetMethodsList(int serviceDeliveryChainId)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objRolesList = null;
            try
            {
                objRolesList = objBusinessLayer.MethodRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });

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

        [HttpGet, Route("v1/Method/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objRolesList = null;

            try
            {
                objRolesList = this.GetMethodsIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objRolesList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        [HttpGet, Route("v1/Method/GetAllMethodIDName/{id}")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllMethodIDName(int id)
        {
            IList<IDName> objRolesList = null;

            try
            {
                objRolesList = this.GetMethodsList(id);
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
        [HttpGet, Route("v1/Method/GetById/{id}")]
        [ResponseType(typeof(ServiceDeliveryChainMethod))]
        public JsonResult<ServiceDeliveryChainMethodDTO> GetById(int id)
        {
            JsonResult<ServiceDeliveryChainMethodDTO> result = null;
            ServiceDeliveryChainMethodDTO objRolesDTO;

            try
            {
                objRolesDTO = this.GetMethodById(id);

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
        [HttpGet, Route("v1/Method/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<ServiceDeliveryChainMethodDTO> objRoleDTOList = this.GetAllMethods(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objRoleDTOList)
                {
                    foreach (ServiceDeliveryChainMethodDTO objRole in objRoleDTOList)
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
        [HttpPost, Route("v1/Method/CreateMethod")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateMethod([FromBody]ServiceDeliveryChainMethodDTO roleDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewMethod(roleDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpPut, Route("v1/Method/UpdateMethod/{ID}")]
        public JsonResult<string> UpdateMethod(int ID, [FromBody]ServiceDeliveryChainMethodDTO roleDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyMethod(ID, roleDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpDelete, Route("v1/Method/DeleteMethod/{ID}")]
        public JsonResult<string> DeleteMethod(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveMethod(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
