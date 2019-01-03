/******************************************************************************
 *                          © 2018 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * PM 01Feb2018 Created the class
 *******************************************************************************/

using SQS.nTier.TTM.BAL;
using SQS.nTier.TTM.GenericFramework;
using SQS.nTier.TTM.WebAPI.Common;
using SQS.nTier.TTM.WebAPI.SessionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SQS.nTier.TTM.WebAPI.RoleAttribute
{
    public class SessionAuthorizeAttribute : AuthorizeAttribute
    {
        private IBusinessLayer repos = null;
        LoginSession ls = new LoginSession();
        private bool flag;

        public SessionAuthorizeAttribute()
        {
            repos = new BusinessLayer(ls);
        }
        public SessionAuthorizeAttribute(params string[] roles) : base()
        {
            repos = new BusinessLayer(ls);
            Roles = string.Join(",", roles);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            List<string> errorResponse;
            try
            {
                if (actionContext.Request.Headers.Contains("userid"))
                {
                    string UserId = actionContext.Request.Headers.GetValues("userid").First();

                    var userSessionManager = new UserSessionManager(repos, UserId, Roles);

                    if (userSessionManager.ReValidateSession(out errorResponse))
                    {
                        base.OnAuthorization(actionContext);
                    }
                    else
                    {
                        // throw new UnauthorizedAccessException(errorResponse.FirstOrDefault());
                        actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse((HttpStatusCode.Unauthorized), errorResponse.FirstOrDefault());
                    }
                }
                else
                {
                    actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse((HttpStatusCode.Unauthorized), "Unauthorized error");
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                throw ex;
            }
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return flag = true;
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}