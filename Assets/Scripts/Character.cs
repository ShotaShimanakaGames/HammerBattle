using DitzelGames.FastIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private bool m_IsLeft;
    [SerializeField] private Transform m_HammerBase;
    [SerializeField] private float m_SwingUpMaxAngleZ;
    [SerializeField] private float m_SwingUpMinAngleZ;
    [SerializeField] private float m_SwingDownAngleZ;
    [SerializeField] private GameObject m_Step;
    private float m_Timer;
    [SerializeField] private float m_RoundTripSpan = 2f;
    [SerializeField] private float m_RoundTripSpanMax = 2f;
    [SerializeField] private float m_RoundTripSpanMin = 1f;
    private E_STATE m_State;
    [SerializeField] private float m_SwingDownTime = 0.1f;
    [SerializeField] private GameObject m_HitEffectPrefab;
    [SerializeField] private Transform m_HitEffectRoot;
    [SerializeField] private bool m_IsUsingAI;
    [SerializeField] private float m_MaxPower;
    [SerializeField] private float m_MinPower;
    private float m_Power;
    [SerializeField] private GameObject m_PowerEffect;
    [SerializeField] private float m_PowerEffectScaleMax = 0.7f;
    [SerializeField] private float m_PowerEffectScaleMin = 0.08f;
    private float m_AiPowerTarget; // AIキャラがパワーが何たまった時に打つか（高いほど強い。最大値：m_MaxPower）
    private int m_AiSwingCountTarget; // AIキャラが何回スイングしたら打つか（少ないほど強い）
    private int m_AiSwingCount = 0;
    private bool m_IsCanSwingDown = false;
    private bool m_IsBoss = false;
    [SerializeField] private List<GameObject> m_DefaultSkinList = new List<GameObject>();
    [SerializeField] private List<GameObject> m_BossSkinList = new List<GameObject>();
    private int m_AiLevel = -1;
    private Rigidbody[] m_RagdollRigidbodies;
    [SerializeField] private GameObject m_BloodEffect;

    private enum E_STATE
    {
        WAIT,
        SWING_CHARGE,
        SWING_DOWN,
    }

    private void Start()
    {
        m_HammerBase.localEulerAngles = new Vector3(m_HammerBase.localEulerAngles.x, m_HammerBase.localEulerAngles.y, m_SwingUpMinAngleZ);
        m_Timer = 0f;
        m_Power = 0f;
        m_RagdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        SetRagdoll(false);
        m_IsCanSwingDown = false;

        if (m_IsUsingAI == true)
        {
            // 10の倍数はボス
            if (m_AiLevel % 10 == 0)
            {
                m_IsBoss = true;
                m_DefaultSkinList.ForEach(e => e.SetActive(false));
                m_BossSkinList.ForEach(e => e.SetActive(true));
            }
            AiSwingPreparation();
            // 初回だけ一度目で打つ
            m_AiSwingCountTarget = 0;
        }

        ChangeState(E_STATE.SWING_CHARGE);
    }

    private void Update()
    {


        switch (m_State)
        {
        case E_STATE.WAIT:
            break;

        case E_STATE.SWING_CHARGE:
            float powerEffectScale = Mathf.Lerp(m_PowerEffectScaleMin, m_PowerEffectScaleMax, m_Power / m_MaxPower);
            m_PowerEffect.transform.localScale = Vector3.one * powerEffectScale;
            m_Power = Mathf.SmoothStep(m_MinPower, m_MaxPower, 1f - m_HammerBase.localEulerAngles.z / m_SwingUpMinAngleZ); // XXX:ChangeStateでやるとなぜかうまくいかない
            if (m_IsUsingAI == true)
            {
                    // TEST: 連打に勝てるかのテスト
                    // ChangeState(E_STATE.SWING_DOWN);

                    if (m_Power >= m_AiPowerTarget && m_AiSwingCount >= m_AiSwingCountTarget)
                    {
                        if ((m_IsCanSwingDown == true))
                        {
                            ChangeState(E_STATE.SWING_DOWN);
                        }
                    } 
            }
            else
            {
                if ((Application.isEditor == true && Input.GetMouseButtonDown(0)) || (Application.isEditor == false && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
                {
                    if ((m_IsCanSwingDown == true))
                    {
                        ChangeState(E_STATE.SWING_DOWN);
                    }
                }
            }
            break;

        case E_STATE.SWING_DOWN:
            break;
        }
    }

    private void FixedUpdate()
    {
        m_Timer += Time.deltaTime;

        switch (m_State)
        {
        case E_STATE.WAIT:
            break;

        case E_STATE.SWING_CHARGE:
            SwingCharge();
            break;

        case E_STATE.SWING_DOWN:
            SwingDown();
            break;
        }
    }

    private void ChangeState(E_STATE state)
    {
        switch (m_State)
        {
        case E_STATE.WAIT:
            break;

        case E_STATE.SWING_CHARGE:
            m_Timer = 0f;
            break;

        case E_STATE.SWING_DOWN:
            m_Timer = 0f;
            m_RoundTripSpan = Random.Range(m_RoundTripSpanMin, m_RoundTripSpanMax);
            break;
        }

        m_State = state;
    }

    private void SwingCharge()
    {
        if (m_Timer < m_RoundTripSpan / 2)
        {
            float nextAngleZ = Mathf.Lerp(m_SwingUpMinAngleZ, m_SwingUpMaxAngleZ, m_Timer / (m_RoundTripSpan / 2));
            m_HammerBase.localEulerAngles = new Vector3(m_HammerBase.localEulerAngles.x, m_HammerBase.localEulerAngles.y, nextAngleZ);
        }

        if (m_Timer >= m_RoundTripSpan / 2 && m_Timer < m_RoundTripSpan)
        {
            float nextAngleZ = Mathf.Lerp(m_SwingUpMaxAngleZ, m_SwingUpMinAngleZ, (m_Timer - m_RoundTripSpan / 2) / (m_RoundTripSpan / 2));
            m_HammerBase.localEulerAngles = new Vector3(m_HammerBase.localEulerAngles.x, m_HammerBase.localEulerAngles.y, nextAngleZ);
        }

        if (m_Timer >= m_RoundTripSpan)
        {
            m_Timer = 0;
            m_AiSwingCount += 1;
        }
    }

    private void SwingDown()
    {
        float nextAngleZ = Mathf.Lerp(transform.localEulerAngles.z, m_SwingDownAngleZ, m_Timer / m_SwingDownTime);
        m_HammerBase.localEulerAngles = new Vector3(m_HammerBase.localEulerAngles.x, m_HammerBase.localEulerAngles.y, nextAngleZ);

        if (nextAngleZ >= m_SwingDownAngleZ)
        {
            AiSwingPreparation();
            Instantiate(m_HitEffectPrefab, m_HitEffectRoot);
            float directionX = m_IsLeft == true ? 1 : -1;
            //m_Step.transform.Translate(m_Power * directionX, 0f, 0f);
            iTween.MoveBy(m_Step, iTween.Hash("x", m_Power * directionX, "time", 0.3f));
            ChangeState(E_STATE.SWING_CHARGE);
        }
    }

    private void AiSwingPreparation()
    {
        m_AiSwingCount = 0;

        int minStep = Mathf.Clamp(Mathf.FloorToInt(m_AiLevel / 10) - 1, 0, 3 + 1);
        int maxStep = Mathf.Clamp(Mathf.FloorToInt(m_AiLevel / 10) + 1, 1, 5 + 1);
        if (m_IsBoss == true)
        {
            minStep = Mathf.Clamp(maxStep - 1, 0, maxStep);
        }
        float minPower = m_MinPower * (minStep / 5);
        float maxPower = m_MaxPower * (maxStep / 5);

        m_AiPowerTarget = Random.Range(minPower, maxPower);

        if (m_AiLevel <= 10)
        {
            m_AiSwingCountTarget = 2;
        }
        else if(m_AiLevel <= 30)
        {
            m_AiSwingCountTarget = Random.Range(1, 3);
        }
        else
        {
            m_AiSwingCountTarget = Random.Range(0, 2);
        }
    }

    private void SetRagdoll(bool isEnabled)
    {
        foreach (Rigidbody rigidbody in m_RagdollRigidbodies)
        {
            rigidbody.isKinematic = !isEnabled;
        }
    }

    public void Pause()
    {
        ChangeState(E_STATE.WAIT);
    }

    public void SetAiLevel(int level)
    {
        m_AiLevel = level;
    }

    public void SetIsCanSwingDown(bool isCanSwingDown)
    {
        m_IsCanSwingDown = isCanSwingDown;
    }

    public void Death()
    {
        SetRagdoll(true);
        m_HammerBase.GetComponent<Rigidbody>().isKinematic = false;
        FastIKFabric[] fastIKArray = GetComponentsInChildren<FastIKFabric>();
        foreach (FastIKFabric fastIk in fastIKArray)
        {
            fastIk.enabled = false;
        }
        m_BloodEffect.SetActive(true);
    }
}
