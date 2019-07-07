namespace Hanabi
{
    public static class Logger
    {
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void InitLogger()
        {
            //BasicConfigurator.Configure();
        }
    }
}
