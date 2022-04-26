using System;
using UnityEngine;

namespace ET
{
    /// <summary> Mono 层战斗管理组件 </summary>
    public class BattleMonoComponent
    {
        public static BattleMonoComponent Instance = new BattleMonoComponent();

        private bool isEnterBattle = false;

        private Action<float, float, long, long> collideAction;

        private Action<long> removeFishUnitAction;

        BattleMonoComponent()
        {
            // 调用注意顺序, 先初始化 Mono 层引用类
            ReferenceHelper.Init();
            ConstHelper.Init(Screen.width, Screen.height, ReferenceHelper.CannoCamera.orthographicSize);
#if UNITY_EDITOR

            BattleDebugComponent.Init();
#endif
        }

        public void EnterBattle(Action<float, float, long, long> collideAction, Action<long> removeFishUnitAction)
        {
            isEnterBattle = true;
            this.collideAction += collideAction;
            this.removeFishUnitAction += removeFishUnitAction;
            UnitMonoComponent.Instance = new UnitMonoComponent();
        }

        public void Update()
        {
            if (!isEnterBattle)
                return;

            ClearDebug();

            var unitMonoComponent = UnitMonoComponent.Instance;
            var fishUnitIdList = unitMonoComponent.FishUnitIdList;
            for (var index = fishUnitIdList.Count - 1; index >= 0; index--)
            {
                var fishUnitId = fishUnitIdList[index];
                var unit = unitMonoComponent.Get(fishUnitId);
                unit.FixedUpdate();
                if ((unit as FishMonoUnit).FishMoveInfo.IsMoveEnd)
                {
                    unitMonoComponent.RemoveFishUnit(index);
                    removeFishUnitAction(fishUnitId);
                }
                else
                    unit.Update();
            }

            var bulletUnitIdList = unitMonoComponent.BulletUnitIdList;
            for (var bulletIndex = bulletUnitIdList.Count - 1; bulletIndex >= 0; bulletIndex--)
            {
                var bulletUnitId = bulletUnitIdList[bulletIndex];
                var unit = unitMonoComponent.Get(bulletUnitId);
                unit.FixedUpdate();
                unit.Update();

                var bulletUnit = unit as BulletMonoUnit;
                if (bulletUnit.Transform == null || bulletUnit.ColliderMonoComponent == null)
                    continue;

                for (var fishIndex = 0; fishIndex < fishUnitIdList.Count; fishIndex++)
                {
                    var fishUnitId = fishUnitIdList[fishIndex];
                    var fishUnit = unitMonoComponent.Get<FishMonoUnit>(fishUnitId);
                    ref long trackFishUnitId = ref bulletUnit.BulletMoveInfo.TrackFishUnitId;
                    if (trackFishUnitId != ConstHelper.DefaultTrackFishUnitId && trackFishUnitId != fishUnitId)
                        continue;

                    if (fishUnit.ColliderMonoComponent == null)
                        continue;

                    if (!bulletUnit.ColliderMonoComponent.IsCollide(fishUnit.ColliderMonoComponent))
                        continue;

                    Vector3 screenPosition;
                    if (trackFishUnitId != ConstHelper.DefaultTrackFishUnitId)
                        screenPosition = fishUnit.FishScreenInfo.AimPoint;
                    else
                        screenPosition = bulletUnit.ColliderMonoComponent.GetBulletCollidePoint();

                    collideAction(screenPosition.x, screenPosition.y, bulletUnitId, fishUnitId);
                    break;
                }
            }
        }

        public void ExitBattle()
        {
            isEnterBattle = true;
            collideAction = null;
            removeFishUnitAction = null;
            UnitMonoComponent.Instance = null;
            ClearDebug();
        }

        public void ClearDebug()
        {
#if UNITY_EDITOR

            BattleDebugComponent.Clear();
#endif
        }
    }
}