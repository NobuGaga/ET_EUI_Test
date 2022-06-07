using UnityEngine;
using ET.EventType;

namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(FisheryComponent))]
    [FriendClass(typeof(CameraComponent))]
    public static class CameraComponentSystem
    {
        public static void SetTransformByAreaId(this CameraComponent self, int areaId)
        {
            FisheryLevelConfig config = FisheryLevelConfigCategory.Instance.Get(areaId);
            string vectorString = config.CameraParamPosition;
            float time = (float)config.CameraMoveTime / FishConfig.MilliSecond;

            // Battle TODO 先不设置鱼节点变换, 因为主界面场景摄像机写死变换鱼影响到这边逻辑
            Vector3 vector;
            if (VectorStringHelper.TryParseVector(vectorString, out vector))
                self.PlayMovePosition(vector, time).Coroutine();

            vectorString = config.CameraParamRotation;
            time = (float)config.CameraRotateTime / FishConfig.MilliSecond;

            if (VectorStringHelper.TryParseVector(vectorString, out vector))
                self.PlayRotate(vector, time);

            self.PlayFieldOfView(config.FieldOfView, time);
        }

        public static void StopTransformTween(this CameraComponent self)
        {
            Transform cameraTrans = self.mainCamera.transform;
            DotweenHelper.DOKill(cameraTrans);
        }

        public static async ETTask PlayMovePosition(this CameraComponent self, Vector3 endValue, float duration)
        {
            Transform cameraTrans = self.mainCamera.transform;
            Transform fishRootTrans = GlobalComponent.Instance.FishRoot;

            DotweenHelper.DOKill(cameraTrans);

            if (duration > 0)
            {
                BattleLogicComponent.Instance.FisheryComponent.IsMovingCamera = true;
                DotweenHelper.DOMove(fishRootTrans, endValue, duration).Coroutine();
                await DotweenHelper.DOMove(cameraTrans, endValue, duration);
                BattleLogicComponent.Instance.FisheryComponent.IsMovingCamera = false;
            }
            else
            {
                fishRootTrans.position = endValue;
                cameraTrans.position = endValue;
            }
        }

        public static void PlayRotate(this CameraComponent self, Vector3 endValue, float duration)
        {
            Transform cameraTrans = self.mainCamera.transform;
            Transform fishRootTrans = GlobalComponent.Instance.FishRoot;

            if (duration > 0)
            {
                DotweenHelper.DORotate(cameraTrans, endValue, duration).Coroutine();
                DotweenHelper.DORotate(fishRootTrans, endValue, duration).Coroutine();
            }
            else
            {
                cameraTrans.eulerAngles = endValue;
                fishRootTrans.eulerAngles = endValue;
            }
        }

        public static void PlayFieldOfView(this CameraComponent self, int fieldOfView, float duration)
        {
            Camera mainCamera = self.mainCamera;

            if (duration > 0)
            {
                // Battle TODO
            }
            else
            {
                mainCamera.fieldOfView = fieldOfView;
            }
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    public class AfterEnterRoom_CameraComponent : AEvent<ReceiveEnterRoom>
    {
        protected override void Run(ReceiveEnterRoom args)
        {
            Scene zoneScene = BattleLogicComponent.Instance.ZoneScene;
            var cameraComponent = zoneScene.GetComponent<CameraComponent>();
            cameraComponent.SetTransformByAreaId(args.AreaId);
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(FisheryComponent))]
    public class AfterExchangeArea_CameraComponent : AEvent<ReceiveExchangeArea>
    {
        protected override void Run(ReceiveExchangeArea args)
        {
            Scene zoneScene = BattleLogicComponent.Instance.ZoneScene;
            var cameraComponent = zoneScene.GetComponent<CameraComponent>();
            cameraComponent.SetTransformByAreaId(args.FisheryComponent.AreaId);
        }
    }

    [FriendClass(typeof(BattleLogicComponent))]
    public class UIFisheriesE_exitEventArgs_CameraComponent : AEventAsync<UIFisheriesE_exitEventArgs>
    {
        protected override async ETTask Run(UIFisheriesE_exitEventArgs args)
        {
            Scene zoneScene = BattleLogicComponent.Instance.ZoneScene;
            var cameraComponent = zoneScene.GetComponent<CameraComponent>();
            cameraComponent.StopTransformTween();
            await ETTask.CompletedTask;
        }
    }
}