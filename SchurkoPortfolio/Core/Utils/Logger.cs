namespace SchurkoPortfolio.Core.Utils
{
    public class Log
    {

        private static ILoggerFactory loggerFactory = (ILoggerFactory)new LoggerFactory();

        private static Microsoft.Extensions.Logging.ILogger? _logger;
        public static Microsoft.Extensions.Logging.ILogger Logger =>
            _logger ?? (_logger = loggerFactory.CreateLogger("Logger"));

        public Log()
        {

        }

    }
}
