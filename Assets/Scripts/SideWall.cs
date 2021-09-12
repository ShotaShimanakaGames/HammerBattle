using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWall : MonoBehaviour
{
    [SerializeField] private float m_MoveStartPosX;
    [SerializeField] private float m_MoveEndPosX;
    [SerializeField] private float m_MoveDuration;

    private void Start()
    {
        transform.position = new Vector3(m_MoveStartPosX, transform.position.y, transform.position.z);
    }

    private void Update()
    {
    }

    public void OpenStart()
    {
        iTween.MoveTo(gameObject, iTween.Hash("x", m_MoveEndPosX, "time", m_MoveDuration));
    }

    public float GetMoveDuration()
    {
        return m_MoveDuration;
    }
}
