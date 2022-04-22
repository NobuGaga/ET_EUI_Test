using UnityEngine;

namespace ET
{
    /// <summary> Mono 层自己运行的逻辑处理 </summary>
    public static class BattleMonoComponent
    {
        static BattleMonoComponent()
        {
            // 调用注意顺序, 先初始化 Mono 层引用类
            ReferenceHelper.Init();
            ConstHelper.Init(Screen.width, Screen.height, ReferenceHelper.CannoCamera.orthographicSize);
            BattleDebugComponent.Init();
        }

        public static void EnterBattle() => Mono.Instance.CodeLoader.Update += Update;

        private static void Update()
        {

        }

        public static void ExitBattle()
        {
            Mono.Instance.CodeLoader.Update -= Update;
            BattleDebugComponent.Clear();
            UnitMonoComponent.Dispose();
        }
    }
}