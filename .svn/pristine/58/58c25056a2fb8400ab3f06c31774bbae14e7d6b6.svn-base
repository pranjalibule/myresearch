/******************************************************************************
 *                          © 2018 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 13Feb2018 Created the class
 *******************************************************************************/

using System;
using System.Configuration;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace SQS.nTier.TTM.Logging.Common
{
    internal class CommonFunctions
    {
        // Standard Function to Read a given key / value pair from Web Config.
        // Usage example = sConnectString = GetConfigurationValue(CommonFunctions.ConfigurationKeys.ConnectString)
        public static string GetConfigurationValue(string sKey)
        {
            return ConfigurationManager.AppSettings[sKey];
        }

        /// <summary>
        /// Create and Format Error Message.
        /// </summary>
        /// <returns></returns>
        public static string CreateErrorMessage(Exception exception)
        {

            var s = new StringBuilder();
            s.Append(Environment.NewLine);
            s.Append(new String('=', 80));
            s.Append(Environment.NewLine);

            // get the current executing user.
            IIdentity identity = WindowsIdentity.GetCurrent();
            if (identity != null)
            {
                s.Append("Executing Identity: " + identity.Name);
            }

            s.Append(BuildWebDetailsMessage());
            s.AppendLine(BuildExceptionMessage(exception));
            s.Append(new String('=', 80));
            s.Append(Environment.NewLine);

            return s.ToString();
        }

        private static string BuildWebDetailsMessage()
        {

            var s = new StringBuilder();

            if ((HttpContext.Current != null))
            {

                // get the ip address, sometimes this is different when setup at crox
                // so we get the ip of the user, not the load balancer
                string ipAddress = HttpContext.Current.Request.Headers["rlnclientipaddr"];
                string userAgent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
                string absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
                string referrer = (HttpContext.Current.Request.UrlReferrer != null)
                    ? HttpContext.Current.Request.UrlReferrer.ToString()
                    : "No Referrer";

                if (string.IsNullOrEmpty(ipAddress))
                {
                    // otherwise get it the normal way ie. host other than crox
                    ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                s.Append(Environment.NewLine);
                s.Append("User Agent: " + userAgent);
                s.Append(Environment.NewLine);
                s.Append("Referrer: " + referrer);
                s.Append(Environment.NewLine);
                s.Append("Path: " + absoluteUri);
                s.Append(Environment.NewLine);
                s.Append("IP Address: " + ipAddress);
                s.Append(Environment.NewLine);
            }

            return s.ToString();
        }

        private static string BuildExceptionMessage(Exception exception)
        {

            if (exception != null)
            {
                var s = new StringBuilder();
                int exceptionLevel = 0;

                s.Append(Environment.NewLine);
                s.Append(new String('.', 80));
                s.Append(Environment.NewLine);
                s.Append(DateTime.Now);
                s.Append(Environment.NewLine);
                s.Append(Environment.NewLine);

                while ((exception != null))
                {
                    //--Loop through all exceptions 
                    exceptionLevel += 1;

                    //-- Build the exception strings 
                    s.Append(exceptionLevel + ": Error Description: " + exception.Message + Environment.NewLine);

                    if (exception.Source != null)
                        s.Append(exceptionLevel + ": Source: " +
                                 exception.Source.Replace(Environment.NewLine,
                                     Environment.NewLine + exceptionLevel + ": ") + Environment.NewLine);

                    if (exception.StackTrace != null)
                        s.Append(exceptionLevel + ": Stack Trace: " +
                                 exception.StackTrace.Replace(Environment.NewLine,
                                     Environment.NewLine + exceptionLevel + ": ") + Environment.NewLine);

                    if (exception.TargetSite != null)
                        s.Append(exceptionLevel + ": Target Site: " +
                                 exception.TargetSite.ToString()
                                     .Replace(Environment.NewLine, Environment.NewLine + exceptionLevel + ": ") +
                                 Environment.NewLine + Environment.NewLine);

                    //-- Get the next inner exception to log 
                    exception = exception.InnerException;
                }

                s.Append(new String('.', 80));
                s.Append(Environment.NewLine);
                return s.ToString();
            }

            return String.Empty;
        }
    }
}
