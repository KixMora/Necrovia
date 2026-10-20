using UnityEngine;
using GameAnalyticsSDK;

public class GameAnalyticsInitializer : MonoBehaviour
{
    private static bool initialized;

    private void Start()
    {
        if (initialized)
            return;

        GameAnalytics.Initialize();
        initialized = true;

        Debug.Log("[GameAnalyticsInitializer] GameAnalytics initialized");
    }
}