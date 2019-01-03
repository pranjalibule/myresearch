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

    public class LoginSession
    {
        private int sessionTimeout;
        private object lockObject;

        public LoginSession()
        {
            sessionTimeout = FrameworkSharedConstants.ClientSession.DefaultSessionTimeout;
            SessionID = Guid.NewGuid();
            IsAuthenticated = false;
            lockObject = new object();
            LoginTime = DateTime.Now;
        }

        public string LoginName { get; set; }
        public int UserID { get; set; }

        public string Password { get; set; }
        public DateTime LoginTime { get; private set; }

        public Guid SessionID { get; set; }


        public bool IsAuthenticated { get; set; }

        public override bool Equals(object obj)
        {
            LoginSession rhs = obj as LoginSession;

            if (rhs == null)
                return false;

            return this.SessionID.Equals(rhs.SessionID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int SessionTimeout
        {
            private get
            {
                return sessionTimeout;
            }
            set
            {
                sessionTimeout = value;
            }
        }

        public object FrameworkServerConstants { get; private set; }
    }
}
