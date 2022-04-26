// Battle Review Before Boss Node

using ET.EventType;

namespace ET
{
    [FriendClass(typeof(Unit))]
    public class RemoveUnit_UnitComponent : AEventClass<RemoveUnit>
    {
        protected override void Run(object obj)
        {
            var args = obj as RemoveUnit;
            UnitComponent unitComponent = args.CurrentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.Get(args.UnitId);
            if (unit.Type != UnitType.Fish)
                return;

            AnimatorComponent animatorComponent = unit.GetComponent<AnimatorComponent>();
            if (animatorComponent != null)
                animatorComponent.PauseAnimator();

            TransformMonoHelper.Remove(args.UnitId);
        }
    }
}