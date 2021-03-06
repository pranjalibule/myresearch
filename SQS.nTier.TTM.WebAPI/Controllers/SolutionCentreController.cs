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
    /// SolutionCentreController
    /// </summary>
    [RoutePrefix("api")]
    public class SolutionCentreController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetSolutionCentreById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>SolutionCentre</returns>
        private SolutionCentreDTO GetSolutionCentreById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            SolutionCentreDTO objSolutionCentreDTO = null;

            try
            {
                SolutionCentre objSolutionCentre = objBusinessLayer.SolutionCentreRepository.GetByID(id);
                if (null != objSolutionCentre)
                {
                    objSolutionCentreDTO = new SolutionCentreDTO();
                    objSolutionCentreDTO = Conversions.ToDTO<SolutionCentreDTO, SolutionCentre>(objSolutionCentre);
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

            return objSolutionCentreDTO;
        }

        /// <summary>
        /// GetAllSolutionCentre
        /// </summary>
        /// <returns>IList<SolutionCentre>></returns>
        private IList<SolutionCentreDTO> GetAllSolutionCentre(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<SolutionCentreDTO> objSolutionCentreDTOList = null;
            try
            {
                IList<SolutionCentre> objSolutionCentreList = objBusinessLayer.SolutionCentreRepository.GetAll(startingRecordNumber, pageSize, x => x.ID, false, out totalRecords);
                if (objSolutionCentreList != null && objSolutionCentreList.Count > 0)
                {
                    objSolutionCentreDTOList = new List<SolutionCentreDTO>();

                    foreach (SolutionCentre objSolutionCentre in objSolutionCentreList)
                    {
                        SolutionCentreDTO objSolutionCentreDTO = new SolutionCentreDTO();
                        objSolutionCentreDTO = Conversions.ToDTO<SolutionCentreDTO, SolutionCentre>(objSolutionCentre);

                        objSolutionCentreDTOList.Add(objSolutionCentreDTO);
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

            return objSolutionCentreDTOList;
        }

        /// <summary>
        /// Function to add new SolutionCentre 
        /// </summary>
        /// <param name="objSolutionCentre">SolutionCentreDTO</param>
        /// <returns>string</returns>
        private string AddNewSolutionCentre(SolutionCentreDTO objSolutionCentreDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objSolutionCentreDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objSolutionCentreDTO.Name, null);
                if (result == true)
                {
                    SolutionCentre objSolutionCentre = Conversions.ToEntity<SolutionCentreDTO, SolutionCentre>(objSolutionCentreDTO);
                    objBusinessLayer.SolutionCentreRepository.Add(objSolutionCentre);
                    returnMessage = "Solution Centre added successfully.";
                }
                else
                {
                    returnMessage = "This " + objSolutionCentreDTO.Name + " Solution Centre already exists.";
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
            var CheckNameExists = objBusinessLayer.SolutionCentreRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update RelevantRepository 
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="objSolutionCentreDTO">SolutionCentreDTO</param>
        /// <returns>string</returns>
        private string ModifySolutionCentre(int ID, SolutionCentreDTO objSolutionCentreDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                SolutionCentreDTO objSolutionCentreDTOById = this.GetSolutionCentreById(ID);

                if (objSolutionCentreDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objSolutionCentreDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objSolutionCentreDTO.Name, ID);
                    if (result == true)
                    {
                        objSolutionCentreDTOById.Name = objSolutionCentreDTO.Name;
                        objSolutionCentreDTOById.Description = objSolutionCentreDTO.Description;
                        returnMessage = "Solution Centre updated successfully.";
                    }
                    else
                    {
                        objSolutionCentreDTOById.Description = objSolutionCentreDTO.Description;
                        returnMessage = "This " + objSolutionCentreDTO.Name + " Solution Centre already exists.";
                    }
                    SolutionCentre objSolutionCentre = Conversions.ToEntity<SolutionCentreDTO, SolutionCentre>(objSolutionCentreDTOById);

                    objBusinessLayer.SolutionCentreRepository.Update(objSolutionCentre);


                }
                else
                {
                    returnMessage = "Solution Centre do not exists.";
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
        /// RemoveSolutionCentre
        /// </summary>
        /// <param name="ID">int</param>
        private string RemoveSolutionCentre(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                SolutionCentreDTO objSolutionCentreDTOById = this.GetSolutionCentreById(ID);

                if (objSolutionCentreDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    SolutionCentre objSolutionCentre = Conversions.ToEntity<SolutionCentreDTO, SolutionCentre>(objSolutionCentreDTOById);
                    objBusinessLayer.SolutionCentreRepository.Delete(objSolutionCentre);
                    returnMessage = "Solution Centre deleted successfully.";

                }
                else
                {
                    returnMessage = "Solution Centre do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified solution center mapped with TSR / TSO.");
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
        /// GetSolutionCentreIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetSolutionCentreIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objSolutionCentreList = null;
            try
            {
                objSolutionCentreList = objBusinessLayer.SolutionCentreRepository.GetPartial<IDName>(x => new IDName { ID = x.ID, Name = x.Name });
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

            return objSolutionCentreList;
        }

        #endregion

        [HttpGet, Route("v1/SolutionCentre/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objSolutionCentreList = null;

            try
            {
                objSolutionCentreList = this.GetSolutionCentreIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objSolutionCentreList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/SolutionCentre/GetById/{id}")]
        [ResponseType(typeof(SolutionCentre))]
        public JsonResult<SolutionCentreDTO> GetById(int id)
        {
            JsonResult<SolutionCentreDTO> result = null;
            SolutionCentreDTO objSolutionCentreDTO;

            try
            {
                objSolutionCentreDTO = this.GetSolutionCentreById(id);

                result = Json(objSolutionCentreDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/SolutionCentre/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<SolutionCentreDTO> objSolutionCentreDTOList = this.GetAllSolutionCentre(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objSolutionCentreDTOList)
                {
                    foreach (SolutionCentreDTO objSolutionCentre in objSolutionCentreDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objSolutionCentre);
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
        /// CreateSolutionCentre
        /// </summary>
        /// <param name="solutionCentreDTO">SolutionCentreDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/SolutionCentre/CreateSolutionCentre")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateSolutionCentre([FromBody]SolutionCentreDTO solutionCentreDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewSolutionCentre(solutionCentreDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// UpdateRelevantRepository
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="solutionCentreDTO">SolutionCentreDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/SolutionCentre/UpdateSolutionCentre/{ID}")]
        public JsonResult<string> UpdateSolutionCentre(int ID, [FromBody]SolutionCentreDTO solutionCentreDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifySolutionCentre(ID, solutionCentreDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }

        /// <summary>
        /// DeleteSolutionCentre
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/SolutionCentre/DeleteSolutionCentre/{ID}")]
        public JsonResult<string> DeleteSolutionCentre(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveSolutionCentre(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// <param name="disposing">bool</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
