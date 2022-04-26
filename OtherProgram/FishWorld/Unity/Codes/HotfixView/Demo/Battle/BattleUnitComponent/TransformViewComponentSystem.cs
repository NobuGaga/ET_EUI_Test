using UnityEngine;

namespace ET
{
    /// <summary>
    /// 拓展类似 Unity Transform 方法, 逻辑层修改值直接调用逻辑组件获取的 TransformComponent 去修改
    /// 这里只作视图层方法的拓展, 挂 Unit 类下
    /// </summary>
    [FriendClass(typeof(GlobalComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(TransformComponent))]
    [FriendClass(typeof(BattleUnitViewComponent))]
    [FriendClass(typeof(BulletUnitComponent))]
    [FriendClass(typeof(GameObjectComponent))]
    public static class TransformViewComponentSystem
    {
        private static BattleUnitViewComponent BattleUnitViewComponent(this Unit self) =>
                                               self.GetComponent<BattleUnitViewComponent>();

        public static void InitTransform(this Unit self)
        {
            TransformInfo info = self.TransformComponent.Info;
            self.SetLocalPos(info.LocalPosition);
            self.SetPos(info.WorldPosition);
            self.SetLocalRotation(info.LocalRotation);
            self.SetRotation(info.Rotation);
            self.SetForward(info.Forward);

            // Battle Warning 使用预设缩放值, 后面看看要不要走配置
            self.SetScale((self.GameObjectComponent as GameObjectComponent).Transform.localScale);
            self.SetParent(self.BattleUnitViewComponent().NodeParent);

            if (Define.IsEditor)
                self.SetName(self.TransformComponent.NodeName);
        }

        private static void SetPos(this Unit self, Vector3 pos)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.position = pos;
            
            self.TransformComponent.Info.LocalPosition = transform.localPosition;
        }

        private static void SetLocalPos(this Unit self, Vector3 localPos)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.localPosition = localPos;

            self.TransformComponent.Info.WorldPosition = transform.position;
        }

        private static void SetRotation(this Unit self, Quaternion rotation)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.rotation = rotation;

            self.TransformComponent.Info.LocalRotation = transform.localRotation;
        }

        public static void SetLocalRotation(this Unit self, Quaternion localRotation)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.localRotation = localRotation;

            self.TransformComponent.Info.Rotation = transform.rotation;
        }

        private static void SetForward(this Unit self, Vector3 forward)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.forward = forward;
        }

        private static void SetScale(this Unit self, Vector3 scale)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.localScale = scale;
        }

        public static void UpdateScreenPosition(this Unit bulletUnit)
        {
            var gameObjectComponent = bulletUnit.GameObjectComponent as GameObjectComponent;
            Camera camera = GlobalComponent.Instance.RawCannonCamera;

            var info = bulletUnit.BulletUnitComponent.Info;
            if (gameObjectComponent != null)
                info.ScreenPos = camera.WorldToScreenPoint(gameObjectComponent.Transform.position);
            else
                info.ScreenPos = camera.WorldToScreenPoint(bulletUnit.TransformComponent.Info.WorldPosition);
        }

        private static void SetParent(this Unit self, Transform parent, bool isKeepMoveState = true)
        {
            if (parent == null)
                return;

            // 设置父节点这种视图相关的数据需要把逻辑层也设置一下
            self.BattleUnitViewComponent().NodeParent = parent;

            GameObjectComponent gameObjectComponent = self.GameObjectComponent as GameObjectComponent;

            if (gameObjectComponent != null)
                gameObjectComponent.Transform.SetParent(parent, isKeepMoveState);
        }

        private static void SetName(this Unit self, string name)
        {
            if (name == null)
                return;

            // 设置节点名这种视图相关的数据需要把逻辑层也设置一下
            self.TransformComponent.NodeName = name;

            GameObjectComponent gameObjectComponent = self.GameObjectComponent as GameObjectComponent;

            if (gameObjectComponent != null)
                gameObjectComponent.GameObject.name = name;
        }
    }
}