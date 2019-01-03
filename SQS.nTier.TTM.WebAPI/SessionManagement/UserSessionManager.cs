﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Security.Claims;
using System.Globalization;
using System.Net;
using SQS.nTier.TTM.BAL;
using SQS.nTier.TTM.DAL;
using SQS.nTier.TTM.Encryption;
using SQS.nTier.TTM.GenericFramework;

namespace SQS.nTier.TTM.WebAPI.SessionManagement
{
    public class UserSessionManager
    {
        protected IBusinessLayer repos { get; private set; }
        string UserId = string.Empty;
        string Roles;
      
        public UserSessionManager(IBusinessLayer repos,string UserId , string Roles)
        {
            this.repos = repos;
            this.UserId = UserId;
            this.Roles = Roles;
        }


        private HttpRequestMessage CurrentRequest
        {
            get
            {
                return (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"];
            }
        }
        private User GetCurrentUser()
        {
            int userID = decriptUser(UserId);
            User user;// = repos.UserRepository.GetSingle(x=>x.ID == userID, x => x.Role);

            LoginSession ls = new LoginSession();

            repos = new BusinessLayer(ls);           
            try
            {
               user =  repos.UserRepository.GetSingle(x => x.ID == userID, x => x.Role);
            }
            catch
            {
                throw;
            }
            finally
            {
                ls = null;
                repos.Dispose();
            }
            
            return user;
        }

      
        public bool ReValidateSession(out List<string> errorResponse)
        {
            bool flag = false;      
            var currentUser = this.GetCurrentUser();
            errorResponse = new List<string>();        
                   
            if (currentUser != null)
            {
               flag = Roles.Contains(currentUser.Role.Name);
                if (flag == true)
                {
                    return true;
                }
                else
                {
                    errorResponse.Add("Do not have permission on it.");
                    return false;
                }
            }
            return flag;
        }

        private int decriptUser(string userid)
        {
            string userID = userid.Replace("~", "=").Replace("!", "+");
            CryptorEngine objCryptorEngine = new CryptorEngine();
            userID = objCryptorEngine.Decrypt(userID, true);
            string[] usrInfo = userID.Split('#');
            userID = usrInfo[usrInfo.Length-2];
            return Convert.ToInt32(userID);
        }

        //public void DeleteExpiredSessions()
        //{
        //   // repos.UserRepository.DeleteExpiredSession();
        //}
    }
}