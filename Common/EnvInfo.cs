namespace KrzaqRPG.Backend.Common
{
    public static class EnvInfo
    {
        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}
