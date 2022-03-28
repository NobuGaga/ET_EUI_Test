using ET.EventType;
using UnityEngine;

namespace ET
{
    public class AfterEnterRoom_CameraComponent : AEvent<ReceiveEnterRoom>
    {
        protected override async ETTask Run(ReceiveEnterRoom args)
        {
            CameraComponent cameraComponent = args.CurrentScene.ZoneScene().GetComponent<CameraComponent>();
            await cameraComponent.SetTransformByAreaId(args.FisheryComponent.AreaId);
        }
    }

    public class AfterExchangeArea_CameraComponent : AEvent<ReceiveExchangeArea>
    {
        protected override async ETTask Run(ReceiveExchangeArea args)
        {
            FisheryComponent fisheryComponent = args.FisheryComponent;
            CameraComponent cameraComponent = fisheryComponent.ZoneScene().GetComponent<CameraComponent>();
            await cameraComponent.SetTransformByAreaId(args.FisheryComponent.AreaId);
        }
    }

    public static class CameraComponentSystem
    {
        public static async ETTask SetTransformByAreaId(this CameraComponent self, int areaId)
        {
            FisheryLevelConfig config = FisheryLevelConfigCategory.Instance.Get(areaId);
            string vectorString = config.CameraParamPosition;
            float time = (float)config.CameraMoveTime / FishConfig.MilliSecond;

            Vector3 vector;
            if (VectorStringHelper.TryParseVector3(vectorString, out vector))
                await self.PlayMovePosition(vector, time);

            vectorString = config.CameraParamRotation;
            time = (float)config.CameraRotateTime / FishConfig.MilliSecond;

            if (VectorStringHelper.TryParseVector3(vectorString, out vector))
                await self.PlayRotate(vector, time);
        }

        public static async ETTask PlayMovePosition(this CameraComponent self, Vector3 endValue, float duration)
        {
            Transform transform = self.mainCamera.transform;

            if (duration > 0)
                DotweenHelper.DOMove(transform, endValue, duration).Coroutine();
            else
                transform.position = endValue;

            await ETTask.CompletedTask;
        }

        public static async ETTask PlayRotate(this CameraComponent self, Vector3 endValue, float duration)
        {
            Transform transform = self.mainCamera.transform;

            if (duration > 0)
                DotweenHelper.DORotate(transform, endValue, duration).Coroutine();
            else
                transform.eulerAngles = endValue;

            await ETTask.CompletedTask;
        }
    }
}