using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StaticFontTextController))]
public class StaticFontTextControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Test FadeOut"))
        {
            StaticFontTextController controller = (StaticFontTextController)target;

            controller.SetAlpha(1f);

            controller.BeginFadeOut(1f, 1f, 2);
        }
    }
}
