using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour
{
    private void Awake()
    {
    }

    private void Start()
    {
        GameAnalyticsManager.Instance.Initialize();
        SceneManager.LoadScene("BattleScene");
    }
}
