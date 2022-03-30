namespace ET
{
    [ObjectSystem]
    public class ColliderLogicComponentAwakeSystem : AwakeSystem<ColliderLogicComponent>
    {
        public override void Awake(ColliderLogicComponent self)
        {
            // Battle TODO
        }
    } 

    [ObjectSystem]
    public class ColliderLogicComponentDestroySystem : DestroySystem<ColliderLogicComponent>
    {
        public override void Destroy(ColliderLogicComponent self)
        {
            // Battle TODO
        }
    }

    public static class ColliderLogicComponentSystem
    {
        public static void FixedUpdate(this ColliderLogicComponent self)
        {
            // Battle TODO
        }
    }
}