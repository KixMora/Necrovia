using UnityEngine;

[ExecuteAlways]
public class MissionPoint : MonoBehaviour
{
    public MissionData missionData;

    public Vector2 GetAnchoredPosition()
    {
        RectTransform rect = transform as RectTransform;
        if (rect == null)
            return Vector2.zero;

        return rect.anchoredPosition;
    }

    public void ApplyToMissionData()
    {
        if (missionData == null)
        {
            Debug.LogWarning("[MissionPoint] missionData is NULL on " + gameObject.name);
            return;
        }

        RectTransform rect = transform as RectTransform;
        if (rect == null)
        {
            Debug.LogError("[MissionPoint] RectTransform not found on " + gameObject.name);
            return;
        }

        missionData.mapPosition = rect.anchoredPosition;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(missionData);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"[MissionPoint] Saved {missionData.missionTitle} position = {missionData.mapPosition}");
    }
}