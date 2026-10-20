using System.Collections;
using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "MainMenu";
    [SerializeField] private float minimumBootstrapTime = 5f;

    private IEnumerator Start()
    {
        Debug.Log("[BootstrapLoader] Start");
        Debug.Log("[BootstrapLoader] Waiting: " + minimumBootstrapTime + " sec");

        yield return new WaitForSecondsRealtime(minimumBootstrapTime);

        if (SceneTransitionManager.Instance != null)
        {
            Debug.Log("[BootstrapLoader] Loading first scene: " + firstSceneName);
            SceneTransitionManager.Instance.LoadScene(firstSceneName);
        }
        else
        {
            Debug.LogError("[BootstrapLoader] SceneTransitionManager.Instance is NULL");
        }
    }
}