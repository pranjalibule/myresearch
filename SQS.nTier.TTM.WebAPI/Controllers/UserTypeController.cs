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
    /// UserTypeController
    /// </summary>
    [RoutePrefix("api")]
    public class UserTypeController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetUserTypeById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>UserType</returns>
        private UserTypeDTO GetUserTypeById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            UserTypeDTO objUserTypeDTO = null;

            try
            {
                UserType objUserType = objBusinessLayer.UserTypeRepository.GetByID(id);
                if (null != objUserType)
                {
                    objUserTypeDTO = new UserTypeDTO();
                    objUserTypeDTO = Conversions.ToDTO<UserTypeDTO, UserType>(objUserType);
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

            if (null == objUserTypeDTO)
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return objUserTypeDTO;
        }

        /// <summary>
        /// GetAllUserType
        /// </summary>
        /// <returns>IList<UserType>></returns>
        private IList<UserTypeDTO> GetAllUserType(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<UserTypeDTO> objUserTypeDTOList = null;
            try
            {
                IList<UserType> objUserTypeList = objBusinessLayer.UserTypeRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objUserTypeList != null && objUserTypeList.Count > 0)
                {
                    objUserTypeDTOList = new List<UserTypeDTO>();

                    foreach (UserType objUserType in objUserTypeList)
                    {
                        UserTypeDTO objUserTypeDTO = new UserTypeDTO();
                        objUserTypeDTO = Conversions.ToDTO<UserTypeDTO, UserType>(objUserType);

                        objUserTypeDTOList.Add(objUserTypeDTO);
                    }
                }
            }
            catch
            {
                totalRecords = 0;
                objUserTypeDTOList = null;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            return objUserTypeDTOList;
        }

        /// <summary>
        /// Function to add new UserType 
        /// </summary>
        /// <param name="objUserType"></param>
        /// <returns></returns>
        private string AddNewUserType(UserTypeDTO objUserTypeDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objUserTypeDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objUserTypeDTO.Name, null);
                if (result == true)
                {
                    UserType objUserType = Conversions.ToEntity<UserTypeDTO, UserType>(objUserTypeDTO);
                    objBusinessLayer.UserTypeRepository.Add(objUserType);
                    returnMessage = "UserType added successfully.";
                }
                else
                {
                    returnMessage = "This " + objUserTypeDTO.Name + " UserType already exists.";
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
            var CheckNameExists = objBusinessLayer.UserTypeRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update UserType 
        /// </summary>
        /// <param name="objUserType"></param>
        /// <returns>string</returns>
        private string ModifyUserType(int ID, UserTypeDTO objUserTypeDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                UserTypeDTO objUserTypeDTOById = this.GetUserTypeById(ID);

                if (objUserTypeDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objUserTypeDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objUserTypeDTO.Name, ID);
                    if (result == true)
                    {
                        objUserTypeDTOById.Name = objUserTypeDTO.Name;
                        objUserTypeDTOById.Description = objUserTypeDTO.Description;
                        returnMessage = "UserType updated successfully.";
                    }
                    else
                    {
                        objUserTypeDTOById.Description = objUserTypeDTO.Description;
                        returnMessage = "This " + objUserTypeDTO.Name + " UserType already exists.";
                    }
                    UserType objUserType = Conversions.ToEntity<UserTypeDTO, UserType>(objUserTypeDTOById);

                    //objUserType = Conversions.ToEntity<UserTypeDTO, UserType>(objUserTypeDTO, objUserType);
                    objBusinessLayer.UserTypeRepository.Update(objUserType);


                }
                else
                {
                    returnMessage = "UserType do not exists.";
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
        /// RemoveUserType
        /// </summary>
        /// <param name="objUserTypeDTO">UserTypeDTO</param>
        private string RemoveUserType(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                UserTypeDTO objUserTypeDTOById = this.GetUserTypeById(ID);

                if (objUserTypeDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    UserType objUserType = Conversions.ToEntity<UserTypeDTO, UserType>(objUserTypeDTOById);
                    objBusinessLayer.UserTypeRepository.Delete(objUserType);
                    returnMessage = "UserType deleted successfully.";

                }
                else
                {
                    returnMessage = "UserType do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified user type mapped with TSR / TSO.");
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
        /// GetUserTypeIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetUserTypeIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objUserTypeList = null;
            try
            {
                objUserTypeList = objBusinessLayer.UserTypeRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

            return objUserTypeList;
        }

        #endregion

        [HttpGet, Route("v1/UserType/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objUserTypeList = null;

            try
            {
                objUserTypeList = this.GetUserTypeIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objUserTypeList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/UserType/GetById/{id}")]
        [ResponseType(typeof(UserType))]
        public JsonResult<UserTypeDTO> GetById(int id)
        {
            JsonResult<UserTypeDTO> result = null;
            UserTypeDTO objUserTypeDTO;

            try
            {
                objUserTypeDTO = this.GetUserTypeById(id);

                result = Json(objUserTypeDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return result;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/UserType/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<UserTypeDTO> objUserTypeDTOList = this.GetAllUserType(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objUserTypeDTOList)
                {
                    foreach (UserTypeDTO objUserType in objUserTypeDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objUserType);
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
        /// CreateUserType
        /// </summary>
        /// <param name="userTypeDTO">UserTypeDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/UserType/CreateUserType")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateUserType([FromBody]UserTypeDTO userTypeDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                if (null != userTypeDTO)
                    result = Json(this.AddNewUserType(userTypeDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateUserType
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="userTypeDTO">UserTypeDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/UserType/UpdateUserType/{ID}")]
        public JsonResult<string> UpdateUserType(int ID, [FromBody]UserTypeDTO userTypeDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyUserType(ID, userTypeDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteUserType
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/UserType/DeleteUserType/{ID}")]
        public JsonResult<string> DeleteUserType(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveUserType(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
