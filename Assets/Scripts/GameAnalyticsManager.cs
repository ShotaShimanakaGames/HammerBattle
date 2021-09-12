using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsManager : SingletonMonoBehaviour<GameAnalyticsManager>
{
    public void Initialize()
    {
        GameAnalytics.Initialize();
    }

    public void LogProgressEvent(GAProgressionStatus state, string eventId)
    {
        GameAnalytics.NewProgressionEvent(state, eventId);
    }

    public void LogDesignEvent(string eventId)
    {
        Debug.Log(eventId);
        GameAnalytics.NewDesignEvent(eventId);
    }
}
