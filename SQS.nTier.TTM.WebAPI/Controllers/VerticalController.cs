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
    /// VerticalController
    /// </summary>
    [RoutePrefix("api")]
    public class VerticalController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetVerticalById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>Vertical</returns>
        private VerticalDTO GetVerticalById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            VerticalDTO objVerticalDTO = null;

            try
            {
                Vertical objVertical = objBusinessLayer.VerticalRepository.GetByID(id);
                if (null != objVertical)
                {
                    objVerticalDTO = new VerticalDTO();
                    objVerticalDTO = Conversions.ToDTO<VerticalDTO, Vertical>(objVertical);
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

            return objVerticalDTO;
        }

        /// <summary>
        /// GetAllVertical
        /// </summary>
        /// <returns>IList<Vertical>></returns>
        private IList<VerticalDTO> GetAllVertical(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<VerticalDTO> objVerticalDTOList = null;
            try
            {
                IList<Vertical> objVerticalList = objBusinessLayer.VerticalRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objVerticalList != null && objVerticalList.Count > 0)
                {
                    objVerticalDTOList = new List<VerticalDTO>();

                    foreach (Vertical objVertical in objVerticalList)
                    {
                        VerticalDTO objVerticalDTO = new VerticalDTO();
                        objVerticalDTO = Conversions.ToDTO<VerticalDTO, Vertical>(objVertical);

                        objVerticalDTOList.Add(objVerticalDTO);
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

            return objVerticalDTOList;
        }

        /// <summary>
        /// Function to add new Vertical 
        /// </summary>
        /// <param name="objVertical"></param>
        /// <returns></returns>
        private string AddNewVertical(VerticalDTO objVerticalDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objVerticalDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objVerticalDTO.Name, null);
                if (result == true)
                {
                    Vertical objVertical = Conversions.ToEntity<VerticalDTO, Vertical>(objVerticalDTO);
                    objBusinessLayer.VerticalRepository.Add(objVertical);
                    returnMessage = "Vertical added successfully.";
                }
                else
                {
                    returnMessage = "This " + objVerticalDTO.Name + " Vertical already exists.";
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
            var CheckNameExists = objBusinessLayer.VerticalRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update Vertical 
        /// </summary>
        /// <param name="objVertical"></param>
        /// <returns></returns>
        private string ModifyVertical(int ID, VerticalDTO objVerticalDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                VerticalDTO objVerticalDTOById = this.GetVerticalById(ID);

                if (objVerticalDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objVerticalDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objVerticalDTO.Name, ID);
                    if (result == true)
                    {
                        objVerticalDTOById.Name = objVerticalDTO.Name;
                        objVerticalDTOById.Description = objVerticalDTO.Description;
                        returnMessage = "Vertical updated successfully.";
                    }
                    else
                    {
                        objVerticalDTOById.Description = objVerticalDTO.Description;
                        returnMessage = "This " + objVerticalDTO.Name + " Vertical already exists.";
                    }
                    Vertical objVertical = Conversions.ToEntity<VerticalDTO, Vertical>(objVerticalDTOById);

                    objBusinessLayer.VerticalRepository.Update(objVertical);


                }
                else
                {
                    returnMessage = "Vertical do not exists.";
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
        /// RemoveVertical
        /// </summary>
        /// <param name="ID">int</param>
        private string RemoveVertical(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                VerticalDTO objVerticalDTOById = this.GetVerticalById(ID);

                if (objVerticalDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    Vertical objVertical = Conversions.ToEntity<VerticalDTO, Vertical>(objVerticalDTOById);
                    objBusinessLayer.VerticalRepository.Delete(objVertical);
                    returnMessage = "Vertical deleted successfully.";

                }
                else
                {
                    returnMessage = "Vertical do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified vertical mapped with TSR / TSO.");
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
        /// GetVerticalIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetVerticalIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objVerticalList = null;
            try
            {
                objVerticalList = objBusinessLayer.VerticalRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

            return objVerticalList;
        }

        #endregion

        [HttpGet, Route("v1/Vertical/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objVerticalList = null;

            try
            {
                objVerticalList = this.GetVerticalIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objVerticalList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/Vertical/GetById/{id}")]
        [ResponseType(typeof(Vertical))]
        public JsonResult<VerticalDTO> GetById(int id)
        {
            JsonResult<VerticalDTO> result = null;
            VerticalDTO objVerticalDTO;

            try
            {
                objVerticalDTO = this.GetVerticalById(id);

                result = Json(objVerticalDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpGet, Route("v1/Vertical/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<VerticalDTO> objVerticalDTOList = this.GetAllVertical(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objVerticalDTOList)
                {
                    foreach (VerticalDTO objVertical in objVerticalDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objVertical);
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
        /// CreateVertical
        /// </summary>
        /// <param name="practiceDTO">VerticalDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/Vertical/CreateVertical")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateVertical([FromBody]VerticalDTO practiceDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewVertical(practiceDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateVertical
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="practiceDTO">VerticalDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/Vertical/UpdateVertical/{ID}")]
        public JsonResult<string> UpdateVertical(int ID, [FromBody]VerticalDTO practiceDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyVertical(ID, practiceDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteVertical
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [HttpDelete, Route("v1/Vertical/DeleteVertical/{ID}")]
        [ResponseType(typeof(string))]
        public JsonResult<string> DeleteVertical(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveVertical(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
