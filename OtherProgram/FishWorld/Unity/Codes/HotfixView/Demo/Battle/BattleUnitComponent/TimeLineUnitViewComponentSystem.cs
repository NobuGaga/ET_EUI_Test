// Battle Review Before Boss Node

using System;

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
                    self.PlayAnimation(info.Arguments);
                    return;
                case TimeLineNodeType.Rotate:
                    self.Rotate(info.Arguments);
                    return;
                case TimeLineNodeType.SpeedChange:

                    return;
                case TimeLineNodeType.AppearUI:

                    return;
            }
        }

        private static void PlayAnimation(this Unit self, string[] arguments)
        {
            int.TryParse(arguments[1], out int loopFlag);
            self.PlayAnimation(arguments[0], loopFlag > 0);
        }

        private static void Rotate(this Unit self, string[] arguments)
        {
            var rotateInfo = self.FishUnitComponent.RotateInfo;
            rotateInfo.Reset();

            if (!float.TryParse(arguments[3], out float time))
                return;

            if (!float.TryParse(arguments[2], out float rotationZ))
                return;

            if (Math.Abs(rotationZ) < 1)
                return;

            rotateInfo.LocalRotationZ = rotationZ;
            rotateInfo.RotationDuration = (uint)(time * 1000);
        }
    }
}