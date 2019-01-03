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
    using Encryption;
    using GenericFramework;
    using Newtonsoft.Json;
    using SQS.nTier.TTM.DTO;
    using SQS.nTier.TTM.GenericFramework.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    /// <summary>
    /// UserController
    /// </summary>
    [RoutePrefix("api")]
    public class UserController : ApiController
    {
        Utility utils = new Utility();

        #region Private functions

        /// <summary>
        /// GetAllUsers
        /// </summary>
        /// <returns>IList<User>></returns>
        private IList<User> GetAllUsers(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<User> objUserList = null;
            try
            {
                objUserList = objBusinessLayer.UserRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, true, out totalRecords, x => x.Role);
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

            if (null == objUserList)
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return objUserList;
        }

        /// <summary>
        /// GetUsersByNameOrEmail
        /// </summary>
        /// <returns>IList<User>></returns>
        private IList<User> GetUsersByNameOrEmail(int startingRecordNumber, int pageSize, String nameOrEmail, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<User> objUserList = null;
            try
            {
                objUserList = objBusinessLayer.UserRepository.GetList(startingRecordNumber, pageSize,
                    x => x.Name.ToLower().Contains(nameOrEmail) || x.EmailID.ToLower().Contains(nameOrEmail) || x.UserId.ToLower().Contains(nameOrEmail),
                    x => x.ID, true, out totalRecords);
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

            if (null == objUserList)
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return objUserList;
        }

        /// <summary>
        /// GetUsersByNameOrEmail
        /// </summary>
        /// <returns>IList<User>></returns>
        private IList<User> GetUsersByNameOrEmail(String nameOrEmail, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<User> objUserList = null;

            totalRecords = 0;
            try
            {
                objUserList = objBusinessLayer.UserRepository.GetList(x => x.Name.ToLower().Contains(nameOrEmail) || x.EmailID.ToLower().Contains(nameOrEmail) || x.UserId.ToLower().Contains(nameOrEmail));
                totalRecords = objUserList.Count;
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

            if (null == objUserList)
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return objUserList;
        }

        /// <summary>
        /// GetLockedUsers
        /// </summary>
        /// <returns>IList<User>></returns>
        private IList<User> GetLockedUsers(int startingRecordNumber, int pageSize, bool locked, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<User> objUserList = null;
            try
            {
                objUserList = objBusinessLayer.UserRepository.GetList(startingRecordNumber, pageSize,
                    x => x.Locked == locked,
                    x => x.ID, true, out totalRecords);
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

            if (null == objUserList)
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return objUserList;
        }

        /// <summary>
        /// GetActiveUsers
        /// </summary>
        /// <returns>IList<User>></returns>
        private IList<User> GetActiveUsers(int startingRecordNumber, int pageSize, bool active, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<User> objUserList = null;
            try
            {
                objUserList = objBusinessLayer.UserRepository.GetList(startingRecordNumber, pageSize,
                    x => x.Activated == active,
                    x => x.ID, true, out totalRecords);
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

            if (null == objUserList)
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return objUserList;
        }

        /// <summary>
        /// CreateADUser
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        private UserDTO CreateADUser(LoginDTO loginDTO)
        {
            CryptorEngine objCryptorEngine = new CryptorEngine();
            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            UserDTO objUserDTO = null;
            int totalRecords = 0;

            try
            {
                IList<User> objUserList = utils.GetUsersFromADOnly(loginDTO.UserName, out totalRecords);
                try
                {
                    if (null != objUserList && objUserList.Count > 0)
                    {
                        for (int i = 0; i < objUserList.Count; i++)
                        {
                            string searchUM = loginDTO.UserName.Trim().ToLower();

                            if (searchUM == objUserList[i].Name.Trim().ToLower() ||
                                searchUM == objUserList[i].UserId.Trim().ToLower() ||
                                searchUM == objUserList[i].EmailID.Trim().ToLower())
                            {
                                objUserDTO = new UserDTO();
                                objUserDTO = Conversions.ToDTO<UserDTO, User>(objUserList[i]);

                                ls.LoginName = loginDTO.UserName;
                                string strPassword = objCryptorEngine.Encrypt(PasswordGenerator(), true);

                                string UId = objUserList[i].UserId.ToLower();
                                //Create a new user
                                objBusinessLayer = new BusinessLayer(ls);
                                User objsearchUser = objBusinessLayer.UserRepository.GetSingle(x => x.UserId.ToLower() == UId);

                                if (objsearchUser == null)
                                {
                                    Roles objroles = null;
                                    if (string.IsNullOrEmpty(loginDTO.roleId.ToString()))
                                    {
                                        objBusinessLayer = new BusinessLayer(ls);
                                        objroles = objBusinessLayer.RoleRepository.GetSingle(x => x.Name.ToLower() == "Guest");
                                    }
                                    else
                                    {
                                        objBusinessLayer = new BusinessLayer(ls);
                                        objroles = objBusinessLayer.RoleRepository.GetSingle(x => x.ID == loginDTO.roleId);
                                    }

                                    if (objroles != null)
                                    {
                                        User objUser = new User
                                        {
                                            UserId = objUserDTO.UserId,
                                            EmailID = objUserDTO.EmailID,
                                            Name = objUserDTO.Name,
                                            Password = strPassword,
                                            Activated = true,
                                            RoleId = (int)objroles.ID
                                        };

                                        objBusinessLayer = new BusinessLayer(ls);
                                        objBusinessLayer.UserRepository.Add(objUser);

                                        if (objUser.ID > 0)
                                        {
                                            objUserDTO = new UserDTO();
                                            objUserDTO = Conversions.ToDTO<UserDTO, User>(objUser);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                    throw;
                }

                objCryptorEngine = null;

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw ex;
            }

            return objUserDTO;
        }
        /// <summary>
        /// PasswordGenerator
        /// </summary>
        /// <returns>string</returns>
        private string PasswordGenerator()
        {
            Random RandomGen = new Random();
            int seed = RandomGen.Next(1, int.MaxValue);
            const string specialCharacters = @"abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

            var chars = new char[12];
            var rd = new Random(seed);

            for (var i = 0; i < 12; i++)
            {
                chars[i] = specialCharacters[rd.Next(0, specialCharacters.Length)];
            }

            return new string(chars);
        }

        #endregion

        /// <summary>
        /// Get
        /// </summary>
        /// <returns>IHttpActionResult></returns>
        [ResponseType(typeof(DataCollection))]
        [HttpGet, Route("v1/User/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<User> objUserList = this.GetAllUsers(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };

                foreach (User objUserType in objUserList)
                {
                    objDataCollection.EntitySummary.Add(objUserType);
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
        /// SearchByNameOrEmail
        /// </summary>
        /// <param name="startingRecordNumber">int</param>
        /// <param name="pageSize">int</param>
        /// <param name="nameOrEmail">String</param>
        /// <returns>JsonResult<DataCollection></returns>
        [ResponseType(typeof(DataCollection))]
        //[VerifyHMACSignature]
        [HttpGet, Route("v1/User/SearchByNameOrEmail/{startingRecordNumber}/{pageSize}/{nameOrEmail}")]
        public JsonResult<DataCollection> SearchByNameOrEmail(int startingRecordNumber, int pageSize, String nameOrEmail)
        {
            int totalRecords = 0;
            IList<User> objUserList = this.GetUsersByNameOrEmail(startingRecordNumber, pageSize, nameOrEmail, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };

                foreach (User objUserType in objUserList)
                {
                    objDataCollection.EntitySummary.Add(objUserType);
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
        /// GetUserByEmail
        /// </summary>
        /// <param name="startingRecordNumber">int</param>
        /// <param name="pageSize">int</param>
        /// <param name="nameOrEmail">String</param>
        /// <returns>JsonResult<DataCollection></returns>
        [ResponseType(typeof(DataCollection))]
        [HttpPost, Route("v1/User/GetUserByEmail")]
        public JsonResult<UserDTO> GetUserByEmail([FromBody] LoginDTO loginDTO)
        {
            int totalRecords = 0;
            IList<User> objUserList = this.GetUsersByNameOrEmail(loginDTO.UserName, out totalRecords);
            UserDTO objUserDTO = null;

            try
            {
                if (null != objUserList && objUserList.Count == 1)
                {
                    objUserDTO = new UserDTO();
                    objUserDTO = Conversions.ToDTO<UserDTO, User>(objUserList[0]);
                }
                else
                {
                    //Create user only if required
                    if (loginDTO.ADUser)
                    {
                        objUserDTO = CreateADUser(loginDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objUserDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        [ResponseType(typeof(DataCollection))]
        [HttpPost, Route("v1/User/GetUsersByNameOrEmail")]
        public JsonResult<List<UserDTO>> GetUsersByNameOrEmail([FromBody] LoginDTO loginDTO)
        {
            int totalRecords = 0;
            IList<User> objUserList = utils.GetUsersFromADOnly(loginDTO.UserName, out totalRecords);
            List<UserDTO> listUserDTO = null;
            UserDTO objUserDTO = null;
            try
            {
                if (null != objUserList && objUserList.Count > 0)
                {
                    listUserDTO = new List<UserDTO>();

                    for (int i = 0; i < objUserList.Count; i++)
                    {
                        objUserDTO = new UserDTO();
                        objUserDTO = Conversions.ToDTO<UserDTO, User>(objUserList[i]);

                        listUserDTO.Add(objUserDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(listUserDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// SearchLockedUsers
        /// </summary>
        /// <param name="startingRecordNumber">int</param>
        /// <param name="pageSize">int</param>
        /// <param name="locked">bool</param>
        /// <returns>sonResult<DataCollection></returns>
        [ResponseType(typeof(DataCollection))]
        [HttpGet, Route("v1/User/SearchLockedUsers/{startingRecordNumber}/{pageSize}/{locked}")]
        public JsonResult<DataCollection> SearchLockedUsers(int startingRecordNumber, int pageSize, bool locked)
        {
            int totalRecords = 0;
            IList<User> objUserList = this.GetLockedUsers(startingRecordNumber, pageSize, locked, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };

                foreach (User objUserType in objUserList)
                {
                    objDataCollection.EntitySummary.Add(objUserType);
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
        /// SearchActiveUsers
        /// </summary>
        /// <param name="startingRecordNumber">int</param>
        /// <param name="pageSize">int</param>
        /// <param name="active">bool</param>
        /// <returns>sonResult<DataCollection></returns>
        [ResponseType(typeof(DataCollection))]
        [HttpGet, Route("v1/User/SearchActiveUsers/{startingRecordNumber}/{pageSize}/{active}")]
        public JsonResult<DataCollection> SearchActiveUsers(int startingRecordNumber, int pageSize, bool active)
        {
            int totalRecords = 0;
            IList<User> objUserList = this.GetActiveUsers(startingRecordNumber, pageSize, active, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };

                foreach (User objUserType in objUserList)
                {
                    objDataCollection.EntitySummary.Add(objUserType);
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
        /// CheckExistingEmail
        /// </summary>
        /// <returns>IHttpActionResult></returns>
        [HttpPost, Route("v1/User/CheckExistingEmail")]
        public Boolean CheckExistingEmail([FromBody] LoginDTO loginDTO)
        {
            Boolean isEmailExists = false;
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.EmailID.ToLower() == loginDTO.UserName.ToLower());

                if (objUser != null)
                {
                    isEmailExists = true;
                }
            }
            catch
            {
                isEmailExists = true;
            }
            finally
            {
                objBusinessLayer.Dispose();
                ls = null;
            }

            return isEmailExists;
        }


        /// <summary>
        /// GetUserByActivationKey
        /// </summary>
        /// <param name="activationKey">String</param>
        /// <returns>IDName</returns>
        [HttpGet, Route("v1/User/GetUserByActivationKey/{activationKey}")]
        public IDName GetUserByActivationKey(String activationKey)
        {
            LoginSession ls = new LoginSession();
            ls.LoginName = "Activate";

            IDName objIDName = null;

            if (!String.IsNullOrWhiteSpace(activationKey))
            {
                IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

                try
                {
                    User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ActivationKey == activationKey);

                    if (objUser != null)
                    {
                        objIDName = new IDName
                        {
                            ID = objUser.ID,
                            Name = objUser.Name
                        };
                    }
                    else
                    {
                        objIDName = new IDName { ID = 0, Name = String.Format("Error - {0}", "Incorrect activation key for resetting password.") };
                        TTMLogger.Logger.LogError(objIDName.Name);
                    }
                }
                catch (Exception ex)
                {
                    TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                    objIDName = new IDName { ID = 0, Name = String.Format("Error - {0}", ex.Message) };
                }
                finally
                {
                    objBusinessLayer.Dispose();
                    ls = null;
                }
            }
            else
            {
                objIDName = new IDName { ID = 0, Name = "Invalid activation key." };
                TTMLogger.Logger.LogError(objIDName.Name);
            }

            return objIDName;
        }

        /// <summary>
        /// ActivateUser
        /// </summary>
        /// <param name="activationKey">String</param>
        /// <returns>String</returns>
        [HttpGet, Route("v1/User/ActivateUser")]
        public JsonResult<String> ActivateUser(String activationKey)
        {
            JsonResult<string> result = Json(string.Empty);
            LoginSession ls = new LoginSession();
            ls.LoginName = "Activate";

            if (!String.IsNullOrWhiteSpace(activationKey))
            {
                IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

                try
                {
                    User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ActivationKey == activationKey);

                    if (objUser != null)
                    {
                        if (!objUser.Activated)
                        {
                            objBusinessLayer = new BusinessLayer(ls);
                            objUser.Activated = true;

                            objBusinessLayer.UserRepository.Update(objUser);

                            result = Json("", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                        }
                        else
                        {
                            result = Json("User already activated.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                        }
                    }
                    else
                    {
                        result = Json(String.Format("Error - User can not be activated."), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                    }
                }
                catch (Exception ex)
                {
                    if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
                    {
                        result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                    }
                    else
                        result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
                finally
                {
                    objBusinessLayer.Dispose();
                    ls = null;
                }
            }
            else
            {
                result = Json(String.Format("Invalid activation key."), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// GetUser
        /// </summary>
        /// <param name="encrUserString">String</param>
        /// <returns>IDName</returns>
        [HttpPost, Route("v1/User/GetUser")]
        public IDName GetUser([FromBody] LoginDTO loginDTO)
        {
            LoginSession ls = new LoginSession();

            IDName objIDName = null;

            if (!String.IsNullOrWhiteSpace(loginDTO.UserName))
            {
                loginDTO.UserName = loginDTO.UserName.Replace("!", "+").Replace("~", "=");
                string wholeUserName = User.Identity.Name; //WindowsIdentity.GetCurrent().Name;
                string UserName = wholeUserName.Substring(wholeUserName.LastIndexOf("\\") + 1);
                IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

                try
                {
                    CryptorEngine objCryptorEngine = new CryptorEngine();

                    var strConcantenated = objCryptorEngine.Decrypt(loginDTO.UserName, true);
                    var arrUserString = strConcantenated.Split('#');
                    if (arrUserString.Length > 0)
                    {
                        User objUser;
                        //if (arrUserString[3].ToLower() == UserName.ToLower() || arrUserString[3].ToLower() == "admin")
                        //{
                        var emailId = arrUserString[1].ToString().ToLower();
                        objUser = objBusinessLayer.UserRepository.GetSingle(x => x.EmailID.ToLower() == emailId, x => x.Role);
                        //}
                        //else
                        //{
                        //    objUser = objBusinessLayer.UserRepository.GetSingle(x => x.UserId.ToLower() == arrUserString[3].ToLower(), x => x.Role);
                        //}

                        if (objUser != null)
                        {
                            if (objUser.Activated && !objUser.Locked && objUser.Role.Name != "N.A.")
                            {
                                objIDName = new IDName
                                {
                                    ID = objUser.ID,
                                    Name = objUser.Name,
                                    Other = objCryptorEngine.Decrypt(objUser.Password, true),
                                    Role = objUser.Role.Name
                                };
                            }
                            else if (!objUser.Activated)
                            {
                                objIDName = new IDName
                                {
                                    Name = String.Format("PendingActivation: {0}", "User '" + objUser.Name + "' is not activated."),
                                    Other = objUser.UserId
                                };
                            }
                            else if (objUser.Locked)
                            {
                                objIDName = new IDName
                                {
                                    Name = String.Format("Error:{0}", "User account is locked."),
                                };

                                TTMLogger.Logger.LogError(objIDName.Name);
                            }
                            else if (objUser.Role.Name == "N.A.")
                            {
                                objIDName = new IDName
                                {
                                    Name = String.Format("PendingActivation: {0}", "User '" + objUser.Name + "' is not activated."),
                                    Other = objUser.UserId
                                };
                            }
                        }
                        else
                        {
                            objIDName = new IDName { ID = 0, Name = "Invalid user." };
                        }
                    }
                    else
                    {
                        objIDName = new IDName { ID = 0, Name = "Invalid user." };
                    }

                    objCryptorEngine = null;
                }
                catch (Exception ex)
                {
                    TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                    objIDName = new IDName { ID = 0, Name = String.Format("Error - {0}", ex.Message) };
                }
                finally
                {
                    objBusinessLayer.Dispose();
                    ls = null;
                }
            }
            else
            {
                objIDName = new IDName { ID = 0, Name = "Invalid user." };
            }

            return objIDName;
        }

        /// <summary>
        /// SearchADUser
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns>JsonResult<List<IDName>></IDName></returns>
        [HttpPost, Route("v1/User/SearchADUser")]
        public JsonResult<List<IDName>> SearchADUser([FromBody] LoginDTO loginDTO)
        {
            List<IDName> objIDNameList = new List<IDName>();
            string adPath = "sqs.group.intl";

            System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(String.Format("LDAP://{0}", adPath));
            System.DirectoryServices.DirectorySearcher searcher = new System.DirectoryServices.DirectorySearcher(entry);
            searcher.Filter = string.Format("(&(objectCategory=person)(objectClass=user)(!(userAccountControl:1.2.840.113556.1.4.803:=2))(|(SAMAccountName={0}*)(DisplayName={0}*)(mail={0}*)))", loginDTO.UserName);

            searcher.PropertiesToLoad.Add("mail");
            searcher.PropertiesToLoad.Add("samaccountname");
            searcher.PropertiesToLoad.Add("cn");
            searcher.PropertiesToLoad.Add("userAccountControl");

            System.DirectoryServices.SearchResultCollection results = null;
            int RecordFound = 0;
            try
            {
                results = searcher.FindAll();
                RecordFound = results.Count;
            }

            catch (Exception ex)
            {
                var objIDName = new IDName { Name = String.Format("Error: {0}", ex.Message) };

                objIDNameList.Add(objIDName);

                return Json(objIDNameList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            if (null != results && RecordFound > 0)
            {
                foreach (System.DirectoryServices.SearchResult p in results)
                {
                    var strName = "";
                    var strMail = "";

                    if (p.Properties.Contains("cn"))
                    {
                        strName = p.Properties["cn"] == null ? "" : Convert.ToString(p.Properties["cn"][0]);
                    }

                    if (p.Properties.Contains("mail"))
                    {
                        strMail = p.Properties["mail"] == null ? "" : Convert.ToString(p.Properties["mail"][0]);
                    }

                    if (!string.IsNullOrWhiteSpace(strMail))
                    {
                        objIDNameList.Add(new IDName { Name = strName, Other = strMail });
                    }
                }
            }
            else
            {
                var objIDName = new IDName { Name = "Error: User details not found." };

                objIDNameList.Add(objIDName);
            }

            objIDNameList = objIDNameList.OrderBy(x => x.Name).ToList<IDName>();

            return Json(objIDNameList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="name">string</param>
        /// <param name="description">string</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, ResponseType(typeof(JsonResult<string>)), Route("v1/User/PostUser")]
        public JsonResult<string> PostUser([FromBody] UserDTO userDTO)
        {
            JsonResult<string> result = Json(string.Empty);

            LoginSession ls = new LoginSession();
            ls.LoginName = "Register";

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {

                User objUser = Conversions.ToEntity<UserDTO, User>(userDTO);

                objBusinessLayer.UserRepository.Add(objUser);

                result = Json("Success!! Please refer to your email for activating your account.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                Utility objUtility = new Utility();
                objUtility.SendMail(objUser.Name, objUser.EmailID, objUser.ActivationKey, MailType.Welcome);

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
                {
                    if (ex.InnerException.InnerException.Message.IndexOf("Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_EmailID'") >= 0)
                    {
                        String strDuplicateEmail = ex.InnerException.InnerException.Message.Replace("Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_EmailID'. ", string.Empty).Replace("The duplicate key value is (", string.Empty).Replace(")", string.Empty).Replace("The statement has been terminated.", string.Empty);
                        result = Json(String.Format("Error - {0} is already registered.", strDuplicateEmail), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                    }
                    else if (ex.InnerException.InnerException.Message.IndexOf("Violation of UNIQUE KEY constraint 'IX_Users_EmailID'") >= 0)
                    {
                        String strDuplicateEmail = ex.InnerException.InnerException.Message.Replace("Violation of UNIQUE KEY constraint 'IX_Users_EmailID'. Cannot insert duplicate key in object 'dbo.Users'. ", string.Empty).Replace("The duplicate key value is (", string.Empty).Replace(")", string.Empty).Replace("The statement has been terminated.", string.Empty);
                        result = Json(String.Format("Error - {0} is already registered.", strDuplicateEmail), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                    }
                    else
                    {
                        result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                    }
                }
                else
                    result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        ///// <summary>
        ///// ResetPassword
        ///// </summary>
        ///// <param name="userDTO"></param>
        ///// <returns>JsonResult<string>UserDTO</returns>
        //[HttpPost, ResponseType(typeof(JsonResult<string>))]
        //[Route("v1/User/ResetPassword")]
        //public JsonResult<string> ResetPassword([FromBody] UserDTO userDTO)
        //{
        //    JsonResult<string> result = Json(string.Empty);

        //    LoginSession ls = new LoginSession();

        //    IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

        //    try
        //    {
        //        User objUser = objBusinessLayer.UserRepository.GetByID(int.Parse(userDTO.Id)); //CountryCode = User ID

        //        if (objUser != null)
        //        {
        //            if (objUser.Locked)
        //            {
        //                result = Json("Error - User account is locked, please contact moderator for unlocking your account.");
        //            }
        //            else if (!objUser.Activated)
        //            {
        //                result = Json("Error - User account is not activated, please refer to your email for activating the account.");
        //            }
        //            else
        //            {
        //                CryptorEngine objCryptorEngine = new CryptorEngine();

        //                if (objCryptorEngine.Decrypt(objUser.Password, true) == userDTO.Password)
        //                {
        //                    result = Json("Error - Old and new passwords can not be same.");
        //                }
        //                else
        //                {
        //                    objUser.Locked = false;
        //                    objUser.Password = userDTO.Password;
        //                    ls.LoginName = objUser.EmailID;

        //                    objBusinessLayer = new BusinessLayer(ls);

        //                    objBusinessLayer.UserRepository.ResetPassword(objUser);

        //                    result = Json("Password reset successful.");

        //                    Utility objUtility = new Utility();
        //                    objUtility.SendMail(String.Format("{0}, {1}", objUser.LastName, objUser.FirstName), objUser.EmailID, objUser.ActivationKey, MailType.PasswordReset);
        //                }

        //                objCryptorEngine = null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
        //        {
        //            result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
        //        }
        //        else
        //            result = Json(String.Format("Error - {0}", ex.Message));
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// ChangePassword
        ///// </summary>
        ///// <param name="userDTO"></param>
        ///// <returns>JsonResult<string>UserDTO</returns>
        //[HttpPost, ResponseType(typeof(JsonResult<string>))]
        //[Route("v1/User/ChangePassword")]
        //public JsonResult<string> ChangePassword([FromBody] UserDTO userDTO)
        //{
        //    JsonResult<string> result = Json(string.Empty);

        //    LoginSession ls = new LoginSession();

        //    IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

        //    try
        //    {
        //        User objUser = objBusinessLayer.UserRepository.GetByID(int.Parse(userDTO.Id));

        //        if (objUser != null)
        //        {
        //            CryptorEngine objCryptorEngine = new CryptorEngine();
        //            if (objCryptorEngine.Decrypt(objUser.Password, true) != userDTO.RePassword)
        //            {
        //                result = Json("Error - Old password is not correct.");
        //            }
        //            else if (objCryptorEngine.Decrypt(objUser.Password, true) == userDTO.Password)
        //            {
        //                result = Json("Error - Old and new passwords can not be same.");
        //            }
        //            else
        //            {
        //                objUser.Password = userDTO.Password;
        //                ls.LoginName = objUser.EmailID;

        //                objBusinessLayer = new BusinessLayer(ls);

        //                objBusinessLayer.UserRepository.ResetPassword(objUser);

        //                result = Json("Password reset successful.");

        //                Utility objUtility = new Utility();
        //                objUtility.SendMail(String.Format("{0}, {1}", objUser.LastName, objUser.FirstName), objUser.EmailID, objUser.ActivationKey, MailType.PasswordReset);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
        //        {
        //            result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
        //        }
        //        else
        //            result = Json(String.Format("Error - {0}", ex.Message));
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// ForgotPassword
        ///// </summary>
        ///// <param name="name">string</param>
        ///// <param name="description">string</param>
        ///// <returns>JsonResult<string></returns>
        //[HttpGet, Route("v1/User/ForgotPassword")]
        //public JsonResult<string> ForgotPassword(String emailID)
        //{
        //    JsonResult<string> result = Json(string.Empty);

        //    LoginSession ls = new LoginSession();
        //    ls.LoginName = "ForgotPassword";

        //    IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

        //    try
        //    {
        //        User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.EmailID.ToLower() == emailID.ToLower());

        //        if (objUser != null)
        //        {
        //            objUser.Locked = true;

        //            objBusinessLayer = new BusinessLayer(ls);
        //            objBusinessLayer.UserRepository.Update(objUser);

        //            result = Json("Please refer to your email for resetting your account password.");

        //            Utility objUtility = new Utility();
        //            objUtility.SendMail(String.Format("{0}, {1}", objUser.LastName, objUser.FirstName), objUser.EmailID, objUser.ActivationKey, MailType.ForgotPassword);
        //        }
        //        else
        //        {
        //            result = Json("Sorry we were not able to find your details in our system.");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
        //        {
        //            result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
        //        }
        //        else
        //            result = Json(String.Format("Error - {0}", ex.Message));
        //    }

        //    return result;
        //}

        /// <summary>
        /// SetActiveOrInactive
        /// </summary>
        /// <param name="userId">int</param>
        /// <param name="active">bool</param>
        /// <returns>JsonResult<string></returns>
        [HttpGet, Route("v1/User/SetActiveOrInactive/{userId}/{active}")]
        public JsonResult<string> SetActiveOrInactive(int userId, bool active)
        {
            JsonResult<string> result = Json(string.Empty);

            LoginSession ls = new LoginSession();

            string User = string.Empty;
            Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
            string userId1 = Request.Headers.GetValues("userid").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(userId1))
            {
                userId1 = userId1.Replace("~", "=").Replace("!", "+");
                User = objCryptorEngine.Decrypt(userId1, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                ls.LoginName = usrInfo[0];
            }

            try
            {
                IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userId);

                if (objUser != null)
                {
                    objUser.Activated = active;

                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.UserRepository.Update(objUser);

                    UserController user = new UserController();
                    string Activation = "Inactive";
                    if (active)
                    {
                        Activation = "Active";
                    }

                    user.UserActivationStatusAndSendMail(objUser.Name, objUser.EmailID, objUser.ActivationKey, "User Activation", Activation);

                    result = Json("User active status changed successfully.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                }
                else
                {
                    result = Json("Error - User not found.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
                {
                    result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
                else
                    result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// SetLockedOrUnlocked
        /// </summary>
        /// <param name="userId">int</param>
        /// <param name="locked">bool</param>
        /// <returns>JsonResult<string></returns>
        [HttpGet, Route("v1/User/SetLockedOrUnlocked/{userId}/{locked}")]
        public JsonResult<string> SetLockedOrUnlocked(int userId, bool locked)
        {
            JsonResult<string> result = Json(string.Empty);

            LoginSession ls = new LoginSession();

            string User = string.Empty;
            Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
            string userId1 = Request.Headers.GetValues("userid").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(userId1))
            {
                userId1 = userId1.Replace("~", "=").Replace("!", "+");
                User = objCryptorEngine.Decrypt(userId1, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                ls.LoginName = usrInfo[0];
            }

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userId);

                if (objUser != null)
                {
                    objUser.Locked = locked;

                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.UserRepository.Update(objUser);

                    result = Json("User locked status changed successfully.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                }
                else
                {
                    result = Json("Error - User not found.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
                {
                    result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
                else
                    result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// SetUserType
        /// </summary>
        /// <param name="userId">int</param>
        /// <param name="userType">int</param>
        /// <returns>JsonResult<string></returns>
        [HttpGet, Route("v1/User/SetUserType/{userId}/{userType}")]
        public JsonResult<string> SetUserType(int userId, int userType)
        {
            JsonResult<string> result = Json(string.Empty);
            LoginSession ls = new LoginSession();

            string User = string.Empty;
            Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
            string userId1 = Request.Headers.GetValues("userid").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(userId1))
            {
                userId1 = userId1.Replace("~", "=").Replace("!", "+");
                User = objCryptorEngine.Decrypt(userId1, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                ls.LoginName = usrInfo[0];
            }

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userId);

                if (objUser != null)
                {
                    if (-1 != userType)
                    {
                        objUser.UserTypeID = userType;
                    }
                    else
                    {
                        objUser.UserTypeID = null;
                    }

                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.UserRepository.Update(objUser);

                    result = Json("User type changed successfully.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                }
                else
                {
                    result = Json("Error - User not found.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
                {
                    result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
                else
                    result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// SetUserType
        /// </summary>
        /// <param name="userId">int</param>
        /// <param name="userType">int</param>
        /// <param name="sessionUserData">string</param>
        /// <returns>JsonResult<string></returns>
        [HttpGet, Route("v1/User/SetRole/{userId}/{roleId}/{sessionUserData}")]
        public JsonResult<string> SetRole(int userId, int roleId, string sessionUserData)
        {
            JsonResult<string> result = Json(string.Empty);

            LoginSession ls = new LoginSession();

            string User = string.Empty;
            Encryption.CryptorEngine objCryptorEngine = new Encryption.CryptorEngine();
            string userId1 = Request.Headers.GetValues("userid").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(userId1))
            {
                userId1 = userId1.Replace("~", "=").Replace("!", "+");
                User = objCryptorEngine.Decrypt(userId1, true);
                bool isAdmin = User.ToLower().Contains("admin");
                string[] usrInfo = User.Split('#');
                ls.LoginName = usrInfo[0];
            }

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == userId);
                CryptorEngine objCryptorEngine1 = new CryptorEngine();
                sessionUserData = sessionUserData.Replace("~", "=").Replace("!", "+").Replace("$", "/");
                string decrpitUserData = objCryptorEngine1.Decrypt(sessionUserData, true);
                string[] userData = Regex.Split(decrpitUserData, "#");

                if (userData != null)
                    if (userId.ToString().Equals(userData[5]))
                    {
                        result = Json("Error - User cann't change own role.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                        return result;
                    }

                if (objUser != null)
                {
                    if (-1 != roleId)
                    {
                        objUser.RoleId = roleId;
                    }
                    else
                    {
                        objUser.RoleId = null;
                    }

                    objBusinessLayer = new BusinessLayer(ls);
                    objBusinessLayer.UserRepository.Update(objUser);

                    result = Json("role changed successfully.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

                }
                else
                {
                    result = Json("Error - User not found.", new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (null != ex.InnerException && null != ex.InnerException.InnerException && !String.IsNullOrWhiteSpace(ex.InnerException.InnerException.Message))
                {
                    result = Json(String.Format("Error - {0}", ex.InnerException.InnerException.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                }
                else
                    result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        [ResponseType(typeof(DataCollection))]
        [HttpPost, Route("v1/User/GetUserByUserId")]
        public JsonResult<UserDTO> GetUserByUserId([FromBody] LoginDTO loginDTO)
        {
            int totalRecords = 0;
            IList<User> objUserList = FindUsersByUserId(loginDTO.UserName, out totalRecords);
            UserDTO objUserDTO = null;
            try
            {
                if (null != objUserList && objUserList.Count == 1)
                {
                    //We will not convert it into UserDTO so that we get object of user but 
                    //with 0 id and depicted as already exist
                    objUserDTO = new UserDTO();
                }
                else if (objUserList.Count == 0)
                {
                    //Create user only if required
                    if (loginDTO.ADUser)
                    {
                        objUserDTO = CreateADUser(loginDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw ex;
            }
            return Json(objUserDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        [HttpPost, Route("v1/User/GetUserRole")]
        public IDName GetUserRole([FromBody] LoginDTO loginDTO)
        {
            LoginSession ls = new LoginSession();

            IDName objIDName = null;

            if (!String.IsNullOrWhiteSpace(loginDTO.UserName))
            {

                loginDTO.UserName = loginDTO.UserName.Replace("!", "+").Replace("~", "=");

                IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

                try
                {
                    //var emailId = arrUserString[1].ToString().ToLower();
                    User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.UserId.ToLower() == loginDTO.UserName, x => x.Role);

                    if (objUser != null)
                    {
                        objIDName = new IDName
                        {
                            ID = objUser.ID,
                            Name = objUser.Name,
                            //Other = objCryptorEngine.Decrypt(objUser.Password, true),
                            Role = objUser.Role.Name
                        };
                    }
                    else
                    {
                        objIDName = new IDName { ID = 0, Name = "Invalid user." };
                    }
                }
                catch (Exception ex)
                {
                    TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                    objIDName = new IDName { ID = 0, Name = String.Format("Error - {0}", ex.Message) };
                }
                finally
                {
                    objBusinessLayer.Dispose();
                    ls = null;
                }
            }
            else
            {
                objIDName = new IDName { ID = 0, Name = "Invalid user." };
            }

            return objIDName;
        }

        private void GetUserByIdAndSendMail(int userid)
        {
            LoginSession ls = null;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                UserDTO userDTO = this.GetUserById(userid);
                Utility objUtility = new Utility();
                objUtility.SendMail(userDTO.Name, userDTO.EmailID, userDTO.ActivationKey, MailType.Welcome);

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
        }

        public void UserAssignmentAndSendMail(int userid, string strType, string strName, string TSRID, string assignBy, string Client)
        {
            LoginSession ls = null;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                UserDTO userDTO = this.GetUserById(userid);
                Utility objUtility = new Utility();
                objUtility.SendAssignMail(userDTO.Name, userDTO.EmailID, MailType.UserAssignment, strType, strName, TSRID, assignBy, Client);

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
        }

        public void TSOStatusUpdation(int userid, string strType, string strName, string TSRID, string assignBy, string status)
        {
            LoginSession ls = null;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                UserDTO userDTO = this.GetUserById(userid);
                Utility objUtility = new Utility();
                objUtility.SendTSOUpdation(userDTO.Name, userDTO.EmailID, MailType.TSOUpdation, strType, strName, TSRID, assignBy, status);

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
        }

        public void UserActivationStatusAndSendMail(string strName, string strEmail, string strActivationKey, string strType, string strActivationStatus)
        {
            LoginSession ls = null;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            try
            {
                Utility objUtility = new Utility();
                objUtility.SendMail(strName, strEmail, strActivationKey, MailType.UserActivation, strType, strActivationStatus);

            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
        }

        private UserDTO GetUserById(int id, bool loadEntities = true)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            UserDTO objUserDTO = null;

            try
            {
                User objUser = null;


                if (loadEntities)
                {
                    objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == id);

                }
                else
                {
                    objUser = objBusinessLayer.UserRepository.GetSingle(x => x.ID == id);
                }
                objUserDTO = Conversions.ToDTO<UserDTO, User>(objUser);
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

            return objUserDTO;
        }

        private IList<User> FindUsersByUserId(String nameOrEmail, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<User> objUserList = null;

            totalRecords = 0;
            try
            {
                objUserList = objBusinessLayer.UserRepository.GetList(x => x.EmailID.Trim().ToLower().Equals(nameOrEmail.Trim().ToLower()) || x.UserId.Trim().ToLower().Equals(nameOrEmail.Trim().ToLower()));
                totalRecords = objUserList.Count;
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw ex;
            }
            finally
            {
                ls = null;
                objBusinessLayer.Dispose();
            }

            if (null == objUserList)
            {
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return objUserList;
        }

        /// <summary>
        /// UpdateClientRegion
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="clientRegionDTO">ClientRegionDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/User/UpdateUser/{ID}")]
        public JsonResult<string> UpdateUser(int ID, [FromBody]UserDTO userDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyUser(ID, userDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        private string ModifyUser(int ID, UserDTO userDTO)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;

            try
            {
                UserDTO objUserDTOById = this.GetUserById(ID);

                if (objUserDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = userDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool recordupdated = false;

                    IBusinessLayer objBusinessLayer1 = new BusinessLayer(ls);

                    var CheckUserIDExists = objBusinessLayer1.UserRepository.GetSingle(x => x.ID == ID && (x.UserId.Trim().ToLower() != userDTO.UserId.Trim().ToLower() ||
                    x.EmailID.Trim().ToLower() != userDTO.EmailID.Trim().ToLower()));

                    if (CheckUserIDExists != null)
                    {
                        objUserDTOById.UserId = userDTO.UserId;
                        objUserDTOById.EmailID = userDTO.EmailID;
                        recordupdated = true;
                    }

                    if (recordupdated)
                        returnMessage = "'" + CheckUserIDExists.Name + "' updated successfully.";
                    else
                        returnMessage = "No changes found for '" + objUserDTOById.Name + "'.";

                    User objUser = Conversions.ToEntity<UserDTO, User>(objUserDTOById);
                    objBusinessLayer.UserRepository.Update(objUser);
                }
                else
                {
                    returnMessage = "User do not exists.";
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
    }
}
