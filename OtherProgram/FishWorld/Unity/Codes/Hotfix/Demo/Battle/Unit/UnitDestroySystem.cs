// Battle Review Before Boss Node

namespace ET
{
    [ObjectSystem]
    public class UnitDestroySystem : DestroySystem<Unit>
    {
        public override void Destroy(Unit self)
        {
            self.AttributeComponent = null;
            self.TransformComponent = null;
            self.FishUnitComponent = null;
            self.BulletUnitComponent = null;
            self.PlayerSkillComponent = null;
            self.GameObjectComponent = null;
            self.BattleUnitViewComponent = null;
        }
    }
}