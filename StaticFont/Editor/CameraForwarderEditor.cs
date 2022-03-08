using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace StaticFont.Extension
{
    /// <summary>
    /// 跟随面向摄像机组件面板类
    /// 为了不存 Camera 类型, 存 Transform 成员变量
    /// 在赋值的时候赋值 Camera, 从而修改 Transform 的值
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraForwarder), true)]
    public sealed class CameraForwarderEditor : GraphicEditor
    {

        private Camera m_lastCamera;
        private SerializedProperty m_forwardCamera;
        private SerializedProperty m_baseScale;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_forwardCamera = serializedObject.FindProperty("m_forwardCamera");
            m_baseScale = serializedObject.FindProperty("m_baseScale");
            m_lastCamera = m_forwardCamera.objectReferenceValue as Camera;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_forwardCamera);
            EditorGUILayout.PropertyField(m_baseScale);
            Camera camera = m_forwardCamera.objectReferenceValue as Camera;
            if (camera == m_lastCamera)
                return;

            m_lastCamera = camera;
            // 跟运行时一样调用属性方法修改
            (target as CameraForwarder).ForwardCamera = camera;
            serializedObject.ApplyModifiedProperties();
        }
    }
}