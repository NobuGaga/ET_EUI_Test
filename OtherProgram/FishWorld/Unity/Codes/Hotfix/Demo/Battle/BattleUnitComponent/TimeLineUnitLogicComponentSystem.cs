// Battle Review Before Boss Node

namespace ET
{
    [FriendClass(typeof(Unit))]
    public static class TimeLineUnitLogicComponentSystem
    {
        internal static void Execute(this Unit self, TimeLineConfigInfo info)
        {
            switch (info.Type)
            {
                case TimeLineNodeType.PlayAnimate:

                    break;
                case TimeLineNodeType.Rotate:

                    break;
                case TimeLineNodeType.SpeedChange:

                    break;
                case TimeLineNodeType.AppearUI:

                    break;
                default:
                    self.ChangeState(info);
                    break;
            }
        }

        private static void PlayAnimate(this Unit self, TimeLineConfigInfo info)
        {

        }

        private static void Rotate(this Unit self, TimeLineConfigInfo info)
        {

        }

        private static void SpeedChange(this Unit self, TimeLineConfigInfo info)
        {

        }

        private static void AppearUI(this Unit self, TimeLineConfigInfo info)
        {

        }

        private static void ChangeState(this Unit self, TimeLineConfigInfo info)
        {
            var battleUnit = UnitMonoComponent.Instance.Get(self.UnitId);
            battleUnit.IsCanCollide = info.Type == TimeLineNodeType.ActiveState;
        }
    }
}