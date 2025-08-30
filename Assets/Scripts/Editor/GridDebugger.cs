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
        EditorGUILayout.LabelField("����� ���", EditorStyles.boldLabel);

        if(GUILayout.Button("�׸��� �缳��"))
        {
            mainScene.RebuildGrid();
        }
    }
}
