using System.Collections.Generic;
using UnityEditor;

#if MultiLanguage_ET
namespace ET
#else
namespace MultiLanguage
#endif
{
#if UNITY_EDITOR
 
    // 框架类型
    internal enum FrameworkType
    {
        ET = 1,
        xLua = 2,
        uLua = 3,
    }
#endif

    public static class LanguageHelper
    {
        // 当前使用的语言标识(具体语言对应关系根据框架类型不同而设置)
        private static ushort currentLanguage = 1;

#if UNITY_EDITOR
        // 设置使用的框架类型
        private const FrameworkType FRAMEWORK_TYPE = FrameworkType.ET;
        // 宏定义分隔符
        private const string SPLIT_SYMBOL = ";";

        // 框架类型宏定义列表
        private static Dictionary<FrameworkType, string> FrameworkSymbolDic = new Dictionary<FrameworkType, string>
        {
            { FrameworkType.ET, "MultiLanguage_ET" },
            { FrameworkType.xLua, "MultiLanguage_xLua" },
            { FrameworkType.uLua, "MultiLanguage_uLua" },
        };
        private static string m_languageSymbol = FrameworkSymbolDic[FRAMEWORK_TYPE];

        // 编辑器使用到组件时设置宏定义
        static LanguageHelper() => SetBuildLanguageSymbol();

        public static void SetBuildLanguageSymbol()
        {
            bool isSetSymbol = false;
            isSetSymbol = SetBuildTargetGroupSymbols(BuildTargetGroup.Standalone) ? true : isSetSymbol;
            isSetSymbol = SetBuildTargetGroupSymbols(BuildTargetGroup.WSA) ? true : isSetSymbol;
            isSetSymbol = SetBuildTargetGroupSymbols(BuildTargetGroup.Android) ? true : isSetSymbol;
            isSetSymbol = SetBuildTargetGroupSymbols(BuildTargetGroup.iOS) ? true : isSetSymbol;

            if (isSetSymbol)
                AssetDatabase.RefreshSettings();
        }

        private static bool SetBuildTargetGroupSymbols(BuildTargetGroup targetGroup)
        {
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            if (symbols.Contains(m_languageSymbol))
                return false;

            symbols = string.Join(SPLIT_SYMBOL, symbols, m_languageSymbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbols);
            return true;
        }
#endif

        public static string GetText(string key)
        {
#if MultiLanguage_ET
            return TextConfigHelper.GetText(key);
#elif MultiLanguage_xLua
            return string.Format("{0}_xLua_value", key);
#elif MultiLanguage_uLua
            return string.Format("{0}_uLua_value", key);
#else
            return key;
#endif
        }

        public static void SetLanguage(ushort language) => currentLanguage = language;

        public static ushort GetLanguage() => currentLanguage;
    }
}