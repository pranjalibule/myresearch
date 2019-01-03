/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 1Dec2016 Created the class
 *******************************************************************************/
namespace SQS.nTier.TTM.GenericFramework
{
    using System;

    public static class LoginSessionManager
    {
        private static object lockObject;
        private static LoginSession systemLogin;

        [ThreadStatic]
        public static string loginSessionID = string.Empty;

        static LoginSessionManager()
        {
            lockObject = new object();
        }

        public static LoginSession StartLoginSession(
            int userID, string loginName, string userCode, int personID, int authenticationType)
        {
            lock (lockObject)
            {
                if (null == systemLogin)
                {
                    Guid g = Guid.NewGuid();
                    loginSessionID = g.ToString(); ;
                    systemLogin = new LoginSession()
                    {
                        LoginName = loginName,
                        UserID = userID,
                        IsAuthenticated = true,
                        SessionID = g
                    };
                }

                return systemLogin;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginSessionID"></param>
        public static void StopLoginSession(string loginSessionID)
        {
            systemLogin = null;
        }
    }
}
