namespace ET
{
    public static class BattleInfoExtension
    {
        public static void Reset(this FishMoveInfo self)
        {
            self.IsPause = false;
            self.IsMoveEnd = false;
            
            self.MoveTime = 0;
            self.MoveDuration = 0;

            self.OffsetPosX = 0;
            self.OffsetPosY = 0;
            self.OffsetPosZ = 0;

            self.NextPos = FishConfig.RemovePoint;
            self.NextForward = TransformDefaultConfig.DefaultForward;

            self.MoveSpeed = FishConfig.DefaultMoveSpeed;

            self.RoadId = 0;
        }
    }
}