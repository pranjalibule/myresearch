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

namespace SQS.nTier.TTM.Logging.CommandInterfaces
{
    public interface ILogCommand
    {
        /// <summary>
        /// Error messages with exceptions should be logged using this
        /// </summary>
        /// <param name="sErrorMessage">Error message to log</param>
        /// <param name="oErrorException">Exception from the error</param>
        void LogError(string sErrorMessage, Exception oErrorException);

        /// <summary>
        /// Error messages with exceptions should be logged using this
        /// </summary>
        /// <param name="oErrorException">Exception from the error</param>
        void LogExceptionError(Exception oErrorException);

        /// <summary>
        /// Info messages, such as method usage should be logged using this
        /// </summary>
        /// <param name="sErrorMessage">Error message to log</param>
        void LogInfo(string sInfoMessage = "");

        /// <summary>
        /// Debug messages, such as method or object parameters should be logged using this
        /// </summary>
        /// <param name="sErrorMessage">Error message to log</param>
        void LogDebug(string sDebugMessage = "");

        /// <summary>
        /// Returns if the logger is configured to error log level
        /// </summary>        
        bool CanLogError { get; }

        /// <summary>
        /// Returns if the logger is configured to info log level
        /// </summary>
        bool CanLogInfo { get; }

        /// <summary>
        /// Returns if the logger is configured to debug log level
        /// </summary>
        bool CanLogDebug { get; }
    }
}
