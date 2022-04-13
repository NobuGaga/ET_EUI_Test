using UnityEngine;

namespace ET
{
    /// <summary>
    /// 拓展类似 Unity Transform 方法, 逻辑层修改值直接调用逻辑组件获取的 TransformComponent 去修改
    /// 这里只作视图层方法的拓展, 挂 Unit 类下, Unit 的 Transform 行为只修改 GameObject 的显示
    /// </summary>
    public static class TransformViewComponentSystem
    {
        // 这里使用私有访问权限不污染外部 Unit 环境
        private static TransformComponent TransformComponent(this Unit self) =>
                                                            self.GetComponent<TransformComponent>();

        private static GameObjectComponent GameObjectComponent(this Unit self) =>
                                                            self.GetComponent<GameObjectComponent>();

        private static BattleUnitViewComponent BattleUnitViewComponent(this Unit self) =>
                                                            self.GetComponent<BattleUnitViewComponent>();

        public static void InitTransform(this Unit self)
        {
            self.UpdateTransform(false);

            // Battle Warning 使用预设缩放值
            self.SetScale(self.GameObjectComponent().Transform.localScale);
            self.SetParent(self.BattleUnitViewComponent().NodeParent);

            // Define 只在视图层定义
            if (Define.IsEditor)
                self.SetName(self.TransformComponent().NodeName);
        }

        private static void UpdateTransform(this Unit self, bool isSetScale = true)
        {
            self.UpdateLocalPos();
            self.UpdatePos();
            self.UpdateLocalRotation();
            self.UpdateRotation();

            // Battle TODO 后面看看要不要走配置
            if (isSetScale)
                self.UpdateScale();

            self.UpdateForward();

            self.UpdateScreenPosition();
        }

        private static void UpdatePos(this Unit self) =>
                                    self.SetPos(self.TransformComponent().LogicPos);

        private static void SetPos(this Unit self, Vector3 pos)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.position = pos;
            
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.LogicLocalPos = transform.localPosition;
        }

        private static void UpdateLocalPos(this Unit self) =>
                                self.SetLocalPos(self.TransformComponent().LogicLocalPos);

        public static void SetLocalPos(this Unit self, Vector3 localPos)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.localPosition = localPos;
            
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.LogicPos = transform.position;
        }

        private static void UpdateRotation(this Unit self) =>
                                    self.SetRotation(self.TransformComponent().LogicRotation);

        private static void SetRotation(this Unit self, Quaternion rotation)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.rotation = rotation;

            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.LogicLocalRotation = transform.localRotation;
        }

        private static void UpdateLocalRotation(this Unit self) =>
                                self.SetLocalRotation(self.TransformComponent().LogicLocalRotation);

        public static void SetLocalRotation(this Unit self, Quaternion localRotation)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.localRotation = localRotation;

            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.LogicRotation = transform.rotation;
        }

        private static void UpdateScale(this Unit self) =>
                                self.SetScale(self.TransformComponent().LogicScale);

        private static void SetScale(this Unit self, Vector3 scale)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.localScale = scale;
        }

        private static void UpdateForward(this Unit self) =>
                                        self.SetForward(self.TransformComponent().LogicForward);

        public static void SetForward(this Unit self, Vector3 forward)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.forward = forward;
        }

        /// <summary> 重置变换会把逻辑数据也重置掉 </summary>
        private static void ResetTransform(this Unit self, bool isSetScale = true)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.ResetTransform(isSetScale);

            self.UpdateLocalPos();
            self.UpdateLocalRotation();
            
            if (isSetScale)
                self.UpdateScale();
        }

        public static void UpdateScreenPosition(this Unit self)
        {
            TransformComponent transformComponent = self.TransformComponent();
            if (!transformComponent.IsScreenPosDirty)
                return;

            transformComponent.ScreenPos = self.GetScreenPoint();
            transformComponent.IsScreenPosDirty = false;

            ref Vector3 pos = ref transformComponent.ScreenPos;

            transformComponent.IsInScreen = pos.x > 0 && pos.y > 0 &&
                                            pos.x < Screen.width && pos.y < Screen.height;
        }

        private static Vector3 GetScreenPoint(this Unit self)
        {
            if (Application.isEditor)
                UnityEngine.Profiling.Profiler.BeginSample("TransformViewComponent GetScreenPoint");

            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            Camera camera = self.GetDisplayCamera();

            if (gameObjectComponent != null)
                return camera.WorldToScreenPoint(gameObjectComponent.Transform.position);

            TransformComponent transformComponent = self.TransformComponent();
            Vector3 point = camera.WorldToScreenPoint(transformComponent.LogicPos);

            if (Application.isEditor)
                UnityEngine.Profiling.Profiler.BeginSample("TransformViewComponent GetScreenPoint");

            return point;
        }

        private static Camera GetDisplayCamera(this Unit self)
        {
            CameraComponent cameraComponent = self.ZoneScene().GetComponent<CameraComponent>();
            Camera camera = cameraComponent.MainCamera;

            if (self.UnitType == UnitType.Bullet)
                camera = GlobalComponent.Instance.CannonCamera;

            return camera;
        }

        private static void SetParent(this Unit self, Transform parent, bool isKeepMoveState = true)
        {
            if (parent == null)
                return;

            // 设置父节点这种视图相关的数据需要把逻辑层也设置一下
            self.BattleUnitViewComponent().NodeParent = parent;

            GameObjectComponent gameObjectComponent = self.GameObjectComponent();

            if (gameObjectComponent != null)
                gameObjectComponent.Transform.SetParent(parent, isKeepMoveState);
        }

        private static void SetName(this Unit self, string name)
        {
            if (name == null)
                return;

            // 设置节点名这种视图相关的数据需要把逻辑层也设置一下
            self.TransformComponent().NodeName = name;

            GameObjectComponent gameObjectComponent = self.GameObjectComponent();

            if (gameObjectComponent != null)
                gameObjectComponent.GameObject.name = name;
        }
    }
}