using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using ET;

namespace MultiLanguage
{
    public static class TextConfigHelper
    {
        // 运行时使用方法
        private static System.Func<string, string> m_getTextDelegate;

        // 外部 Model TextConfigCategory 设置运行时使用方法
        public static void SetGetTextFunction(System.Func<string, string> func) =>
                            m_getTextDelegate = func;

#if UNITY_EDITOR

        // 编辑器模式非运行时调用方法
        private static MethodInfo m_getTextMethod;
        private static object[] m_arguments;

        static TextConfigHelper()
        {
            if (!Application.isEditor)
                return;
            
            m_arguments = new object[] { string.Empty };
            // 编辑器模式非运行时反射调用读取配置表
            LoadDLL();
        }

        // 编辑器模式非运行时调用方法
        private static void LoadDLL()
        {
            string abName = $"code{Define.ABSuffix}";
            //(AssetBundle assetsBundle, Dictionary<string, Object> assetsBundlDic) = AssetsBundleHelper.LoadBundle(abName);
            Dictionary<string, Object> assetsBundlDic = AssetsBundleHelper.LoadBundle(abName, out AssetBundle assetsBundle);
            TextAsset codeTextAsset = assetsBundlDic["Code.dll"] as TextAsset;
            byte[] assBytes = codeTextAsset.bytes;
            codeTextAsset = assetsBundlDic["Code.pdb"] as TextAsset;
            byte[] pdbBytes = codeTextAsset.bytes;
            Assembly assembly = Assembly.Load(assBytes, pdbBytes);
            if (assetsBundle != null)
                assetsBundle.Unload(true);

            BindingFlags funcFlags = BindingFlags.Static | BindingFlags.Public;
            System.Type textConfigCategoryType = assembly.GetType("ET.TextConfigCategory");
            m_getTextMethod = textConfigCategoryType.GetMethod("GetText", funcFlags);
            MethodInfo initMethod = textConfigCategoryType.GetMethod("LoadAndInstantiateConfig", funcFlags);
            initMethod.Invoke(null, null);
        }
#endif

        public static string GetText(string key)
        {
#if UNITY_EDITOR
            if (!Application.isEditor)
                return RuntimeGetText(key);

            m_arguments[0] = key;
            try
            {
                return m_getTextMethod.Invoke(null, m_arguments) as string;
            }
            catch (System.Exception)
            //catch (System.Exception exception)
            {
                //Log.Error($"TextConfigHelper.GetText() exception msg = { exception.Message }");
                //Debug.LogErrorFormat($"TextConfigHelper.GetText() exception msg = { exception.Message }");
                return key;
            }
#else
            return RuntimeGetText(key);
#endif
        }

        private static string RuntimeGetText(string key)
        {
            // 如果为空则 ET.TextConfigCategory.AfterEndInit 的运行标识 Application.isEditor 有问题
            if (m_getTextDelegate == null)
            {
                Log.Error("Model ET.TextConfigCategory not call TextConfigHelper.SetGetTextFunction");
                //Debug.LogError("Model ET.TextConfigCategory not call TextConfigHelper.SetGetTextFunction");
                return key;
            }

            return m_getTextDelegate(key);
        }
    }
}