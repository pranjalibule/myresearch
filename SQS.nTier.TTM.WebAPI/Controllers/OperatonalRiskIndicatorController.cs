/******************************************************************************
 *                          © 2018 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * Pranjali Mankar 09Sept2018 Created the class
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
    /// OperatonalRiskIndicatorController
    /// </summary>
    [RoutePrefix("api")]
    public class OperatonalRiskIndicatorController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetOperatonalRiskIndicatorById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>OperatonalRiskIndicator</returns>
        private OperationalRiskIndicatorDTO GetOperatonalRiskIndicatorById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            OperationalRiskIndicatorDTO objPracticeDTO = null;

            try
            {
                OperationalRiskIndicator objPractice = objBusinessLayer.OperationalRiskIndicatorRepository.GetByID(id);
                if (null != objPractice)
                {
                    objPracticeDTO = new OperationalRiskIndicatorDTO();
                    objPracticeDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objPractice);
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

            return objPracticeDTO;
        }

        /// <summary>
        /// GetAllPractice
        /// </summary>
        /// <returns>IList<Practice>></returns>
        private IList<OperationalRiskIndicatorDTO> GetAllOperatonalRiskIndicator(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<OperationalRiskIndicatorDTO> objPracticeDTOList = null;
            try
            {
                IList<OperationalRiskIndicator> objPracticeList = objBusinessLayer.OperationalRiskIndicatorRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objPracticeList != null && objPracticeList.Count > 0)
                {
                    objPracticeDTOList = new List<OperationalRiskIndicatorDTO>();

                    foreach (OperationalRiskIndicator objPractice in objPracticeList)
                    {
                        OperationalRiskIndicatorDTO objPracticeDTO = new OperationalRiskIndicatorDTO();
                        objPracticeDTO = Conversions.ToDTO<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objPractice);

                        objPracticeDTOList.Add(objPracticeDTO);
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

            return objPracticeDTOList;
        }

        /// <summary>
        /// Function to add new OperatonalRiskIndicator 
        /// </summary>
        /// <param name="objPractice"></param>
        /// <returns></returns>
        private string AddNewOperatonalRiskIndicator(OperationalRiskIndicatorDTO objPracticeDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objPracticeDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objPracticeDTO.Name, null);
                if (result == true)
                {
                    OperationalRiskIndicator objPractice = Conversions.ToEntity<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objPracticeDTO);
                    objBusinessLayer.OperationalRiskIndicatorRepository.Add(objPractice);
                    returnMessage = "Category added successfully.";
                }
                else
                {
                    objPracticeDTO.Description = objPracticeDTO.Description;
                    returnMessage = "This " + objPracticeDTO.Name + " record already exists.";
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
            var CheckNameExists = objBusinessLayer.OperationalRiskIndicatorRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update OperatonalRiskIndicator 
        /// </summary>
        /// <param name="objPractice"></param>
        /// <returns></returns>
        private string ModifyOperatonalRiskIndicator(int ID, OperationalRiskIndicatorDTO objPracticeDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                OperationalRiskIndicatorDTO objPracticeDTOById = this.GetOperatonalRiskIndicatorById(ID);

                if (objPracticeDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objPracticeDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objPracticeDTO.Name, ID);
                    if (result == true)
                    {
                        objPracticeDTOById.Name = objPracticeDTO.Name;
                        objPracticeDTOById.Description = objPracticeDTO.Description;
                        returnMessage = "Category updated successfully.";
                    }
                    else
                    {
                        objPracticeDTOById.Description = objPracticeDTO.Description;
                        returnMessage = "This " + objPracticeDTO.Name + " record already exists.";
                    }
                    OperationalRiskIndicator objPractice = Conversions.ToEntity<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objPracticeDTOById);

                    objBusinessLayer.OperationalRiskIndicatorRepository.Update(objPractice);


                }
                else
                {
                    returnMessage = "Category do not exists.";
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
        /// Remove OperatonalRiskIndicator
        /// </summary>
        /// <param name="ID">int</param>
        private string RemoveOperatonalRiskIndicator(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                OperationalRiskIndicatorDTO objPracticeDTOById = this.GetOperatonalRiskIndicatorById(ID);

                if (objPracticeDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    OperationalRiskIndicator objPractice = Conversions.ToEntity<OperationalRiskIndicatorDTO, OperationalRiskIndicator>(objPracticeDTOById);
                    objBusinessLayer.OperationalRiskIndicatorRepository.Delete(objPractice);
                    returnMessage = "Category deleted successfully.";

                }
                else
                {
                    returnMessage = "Category do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified practice mapped with TSR / TSO.");
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
        /// GetOperatonalRiskIndicatorIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetOperatonalRiskIndicatorIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objPracticeList = null;
            try
            {
                objPracticeList = objBusinessLayer.OperationalRiskIndicatorRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

            return objPracticeList;
        }

        #endregion

        [HttpGet, Route("v1/OperatonalRiskIndicator/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objPracticeList = null;

            try
            {
                objPracticeList = this.GetOperatonalRiskIndicatorIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objPracticeList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/OperatonalRiskIndicator/GetById/{id}")]
        [ResponseType(typeof(Practice))]
        public JsonResult<OperationalRiskIndicatorDTO> GetById(int id)
        {
            JsonResult<OperationalRiskIndicatorDTO> result = null;
            OperationalRiskIndicatorDTO objPracticeDTO;

            try
            {
                objPracticeDTO = this.GetOperatonalRiskIndicatorById(id);

                result = Json(objPracticeDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpGet, Route("v1/OperatonalRiskIndicator/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<OperationalRiskIndicatorDTO> objPracticeDTOList = this.GetAllOperatonalRiskIndicator(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objPracticeDTOList)
                {
                    foreach (OperationalRiskIndicatorDTO objPractice in objPracticeDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objPractice);
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
        /// CreatePractice
        /// </summary>
        /// <param name="practiceDTO">PracticeDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/OperatonalRiskIndicator/CreateOperatonalRiskIndicator")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateOperatonalRiskIndicator([FromBody]OperationalRiskIndicatorDTO practiceDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewOperatonalRiskIndicator(practiceDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdatePractice
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="practiceDTO">PracticeDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/OperatonalRiskIndicator/UpdateOperatonalRiskIndicator/{ID}")]
        public JsonResult<string> UpdateOperatonalRiskIndicator(int ID, [FromBody]OperationalRiskIndicatorDTO practiceDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyOperatonalRiskIndicator(ID, practiceDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeletePractice
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [HttpDelete, Route("v1/OperatonalRiskIndicator/DeleteOperatonalRiskIndicator/{ID}")]
        [ResponseType(typeof(string))]
        public JsonResult<string> DeleteOperatonalRiskIndicator(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveOperatonalRiskIndicator(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
