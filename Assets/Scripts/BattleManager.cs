using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private MainCamera m_MainCamera;
    [SerializeField] private GameObject m_Step;
    [SerializeField] private Character m_PlayerChara;
    [SerializeField] private Character m_EnemyChara;
    [SerializeField] private float m_PlayerDeadLineX = -3f;
    [SerializeField] private float m_EnemyDeadLineX = 3f;
    [SerializeField] private GameObject m_SmokePanel;
    [SerializeField] private GameObject m_WinResult;
    [SerializeField] private GameObject m_LoseResult;
    [SerializeField] private GameObject m_PreparetionFooter;
    [SerializeField] private Text m_LevelText;
    private int m_LevelProgressNunmber;
    [SerializeField] private ProgressBar m_ProgressBar;
    [SerializeField] private List<SideWall> m_SideWallList = new List<SideWall>();
    [SerializeField] private CountDownText m_CountdownText;
    [SerializeField] private GameObject m_CustomizeButton;
    [SerializeField] private Text m_CoinText;
    private int m_Coin;
    [SerializeField] private GameObject m_ConfettiRoot;
    [SerializeField] private Text m_AcquireCoinText;

    public enum E_BATTLE_STATE
    {
        MENU,
        PREPARATION,
        COUNT_DOWN,
        BATTLE,
        WIN,
        LOSE,
    }
    private E_BATTLE_STATE m_State;
    private float m_WaitTime;

    private void Awake()
    {
        m_LevelProgressNunmber = PlayerPrefs.GetInt("LEVEL_PROGRESS", 1);
        m_Coin = PlayerPrefs.GetInt("Coin", 0);
        m_EnemyChara.SetAiLevel(m_LevelProgressNunmber);
    }

    private void Start()
    {
        RefreshLevelText();
        m_WaitTime = 0f;

        ChangeState(E_BATTLE_STATE.MENU);
        RefreshCoinText();
    }

    private void Update()
    {
        if(m_WaitTime >= 0f)
        {
            m_WaitTime -= Time.deltaTime;
        }

        StepUpdate();

        switch (m_State)
        {
            case E_BATTLE_STATE.MENU:
                // ボタンをタッチした場合は無視
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                if ((Application.isEditor == true && Input.GetMouseButtonDown(0)) || (Application.isEditor == false && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
                {
                    ChangeState(E_BATTLE_STATE.PREPARATION);
                }
                break;

            case E_BATTLE_STATE.PREPARATION:
                if (m_WaitTime <=  0f)
                {
                    ChangeState(E_BATTLE_STATE.COUNT_DOWN);
                }
                else if (m_WaitTime <= 0.5f) 
                {
                    m_MainCamera.SetState(MainCamera.E_CAMERA_STATE.ZOOM_IN);
                }
                break;

            case E_BATTLE_STATE.COUNT_DOWN:
                if(m_WaitTime <= 0f)
                {
                    ChangeState(E_BATTLE_STATE.BATTLE);
                }
                break;

            case E_BATTLE_STATE.BATTLE:
                if (m_Step.transform.position.x >= m_EnemyDeadLineX)
                {
                    ChangeState(E_BATTLE_STATE.WIN);
                }
                else if (m_Step.transform.position.x <= m_PlayerDeadLineX)
                {
                    ChangeState(E_BATTLE_STATE.LOSE);
                }
                break;

            case E_BATTLE_STATE.WIN:
                break;

            case E_BATTLE_STATE.LOSE:
                break;
        }


        float barRatio = (m_Step.transform.position.x - m_PlayerDeadLineX) / (m_EnemyDeadLineX - m_PlayerDeadLineX);
        m_ProgressBar.UpdateBarRatio(barRatio);
    }

    private void ChangeState(E_BATTLE_STATE state)
    {
        switch (state)
        {
            case E_BATTLE_STATE.MENU:
                m_PreparetionFooter.SetActive(true);
                break;

            case E_BATTLE_STATE.PREPARATION:
                m_MainCamera.SetState(MainCamera.E_CAMERA_STATE.ZOOM_OUT);
                m_PreparetionFooter.SetActive(false);
                m_CustomizeButton.SetActive(false);

                m_SideWallList.ForEach(e => e.OpenStart());
                m_WaitTime = m_SideWallList[0].GetMoveDuration()-0.5f;
                break;

            case E_BATTLE_STATE.COUNT_DOWN:
                m_CountdownText.gameObject.SetActive(true);
                m_CountdownText.StartCountDown();
                m_WaitTime = 3.0f;
                break;

            case E_BATTLE_STATE.BATTLE:
                m_MainCamera.SetState(MainCamera.E_CAMERA_STATE.TRACE);
                m_PlayerChara.SetIsCanSwingDown(true);
                m_EnemyChara.SetIsCanSwingDown(true);
                break;

            case E_BATTLE_STATE.WIN:
                if (GameAnalyticsManager.Instance != null)
                {
                    GameAnalyticsManager.Instance.LogDesignEvent("StageProgress:" + m_LevelProgressNunmber.ToString() + ":Clear");
                }

                // 勝利
                m_LevelProgressNunmber += 1;
                PlayerPrefs.SetInt("LEVEL_PROGRESS", m_LevelProgressNunmber);
                PlayerPrefs.Save();

                StartCoroutine("WinProduction");
                break;

            case E_BATTLE_STATE.LOSE:
                if (GameAnalyticsManager.Instance != null)
                {
                    GameAnalyticsManager.Instance.LogDesignEvent("StageProgress:" + m_LevelProgressNunmber.ToString() + ":Failed");
                }

                // 敗北
                StartCoroutine("LoseProduction");

                ShowInterstitial();
                break;
        }

        m_State = state;
    }

    private void StepUpdate()
    {
        if (m_Step.transform.position.x >= m_EnemyDeadLineX)
        {
            m_Step.transform.position = new Vector3(m_EnemyDeadLineX, m_Step.transform.position.y, m_Step.transform.position.z);
        }
        else if (m_Step.transform.position.x <= m_PlayerDeadLineX)
        {
            m_Step.transform.position = new Vector3(m_PlayerDeadLineX, m_Step.transform.position.y, m_Step.transform.position.z);
        }
    }

    private IEnumerator WinProduction()
    {
        m_PlayerChara.Pause();
        m_EnemyChara.Pause();
        m_EnemyChara.Death();

        int level = m_LevelProgressNunmber - 1; // XXX: クリア後に加算した後だから引く
        int acquireCoin = 100 + 10 * level;
        m_AcquireCoinText.text = "+" + acquireCoin.ToString();
        AddCoin(acquireCoin);

        yield return new WaitForSeconds(0.5f);

        m_WinResult.SetActive(true);
        Button button = m_WinResult.GetComponentInChildren<Button>();
        m_SmokePanel.SetActive(true);

        DoConfetti();

        yield return new WaitForSeconds(0.5f);
        button.interactable = true;

        ShowInterstitial();
    }

    private IEnumerator LoseProduction()
    {
        m_PlayerChara.Pause();
        m_EnemyChara.Pause();
        m_PlayerChara.Death();

        yield return new WaitForSeconds(0.5f);

        m_LoseResult.SetActive(true);
        Button button = m_LoseResult.GetComponentInChildren<Button>();
        m_SmokePanel.SetActive(true);

        DoConfetti();

        yield return new WaitForSeconds(0.5f);
        button.interactable = true;

        ShowInterstitial();
    }

    private void DoConfetti()
    {
        ParticleSystem[] m_ParticleArray = m_ConfettiRoot.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem particle in m_ParticleArray)
        {
            particle.gameObject.SetActive(true);
        }
    }

    private void RefreshLevelText()
    {
        m_LevelText.text = "LEVEL" + m_LevelProgressNunmber.ToString();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("BattleScene");
    }

    private void ShowInterstitial()
    {
        if (AdMob.Instance == null)
        {
            return;
        }

        // インタースティシャル広告表示。とりあえず40%の確率で（ステージ5までは出ない）
        if (m_LevelProgressNunmber > 5 && Random.Range(0, 101) <= 40)
        {
            AdMob.Instance.DisplayInterstitial();
        }
    }

    private void AddCoin(int add)
    {
        m_Coin = Mathf.Clamp(m_Coin + add, 0, 999999999);
        PlayerPrefs.SetInt("Coin", m_Coin);
        PlayerPrefs.Save();
    }

    private void RefreshCoinText()
    {
        m_CoinText.text = m_Coin.ToString();
    }

    public void OnTappedNextButton()
    {
        RestartGame();
    }

    public void OnTappedRetryButton()
    {
        RestartGame();
    }
}
