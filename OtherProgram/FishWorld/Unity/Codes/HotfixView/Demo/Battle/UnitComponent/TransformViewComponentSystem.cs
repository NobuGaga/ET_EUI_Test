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

        public static void UpdateTransform(this Unit self, bool isSetScale = true)
        {
            self.UpdateLocalPos();
            self.UpdatePos();
            self.UpdateLocalRotation();
            self.UpdateRotation();

            // Battle TODO 后面看看要不要走配置
            if (isSetScale)
                self.UpdateScale();

            self.UpdateForward();
        }

        public static void UpdatePos(this Unit self) =>
                                    self.SetPos(self.TransformComponent().LogicPos);

        public static void SetPos(this Unit self, Vector3 pos)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.position = pos;
            
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.LogicLocalPos = transform.localPosition;
        }

        public static void UpdateLocalPos(this Unit self) =>
                                self.SetLocalPos(self.TransformComponent().LogicLocalPos);

        public static void SetLocalPos(this Unit self, Vector3 localPos)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;

            if (transform == null)
            {
                // Battle TODO delete
                Log.Error($"模型资源有问题, 烦请把鱼基础表 ID = { self.ConfigId }, 发给基层员工 = 毕伟雄");
                return;
            }

            transform.localPosition = localPos;
            
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.LogicPos = transform.position;
        }

        public static void UpdateRotation(this Unit self) =>
                                    self.SetRotation(self.TransformComponent().LogicRotation);

        public static void SetRotation(this Unit self, Quaternion rotation)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.rotation = rotation;

            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.LogicLocalRotation = transform.localRotation;
        }

        public static void UpdateLocalRotation(this Unit self) =>
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

        public static void UpdateScale(this Unit self) =>
                                self.SetScale(self.TransformComponent().LogicScale);

        public static void SetScale(this Unit self, Vector3 scale)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.localScale = scale;
        }

        public static void UpdateForward(this Unit self) =>
                                        self.SetForward(self.TransformComponent().LogicForward);

        public static void SetForward(this Unit self, Vector3 forward)
        {
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            if (gameObjectComponent.Transform == null)
            {
                // Battle TODO delete
                Log.Error($"模型资源有问题, 烦请把鱼基础表 ID = { self.ConfigId }, 发给基层员工 = 毕伟雄");
                return;
            }

            gameObjectComponent.Transform.forward = forward;
        }

        /// <summary> 重置变换会把逻辑数据也重置掉 </summary>
        public static void ResetTransform(this Unit self, bool isSetScale = true)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.ResetTransform(isSetScale);

            self.UpdateLocalPos();
            self.UpdateLocalRotation();
            
            if (isSetScale)
                self.UpdateScale();
        }

        public static Vector3 GetScreenPos(this Unit self)
        {
            TransformComponent transformComponent = self.TransformComponent();
            if (!transformComponent.IsScreenPosDirty)
                return transformComponent.ScreenPos;

            transformComponent.ScreenPos = self.GetScreenPoint();
            transformComponent.IsScreenPosDirty = false;
            return transformComponent.ScreenPos;
        }

        private static Vector3 GetScreenPoint(this Unit self)
        {
            GameObjectComponent gameObjectComponent1 = self.GameObjectComponent();
            Camera camera = self.GetDisplayCamera();

            if (gameObjectComponent1 != null)
                return camera.WorldToScreenPoint(gameObjectComponent1.Transform.position);

            TransformComponent transformComponent = self.TransformComponent();
            return camera.WorldToScreenPoint(transformComponent.LogicPos);
        }

        private static Camera GetDisplayCamera(this Unit self)
        {
            CameraComponent cameraComponent = self.ZoneScene().GetComponent<CameraComponent>();
            Camera camera = cameraComponent.MainCamera;

            if (self.UnitType == UnitType.Bullet)
                camera = GlobalComponent.Instance.CannonCamera;

            return camera;
        }

        public static void SetParent(this Unit self, Transform parent, bool isKeepMoveState = true)
        {
            if (parent == null)
                return;

            // 设置父节点这种视图相关的数据需要把逻辑层也设置一下
            self.BattleUnitViewComponent().NodeParent = parent;

            GameObjectComponent gameObjectComponent = self.GameObjectComponent();

            if (gameObjectComponent != null)
                gameObjectComponent.Transform.SetParent(parent, isKeepMoveState);
        }

        public static void SetName(this Unit self, string name)
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