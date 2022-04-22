// Battle Review Before Boss Node

using ET.EventType;

namespace ET
{
    [ObjectSystem]
    public class BattleLogicComponentAwakeSystem : AwakeSystem<BattleLogicComponent>
    {
        public override void Awake(BattleLogicComponent self)
        {
            BattleLogicComponent.Instance = self;
            self.FireInfo = new C2M_Fire();
            self.HitInfo = new C2M_Hit();
            self.UseSkillInfo = new C2M_SkillUse();
        }
    }

    [ObjectSystem]
    public class BattleLogicComponentDestroySystem : DestroySystem<BattleLogicComponent>
    {
        public override void Destroy(BattleLogicComponent self)
        {
            BattleLogicComponent.Instance = null;
            self.FireInfo = null;
            self.HitInfo = null;
            self.UseSkillInfo = null;
        }
    }

    public static class BattleLogicComponentSystem
    {
        /// <summary> 加快获取 CurrentScene 效率, 覆盖重写 Entity 的 CurrentScene() </summary>
        internal static Scene CurrentScene(this BattleLogicComponent self)
        {
            Scene scene = self.Parent as Scene;
            return BattleConfig.IsAddToCurrentScene ? scene : scene.CurrentScene();
        }

        /// <summary> 加快获取 ZoneScene 效率, 覆盖重写 Entity 的 ZoneScene() </summary>
        internal static Scene ZoneScene(this BattleLogicComponent self)
        {
            Scene scene = self.Parent as Scene;
            return BattleConfig.IsAddToZoneScene ? scene : scene.Parent.Parent as Scene;
        }

        /// <summary>
        /// 获取战斗组件所在的 Scene, 所有战斗相关的管理组件都在这个 Scene 下
        /// 因为 Game.Scene 里获取到的可能为空(ZoneScene or CurrentScene)
        /// 所以调用的时候随便传入一个 Scene (客户端只有一个 ZoneScene 跟一个 CurrentScene)
        /// </summary>
        public static Scene BattleScene(this Scene self)
        {
            bool isCurrentScene = self.SceneType == SceneType.Current;
            bool isZoneScene = self.SceneType == SceneType.Zone;
            if ((BattleConfig.IsAddToCurrentScene && isCurrentScene) ||
                (BattleConfig.IsAddToZoneScene && isZoneScene))
                return self;

            return isZoneScene ? self.CurrentScene() : self.Parent.Parent as Scene;
        }

        /// <summary> 拓展实现 Scene 方法, 方便获取 BattleLogicComponent </summary>
        public static BattleLogicComponent GetBattleLogicComponent(this Scene scene)
        {
            Scene battleScene = scene.BattleScene();
            return battleScene.GetComponent<BattleLogicComponent>();
        }

        /// <summary> 碰撞处理 </summary>
        public static void Collide(this BattleLogicComponent self, float screenPosX, float screenPosY,
                                                                   long bulletUnitId, long fishUnitId)
        {
            Scene currentScene = self.CurrentScene();
            if (currentScene == null)
                return;

            // 子弹的移除都是本地客户端判定是否碰撞来进行
            BulletLogicComponent bulletLogicComponent = currentScene.GetComponent<BulletLogicComponent>();
            Unit bulletUnit = bulletLogicComponent.GetChild<Unit>(bulletUnitId);
            var attributeComponent = bulletUnit.GetComponent<NumericComponent>();
            int seatId = attributeComponent.GetAsInt(NumericType.Pos);
            var fisheryComponent = currentScene.GetComponent<FisheryComponent>();
            Unit playerUnit = fisheryComponent.GetPlayerUnit(seatId);
            long playerUnitId = playerUnit.Id;
            Unit selfPlayerUnit = UnitHelper.GetMyUnitFromCurrentScene(currentScene);

            if (playerUnitId == selfPlayerUnit.Id)
                self.C2M_Hit(screenPosX, screenPosY, bulletUnitId, fishUnitId);

            bulletLogicComponent.RemoveUnit(bulletUnitId);

            Game.EventSystem.Publish(new BulletCollideFish()
            {
                CurrentScene = currentScene,
                ScreenPosX = screenPosX,
                ScreenPosY = screenPosY,
                PlayerUnitId = playerUnitId,
                FishUnitId = fishUnitId,
            });
        }
    }

    // Event 的执行不依赖顺序
    // 目前订阅了 AfterCreateZoneScene 事件的处理只有 AfterCreateZoneScene_AddComponent
    // 避免别的事件执行顺序的依赖, 战斗相关组件释放要做好解耦断引用
    public class AfterCreateZoneScene_BattleLogicComponent : AEvent<AfterCreateZoneScene>
    {
        protected override void Run(AfterCreateZoneScene args)
        {
            if (BattleConfig.IsAddToZoneScene)
                args.ZoneScene.AddComponent<BattleLogicComponent>();
        }
    }

    public class AfterCreateCurrentScene_BattleLogicComponent : AEvent<AfterCreateCurrentScene>
    {
        protected override void Run(AfterCreateCurrentScene args)
        {
            Scene currentScene = args.CurrentScene;
            
            if (BattleConfig.IsAddToCurrentScene)
                currentScene.AddComponent<BattleLogicComponent>();

            currentScene.AddComponent<FisheryComponent>();
            currentScene.AddComponent<SkillComponent>();
            currentScene.AddComponent<BulletLogicComponent>();
        }
    }
}