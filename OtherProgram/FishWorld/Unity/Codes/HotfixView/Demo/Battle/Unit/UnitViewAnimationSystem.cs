using UnityEngine;

namespace ET
{

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BattleViewComponent))]
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(BattleUnitViewComponent))]
    public static class UnitViewAnimationSystem
    {
        public static async ETTask InitFishAnimator(this Unit self)
        {
            bool isUseModelPool = BattleConfig.IsUseModelPool;
            int configId = self.ConfigId;
            string resId = self.Config.ResId;
            var animatorComponent = self.AddComponent<AnimatorComponent, int>(configId, isUseModelPool);
            animatorComponent.Reset();
            self.AnimatorComponent = animatorComponent;

            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;
            if (AnimationClipHelper.Contains(resId))
            {
                if (!string.IsNullOrEmpty(battleUnitViewComponent.MotionName))
                    self.PlayAnimation(battleUnitViewComponent.MotionName, 0, true);
                return;
            }

            var assetBundleData = UnitConfigCategory.Instance.GetAssetBundleData(self.Config.ResId);
            string clipBundlePath = assetBundleData.ClipPath;
            Scene currentScene = BattleLogicComponent.Instance.CurrentScene;
            var ResourcesLoaderComponent = currentScene.GetComponent<ResourcesLoaderComponent>();
            await ResourcesLoaderComponent.LoadAsync(clipBundlePath);

            if (battleUnitViewComponent == null || battleUnitViewComponent.IsDisposed)
                return;

            string motionName = battleUnitViewComponent.MotionName;
            if (AnimationClipHelper.Contains(resId))
            {
                if (!string.IsNullOrEmpty(motionName))
                    self.PlayAnimation(motionName, 0, true);
                return;
            }

            var assetMap = ResourcesComponent.Instance.GetBundleAll(clipBundlePath);
            BattleLogicComponent.Instance.Argument_String = resId;

            // 将每个模型动作存到 Mono 层, 新的模型加载后只执行一次
            ForeachHelper.Foreach(assetMap, BattleViewComponent.Instance.Action_String_UnityObject);
            if (!string.IsNullOrEmpty(motionName))
                self.PlayAnimation(motionName, 0, true);
        }

        internal static void ForeachBundleAsset(string assetName, UnityEngine.Object asset)
        {
            string resId = BattleLogicComponent.Instance.Argument_String;
            AnimationClipHelper.Add(resId, assetName, asset as AnimationClip);
        }

        internal static AnimationClip PlayAnimation(this Unit self, string motionName, float time, bool isLoop)
        {
            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;

            // 这里只储存循环播放的动作名, 用于作异步加载完后播放
            if (isLoop) battleUnitViewComponent.MotionName = motionName;

            if (self.GameObjectComponent == null) return null;

            string resId = self.Config.ResId;
            var gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            AnimationClipHelper.Play(gameObjectComponent.GameObject, resId, motionName, time, isLoop);

            if (isLoop) return null;

            var clip = AnimationClipHelper.GetClip(resId, motionName);
            if (clip != null)
                self.ResumeMainMotion(clip.length).Coroutine();

            return clip;
        }

        internal static async ETTask ResumeMainMotion(this Unit self, float time)
        {
            await TimerComponent.Instance.WaitAsync((long)(time * FishConfig.MilliSecond));
            if (self == null || self.IsDisposed) return;

            var battleUnitViewComponent = self.BattleUnitViewComponent as BattleUnitViewComponent;
            self.PlayAnimation(battleUnitViewComponent.MotionName, 0, true);
        }

        internal static void PauseAnimation(this Unit self)
        {
            if (self.GameObjectComponent == null)
                return;

            var gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            AnimationClipHelper.Pause(gameObjectComponent.GameObject);
        }

        internal static void StopAnimation(this Unit self)
        {
            if (self.GameObjectComponent == null)
                return;

            var gameObjectComponent = self.GameObjectComponent as GameObjectComponent;
            AnimationClipHelper.Stop(gameObjectComponent.GameObject);
        }
    }
}