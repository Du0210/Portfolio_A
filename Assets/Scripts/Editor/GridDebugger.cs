using HDU.Managers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MainScene))]
public class GridDebugger : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MainScene mainScene = (MainScene)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("디버그 기능", EditorStyles.boldLabel);

        if(GUILayout.Button("그리드 재설정"))
        {
            mainScene.RebuildGrid();
        }
    }
}
