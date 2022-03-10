using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ET;

namespace MultiLanguage
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MultiLanguageText), true)]
    public sealed class MultiLanguageTextEditor : UnityEditor.UI.TextEditor
    {
        // 节点名前缀
        const string TextObjNameFormat = "text_{0}";

        string m_lastKey = string.Empty;
        string m_lastText = string.Empty;
        SerializedProperty m_Key;
        SerializedProperty m_Text;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Key = serializedObject.FindProperty("m_Key");
            m_Text = serializedObject.FindProperty("m_Text");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Key);
            string key = m_Key.stringValue;
            string text = m_Text.stringValue;
            if (key == m_lastKey && text == m_lastText)
            {
                base.OnInspectorGUI();
                return;
            }

            m_lastKey = key;
            target.name = string.Format(TextObjNameFormat, key);
            m_lastText = LanguageHelper.GetText(key);
            m_Text.stringValue = m_lastText;
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }

        // 设置菜单位置在 Text 后面 2001
        [MenuItem("GameObject/UI/Text - MultiLanguage", false, 2001)]
        public static void AddMultiLanguageText()
        {
            // MenuOption 类 Unity 未开放出来, 这里使用调用菜单栏方法添加 Text 组件
            // 再将 Text 组件序列化信息复制给 MultiLanguageText 组件
            EditorApplication.ExecuteMenuItem("GameObject/UI/Text");

            // Unity 创建完 Text 组件后会设置选定
            GameObject go = Selection.activeGameObject;
            Text textComponent = go.GetComponent<Text>();
            SerializedObject serializedData = new SerializedObject(textComponent);
            serializedData.Update();

            bool oldEnable = textComponent.enabled;
            textComponent.enabled = false;

            Type targetType = typeof(MultiLanguageText);
            // 只能获取所有 MonoScript 脚本, 不能直接找到 MultiLanguageText 脚本
            foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>())
            {
                if (script.GetClass() == targetType)
                {
                    serializedData.FindProperty("m_Script").objectReferenceValue = script;
                    serializedData.ApplyModifiedProperties();
                    break;
                }
            }
            (serializedData.targetObject as MonoBehaviour).enabled = oldEnable;
        }
    }
}