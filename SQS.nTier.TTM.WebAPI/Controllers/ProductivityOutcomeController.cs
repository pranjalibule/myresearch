
/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * Tushar Bharambe - 12th June 2018 Created the class
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
    /// ProductivityOutcomeController
    /// </summary>
    [RoutePrefix("api")]
    public class ProductivityOutcomeController : ApiController
    {
        #region Private Functions

        /// <summary>
        /// GetProductivityOutcomeById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>ProductivityOutcome</returns>
        private ProductivityOutcomeDTO GetProductivityOutcomeById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            ProductivityOutcomeDTO objProductivityOutcomeDTO = null;

            try
            {
                ProductivityOutcome objProductivityOutcome = objBusinessLayer.ProductivityOutcomeRepository.GetByID(id);
                if (null != objProductivityOutcome)
                {
                    objProductivityOutcomeDTO = new ProductivityOutcomeDTO();
                    objProductivityOutcomeDTO = Conversions.ToDTO<ProductivityOutcomeDTO, ProductivityOutcome>(objProductivityOutcome);
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

            return objProductivityOutcomeDTO;
        }


        /// <summary>
        /// GetProductivityOutcomeNameById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>ProductivityOutcome</returns>
        public string GetProductivityOutcomeNameById(int id)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            string pmName = "";
            try
            {
                ProductivityOutcome objProductivityOutcome = objBusinessLayer.ProductivityOutcomeRepository.GetByID(id);
                if (null != objProductivityOutcome)
                {
                    ProductivityOutcomeDTO objOutcomeDTO = new ProductivityOutcomeDTO();
                    objOutcomeDTO = Conversions.ToDTO<ProductivityOutcomeDTO, ProductivityOutcome>(objProductivityOutcome);
                    pmName = objOutcomeDTO.Name;
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

            return pmName;
        }

        /// <summary>
        /// GetAllProductivityOutcome
        /// </summary>
        /// <returns>IList<ProductivityOutcome>></returns>
        private IList<ProductivityOutcomeDTO> GetAllProductivityOutcome(int startingRecordNumber, int pageSize, out int totalRecords)
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<ProductivityOutcomeDTO> objPOutcomeDTOList = null;
            try
            {
                IList<ProductivityOutcome> objPOutcomeList = objBusinessLayer.ProductivityOutcomeRepository.GetAll(startingRecordNumber, pageSize, x => x.DisplayOrder, false, out totalRecords);
                if (objPOutcomeList != null && objPOutcomeList.Count > 0)
                {
                    objPOutcomeDTOList = new List<ProductivityOutcomeDTO>();

                    foreach (ProductivityOutcome objOutcome in objPOutcomeList)
                    {
                        ProductivityOutcomeDTO objPOutcomeDTO = new ProductivityOutcomeDTO();
                        objPOutcomeDTO = Conversions.ToDTO<ProductivityOutcomeDTO, ProductivityOutcome>(objOutcome);

                        objPOutcomeDTOList.Add(objPOutcomeDTO);
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

            return objPOutcomeDTOList;
        }

        /// <summary>
        /// Function to add new ProductivityOutcome
        /// </summary>
        /// <param name="objProductivityOutcomeDTO">ProductivityOutcomeDTO</param>
        /// <returns>string</returns>
        private string AddNewProductivityOutcome(ProductivityOutcomeDTO objProductivityOutcomeDTO)
        {
            string returnMessage = string.Empty;

            LoginSession ls = new LoginSession();
            ls.LoginName = objProductivityOutcomeDTO.CreatedBy;

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            try
            {
                bool result = CheckNameExists(objProductivityOutcomeDTO.Name, null);
                if (result == true)
                {

                    ProductivityOutcome objPOutcome = Conversions.ToEntity<ProductivityOutcomeDTO, ProductivityOutcome>(objProductivityOutcomeDTO);
                    objBusinessLayer.ProductivityOutcomeRepository.Add(objPOutcome);

                    returnMessage = "Productivity Outcome added successfully.";
                }
                else
                {
                    returnMessage = "This " + objProductivityOutcomeDTO.Name + " record already exists.";
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
            var CheckNameExists = objBusinessLayer.ProductivityOutcomeRepository.GetList(x => x.Name == Name && x.ID == ID);
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
        /// Function to update ProductivityOutcome 
        /// </summary>
        /// <param name="objProductivityOutcomeDTO">ProductivityOutcomeDTO</param>
        /// <returns>string</returns>
        private string ModifyProductivityOutcome(int ID, ProductivityOutcomeDTO objProductivityOutcomeDTO)
        {

            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                ProductivityOutcomeDTO objPInputDTOById = this.GetProductivityOutcomeById(ID);

                if (objPInputDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = objProductivityOutcomeDTO.CreatedBy;

                    objBusinessLayer = new BusinessLayer(ls);
                    bool result = CheckNameExists(objProductivityOutcomeDTO.Name, ID);
                    if (result == true)
                    {
                        objPInputDTOById.Name = objProductivityOutcomeDTO.Name;
                        objPInputDTOById.Description = objProductivityOutcomeDTO.Description;
                        returnMessage = "Productivity Outcome updated successfully.";
                    }
                    else
                    {
                        objPInputDTOById.Description = objProductivityOutcomeDTO.Description;
                        returnMessage = "This " + objProductivityOutcomeDTO.Name + " record already exists.";
                    }
                    ProductivityOutcome objPInput = Conversions.ToEntity<ProductivityOutcomeDTO, ProductivityOutcome>(objPInputDTOById);
                    objBusinessLayer.ProductivityOutcomeRepository.Update(objPInput);
                }
                else
                {
                    returnMessage = "Productivity Outcome do not exists.";
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
        /// RemoveProductivityOutcome
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>string</returns>
        private string RemoveProductivityOutcome(int ID)
        {
            string returnMessage = string.Empty;
            LoginSession ls = null;
            IBusinessLayer objBusinessLayer = null;
            try
            {
                ProductivityOutcomeDTO objProductivityOutcomeDTOById = this.GetProductivityOutcomeById(ID);

                if (objProductivityOutcomeDTOById != null)
                {
                    ls = new LoginSession();
                    ls.LoginName = "Admin";

                    objBusinessLayer = new BusinessLayer(ls);

                    ProductivityOutcome objPOutcome = Conversions.ToEntity<ProductivityOutcomeDTO, ProductivityOutcome>(objProductivityOutcomeDTOById);
                    objBusinessLayer.ProductivityOutcomeRepository.Delete(objPOutcome);
                    returnMessage = "Productivity Outcome deleted successfully.";

                }
                else
                {
                    returnMessage = "Productivity Outcome do not exists.";
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.InnerException.InnerException.Message));
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    throw new Exception("Specified project model mapped with TSR / TSO.");
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
        /// GetProductivityOutcomeIDNameList
        /// </summary>
        /// <returns>IList<IDName></returns>
        internal IList<IDName> GetProductivityOutcomeIDNameList()
        {
            LoginSession ls = new LoginSession();

            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

            IList<IDName> objProductivityOutcomeList = null;
            try
            {
                objProductivityOutcomeList = objBusinessLayer.ProductivityOutcomeRepository.GetPartial(x => new IDName { ID = x.ID, Name = x.Name }, x => x.DisplayOrder, null);
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

            return objProductivityOutcomeList;
        }

        #endregion

        [HttpGet, Route("v1/ProductivityOutcome/GetAllIDName")]
        [ResponseType(typeof(IList<IDName>))]
        public JsonResult<IList<IDName>> GetAllIDName()
        {
            IList<IDName> objProductivityOutcomeList = null;

            try
            {
                objProductivityOutcomeList = this.GetProductivityOutcomeIDNameList();
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw;
            }
            return Json(objProductivityOutcomeList, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }


        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>IHttpActionResult></returns>
        [HttpGet, Route("v1/ProductivityOutcome/GetById/{id}")]
        [ResponseType(typeof(ProductivityOutcome))]
        public JsonResult<ProductivityOutcomeDTO> GetById(int id)
        {
            JsonResult<ProductivityOutcomeDTO> result = null;
            ProductivityOutcomeDTO objProductivityOutcomeDTO;

            try
            {
                objProductivityOutcomeDTO = this.GetProductivityOutcomeById(id);

                result = Json(objProductivityOutcomeDTO, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        [HttpGet, Route("v1/ProductivityOutcome/GetAllPaged/{startingRecordNumber}/{pageSize}")]
        [ResponseType(typeof(DataCollection))]
        public JsonResult<DataCollection> GetAllPaged(int startingRecordNumber, int pageSize)
        {
            int totalRecords = 0;
            IList<ProductivityOutcomeDTO> objProductivityOutcomeDTOList = this.GetAllProductivityOutcome(startingRecordNumber, pageSize, out totalRecords);

            DataCollection objDataCollection;

            try
            {
                objDataCollection = new DataCollection { TotalRecords = totalRecords, EntitySummary = new List<IBaseObject>() };
                if (null != objProductivityOutcomeDTOList)
                {
                    foreach (ProductivityOutcomeDTO objPOutcome in objProductivityOutcomeDTOList)
                    {
                        objDataCollection.EntitySummary.Add(objPOutcome);
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
        /// CreateProductivityOutcome
        /// </summary>
        /// <param name="ProductivityOutcomeDTO">ProductivityOutcomeDTO</param>
        /// <returns>JsonResult<string></returns>
        [HttpPost, Route("v1/ProductivityOutcome/CreateProductivityOutcome")]
        [ResponseType(typeof(string))]
        public JsonResult<string> CreateProductivityOutcome([FromBody]ProductivityOutcomeDTO ProductivityOutcomeDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.AddNewProductivityOutcome(ProductivityOutcomeDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }


        /// <summary>
        /// ProductivityOutcome
        /// </summary>
        /// <param name="ID">int</param>
        /// <param name="ProductivityOutcomeDTO">ProductivityOutcomeDTO</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpPut, Route("v1/ProductivityOutcome/UpdateProductivityOutcome/{ID}")]
        [ResponseType(typeof(string))]
        public JsonResult<string> UpdateProductivityOutcome(int ID, [FromBody]ProductivityOutcomeDTO ProductivityOutcomeDTO)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {
                result = Json(this.ModifyProductivityOutcome(ID, ProductivityOutcomeDTO), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            catch (Exception ex)
            {
                result = Json(String.Format("Error - {0}", ex.Message), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }

            return result;
        }


        /// <summary>
        /// DeleteProductivityOutcome
        /// </summary>
        /// <param name="ID">int</param>
        /// <returns>JsonResult<string></returns>
        [ResponseType(typeof(string))]
        [HttpDelete, Route("v1/ProductivityOutcome/DeleteProductivityOutcome/{ID}")]
        public JsonResult<string> DeleteProductivityOutcome(int ID)
        {
            JsonResult<string> result = Json(string.Empty);
            try
            {

                result = Json(this.RemoveProductivityOutcome(ID), new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
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
        /// <param name="disposing">bool</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
