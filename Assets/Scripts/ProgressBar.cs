using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform m_PlayerBar;
    [SerializeField] private RectTransform m_EnemyBar;
    [SerializeField] private RectTransform m_Handle;
    [SerializeField] private RectTransform m_HandleLeftLimitPos;
    [SerializeField] private RectTransform m_HandleRightLimitPos;

    private void Start()
    {

    }

    private void Update()
    {
        m_PlayerBar.position = new Vector3(m_Handle.position.x, m_PlayerBar.position.y, m_PlayerBar.position.z);
        m_EnemyBar.position = new Vector3(m_Handle.position.x, m_EnemyBar.position.y, m_EnemyBar.position.z);
    }

    public void UpdateBarRatio(float ratio)
    {
        float posX = Mathf.Lerp(m_HandleLeftLimitPos.position.x, m_HandleRightLimitPos.position.x, ratio);
        m_Handle.position = new Vector3(posX, m_Handle.position.y, m_Handle.position.z);
    }
}
