/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 18Sep2017 Created the class
 *******************************************************************************/

using SQS.nTier.TTM.BAL;
using SQS.nTier.TTM.DAL;
using SQS.nTier.TTM.Encryption;
using SQS.nTier.TTM.GenericFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace SQS.nTier.TTM.WebAPI.Common
{
    public class Utility
    {
        //string[] adPathArr = { "sqs.group.intl", "de.group.intl" };
        string adPaths = ConfigurationManager.AppSettings["adPathArr"].ToString();

        #region Private Variables

        private readonly string Host;
        int Port;
        string EmailUserName;
        string EmailPassword;
        string FromEmailID;
        string FromEmailDisplay;
        string AdministratorEmail;
        string ApplicationUrl;
        string Domain;
        string IsLive;
        string DefaultMail;

        #endregion

        #region Private Methods

        /// <summary>
        /// GetSMTPClient
        /// </summary>
        /// <returns>SmtpClient</returns>
        private SmtpClient GetSMTPClient()
        {
            SmtpClient oclient = new SmtpClient(Host, Port);

            oclient.EnableSsl = true;
            oclient.DeliveryMethod = SmtpDeliveryMethod.Network;
            oclient.UseDefaultCredentials = false;
            oclient.Credentials = new NetworkCredential(EmailUserName, EmailPassword);

            return oclient;
        }

        /// <summary>
        /// GetMailMessage
        /// </summary>
        /// <param name="message">string</param>
        /// <param name="subject">string</param>
        /// <returns></returns>
        private MailMessage GetMailMessage(string message, string subject)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FromEmailID, FromEmailDisplay); //From Email Id
            mailMessage.Subject = subject; //Subject of Email
            mailMessage.Body = message; //body or message of Email
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }

        #endregion

        #region Default Constructor

        /// <summary>
        /// Utility
        /// </summary>
        public Utility()
        {
            CryptorEngine objCryptorEngine = new CryptorEngine();

            Host = ConfigurationManager.AppSettings["Host"];
            Port = ConfigurationManager.AppSettings["Port"] == null ? 0 : int.Parse(ConfigurationManager.AppSettings["Port"]);
            EmailUserName = ConfigurationManager.AppSettings["EmailUserName"];
            EmailPassword = ConfigurationManager.AppSettings["EmailPwd"];
            FromEmailID = ConfigurationManager.AppSettings["FromEmail"];
            FromEmailDisplay = ConfigurationManager.AppSettings["FromEmailDisplay"];
            AdministratorEmail = ConfigurationManager.AppSettings["AdministratorEmail"];
            ApplicationUrl = ConfigurationManager.AppSettings["ApplicationUrl"];
            Domain = ConfigurationManager.AppSettings["Domain"];
            IsLive = ConfigurationManager.AppSettings["IsLive"];
            DefaultMail = ConfigurationManager.AppSettings["DefultMail"];

            objCryptorEngine = null;
        }

        #endregion

        public IList<User> GetUsersFromADOnly(String nameOrEmail, out int totalRecords)
        {
            SearchResultCollection results = null;

            LoginSession ls = new LoginSession();
            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);
            List<User> objUserList = new List<User>();
            totalRecords = 0;

            nameOrEmail = nameOrEmail.Trim().ToLower();
            string[] adPathArr = adPaths.Split(',');

            try
            {
                Parallel.For(0, adPathArr.Length, index =>
                {
                    string currentADPath = adPathArr[index].Trim();
                    if (!string.IsNullOrEmpty(currentADPath))
                    {
                        try
                        {
                            string LDAPPath = "LDAP://" + currentADPath;

                            using (HostingEnvironment.Impersonate())
                            {
                                DirectoryEntry entry = new DirectoryEntry(LDAPPath);
                                entry.RefreshCache();

                                DirectorySearcher searcher = new DirectorySearcher(entry);
                                searcher.CacheResults = false;

                                searcher.Filter = string.Format("(&(objectClass=user)(|(sAMAccountName={0}*)(SN={0}*)(GivenName={0}*)(CN={0}*)(Name={0}*)(mail={0}*)(DisplayName={0}*)))", nameOrEmail);

                                searcher.PropertiesToLoad.Add("mail");
                                searcher.PropertiesToLoad.Add("samaccountname");
                                searcher.PropertiesToLoad.Add("givenname");
                                searcher.PropertiesToLoad.Add("sn");
                                searcher.PropertiesToLoad.Add("cn");
                                searcher.PropertiesToLoad.Add("description");
                                searcher.PropertiesToLoad.Add("department");
                                searcher.PropertiesToLoad.Add("manager");
                                searcher.PropertiesToLoad.Add("userAccountControl");

                                results = searcher.FindAll();

                                foreach (SearchResult p in results)
                                {
                                    User currentuser = new User();

                                    if (p.Properties.Contains("cn"))
                                        currentuser.Name = p.Properties["cn"] == null ? "" : Convert.ToString(p.Properties["cn"][0]);
                                    else
                                        currentuser.Name = "";

                                    if (p.Properties.Contains("mail"))
                                        currentuser.EmailID = p.Properties["mail"] == null ? "" : Convert.ToString(p.Properties["mail"][0]);
                                    else
                                        currentuser.EmailID = "";

                                    if (p.Properties.Contains("samaccountname"))
                                        currentuser.UserId = p.Properties["samaccountname"] == null ? "" : Convert.ToString(p.Properties["samaccountname"][0]);
                                    else
                                        currentuser.UserId = "";

                                    if (p.Properties.Contains("userAccountControl"))
                                    {
                                        int flags = (int)p.Properties["userAccountControl"][0];
                                        bool IsActive = !Convert.ToBoolean(flags & 0x0002);
                                        if (IsActive == true)
                                        {
                                            if (!objUserList.Select(x => x.Name).Contains(currentuser.Name) &&
                                               !objUserList.Select(x => x.EmailID).Contains(currentuser.EmailID))
                                                objUserList.Add(currentuser);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                            //throw;
                        }
                    }
                });
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

            return objUserList;
        }

        /// <summary>
        /// SendWelcomeMail
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        public void SendMail(string userName, string emailId, string activationKey, MailType objMailType, string strType = "", string strName = "")
        {
            try
            {
                //SmtpClient settings
                SmtpClient oclient = GetSMTPClient();

                string urlForPasting = string.Empty;

                if (ApplicationUrl.Contains("https"))
                {
                    urlForPasting = ApplicationUrl.Replace("https:", "<span>https:</span>");
                }
                else
                {
                    urlForPasting = ApplicationUrl.Replace("http:", "<span>http:</span>");
                }

                urlForPasting = urlForPasting.Replace("//", "<span>//</span>");

                string strMessage = string.Empty;
                string strSubject = string.Empty;

                if (objMailType == MailType.Welcome)
                {
                    strMessage = string.Format(ServiceWebConstants.WelcomeMailText, userName, ApplicationUrl, activationKey, urlForPasting, activationKey);
                    strSubject = "Welcome to TTM Portal";
                }
                else if (objMailType == MailType.ForgotPassword)
                {
                    strMessage = string.Format(ServiceWebConstants.ForgotMailText, userName, ApplicationUrl, activationKey, urlForPasting, activationKey);
                    strSubject = "TTM Portal - Reset account password";
                }
                else if (objMailType == MailType.PasswordReset)
                {
                    strMessage = string.Format(ServiceWebConstants.PasswordResetMailText, userName);
                    strSubject = "TTM Portal - Reset account password";
                }
                else if (objMailType == MailType.UserAssignment)
                {
                    strMessage = string.Format(ServiceWebConstants.UserAssignmentMailText, userName, strType, strName);
                    strSubject = "TTM Portal - You are assigned";
                }
                else if (objMailType == MailType.UserActivation)
                {
                    strMessage = string.Format(ServiceWebConstants.UserActivationMailText, userName, strType, strName);
                    strSubject = "TTM Portal - Your account status is changed to ‘Active/Inactive’ ";
                }
                //Getting the object of MailMessage
                MailMessage mailMessage = GetMailMessage(strMessage, strSubject);

                if (IsLive.ToLower() == "true")
                {
                    mailMessage.To.Add(emailId);
                }
                else
                {
                    mailMessage.To.Add(DefaultMail);
                }

                if (!string.IsNullOrWhiteSpace(AdministratorEmail))
                    mailMessage.CC.Add(AdministratorEmail);

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                oclient.Send(mailMessage); //sending Email
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendAssignMail(string userName, string emailId, MailType objMailType, string strType, string strName, string TSRID, string assignBy, string Client)
        {
            try
            {
                //SmtpClient settings
                SmtpClient oclient = GetSMTPClient();

                string urlForPasting = string.Empty;

                if (ApplicationUrl.Contains("https"))
                {
                    urlForPasting = ApplicationUrl.Replace("https:", "<span>https:</span>");
                }
                else
                {
                    urlForPasting = ApplicationUrl.Replace("http:", "<span>http:</span>");
                }

                urlForPasting = urlForPasting.Replace("//", "<span>//</span>");

                string strMessage = string.Empty;
                string strSubject = string.Empty;
                if (objMailType == MailType.UserAssignment)
                {
                    strMessage = string.Format(ServiceWebConstants.UserAssignmentMailText, userName, strType, strName, TSRID, assignBy, Client);
                    strSubject = "TTM Portal - You are assigned";
                }
                //Getting the object of MailMessage
                MailMessage mailMessage = GetMailMessage(strMessage, strSubject);

                if (IsLive.ToLower() == "true")
                {
                    mailMessage.To.Add(emailId);
                }
                else
                {
                    mailMessage.To.Add(DefaultMail);
                }

                if (!string.IsNullOrWhiteSpace(AdministratorEmail))
                    mailMessage.CC.Add(AdministratorEmail);

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                oclient.Send(mailMessage); //sending Email
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendMail(string userName, string Useremailid)
        {
            try
            {
                //SmtpClient settings
                SmtpClient oclient = GetSMTPClient();

                string urlForPasting = string.Empty;

                if (ApplicationUrl.Contains("https"))
                    urlForPasting = ApplicationUrl.Replace("https:", "<span>https:</span>");
                else
                    urlForPasting = ApplicationUrl.Replace("http:", "<span>http:</span>");

                urlForPasting = urlForPasting.Replace("//", "<span>//</span>");

                string strMessage = string.Empty;
                string strSubject = string.Empty;

                strMessage = string.Format(ServiceWebConstants.ActivationEmailText, userName);
                strSubject = "TTM Portal Access Request";


                MailMessage mailMessage = GetMailMessage(strMessage, strSubject);
                mailMessage.To.Add(AdministratorEmail);
                mailMessage.CC.Add(Useremailid);

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                oclient.Send(mailMessage); //sending Email
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SendTSOUpdation(string userName, string emailId, MailType objMailType, string strType, string strName, string TSRID, string assignBy, string Status)
        {
            try
            {
                //SmtpClient settings
                SmtpClient oclient = GetSMTPClient();

                string urlForPasting = string.Empty;

                if (ApplicationUrl.Contains("https"))
                {
                    urlForPasting = ApplicationUrl.Replace("https:", "<span>https:</span>");
                }
                else
                {
                    urlForPasting = ApplicationUrl.Replace("http:", "<span>http:</span>");
                }

                urlForPasting = urlForPasting.Replace("//", "<span>//</span>");

                string strMessage = string.Empty;
                string strSubject = string.Empty;
                if (objMailType == MailType.TSOUpdation)
                {
                    strMessage = string.Format(ServiceWebConstants.TSOUpdationMail, userName, strType, strName, TSRID, assignBy, Status);
                    strSubject = "TTM Portal - TSO status update";
                }
                //Getting the object of MailMessage
                MailMessage mailMessage = GetMailMessage(strMessage, strSubject);

                if (IsLive.ToLower() == "true")
                {
                    mailMessage.To.Add(emailId);
                }
                else
                {
                    mailMessage.To.Add(DefaultMail);
                }

                if (!string.IsNullOrWhiteSpace(AdministratorEmail))
                    mailMessage.CC.Add(AdministratorEmail);

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                oclient.Send(mailMessage); //sending Email
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
