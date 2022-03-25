using System;
using UnityEngine;

namespace ET
{
    /// <summary> 
    /// 视图层拓展 CannonComponent 方法
    /// CannonComponent 挂在 Player 类型的 Unit 下
    /// </summary>
    public static class CannonComponenViewtSystem
    {
        public static bool TryGetLocalRotation(this CannonComponent self, out Quaternion localRotation)
        {
            localRotation = Quaternion.identity;

            try
            {
                // 拆开引用调用方便异常时查错, 提高效率
                GameObjectComponent gameObjectComponent = self.Cannon.GetComponent<GameObjectComponent>();
                Transform transform = gameObjectComponent.Transform;
                localRotation = transform.localRotation;
                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"CannonComponent.TryGetLocalRotation() exception msg = { exception.Message }");
            }

            return false;
        }
    }
}