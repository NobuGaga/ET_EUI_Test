namespace ET
{
    public class ComputerAwakeSystem: AwakeSystem<Computer>
    {
        public override void Awake(Computer self)
        {
            Log.Debug("ComputerAwake!!!!");
        }
    }

    public class ComputerUpdateSystem : UpdateSystem<Computer>
    {
        public override void Update(Computer self)
        {
            //Log.Debug("Computer update!!!!");
        }
    }

    public class ComputerDestorySystem : DestroySystem<Computer>
    {
        public override void Destroy(Computer self)
        {
            Log.Debug("Computer Destory");
        }
    }

    public static class ComputerSystem
    {
        public static void Start(this Computer self)
        {
            Log.Debug("Computer Start!!!!!!!");
            self.GetComponent<PCCaseComponent>().StartPower();
            self.GetComponent<MonitorsComponent>().Display();

            self.ZoneScene();
            self.DomainScene();
            var domain = self.Domain;
        }
    }
}