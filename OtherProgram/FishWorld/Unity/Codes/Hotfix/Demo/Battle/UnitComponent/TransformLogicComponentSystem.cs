using UnityEngine;

namespace ET
{
    [ObjectSystem]
    public class TransformComponentAwakeSystem : AwakeSystem<TransformComponent>
    {
        public override void Awake(TransformComponent self)
        {
            self.NodeName = TransformDefaultConfig.DefaultName;
            self.ResetTransform();
        }
    }

    [ObjectSystem]
    public class TransformComponentDestroySystem : DestroySystem<TransformComponent>
    {
        public override void Destroy(TransformComponent self) => self.NodeName = null;
    }

    /// <summary>
    /// 拓展 TransformComponent Setter 方法, Getter 直接访问成员变量
    /// 使用 Setter 方法赋值主要为了修改脏标记
    /// 原来通过接口约束变换组件跟实体的行为, 使他们保持一致
    /// 现在不作约束, 直接定义实现方法
    /// </summary>
    public static class TransformLogicComponentSystem
    {
        public static void SetPos(this TransformComponent self, Vector3 Pos)
        {
            self.LogicPos = Pos;
            self.IsScreenPosDirty = true;
        }

        public static void SetLocalPos(this TransformComponent self, Vector3 LocalPos)
        {
            self.LogicLocalPos = LocalPos;
            self.IsScreenPosDirty = true;
        }

        public static void SetRotation(this TransformComponent self, Quaternion Rotation)
        {
            self.LogicRotation = Rotation;
            self.IsScreenPosDirty = true;
        }

        public static void SetLocalRotation(this TransformComponent self, Quaternion LocalRotation)
        {
            self.LogicLocalRotation = LocalRotation;
            self.IsScreenPosDirty = true;
        }

        public static void SetScale(this TransformComponent self, Vector3 Scale)
        {
            self.LogicScale = Scale;
            self.IsScreenPosDirty = true;
        }

        public static void SetForward(this TransformComponent self, Vector3 Forward)
        {
            self.LogicForward = Forward;
            self.IsScreenPosDirty = true;
        }

        public static void ResetTransform(this TransformComponent self, bool isSetScale = true)
        {
            self.SetLocalPos(TransformDefaultConfig.DefaultPosition);
            self.SetLocalRotation(TransformDefaultConfig.DefaultRotation);

            if (isSetScale)
                self.SetScale(TransformDefaultConfig.DefaultScale);

            self.ScreenPos = TransformDefaultConfig.DefaultScreenPos;
        }
    }
}