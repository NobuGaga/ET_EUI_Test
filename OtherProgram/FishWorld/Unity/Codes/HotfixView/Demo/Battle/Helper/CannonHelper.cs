using UnityEngine;

namespace ET
{
    [FriendClass(typeof(CannonComponent))]
    public static class CannonHelper
    {
        public static Transform GetShootPointNode(Scene currentScene, int seatId)
        {
            Unit playerUnit = FisheryHelper.GetPlayerUnit(seatId);
            CannonComponent cannonComponent = playerUnit.GetComponent<CannonComponent>();
            GameObjectComponent gameObjectComponent = cannonComponent.Cannon.GetComponent<GameObjectComponent>();
            Transform cannonTrans = gameObjectComponent.GameObject.transform;
            ReferenceCollector collector = cannonTrans.gameObject.GetComponent<ReferenceCollector>();
            return collector.Get<GameObject>("shoot_point").transform;
        }

        /// <summary> 屏幕坐标转换成炮台本地坐标 </summary>
        public static Vector3 ScreenPointToCannonPosition(Vector3 screenPosition)
        {
            CannonShootInfo cannonShootInfo = CannonShootHelper.PopInfo();
            CannonShootHelper.InitInfo(cannonShootInfo, screenPosition);
            Vector3 localPosition = cannonShootInfo.ShootLocalPosition;
            CannonShootHelper.PushPool(cannonShootInfo);
            return localPosition;
        }
    }
}