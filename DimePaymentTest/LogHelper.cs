using log4net;
using System;

namespace DimePaymentTest
{
    public class LogHelper
    {
        private static readonly ILog _debugLogger;
        private static ILog GetLogger(string logName)
        {
            return LogManager.GetLogger(logName);
        }

        static LogHelper()
        {
            _debugLogger = GetLogger("DimePaymentTestLog");
        }

        public static void Debug(string message, Exception ex)
        {
            _debugLogger.Debug(message, ex);
        }

        public static void Info(string message, Exception ex)
        {
            _debugLogger.Info(message, ex);
        }

        public static void Error(string message, Exception ex)
        {
            _debugLogger.Error(message, ex);
        }
    }
}
