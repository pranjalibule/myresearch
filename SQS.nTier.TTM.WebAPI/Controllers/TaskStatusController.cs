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
    using RoleAttribute;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Results;

    /// <summary>
    /// TaskStatusController
    /// </summary>
    [RoutePrefix("api")]
    public class TaskStatusController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetTaskStatusById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>TaskStatus</returns>
        private TaskStatusDTO GetTaskStatusById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            TaskStatusDTO objTaskStatusDTO = null;

            try
            {
                TaskStatus objTaskStatus = objBusinessLayer.TaskStatusRepository.GetByID(id);
                if (null != objTaskStatus)
                {
                    objTaskStatusDTO = new TaskStatusDTO();
                    objTaskStatusDTO = Conversions.ToDTO<TaskStatusDTO, TaskStatus>(objTaskStatus);
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

            return objTaskStatusDTO;
        }

        /// <summary>
        /// GetAllTaskStatus
        /// </summary>
        /// <returns>IList<TaskStatus>></returns>
        private IList<TaskStatusDTO> GetAllTaskStatus(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<TaskStatusDTO> objTaskStatusDTOList = null;
            try
            {
                IList<TaskStatus> objTaskStatusList = objBusinessLayer.TaskStatusRepository.GetAll(startingRecordNumber, pageSize, x => x.DisplayOrder, false, out totalRecords);
                if (objTaskStatusList != null && objTaskStatusList.Count > 0)
                {
                    objTaskStatusDTOList = new List<TaskStatusDTO>();

                    foreach (TaskStatus objTaskStatus in objTaskStatusList)
                    {
                        TaskStatusDTO objTaskStatusDTO = new TaskStatusDTO();
                        objTaskStatusDTO = Conversions.ToDTO<TaskStatusDTO, TaskStatus>(objTaskStatus);

                        objTaskStatusDTOList.Add(objTaskStatusDTO);
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

            return objTaskStatusDTOList;
        }

        /// <summary>
        /// Function to add new TaskStatus 
        /// </summary>
        /// <param name="objTaskStatus"></param>
        /// <returns></returns>
        private string AddNewTaskStatus(TaskStatusDTO objTaskStatusDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objTaskStatusDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objTaskStatusDTO.Name, null);
                if (result == true)
                {
                    TaskStatus objTaskStatus = Conversions.ToEntity<TaskStatusDTO, TaskStatus>(objTaskStatusDTO);
                    objBusinessLayer.TaskStatusRepository.Add(objTaskStatus);
                    returnMessage = "Task Status added successfully.";
                }
                else
                {
                    returnMessage = "This " + objTaskStatusDTO.Name + " Task Status already exists.";
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
            var CheckNameExists = objBusinessLayer.TaskStatusRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update TaskStatus 
        /// </summary>
        /// <param name="objTaskStatus"></param>
        /// <returns>string</returns>
        private string ModifyTaskStatus(int ID, TaskStatusDTO objTaskStatusDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                TaskStatusDTO objTaskStatusDTOById = this.GetTaskStatusById(ID);

                if (objTaskStatusDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objTaskStatusDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objTaskStatusDTO.Name, ID);
                    if (result == true)
                    {
                        objTaskStatusDTOById.Name = objTaskStatusDTO.Name;
                        objTaskStatusDTOById.Description = objTaskStatusDTO.Description;
                        returnMessage = "Task Status updated successfully.";
                    }
                    else
                    {
                        objTaskStatusDTOById.Description = objTaskStatusDTO.Description;
                        returnMessage = "This " + objTaskStatusDTO.Name + " Task Status already exists.";
                    }
                    TaskStatus objTaskStatus = Conversions.ToEntity<TaskStatusDTO, TaskStatus>(objTaskStatusDTOById);

                    //objTaskStatus = Conversions.ToEntity<TaskStatusDTO, TaskStatus>(objTaskStatusDTO, objTaskStatus);
                    objBusinessLayer.TaskStatusRepository.Update(objTaskStatus);


                }
                else
                {
                    returnMessage = "Task Status do not exists.";
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
        /// DeleteTaskStatus
        /// </summary>
        /// <param name="objTaskStatusDTO">TaskStatusDTO</param>
        private string RemoveTaskStatus(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                TaskStatusDTO objTaskStatusDTOById = this.GetTaskStatusById(ID);

                if (objTaskStatusDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    TaskStatus objTaskStatus = Conversions.ToEntity<TaskStatusDTO, TaskStatus>(objTaskStatusDTOById);
                    objBusinessLayer.TaskStatusRepository.Delete(objTaskStatus);
                    returnMessage = "Task Status deleted successfully.";

                }
                else
                {
                    returnMessage = "Task Status do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified Task Status mapped with Task.");
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
        /// GetTaskStatusIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetTaskStatusIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objTaskStatusList = null;
            try
            {
                objTaskStatusList = objBusinessLayer.TaskStatusRepository.GetPartial(x => new IDName { ID = x.ID, Name = x.Name }, x => x.DisplayOrder, null);
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

            return objTaskStatusList;
        }

        #endregion

        [HttpGet, Route("v1/TaskStatus/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        [SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,TMO,Team Lead,PQM,Guest")]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objTaskStatusList = null;

            try
            {
                objTaskStatusList = this.GetTaskStatusIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objTaskStatusList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/TaskStatus/GetById/{id}")]
        [ResponseType(typeof(TaskStatus))]
        public JsonResult<TaskStatusDTO> GetById(int id)
        {
            JsonResult<TaskStatusDTO> result = null;
            TaskStatusDTO objTaskStatusDTO;

            try
            {
                objTaskStatusDTO = this.GetTaskStatusById(id);

                result = Json(objTaskStatusDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpGet, Route("v1/TaskStatus/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        // comment : When we come to Task Status html page, error shown for userid. need to work on it.
        //[SessionAuthorize(Roles = "Admin,Account Manager,Delivery Manager,Test Manager,Team Lead,PQM,Guest")]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<TaskStatusDTO> objTaskStatusDTOList = this.GetAllTaskStatus(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objTaskStatusDTOList)
                {
                    foreach (TaskStatusDTO objTaskStatus in objTaskStatusDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objTaskStatus);
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
        /// CreateTaskStatus
        /// </summary>
        /// <param name="TaskStatusDTO">TaskStatusDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/TaskStatus/CreateTaskStatus")]
        [ResponseType(typeof(string))]
        [SessionAuthorize(Roles = "Admin")]
        public JsonResult<string> CreateTaskStatus([FromBody]TaskStatusDTO TaskStatusDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewTaskStatus(TaskStatusDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateTaskStatus
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="userTypeDTO">UserTypeDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/TaskStatus/UpdateTaskStatus/{ID}")]
        [SessionAuthorize(Roles = "Admin,Test Manager,TMO,Team Lead")]
        public JsonResult<string> UpdateTaskStatus(int ID, [FromBody]TaskStatusDTO TaskStatusDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyTaskStatus(ID, TaskStatusDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteTaskStatus
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/TaskStatus/DeleteTaskStatus/{ID}")]
        [SessionAuthorize(Roles = "Admin")]
        public JsonResult<string> DeleteTaskStatus(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveTaskStatus(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
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
