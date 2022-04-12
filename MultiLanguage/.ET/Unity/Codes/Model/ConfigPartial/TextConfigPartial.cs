using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using MultiLanguage;

namespace ET
{
    public partial class TextConfigCategory
    {
        private Dictionary<string, int> m_KeyIdDic = new Dictionary<string, int>(ushort.MaxValue);

        public override void AfterEndInit()
        {
            base.AfterEndInit();
            foreach (TextConfig config in dict.Values)
                m_KeyIdDic.Add(config.Key, config.Id);

            if (Application.isPlaying)
                TextConfigHelper.SetGetTextFunction(GetText);
        }

        #region 编辑器模式非运行时使用

        // 加载 ab 资源并使用其来实例化配置表类(编辑时使用)
        public static void LoadAndInstantiateConfig()
        {
            if (!Define.IsEditor || Application.isPlaying)
                return;

            Type configType = typeof(TextConfigCategory);
            string configTypeName = configType.Name;
            string configABName = $"config{Define.ABSuffix}";
            try
            {
                string[] assetBundlePaths = Define.GetAssetPathsFromAssetBundle(configABName);
                foreach (string assetBundlePath in assetBundlePaths)
                {
                    if (Path.GetFileNameWithoutExtension(assetBundlePath) != configTypeName)
                        continue;

                    UnityEngine.Object resource = Define.LoadAssetAtPath(assetBundlePath);
                    TextAsset textAsset = resource as TextAsset;
                    byte[] configBytes = textAsset.bytes;
                    object category = ProtobufHelper.FromBytes(configType, configBytes, 0, configBytes.Length);
                    return;
                }
            }
            catch (Exception exception)
            {
                Log.Error($"TextConfigCategory.LoadAndInstantiateConfig() exception msg = { exception.Message }");
            }
        }

        #endregion

        public string GetText(string key)
        {
            if (!m_KeyIdDic.ContainsKey(key))
                throw new Exception($"文本配置找不到 Key，配置表名 : { nameof(TextConfig) }，配置 Key : { key }");

            int id = m_KeyIdDic[key];
            TextConfig config = null;
            try
            {
                config = Get(id);
            }
            catch (Exception exception)
            {
                Log.Error($"TextConfigCategory.GetText() exception msg = { exception.Message }");
                Debug.LogErrorFormat($"TextConfigCategory.GetText() exception msg = { exception.Message }");
            }

            if (config == null)
                return key;

            // 根据语言标识不同进行不同的返回, 这里直接简单写死区分获取对应字段了
            switch (LanguageHelper.GetLanguage())
            {
                case 0:
                    return config.Text0;
                case 1:
                    return config.Text1;
                default:
                    return key;
            }
        }
    }
}