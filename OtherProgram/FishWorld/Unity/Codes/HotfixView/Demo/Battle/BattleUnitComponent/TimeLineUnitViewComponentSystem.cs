// Battle Review Before Boss Node

using UnityEngine;

namespace ET
{
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    public static class TimeLineUnitViewComponentSystem
    {
        /// <summary> 重新进入渔场恢复时间轴相关表现 </summary>
        internal static void InitTimeLine(this Unit self, TimeLineConfigInfo info)
        {
            switch (info.Type)
            {
                case TimeLineNodeType.PlayAnimate:
                    float survivalTime = LifeCycleInfoHelper.GetSurvivalTime(self.UnitId);
                    float totalLifeTime = LifeCycleInfoHelper.GetTotalLifeTime(self.UnitId);
                    float triggerTime = totalLifeTime * info.LifeTime;
                    float playTime = survivalTime - triggerTime;
                    if (playTime < 0)
                        playTime = 0;
                    var clip = self.PlayAnimation(playTime, info);
                    if (clip != null)
                        info.ExecuteTime = clip.length;
                    break;
                //case TimeLineNodeType.PauseFishLine:
                //    self.PauseFishLineMove(info).Coroutine();
                //    break;
            }

            //if (info.IsAutoNext && info.ExecuteTime > 0)
            //    Execute(self.UnitId, info).Coroutine();
        }

        internal static void Execute(this Unit self, TimeLineConfigInfo info)
        {
            switch (info.Type)
            {
                case TimeLineNodeType.PlayAnimate:
                    var clip = self.PlayAnimation(0, info);
                    if (clip != null)
                        info.ExecuteTime = clip.length;
                    break;
                case TimeLineNodeType.PauseFishLine:
                    self.PauseFishLineMove(info).Coroutine();
                    break;
            }
            
            if (info.IsAutoNext && info.ExecuteTime > 0)
                Execute(self.UnitId, info).Coroutine();
        }

        private static async ETTask Execute(long unitId, TimeLineConfigInfo info)
        {
            await TimerComponent.Instance.WaitAsync((long)(info.ExecuteTime * FishConfig.MilliSecond));
            FishTimelineConfigCategory.Instance.PublishExecuteTimeLineEvent(unitId, info);
        }

        private static AnimationClip PlayAnimation(this Unit self, float playTime, TimeLineConfigInfo timeLineInfo)
        {
            string motionName = timeLineInfo.Arguments[0];
            bool isLoop = false;
            if (int.TryParse(timeLineInfo.Arguments[1], out int loopFlag))
                isLoop = loopFlag > 0;

            return self.PlayAnimation(motionName, playTime, isLoop);
        }

        private static async ETTask PauseFishLineMove(this Unit self, TimeLineConfigInfo timeLineInfo)
        {
            if (!float.TryParse(timeLineInfo.Arguments[0], out float changeTime))
                return;

            if (self.GameObjectComponent == null)
                return;

            var fishUnitComponent = self.FishUnitComponent;
            fishUnitComponent.PauseMove();
            await TimerComponent.Instance.WaitAsync((long)(changeTime * FishConfig.MilliSecond));
            if (self == null || self.IsDisposed)
                return;

            fishUnitComponent.ResumeMove();
        }
    }
}