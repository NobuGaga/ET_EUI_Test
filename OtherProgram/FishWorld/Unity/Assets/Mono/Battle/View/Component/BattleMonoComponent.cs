using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    /// <summary> Mono 层战斗管理组件 </summary>
    public class BattleMonoComponent
    {
        public static BattleMonoComponent Instance = new BattleMonoComponent();

        private bool isEnterBattle = false;

        private Action updateSkillBeforeFish;

        private Action updateSkillBeforeBullet;

        private Action<float, float, long, long> collideAction;

        private Action<long> removeFishUnitAction;

        BattleMonoComponent()
        {
            // 调用注意顺序, 先初始化 Mono 层引用类
            ReferenceHelper.Init();
            ConstHelper.Init(Screen.width, Screen.height, ReferenceHelper.CannoCamera.orthographicSize);

            var monoPool = MonoPool.Instance;
            for (int index = 0; index < ConstHelper.PreCreateFishClassCount; index++)
            {
                monoPool.Recycle(new TransformInfo());
                monoPool.Recycle(new TransformInfo());
                monoPool.Recycle(new FishMoveInfo());
                monoPool.Recycle(new FishScreenInfo());
            }
#if UNITY_EDITOR

            BattleDebugComponent.Init();
#endif
        }

        public void EnterGame()
        {

        }

        public void EnterBattle(Action<float, float, long, long> collideAction, Action<long> removeFishUnitAction,
                                Action updateSkillBeforeFish, Action updateSkillBeforeBullet)
        {
            isEnterBattle = true;
            this.collideAction += collideAction;
            this.removeFishUnitAction += removeFishUnitAction;
            this.updateSkillBeforeFish = updateSkillBeforeFish;
            this.updateSkillBeforeBullet = updateSkillBeforeBullet;
            UnitMonoComponent.Instance = new UnitMonoComponent();
        }

        public void Update()
        {
            if (!isEnterBattle)
                return;

            ClearDebug();

            var unitMonoComponent = UnitMonoComponent.Instance;
            updateSkillBeforeFish();
            var fishUnitIdList = UpdateFishUnitList(unitMonoComponent);
            updateSkillBeforeBullet();
            UpdateBulletUnitList(unitMonoComponent, fishUnitIdList);
        }
        
        private List<long> UpdateFishUnitList(UnitMonoComponent unitMonoComponent)
        {
            var fishUnitIdList = unitMonoComponent.FishUnitIdList;
            long unitId;
            FishMonoUnit fishUnit;
            for (var index = fishUnitIdList.Count - 1; index >= 0; index--)
            {
                unitId = fishUnitIdList[index];
                fishUnit = unitMonoComponent.Get(unitId) as FishMonoUnit;
                fishUnit.FixedUpdate();
                if (!fishUnit.FishMoveInfo.IsMoveEnd)
                {
                    fishUnit.Update();
                    continue;
                }

                unitMonoComponent.RemoveFishUnit(index);
                removeFishUnitAction(unitId);
            }

            return fishUnitIdList;
        }

        private void UpdateBulletUnitList(UnitMonoComponent unitMonoComponent, List<long> fishUnitIdList)
        {
            var bulletUnitIdList = unitMonoComponent.BulletUnitIdList;
            long unitId;
            BulletMonoUnit bulletUnit;
            for (var bulletIndex = bulletUnitIdList.Count - 1; bulletIndex >= 0; bulletIndex--)
            {
                unitId = bulletUnitIdList[bulletIndex];
                bulletUnit = unitMonoComponent.Get(unitId) as BulletMonoUnit;
                bulletUnit.FixedUpdate();
                bulletUnit.Update();

                if (bulletUnit.Transform == null || bulletUnit.ColliderMonoComponent == null)
                    continue;

                CheckCollidFish(bulletUnit, unitId, unitMonoComponent, fishUnitIdList);
            }
        }

        private void CheckCollidFish(BulletMonoUnit bulletUnit, long bulletUnitId,
                                     UnitMonoComponent unitMonoComponent, List<long> fishUnitIdList)
        {
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

        public void ExitBattle()
        {
            isEnterBattle = false;
            collideAction = null;
            removeFishUnitAction = null;
            updateSkillBeforeFish = null;
            updateSkillBeforeBullet = null;
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