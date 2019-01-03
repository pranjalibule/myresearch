using SQS.nTier.TTM.Logging.Command;

namespace SQS.nTier.TTM.WebAPI.Common
{
    /// <summary>
    /// TTMLogger
    /// </summary>
    public class TTMLogger
    {
        private static LogCommand logger = null;

        public TTMLogger()
        {
            logger = new LogCommand();
        }

        /// <summary>
        /// Logger
        /// </summary>
        public static LogCommand Logger
        {
            get
            {
                if (null == logger)
                {
                    logger = new LogCommand();
                }
                return logger;
            }
        }
    }
}