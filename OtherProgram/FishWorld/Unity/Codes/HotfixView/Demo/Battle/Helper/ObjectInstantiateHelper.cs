using System;
using UnityEngine;

namespace ET
{
    public static class ObjectInstantiateHelper
    {
        /// <summary>
        /// 通用加载战斗用预设模型
        /// 外部使用 xxx.Coroutine() 调用, 不用等待返回值
        /// </summary>
        public static async ETTask<GameObject> InitModel(Scene currentScene, Entity unit, string assetBundlePath, string assetName)
        {
            GameObject gameObject = await LoadModelPrefab(currentScene, assetBundlePath, assetName);

            if (gameObject == null)
            {
                string errorMsg = $"ObjectInstantiateHelper.InitModel error assetBundlePath = { assetBundlePath }, assetName = { assetName }";
                throw new Exception(errorMsg);
            }

            if (unit.IsDisposed)
            {
                // Battle Warning 异步加载完回来如果 Unit 已经被销毁则直接销毁 gameObject
                UnityEngine.Object.Destroy(gameObject);
                return null;
            }

            return gameObject;
        }

        /// <summary>
        /// 通用加载战斗用预设模型
        /// 原逻辑先走 AssetBundle 加载, 再调用对象池获取, 正常逻辑, 对象池没有走加载流程
        /// 判断 AssetBundle 是否加载, 没有则加载 ResourcesLoaderComponent.LoadAsync 完成了这一步, 直接调用即可
        /// 再实例化预设, 这里的加载 AssetBundle 跟获取 AssetBundle 里的资源都会抛出异常
        /// </summary>
        /// <param name = "currentScene" > 挂载加载组件的场景 </ param >
        /// <param name = "assetBundlePath" > AssetBundle 路径</param>
        /// <param name = "assetName" > AssetName 名</param>
        /// <returns>GameObject 实例化对象</returns>
        private static async ETTask<GameObject> LoadModelPrefab(Scene currentScene, string assetBundlePath, string assetName)
        {
            try
            {
                ResourcesLoaderComponent resourcesLoaderCom = currentScene.GetComponent<ResourcesLoaderComponent>();

                await resourcesLoaderCom.LoadAsync(assetBundlePath);
                // Battle TODO 后面改用异步
                GameObject prefab = ResourcesComponent.Instance.GetAsset(assetBundlePath, assetName) as GameObject;

                GameObject go = UnityEngine.Object.Instantiate(prefab);
                return go;
            }
            catch (Exception exception)
            {
                throw new Exception($"ObjectInstantiateHelper.LoadModelPrefab()", exception);
            }
        }
    }
}