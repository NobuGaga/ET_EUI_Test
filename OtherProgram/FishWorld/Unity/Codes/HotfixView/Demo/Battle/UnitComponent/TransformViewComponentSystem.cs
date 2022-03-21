using UnityEngine;

namespace ET
{
    /// <summary>
    /// 拓展类似 Unity Transform 方法, 逻辑层修改值直接调用逻辑组件获取的 TransformComponent 去修改
    /// 这里只作视图层方法的拓展
    /// </summary>
    public static class TransformViewComponentSystem
    {
        public static async ETTask InitTransform(this BattleUnitViewComponent self)
        {
            TransformComponent transformComponent = self.TransformComponent();
            self.UpdateTransform(false);
            // Battle Warning 使用预设缩放值
            self.SetScale(self.GameObjectComponent().Transform.localScale);
            self.SetParent(self.NodeParent);
            self.SetName(transformComponent.NodeName);
            await ETTask.CompletedTask;
        }

        public static bool IsInitModel(this BattleUnitViewComponent self) => self.GameObjectComponent() != null;

        public static void UpdateTransform(this BattleUnitViewComponent self, bool isSetScale = true)
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

        public static void UpdatePos(this BattleUnitViewComponent self) =>
                                    self.SetPos(self.TransformComponent().LogicPos);

        public static void SetPos(this BattleUnitViewComponent self, Vector3 pos)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.SetPos(pos);
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.position = pos;
            transformComponent.LogicLocalPos = transform.localPosition;
        }

        public static void UpdateLocalPos(this BattleUnitViewComponent self) =>
                                self.SetLocalPos(self.TransformComponent().LogicLocalPos);

        public static void SetLocalPos(this BattleUnitViewComponent self, Vector3 localPos)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.SetLocalPos(localPos);
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.localPosition = localPos;
            transformComponent.LogicPos = transform.position;
        }

        public static void UpdateRotation(this BattleUnitViewComponent self) =>
                                    self.SetRotation(self.TransformComponent().LogicRotation);

        public static void SetRotation(this BattleUnitViewComponent self, Quaternion rotation)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.SetRotation(rotation);
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.rotation = rotation;
            transformComponent.LogicLocalRotation = transform.localRotation;
        }

        public static void UpdateLocalRotation(this BattleUnitViewComponent self) =>
                                self.SetLocalRotation(self.TransformComponent().LogicLocalRotation);

        public static void SetLocalRotation(this BattleUnitViewComponent self, Quaternion localRotation)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.SetLocalRotation(localRotation);
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent == null)
                return;

            Transform transform = gameObjectComponent.Transform;
            transform.localRotation = localRotation;
            transformComponent.LogicRotation = transform.rotation;
        }

        public static void UpdateScale(this BattleUnitViewComponent self) =>
                                self.SetScale(self.TransformComponent().LogicScale);

        public static void SetScale(this BattleUnitViewComponent self, Vector3 scale)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.SetScale(scale);
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.localScale = scale;
        }

        public static void UpdateForward(this BattleUnitViewComponent self) =>
                                        self.SetForward(self.TransformComponent().LogicForward);

        public static void SetForward(this BattleUnitViewComponent self, Vector3 forward)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.SetForward(forward);
            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.forward = forward;
        }

        public static void ResetTransform(this BattleUnitViewComponent self, bool isSetScale = true)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.ResetTransform(isSetScale);
            self.UpdateLocalPos();
            self.UpdateLocalRotation();
            if (isSetScale)
                self.UpdateScale();
        }

        public static Vector3 WorldToScreenPoint(this BattleUnitViewComponent self, Vector3 position)
        {
            Unit unit = self.Unit();
            CameraComponent cameraComponent = self.ZoneScene().GetComponent<CameraComponent>();
            Camera camera = cameraComponent.MainCamera;
            if (unit.UnitType == UnitType.Bullet)
                camera = GlobalComponent.Instance.CannonCamera;

            return camera.WorldToScreenPoint(position);
        }

        public static Vector3 CalculateScreenPos(this BattleUnitViewComponent self)
        {
            GameObjectComponent gameObjectComponent1 = self.GameObjectComponent();
            if (gameObjectComponent1 == null)
                return self.WorldToScreenPoint(self.TransformComponent().LogicPos);

            return self.WorldToScreenPoint(gameObjectComponent1.Transform.position);
        }

        public static Vector3 GetScreenPos(this BattleUnitViewComponent self)
        {
            TransformComponent transformComponent = self.TransformComponent();
            if (!transformComponent.IsScreenPosDirty)
                return transformComponent.ScreenPos;

            transformComponent.ScreenPos = self.CalculateScreenPos();
            transformComponent.IsScreenPosDirty = false;
            return transformComponent.ScreenPos;
        }

        public static void SetParent(this BattleUnitViewComponent self, Transform parent, bool isKeepMoveState = true)
        {
            if (!parent)
                return;

            self.NodeParent = parent;

            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent != null)
                gameObjectComponent.Transform.SetParent(parent, isKeepMoveState);
        }

        public static void SetName(this BattleUnitViewComponent self, string name)
        {
            TransformComponent transformComponent = self.TransformComponent();
            transformComponent.NodeName = name;

            GameObjectComponent gameObjectComponent = self.GameObjectComponent();
            if (gameObjectComponent != null)
                gameObjectComponent.GameObject.name = name;
        }
    }
}