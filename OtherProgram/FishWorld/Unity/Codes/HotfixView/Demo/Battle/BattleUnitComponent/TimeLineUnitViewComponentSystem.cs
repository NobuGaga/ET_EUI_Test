// Battle Review Before Boss Node

using UnityEngine;

namespace ET
{
    [FriendClass(typeof(Unit))]
    [FriendClass(typeof(FishUnitComponent))]
    public static class TimeLineUnitViewComponentSystem
    {
        internal static void Execute(this Unit self, TimeLineConfigInfo info)
        {
            switch (info.Type)
            {
                case TimeLineNodeType.PlayAnimate:
                    var clip = self.PlayAnimation(info);
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
            FishTimelineConfigCategory.Instance.PublishTimeLineEvent(unitId, info);
        }

        private static AnimationClip PlayAnimation(this Unit self, TimeLineConfigInfo timeLineInfo)
        {
            string motionName = timeLineInfo.Arguments[0];
            bool isLoop = false;
            if (int.TryParse(timeLineInfo.Arguments[1], out int loopFlag))
                isLoop = loopFlag > 0;

            var fishUnitComponent = self.FishUnitComponent;
            var moveInfo = fishUnitComponent.MoveInfo;

            // Battle Warning ��ʱֻ����һ����ʱ����, ʱ������ʱ��������ʱ��һ��
            long currentTime = TimeHelper.ServerNow();
            long startMoveTime = currentTime - moveInfo.MoveTime;

            // Battle Warning ��ʱ���ᴥ��ʱ����ʱʹ�����������ڿ�ʼʱ��(�����û��, ��Ҫ�������˼�¼��ʱ���ᴥ��ʱ��)
            //long triggerTime = (long)(timeLineInfo.LifeTime * moveInfo.MoveDuration) + triggerTimeLineTime;
            long triggerTime = (long)(timeLineInfo.LifeTime * moveInfo.MoveDuration) + startMoveTime;

            // �Ѳ���ʱ��
            float playTime = (float)(currentTime - triggerTime) / FishConfig.MilliSecond;

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