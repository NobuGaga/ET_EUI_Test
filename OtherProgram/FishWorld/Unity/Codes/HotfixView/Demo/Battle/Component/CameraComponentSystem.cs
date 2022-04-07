using ET.EventType;
using UnityEngine;

namespace ET
{
    public static class CameraComponentSystem
    {
        public static void SetTransformByAreaId(this CameraComponent self, int areaId)
        {
            FisheryLevelConfig config = FisheryLevelConfigCategory.Instance.Get(areaId);
            string vectorString = config.CameraParamPosition;
            float time = (float)config.CameraMoveTime / FishConfig.MilliSecond;

            // Battle TODO 先不设置鱼节点变换, 因为主界面场景摄像机写死变换鱼影响到这边逻辑
            Vector3 vector;
            if (VectorStringHelper.TryParseVector3(vectorString, out vector))
                self.PlayMovePosition(vector, time);

            vectorString = config.CameraParamRotation;
            time = (float)config.CameraRotateTime / FishConfig.MilliSecond;

            if (VectorStringHelper.TryParseVector3(vectorString, out vector))
                self.PlayRotate(vector, time);
        }

        public static void StopTransformTween(this CameraComponent self)
        {
            Transform cameraTrans = self.mainCamera.transform;
            DotweenHelper.DOKill(cameraTrans);
        }

        public static void PlayMovePosition(this CameraComponent self, Vector3 endValue, float duration)
        {
            Transform cameraTrans = self.mainCamera.transform;
            Transform fishRootTrans = GlobalComponent.Instance.FishRoot;
            //Vector3 fishRootEndValue = endValue - cameraTrans.position + fishRootTrans.position;

            if (duration > 0)
            {
                DotweenHelper.DOMove(cameraTrans, endValue, duration).Coroutine();
                //DotweenHelper.DOMove(fishRootTrans, fishRootEndValue, duration).Coroutine();
            }
            else
            {
                cameraTrans.position = endValue;
                //fishRootTrans.position = fishRootEndValue;
            }
        }

        public static void PlayRotate(this CameraComponent self, Vector3 endValue, float duration)
        {
            Transform cameraTrans = self.mainCamera.transform;
            Transform fishRootTrans = GlobalComponent.Instance.FishRoot;
            //Vector3 fishRootEndValue = endValue - cameraTrans.eulerAngles + fishRootTrans.eulerAngles;

            if (duration > 0)
            {
                DotweenHelper.DORotate(cameraTrans, endValue, duration).Coroutine();
                //DotweenHelper.DOMove(fishRootTrans, fishRootEndValue, duration).Coroutine();
            }
            else
            {
                cameraTrans.eulerAngles = endValue;
                //fishRootTrans.eulerAngles = fishRootEndValue;
            }
        }
    }

    public class AfterEnterRoom_CameraComponent : AEvent<ReceiveEnterRoom>
    {
        protected override void Run(ReceiveEnterRoom args)
        {
            CameraComponent cameraComponent = args.CurrentScene.ZoneScene().GetComponent<CameraComponent>();
            cameraComponent.SetTransformByAreaId(args.AreaId);
        }
    }

    public class AfterExchangeArea_CameraComponent : AEvent<ReceiveExchangeArea>
    {
        protected override void Run(ReceiveExchangeArea args)
        {
            FisheryComponent fisheryComponent = args.FisheryComponent;
            CameraComponent cameraComponent = fisheryComponent.ZoneScene().GetComponent<CameraComponent>();
            cameraComponent.SetTransformByAreaId(args.FisheryComponent.AreaId);
        }
    }

    public class UIFisheriesE_exitEventArgs_CameraComponent : AEvent<UIFisheriesE_exitEventArgs>
    {
        protected override void Run(UIFisheriesE_exitEventArgs args)
        {
            UIFisheriesComponent fisheryComponent = args.UIFisheriesComponent;
            CameraComponent cameraComponent = fisheryComponent.ZoneScene().GetComponent<CameraComponent>();
            cameraComponent.StopTransformTween();
        }
    }
}