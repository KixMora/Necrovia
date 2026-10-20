using TMPro;
using UnityEngine;

public class MissionObjectiveLineUI : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;

    public void SetText(string text, bool completed)
    {
        if (objectiveText == null)
            return;

        objectiveText.text = text;
        objectiveText.color = completed ? completedColor : normalColor;
    }
}