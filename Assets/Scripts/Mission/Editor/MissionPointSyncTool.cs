#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionPoint))]
public class MissionPointSyncTool : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        MissionPoint point = (MissionPoint)target;

        if (GUILayout.Button("Sync To Mission Data"))
        {
            point.ApplyToMissionData();
        }
    }
}
#endif