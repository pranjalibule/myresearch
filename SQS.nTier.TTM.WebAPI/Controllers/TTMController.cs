using SQS.nTier.TTM.WebAPI.Helpers;
using SQS.nTier.TTM.WebAPI.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace SQS.nTier.TTM.WebAPI.Controllers
{
    [RoutePrefix("api")]
    public class TTMController : ApiController
    {

        /// <summary>
        /// API HttpGet call to return all tasks
        /// </summary>
        /// <returns>Collection of <seealso cref="TTMTask"/></returns>
        [HttpGet, Route("v1/TTM/GetAllTasksAPI")]
        [AllowAnonymous]
        public IEnumerable<TTMTask> Get()
        {
            return DataHelper.GetAllTasks();
        }

    }
}