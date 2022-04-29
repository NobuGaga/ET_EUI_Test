using System;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(BattleLogicComponent))]
    [FriendClass(typeof(BattleViewComponent))]
    public static class ObjectInstantiateHelper
    {
        /// <summary>
        /// 通用加载战斗用预设模型
        /// 原逻辑先走 AssetBundle 加载, 再调用对象池获取, 正常逻辑, 对象池没有走加载流程
        /// 判断 AssetBundle 是否加载, 没有则加载 ResourcesLoaderComponent.LoadAsync 完成了这一步, 直接调用即可
        /// 再实例化预设, 这里的加载 AssetBundle 跟获取 AssetBundle 里的资源都会抛出异常
        /// </summary>
        /// <param name = "assetBundlePath" > AssetBundle 路径</param>
        /// <param name = "assetName" > AssetName 名</param>
        /// <returns>GameObject 实例化对象</returns>
        public static async ETTask<GameObject> LoadModelPrefab(string assetBundlePath, string assetName)
        {
            Scene currentScene = BattleLogicComponent.Instance.CurrentScene;
            var ResourcesLoaderComponent = currentScene.GetComponent<ResourcesLoaderComponent>();
            try
            {
                await ResourcesLoaderComponent.LoadAsync(assetBundlePath);
                return ResourcesComponent.Instance.GetAsset(assetBundlePath, assetName) as GameObject;
            }
            catch (Exception exception)
            {
                Log.Error($"ObjectInstantiateHelper.LoadModelPrefab()", exception.Message);
                return null;
            }
        }
    }
}