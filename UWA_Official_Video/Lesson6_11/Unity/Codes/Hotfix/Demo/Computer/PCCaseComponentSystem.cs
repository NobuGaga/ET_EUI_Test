namespace ET
{
    public static class PCCaseComponentSystem
    {

        public static void StartPower(this PCCaseComponent self)
        {
            Log.Debug(self.ToString());
            Log.Debug("start power!!!!!!");
        }
    }
}