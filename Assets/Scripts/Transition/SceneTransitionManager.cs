using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private float minimumBlackScreenTime = 0.05f;

    private bool isLoading;

    private void Awake()
    {
        Debug.Log("[SceneTransitionManager] Awake on " + gameObject.name);

        if (Instance != null && Instance != this)
        {
            Debug.Log("[SceneTransitionManager] Duplicate destroyed: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // public void LoadScene(string sceneName)
    // {
    //     Debug.Log("[SceneTransitionManager] LoadScene called: " + sceneName);

    //     if (!isLoading)
    //         StartCoroutine(LoadSceneRoutine(sceneName));
    // }
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;

        Debug.Log("[SceneTransitionManager] LoadSceneRoutine start");

        if (ScreenFader.Instance != null)
        {
            Debug.Log("[SceneTransitionManager] Calling FadeOut");
            yield return ScreenFader.Instance.FadeOut();
        }
        else
        {
            Debug.LogError("[SceneTransitionManager] ScreenFader.Instance is NULL");
        }

        if (minimumBlackScreenTime > 0f)
            yield return new WaitForSecondsRealtime(minimumBlackScreenTime);

        Debug.Log("[SceneTransitionManager] Loading scene async: " + sceneName);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
            yield return null;

        Debug.Log("[SceneTransitionManager] Scene load complete: " + sceneName);

        isLoading = false;
    }
}