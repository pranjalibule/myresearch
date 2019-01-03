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

namespace SQS.nTier.TTM.Logging.Command
{
    using CommandInterfaces;
    using Common;
    using NLog;
    using System;


    public class LogCommand : ILogCommand
    {
        private Logger nLogger;

        #region Properties        

        /// Returns if the logger is configured to error log level
        bool ILogCommand.CanLogError => nLogger.IsErrorEnabled;

        /// Returns if the logger is configured to info log level        
        bool ILogCommand.CanLogInfo => nLogger.IsInfoEnabled;

        // Returns if the logger is configured to debug log level
        bool ILogCommand.CanLogDebug => nLogger.IsDebugEnabled;

        #endregion

        /// <summary>
        /// THIS METHOD FOR INTILIAZING Logger Config file
        /// </summary>
        public LogCommand()
        {
            nLogger = LogManager.GetLogger(CommonConstants.LoggerName);
        }

        public string FormatErrorMessage(string ErrMessage)
        {
            if (ErrMessage.Contains("Error:"))
            {
                ErrMessage = ErrMessage.Replace("Error:", "");
            }
            else if (ErrMessage.Contains("Error -"))
            {
                ErrMessage = ErrMessage.Replace("Error -", "");
            }
            return ErrMessage;
        }



        /// <summary>
        /// this method used to log the if any error happened to log it based on the logger.config location
        /// </summary>
        /// <param name="sErrorMessage">method name</param>
        /// <param name="oErrorException">Exception</param>
        public void LogError(string sErrorMessage, Exception oErrorException = null)
        {
            //Only need to log the error if Error Level logger is enabled
            if (nLogger.IsErrorEnabled)
            {
               
                nLogger.Error(FormatErrorMessage(sErrorMessage));

                //Only log the exception if it happens. Also build the exception details with the current user
                if (oErrorException != null)
                    nLogger.Error(CommonFunctions.CreateErrorMessage(oErrorException));
            }
        }

        /// <summary>
        /// this method used to log the if any error happened to log it based on the logger.config location
        /// </summary>
        /// <param name="oErrorException">Exception</param>
        public void LogExceptionError(Exception oErrorException)
        {
            //Only need to log the error if Error Level logger is enabled
            if (nLogger.IsErrorEnabled)
            {
                //Only log the exception if it happens. Also build the exception details with the current user
                if (oErrorException != null)
                    nLogger.Error(CommonFunctions.CreateErrorMessage(oErrorException));
            }
        }

        /// <summary>
        /// log the Entity info message
        /// </summary>
        /// <param name="sInfoMessage">string of message </param>
        public void LogInfo(string sInfoMessage = "")
        {
            //Log the info if it is turned on
            if (nLogger.IsInfoEnabled)
            {
                nLogger.Info(FormatErrorMessage(sInfoMessage));
            }
        }

        public void LogDebug(string sDebugMessage = "")
        {
            //Log the info if it is turned on
            if (nLogger.IsInfoEnabled)
            {
                nLogger.Debug(sDebugMessage);
            }
        }
    }
}
